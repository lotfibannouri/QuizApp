using Authentication.web.Enum;

namespace Authentication.web.Model
{
    public class User
    {

        public User()
        {
            
        }
        public string id { get; set; }
        public int rowNumber { get; set; }
        public string userName { get; set; }
        public string lastName {
            get
            {
                return userName.Split('.')[0];
            }

            set { } }
        public string firstName
        {
            get
            {
                return userName.Split('.')[1];
            }

            set { }
        }
        public string adresse   { get; set; }
        public string email { get; set; }
        public DateTime dateCreation { get; set; }
        public List<string> role { get; set; }=new List<string>();
    }
}
