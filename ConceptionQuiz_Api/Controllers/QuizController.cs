using ConceptionQuiz_Api.Models;
using ConceptionQuiz_Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace ConceptionQuiz_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        #region properties
        private readonly IQuizRepository _quizRepository;
        #endregion
        #region Constructor
        public QuizController(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }
        #endregion

        #region QuizMethods
            
        [HttpPost("AddQuiz")]
        public async Task<Response> CreateQuiz([FromBody] CreationQuizDTO quiz)
        {
            try
            {
                var result = await _quizRepository.CreateQuiz(quiz);
                return result; 
               

            }
           catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet("ListQuiz")]
        public async Task<IActionResult> GetQuiz()
        {
            try { 
            var result = await _quizRepository.ListQuiz();
            if (result != null)
                return Ok(result);
            else
                return NotFound("liste des quiz est vide ");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }
        //#endregion
        //#region QuestionMethods
        //[HttpPost("AddQuestion")]
        //public async Task<IActionResult> CreateQuestion([FromBody] Question question)
        //{
        //    try
        //    {
        //        var result = await _quizRepository.CreateQuestion(question);
        //        if (result)
        //        {
        //            return Ok(result);
        //        }

        //        return BadRequest();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //}


        //[HttpGet("ListQuestions")]
        //public async Task<IActionResult> GetQuestions()
        //{
        //    try
        //    {
        //        var result = await _quizRepository.ListQuestions();
        //        if (result != null)
        //            return Ok(result);
        //        else
        //            return NotFound("liste des Question est vide ");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }

        //}

        [HttpPost("AddToQuiz")]
        public async Task<IActionResult> AddToQuiz(string idQuiz, string idQuestion)
        {
            try
            {
                var result = await _quizRepository.AddToQuiz(idQuiz, idQuestion);
                if (result)
                {
                    return Ok(result);
                }

                return BadRequest();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        [HttpPost("BindQuiz")]
        public async Task<Response> BindQuiz([FromBody] QuizUserDTO quizUser)
        {
            try
            {
                var result = await _quizRepository.BindQuiz(quizUser);
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        #endregion
    }
}
