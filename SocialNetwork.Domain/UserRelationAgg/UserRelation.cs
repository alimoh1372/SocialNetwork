using _00_Framework.Domain;
using SocialNetwork.Domain.UserAgg;

namespace SocialNetwork.Domain.UserRelationAgg
{
    /// <summary>
    /// The Entity and valueObject to handle the Relation of users between
    /// </summary>
    public class UserRelation:EntityBase
    {
        public long FkUserAId { get; private set; }
        public User UserA { get; private set; }

        public long FkUserBId { get; private set; }
        public User UserB { get; private set; }


        public bool Approve { get; private set; }

        #region UserMethods
        /// <summary>
        /// Make the Request of relation from User A To User B
        /// </summary>
        /// <param name="fkUserAId">Id of user A</param>
        /// <param name="fkUserBId">Id of user B</param>
        public UserRelation(long fkUserAId, long fkUserBId)
        {
            if (fkUserAId == fkUserBId)
                return;
            FkUserAId = fkUserAId;
            FkUserBId = fkUserBId;
            Approve = false;
        }

        /// <summary>
        /// Accepting The relation by User B
        /// </summary>
        public void AcceptRelation()
        {
            Approve = true;
        }

        /// <summary>
        /// Decline Relation By user B
        /// </summary>
        public void DeclineRelation()
        {
            Approve = false;
        }


        #endregion


    }
}