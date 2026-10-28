using QuizApp.Entities.Base_DTO;

namespace QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO
{
    public class ListQuizDTO :BaseListDTO
    {
        public string titre { get; set; }
        public string categorieId { get; set; }
        public string categorieTitre { get; set; }
        public int duree_quiz { get; set; }
        public string description { get; set; }
        public int nbr_questions { get; set; }
        public int icon { get; set; }

        /// <summary>Questions réellement rattachées (nbr_questions est le nombre prévu).</summary>
        public int nbQuestionsRattachees { get; set; }

        /// <summary>Utilisateurs à qui le quiz est assigné.</summary>
        public int nbUtilisateursAssignes { get; set; }

        /// <summary>Actif : le nombre de questions rattachées correspond au nombre prévu.</summary>
        public bool isComplet => nbQuestionsRattachees == nbr_questions;

        public bool isAssigne => nbUtilisateursAssignes > 0;
    }
}
