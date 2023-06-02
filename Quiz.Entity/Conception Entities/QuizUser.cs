using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Entities.Conception_Entities
{
    public class QuizUser
    {

        public Guid quizid { get; set; }
        public Quiz? quiz { get; set; }
        public Guid userid { get; set; }      


    }
}
