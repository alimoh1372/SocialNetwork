namespace _00_Framework.Application
{
    /// <summary>
    /// To helping Implementation of authorize and authenticate
    /// </summary>
    public interface IAuthHelper
    {
        /// <summary>
        /// Sign out the current user
        /// </summary>
        void SignOut();
        /// <summary>
        /// check the client is authenticate or not
        /// </summary>
        /// <returns>if user was authenticate return <see langword="true"/> else return <see langword="false"/> </returns>
        bool IsAuthenticated();

        /// <summary>
        /// To sign in the user with needed Item 
        /// </summary>
        /// <param name="account"></param>
        void Signin(AuthViewModel account);


        /// <summary>
        /// Get the current user authenticate information
        /// </summary>
        /// <returns></returns>
        AuthViewModel CurrentAccountInfo();

    }
}