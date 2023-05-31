using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO
{
    public class QuizUserDTO
    {
        public string UserId { get; set; }
        public string QuizId { get; set; }
    }
}
