using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Entities.Conception_Entities.DTO.Reponse_DTO
{
    public class CreationReponseDTO
    {
        public string Body { get; set; }
        public bool IsRawAnswer { get; set; }
        public bool? IsAnswer { get; set; }
        public string? Output { get; set; }

    }
}
