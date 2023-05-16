using System.Collections.Generic;
using _00_Framework.Application;

namespace SocialNetwork.Application.Contracts.UserRelationContracts
{
    public interface IUserRelationApplication
    {
        OperationResult Create(CreateUserRelation command);


        /// <summary>
        /// accept the relationship request user A
        /// </summary>
        /// <param name="id">id of request</param>
        /// <returns></returns>
        OperationResult Accept(long id);


        /// <summary>
        /// decline the relationship request from user A
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OperationResult Decline(long id);

        List<UserWithRequestStatusVieModel> GetAllUserWithRequestStatus(long currentUserId);

    }
}