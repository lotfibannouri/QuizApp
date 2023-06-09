﻿using AutoMapper;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Proposition_DTO;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using QuizApp.Entities.Conception_Entities.DTO.Reponse_DTO;

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
            CreateMap<CreationQuestionDTO, Question>();
            CreateMap<Question, CreationQuestionDTO>();
            CreateMap<PropositionDTO, Proposition>().ForMember(dest => dest.textProposition, opt => opt.MapFrom(src => src._textPropositon));
            CreateMap<Proposition, PropositionDTO>();
            CreateMap<CreationReponseDTO, Reponse>();
            CreateMap<Reponse, CreationReponseDTO>();
            CreateMap<QuizUserDTO, QuizUser>();
            CreateMap<QuizUser, QuizUserDTO>();
            CreateMap<ListQuestionDTO, Question>();
            CreateMap<Question, ListQuestionDTO>();

        }
    }
}
