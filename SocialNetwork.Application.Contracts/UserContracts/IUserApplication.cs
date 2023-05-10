using _00_Framework.Application;

namespace SocialNetwork.Application.Contracts.UserContracts
{
    public interface IUserApplication
    {

        OperationResult Create(CreateUser command);
        OperationResult Edit(EditUser command);

        OperationResult ChangePassword(ChangePassword command);

    }
}