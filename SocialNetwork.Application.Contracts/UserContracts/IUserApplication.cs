using System.Collections.Generic;
using System.Threading.Tasks;
using _00_Framework.Application;

namespace SocialNetwork.Application.Contracts.UserContracts
{
    public interface IUserApplication
    {

        OperationResult Create(CreateUser command);
        OperationResult Edit(EditUser command);

        OperationResult ChangePassword(ChangePassword command);
        EditUser GetDetails(long id);

       Task<List<UserViewModel>> SearchAsync(SearchModel searchModel);

    }

    public class SearchModel
    {
        public string Email { get; set; }
    }

    public class UserViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
    }
}