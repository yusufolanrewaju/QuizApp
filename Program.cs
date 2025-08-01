using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using QuizApp;


namespace QuizApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            do
            {
                await RunQuizAsync();
            }
            while (AskToRetry());
            Console.Clear();
            Console.WriteLine("Thank you for playing the quiz!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task RunQuizAsync()
        {
            int questionCount = GetQuestionCountFromUser();
            int categoryId = await GetCategoryIdFromUserAsync();

            Question[] questions = await FetchQuestionsAsync(questionCount, categoryId);

            Quiz myQuiz = new Quiz(questions);
            myQuiz.StartQuiz();
        }

        private static bool AskToRetry()
        {
            Console.WriteLine();
            Console.ResetColor();
            Console.Write("Would you like to take another quiz? (Y/N): ");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                {
                    Console.WriteLine("Y");
                    return true;
                }
                else if (key.Key == ConsoleKey.N)
                {
                    Console.WriteLine("N");
                    return false;
                }
                else
                {
                    Console.WriteLine();
                    Console.Write("Invalid input. Please press 'Y' for Yes or 'N' for No: ");
                }
            }
        }

        private static int GetQuestionCountFromUser()
        {
            int min = 1, max = 100;
            int count;
            Console.Write($"How many questions would you like? ({min}-{max}): ");
            while (!int.TryParse(Console.ReadLine(), out count) || count < min || count > max)
            {
                Console.WriteLine($"Please enter a valid number between {min} and {max}.");
                Console.Write($"How many questions would you like? ({min}-{max}): ");
            }
            return count;
        }

        private static async Task<int> GetCategoryIdFromUserAsync()
        {
            var categories = await FetchCategoriesAsync();
            Console.WriteLine("Available Categories:");
            char letter = 'A';
            foreach (var category in categories)
            {
                Console.WriteLine($"{letter}. {category.name}");
                letter++;
            }

            while (true)
            {
                Console.Write("Select a category by letter (or press Enter for random): ");
                string input = Console.ReadLine().ToUpper();
                if (string.IsNullOrWhiteSpace(input))
                    return 0; // 0 means random

                if (input.Length == 1)
                {
                    int index = input[0] - 'A';
                    if (index >= 0 && index < categories.Count)
                        return categories[index].id;
                }

                Console.WriteLine("Invalid selection. Please enter a valid letter corresponding to a category.");
            }
        }

        public static async Task<Question[]> FetchQuestionsAsync(int count = 20, int categoryId = 0)
        {
            using var client = new HttpClient();
            string url = $"https://opentdb.com/api.php?amount={count}&type=multiple";
            if (categoryId != 0)
                url += $"&category={categoryId}";
            var response = await client.GetStringAsync(url);

            var triviaResponse = JsonSerializer.Deserialize<TriviaApiResponse>(response);

            return triviaResponse.results.Select(q =>
            {
                var allAnswers = q.incorrect_answers.Append(q.correct_answer).OrderBy(_ => Guid.NewGuid()).ToArray();
                int correctIndex = Array.IndexOf(allAnswers, q.correct_answer);
                return new Question(
                    System.Net.WebUtility.HtmlDecode(q.question),
                    allAnswers,
                    correctIndex
                );
            }).ToArray();
        }

        public static async Task<List<TriviaCategory>> FetchCategoriesAsync()
        {
            using var client = new HttpClient();
            string url = "https://opentdb.com/api_category.php";
            var response = await client.GetStringAsync(url);
            var categoryResponse = JsonSerializer.Deserialize<TriviaCategoryResponse>(response);
            return categoryResponse.trivia_categories;
        }

        // Models for deserialization
        public class TriviaApiResponse
        {
            public List<TriviaQuestion> results { get; set; }
        }

        public class TriviaQuestion
        {
            public string question { get; set; }
            public string correct_answer { get; set; }
            public List<string> incorrect_answers { get; set; }
        }

        public class TriviaCategoryResponse
        {
            public List<TriviaCategory> trivia_categories { get; set; }
        }

        public class TriviaCategory
        {
            public int id { get; set; }
            public string name { get; set; }
        }

    }
}
