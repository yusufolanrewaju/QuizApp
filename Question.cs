﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp
{
    internal class Question
    {

        public string QuestionText { get; set; }

        public string[] Answers { get; set; }

        public int CorrectAnswerIndex { get; set; }

        public Question(string questionText, string[] answers, int correctAnswerIndex)
        {
            QuestionText = questionText;
            Answers = answers;
            CorrectAnswerIndex = correctAnswerIndex;
        }

        public bool IsCorrect(int userChoice)
        {
            return userChoice == CorrectAnswerIndex;
        }

    }
}
