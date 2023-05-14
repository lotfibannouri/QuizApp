namespace Authentication.web.Model
{
    public class Item
    {
        public Item(User user, string name)
        {
            _user = user;
            _name = name;
            if (_user.role.Exists(x => x == _name))
                _Identifier = "User";
            else
                _Identifier = "Roles";

        }
        public Item()
        {
            if (_user.role.Exists(x => x == _name))
                _Identifier = "User";
            else
                _Identifier = "Roles";
        }
        public User _user { get; set; }
        public string _name { get; set; }
        public string _team { get; set; }
        public string _Identifier { get; set; }
        
    }
}
