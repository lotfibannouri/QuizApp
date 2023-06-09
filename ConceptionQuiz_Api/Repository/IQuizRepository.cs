﻿using ConceptionQuiz_Api.Models;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace ConceptionQuiz_Api.Repository
{
    public interface IQuizRepository
    {
        Task<Response> CreateQuiz(Quiz quiz);
        Task<Response> DeleteQuiz(string id);
        Task<bool> UpdateQuiz(string id, Quiz quiz);
        Task<List<ListQuizDTO>> ListQuiz();
        Task<Quiz> GetQuizById(string id);
        Task<Quiz> GetQuizByName(string name);
        Task<Response> BindQuizToQuestion(string idQuiz, string idQuestion);
        Task<Response> BindQuizToUser(QuizUser QuizUser);
        Task<List<QuizUser>> ListQuizUser();
        


    }
}
