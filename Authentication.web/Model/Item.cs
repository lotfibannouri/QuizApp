namespace Authentication.web.Model
{
    public class Item
    {
        public Item(User user)
        {
            _user = user;
            if (_user.role.Exists(x => x == name))
                Identifier = "User";
            else
                Identifier = "Roles";

        }
        public Item()
        {
            if (_user.role.Exists(x => x == name))
                Identifier = "User";
            else
                Identifier = "Roles";
        }
        public User _user { get; set; }
        public string name { get; set; }
        public string team { get; set; }
        public string Identifier { get; set; }
        
    }
}
