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

        public UserApplication(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public OperationResult Create(CreateUser command)
        {
            var result = new OperationResult();
            //check if user with same email is exist return fail message;
            if (_userRepository.IsExists(x => x.Email == command.Email))
                return result.Failed(ApplicationMessage.Duplication);
            var user = new User(command.Name, command.LastName, command.Email, command.BirthDay, command.Password,
                command.AboutMe, command.ProfilePicture);
            _userRepository.Create(user);
            _userRepository.SaveChanges();
            return result.Succedded();
        }

        public OperationResult Edit(EditUser command)
        {
            var result = new OperationResult();
            
            var user = _userRepository.Get(command.Id);
            //check if user not found return failed operation result
            if (user == null)
                return result.Failed(ApplicationMessage.NotFound);
            
            user.Edit(command.Name,command.LastName,command.AboutMe,command.ProfilePicture);

            _userRepository.SaveChanges();
            return result.Succedded();
        }

        public OperationResult ChangePassword(ChangePassword command)
        {
            var result = new OperationResult();

            var user = _userRepository.Get(command.Id);

            //check if user not found return failed operation result
            if (user == null)
                return result.Failed(ApplicationMessage.NotFound);

            user.ChangePassword(command.Password);

            _userRepository.SaveChanges();
            return result.Succedded();
        }

        public EditUser GetDetails(long id)
        {
            return _userRepository.GetDetails(id);
        }

        public Task<List<UserViewModel>> SearchAsync(SearchModel searchModel)
        {
            return _userRepository.SearchAsync(searchModel);
        }
    }
}