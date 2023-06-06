using ConceptionQuiz_Api.Models;
using ConceptionQuiz_Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

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
        public async Task<Response> CreateQuiz([FromBody] Quiz quiz)
        {
            try
            {
                var result = await _quizRepository.CreateQuiz(quiz);
                return result;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet("ListQuiz")]
        public async Task<IActionResult> GetQuiz()
        {
            try
            {
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
        [HttpGet("ListQuizByUser")]
        public async Task<IActionResult> GetQuiz(string UserId)
        {
            try
            {
                var binds = await _quizRepository.ListQuizUser();

                if (binds == null)
                    return NotFound("liste des quiz est vide ");

                if (binds != null)
                {
                    var filteredlist = binds.Where(x => x.userid.ToString() == UserId).Select(x => x.quiz).ToList();
                    return Ok(filteredlist);
                }      
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

        [HttpPost("DeleteQuiz")]
        public async Task<Response> DeleteQuiz(string id)
        {
            try
            {
                var result = await _quizRepository.DeleteQuiz(id);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("BindQuizToQuestion")]
        public async Task<Response> BindQuizToQuestion(string idQuestion, string idQuiz)
        {
            try
            {
                var result = await _quizRepository.BindQuizToQuestion(idQuestion, idQuiz);
             
                return result;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        [HttpPost("BindQuizToUser")]
        public async Task<Response> BindQuizToUser([FromBody] QuizUser quizUser)
        {
            try
            {
                var result = await _quizRepository.BindQuizToUser(quizUser);
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        [HttpGet("GetQuizById")]
        public async Task<Quiz> GetQuizById(string id)
        {
            try
            {
                var result = await _quizRepository.GetQuizById(id);
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
