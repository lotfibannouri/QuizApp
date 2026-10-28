using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Entities.Conception_Entities.DTO.QuestionDTO
{
    /// <summary>
    /// Code à exécuter. Ce DTO est partagé avec le site Blazor, donc téléchargé par le
    /// navigateur : les identifiants JDoodle n'y figurent pas, l'API les ajoute elle-même.
    /// </summary>
    public class AttemptCode
    {
        public string language { get; set; }
        public string script { get; set; }
        public string versionIndex { get; set; }
    }
}
