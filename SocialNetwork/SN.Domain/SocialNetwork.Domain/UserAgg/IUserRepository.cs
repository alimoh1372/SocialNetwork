using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _00_Framework.Domain;
using SocialNetwork.Application.Contracts.UserContracts;


namespace SocialNetwork.Domain.UserAgg
{
    public interface IUserRepository:IBaseRepository<long,User>
    {
        EditUser GetDetails(long id);
        Task<EditProfilePicture> GetAsyncEditProfilePicture(long id);
        Task<List<UserViewModel>> SearchAsync(SearchModel searchModel);
        User GetBy(string userName);
        Task<UserViewModel> GetUserInfoAsyncBy(long id);
    }
}