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
        public string body { get; set; }
        public string questionId { get; set; }
        public virtual Question question { get; set; }
    }
}
