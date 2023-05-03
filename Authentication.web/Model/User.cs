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
        public string userName 
        {
            get { return firstName + '.' + lastName; }
         
            set { 
                firstName = value.Split('.')[0];
                lastName = value.Split('.')[1];
            } 
        }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string adresse   { get; set; }
        public string email { get; set; }
        public DateTime dateCreation { get; set; }
        public List<string> role { get; set; }=new List<string>();
    }
}
