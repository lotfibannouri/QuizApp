using QuizApp.Entities.Base_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Entities.Conception_Entities
{
    public class Reponse : BaseEntity
    {
        public string Body { get; set; }
        public Guid QuestionId { get; set; }
        public bool IsRawAnswer { get; set; }
        public bool IsAnswer { get; set; }
        public string output { get; set; }
        public  Question? Question { get; set; }
    }
}
