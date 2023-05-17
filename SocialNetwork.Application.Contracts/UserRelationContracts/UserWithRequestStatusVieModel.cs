using System;
using _00_Framework.Application;

namespace SocialNetwork.Application.Contracts.UserRelationContracts
{
    public class UserWithRequestStatusVieModel
    {
        //User that request sent to 
        public long UserId { get; set; }
        //User that request sent to
        public string Name { get; set; }

        public string LastName { get; set; }

        //Status of request
        public RequestStatus RequestStatusNumber { get; set; }
        public DateTimeOffset TimeOffset { get; set; }
    }
}