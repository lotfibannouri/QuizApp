using Authentication.web.Shared;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using System.ComponentModel;

namespace Authentication.web.Pages
{
    public partial class QuestionManagement
    {
        private List<Object> components;
        public string selectedOption { get; set; }
        CreationQuestionDTO model = new CreationQuestionDTO();

        public void OnValidSubmit() {

           
        }

        


    }
}
