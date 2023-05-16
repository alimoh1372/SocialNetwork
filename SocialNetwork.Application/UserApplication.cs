using System.Collections.Generic;
using System.Threading.Tasks;
using _00_Framework.Application;
using SocialNetwork.Application.Contracts.UserContracts;
using SocialNetwork.Domain.UserAgg;

namespace SocialNetwork.Application
{
    public class UserApplication:IUserApplication
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthHelper _authHelper;
        public UserApplication(IUserRepository userRepository, IPasswordHasher passwordHasher, IAuthHelper authHelper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _authHelper = authHelper;
        }

        public OperationResult Create(CreateUser command)
        {
            var operation = new OperationResult();

            if (_userRepository.IsExists(x => x.Email == command.Email))
                return operation.Failed(ApplicationMessage.Duplication);

            var password = _passwordHasher.Hash(command.Password);
            var account = new User(command.Name, command.LastName, command.Email, command.BirthDay,password,
                command.AboutMe, "~/Images/DefaultProfile.png");
            _userRepository.Create(account);
            _userRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult Edit(EditUser command)
        {

            var operation = new OperationResult();
            var user = _userRepository.Get(command.Id);
            if (user == null)
                return operation.Failed(ApplicationMessage.NotFound);


            
            user.Edit(command.Name,command.LastName,command.AboutMe, "~/Images/DefaultProfile.png");
            _userRepository.SaveChanges();
            return operation.Succedded();
        }

        public OperationResult ChangePassword(ChangePassword command)
        {
            var operation = new OperationResult();
            var user = _userRepository.Get(command.Id);
            if (user == null)
                return operation.Failed(ApplicationMessage.NotFound);

            if (command.Password != command.ConfirmPassword)
                return operation.Failed(ApplicationMessage.PasswordsNotMatch);

            var password = _passwordHasher.Hash(command.Password);
            user.ChangePassword(password);
            _userRepository.SaveChanges();
            return operation.Succedded();
        }

        public EditUser GetDetails(long id)
        {
            return _userRepository.GetDetails(id);
        }

        public Task<List<UserViewModel>> SearchAsync(SearchModel searchModel)
        {
            return _userRepository.SearchAsync(searchModel);
        }

        public  OperationResult Login(Login command)
        {
            var operation = new OperationResult();
            var user = _userRepository.GetBy(command.UserName);
            if (user == null)
                return  operation.Failed(ApplicationMessage.WrongUserPass);

            var result = _passwordHasher.Check(user.Password, command.Password);
            if (!result.Verified  )
                return operation.Failed(ApplicationMessage.WrongUserPass);



            var authViewModel = new AuthViewModel(user.Id, user.Email);

            _authHelper.Signin(authViewModel);

            return operation.Succedded();
        }

        public void Logout()
        {
            _authHelper.SignOut();
        }

        
    }
}