namespace Authentication.web.Model
{
    public class Question
    {
        public string id { get; set; }
        public string questionText { get; set; }
        public string type { get; set; }
        public string body { get; set; }
        public virtual ICollection<Quiz>? quiz { get; set; }
        public virtual ICollection<Reponse>? reponses { get; set; }
    }
}
