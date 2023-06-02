using QuizApp.Entities.Base_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Entities.Conception_Entities
{
    public class Proposition : BaseEntity
    {
        public string textProposition { get; set; }
        public Question? question { get; set; }
        public Guid questionId { get; set; }
    }
}
