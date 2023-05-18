using ConceptionQuiz_Api.utility;

namespace ConceptionQuiz_Api.Models

{
    
    [Serializable]
    public class Question
    {

        public Question()
        {
            id = Guid.NewGuid().ToString();
            this.quiz = new HashSet<Quiz>();
            this.reponses = new HashSet<Reponse>();
        }
        public string id { get; set; }
        public string questionText { get; set; }
        public string type { get; set; }
        public string body { get; set; }
        public virtual ICollection<Quiz>? quiz { get; set; }
        public virtual ICollection<Reponse>? reponses { get; set; }


    }
}
