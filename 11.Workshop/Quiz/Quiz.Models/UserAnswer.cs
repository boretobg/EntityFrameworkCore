﻿using Microsoft.AspNetCore.Identity;

namespace Quiz.Models
{
    public class UserAnswer
    {
        public string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; }

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int AnsweId { get; set; }
        public Answer Answer { get; set; }
    }
}
