using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Entities.Conception_Entities.DTO.QuestionDTO
{
    public class OutputCode
    {
        public string output { get; set; }
        public int statusCode { get; set; }
        public string? memory { get; set; }
        public string? compilationStatus { get; set; }
    }
}
