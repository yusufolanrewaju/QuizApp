using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace QuizApp
{
    internal class Quiz
    {

        private Question[] _questions;
        private int _score;

        public Quiz(Question[] questions)
        {
            this._questions = questions;
            this._score = 0;
        }

        public void StartQuiz() {             
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔═══════════════ ▓▓▓▓▓▓▓▓ ═══════════════╗");
            Console.WriteLine();
            Console.WriteLine("          WELCOME TO THE QUIZ APP         ");
            Console.WriteLine();
            Console.WriteLine("╚═══════════════ ▓▓▓▓▓▓▓▓ ═══════════════╝");
            Console.WriteLine();
            Console.WriteLine("You have 30s for each question!");
            Console.ResetColor();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            int questionNumber = 1;

            //Scope to display questions one after the other
            foreach (Question question in _questions)
            {
                Console.Clear();
                Console.WriteLine($"Question {questionNumber++}: ");
                DisplayQuestion(question);

                int userChoice = GetUserChoiceWithTimer(30);


                string[] optionLetters = { "A", "B", "C", "D" };
                string correctOption = question.CorrectAnswerIndex >= 0 && question.CorrectAnswerIndex < optionLetters.Length
                    ? optionLetters[question.CorrectAnswerIndex]
                    : (question.CorrectAnswerIndex + 1).ToString();

                if (question.IsCorrect(userChoice))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Correct!"); Console.WriteLine();
                    _score++;
                    Console.ResetColor();
                }
                else if (userChoice == -1) // User did not answer in time
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.Write($"Time's up! No answer was submitted. The correct answer is: ");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{correctOption}. {question.Answers[question.CorrectAnswerIndex]}");
                    Console.ResetColor();

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Map index to letter
                    
                    Console.Write($"Incorrect. The correct answer was: ");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{correctOption}. {question.Answers[question.CorrectAnswerIndex]}");
                    Console.ResetColor();
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            DisplayScore();
        }

        private void DisplayQuestion(Question question)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("╔═══════════════ ▓▓▓▓▓▓▓▓ ═══════════════╗");
            Console.WriteLine();
            Console.WriteLine("                 QUESTIONS                ");
            Console.WriteLine();
            Console.WriteLine("╚═══════════════ ▓▓▓▓▓▓▓▓ ═══════════════╝");
            Console.WriteLine();

            Console.ResetColor(); 

            Console.WriteLine(question.QuestionText);
            for (int i = 0; i < question.Answers.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan; // Changes the text color to cyan

                switch (i)
                {
                    case 0:
                        Console.Write("   A. ");
                        break;
                    case 1:
                        Console.Write("   B. ");
                        break;             
                    case 2:                
                        Console.Write("   C. ");
                        break;             
                    case 3:                
                        Console.Write("   D. ");
                        break;
                    default:
                        Console.Write($"{i + 1}. ");
                        break;
                }

                Console.ResetColor(); // Resets the text color to default
                Console.WriteLine($"{question.Answers[i]}");
            }

        }

        private void DisplayScore()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("╔═══════════════ ▓▓▓▓▓▓▓▓ ═══════════════╗");
            Console.WriteLine();
            Console.WriteLine("                YOUR SCORE                ");
            Console.WriteLine();
            Console.WriteLine("╚═══════════════ ▓▓▓▓▓▓▓▓ ═══════════════╝");
            Console.ResetColor();

            double percentage = (double)_score / _questions.Length * 100;

            Console.WriteLine($"Your score is: {_score} out of {_questions.Length}, ending with {percentage:F2}%.");

            if (percentage >= 75)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Congratulations! You performed excellently!");
            }
            else if (percentage >= 50)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Good job! You passed the quiz.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Better luck next time!");
            }

        }

        private int GetUserChoice()
        {
            Console.Write("Your answer: ");
            string input = Console.ReadLine().ToUpper();
            switch (input)
            {
                case "A":
                    return 0;
                case "B":
                    return 1;
                case "C":
                    return 2;
                case "D":
                    return 3;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    return GetUserChoice();
            }
        }

        private int GetUserChoiceWithTimer(int timeoutSeconds)
        {
            // Write the prompt and timer together
            Console.Write("Your answer: ");
            int promptLeft = Console.CursorLeft;
            int promptTop = Console.CursorTop;

            // Write initial timer
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write($"[Time left: {timeoutSeconds,2}s] ");
            Console.ResetColor();

            // Place cursor right after the timer for user input
            int inputLeft = Console.CursorLeft;
            int inputTop = Console.CursorTop;

            string buffer = "";
            DateTime start = DateTime.Now;
            TimeSpan timeout = TimeSpan.FromSeconds(timeoutSeconds);

            while ((DateTime.Now - start) < timeout)
            {
                // Update timer only
                int secondsLeft = timeoutSeconds - (int)(DateTime.Now - start).TotalSeconds;
                Console.SetCursorPosition(promptLeft, promptTop);
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.Write($"[Time left: {secondsLeft,2}s] ");
                Console.ResetColor();

                // Restore cursor to input position
                Console.SetCursorPosition(inputLeft + buffer.Length, inputTop);

                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);
                    char keyChar = char.ToUpper(keyInfo.KeyChar);

                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        if (buffer.Length == 1 && "ABCD".Contains(buffer))
                        {
                            Console.WriteLine();
                            return buffer[0] - 'A';
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.Write("Invalid choice. Please try again: ");
                            buffer = "";
                            inputLeft = Console.CursorLeft;
                            inputTop = Console.CursorTop;
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (buffer.Length > 0)
                        {
                            buffer = "";
                            Console.SetCursorPosition(inputLeft, inputTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(inputLeft, inputTop);
                        }
                    }
                    else if ("ABCD".Contains(keyChar))
                    {
                        buffer = keyChar.ToString();
                        Console.SetCursorPosition(inputLeft, inputTop);
                        Console.Write(buffer);
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }

            // Time's up
            Console.SetCursorPosition(promptLeft, promptTop);
            Console.Write(new string(' ', 30));
            Console.SetCursorPosition(inputLeft, inputTop);
            if (buffer.Length == 1 && "ABCD".Contains(buffer))
            {
                Console.WriteLine();
                Console.WriteLine($"Time's up! Your last selected answer '{buffer}' was submitted.");
                return buffer[0] - 'A';
            }
            else
            {
                return -1; // Mark as missed
            }
        }

        /*private void ShowTimer(int left, int top, int seconds)
        {
            for (int i = seconds; i >= 0; i--)
            {
                Console.SetCursorPosition(left, top);
                Console.Write($"[Time left: {i,2}s] ");
                Thread.Sleep(1000);
            }
        }*/

    }
}
