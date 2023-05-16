namespace _00_Framework.Application
{
    public interface IAuthHelper
    {
        void SignOut();
        bool IsAuthenticated();
        void Signin(AuthViewModel account);
        AuthViewModel CurrentAccountInfo();

    }
}