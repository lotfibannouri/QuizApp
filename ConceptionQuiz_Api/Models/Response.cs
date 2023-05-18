namespace ConceptionQuiz_Api.Models
{
    public class Response
    {
        public Response(bool status = false, string content = "")
        {
            this.status = status;
            this.content = content;
        }
        public bool status { get; set; }
        public string content { get; set; }
    }
}
