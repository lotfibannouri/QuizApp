using AutoMapper;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace ConceptionQuiz_Api.utility
{
    public class AutoMapperHandler :Profile
    {
        public AutoMapperHandler()
        {
            CreateMap<CreationQuizDTO,Quiz>();
            CreateMap<Quiz,CreationQuizDTO>();
            CreateMap<ListQuizDTO, Quiz>();
            CreateMap<Quiz,ListQuizDTO>();

        }
    }
}
