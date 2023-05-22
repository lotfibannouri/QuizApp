namespace Authentication.web.Model
{
    public class ToolBarParam
    {
        public ToolBarParam(string icon, string func)
        {
            this.icon = icon;
            this.func = func;
        }
        public string icon { get; set; }
        public string func { get; set; }


    }
}
