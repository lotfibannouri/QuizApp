namespace ConceptionQuiz_Api.Models
{
    public class Reponse
    {
        public string id { get; set; }
        public string body { get; set; }
        public string questionId { get; set; }
        public virtual Question question { get; set; }

    }
}
