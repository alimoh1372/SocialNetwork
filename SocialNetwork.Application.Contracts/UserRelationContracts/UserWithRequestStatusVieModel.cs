using _00_Framework.Application;

namespace SocialNetwork.Application.Contracts.UserRelationContracts
{
    public class UserWithRequestStatusVieModel
    {
        //User that request sent to 
        public long UserId { get; set; }
        //User that request sent to
        public string FullName { get; set; }
        //Status of request
        public RequestStatus RequestStatusNumber { get; set; }
        
    }
}