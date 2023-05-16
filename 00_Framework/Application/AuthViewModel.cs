namespace _00_Framework.Application
{
    public class AuthViewModel
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public AuthViewModel()
        {
        }

        public AuthViewModel(long id,   string username)
        {
            Id = id;
            Username = username;
        }

    }
}