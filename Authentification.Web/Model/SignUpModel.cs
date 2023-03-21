using System.ComponentModel.DataAnnotations;

namespace Authentification.Web.Model
{
    public class SignUpModel
    {
        [Required]
        public string nom { get; set; }
        [Required]
        public string prenom { get; set; }

        [Required]
        public string adresse { get; set; }
        [Required]
        public DateTime dateCreation { get; set; }
        [Required]
        [EmailAddress]
        public string login { get; set; }
        [Required]
        public string password { get; set; }
    }
}
