﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetwork.Application.Contracts.UserRelationContracts;

namespace _01_SocialNetworkQuery.Contract
{
    public interface IUserRelationQuery
    {
        Task<List<UserWithRequestStatusVieModel>> GetAllUsersWithRelationStatusAsync(long currentUserId);
    }
}