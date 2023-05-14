namespace Authentication.web.Model
{
    public class Response
    {
        public Response()
        {
            
        }
        public Response(bool status, string content)
        {
            this.status = status;
            this.content = content;
        }

        public bool status { get; set; }
        public string content { get; set; }
    }
}
