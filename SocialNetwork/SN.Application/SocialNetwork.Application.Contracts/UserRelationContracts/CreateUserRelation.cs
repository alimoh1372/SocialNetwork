﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Application.Contracts.UserRelationContracts
{
    /// <summary>
    /// To create request of User relation from user A to user B
    /// </summary>
    public class CreateUserRelation
    {
        
        [DisplayName("Request From User A")]
        [Range(1,long.MaxValue)]
        public long FkUserAId { get;  set; }

        [DisplayName("Request To User B")]
        [Range(1, long.MaxValue)]
        public long FkUserBId { get;  set; }

    }
}