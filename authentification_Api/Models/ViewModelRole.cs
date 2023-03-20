using System.ComponentModel.DataAnnotations;

namespace authentification_Api.Models
{
    public class ViewModelRole
    {
       
        [Required] 
        public string name { get; set; }

    }
}
