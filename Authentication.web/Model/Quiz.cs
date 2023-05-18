using System.ComponentModel.DataAnnotations;

namespace Authentication.web.Model
{
    public class Quiz
    {
        public string id { get; set; }
        [Required]
        public string titre { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public int niv_deficulte { get; set; }
        [Required]
        public int duree_quiz { get; set; }
        [Required]
        public DateTime dateCreation { get; set; }
        [Required]
        public int nbr_questions { get; set; }
        public virtual ICollection<Question>? questions { get; set; }
    }
}
