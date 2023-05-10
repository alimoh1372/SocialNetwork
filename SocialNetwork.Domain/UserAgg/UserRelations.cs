using _00_Framework.Domain;

namespace SocialNetwork.Domain.UserAgg
{
    public class UserRelation:EntityBase
    {
        public long FkUserAId { get; private set; }
        public User UserA { get; private set; }

        public long FkUserBId { get; private set; }
        public User UserB { get; private set; }


        public bool Approve { get; private set; }

        #region UserMethods
        /// <summary>
        /// Make the relation between User A and User B
        /// </summary>
        /// <param name="fkUserAId">Id of user A</param>
        /// <param name="fkUserBId">Id of user B</param>
        public UserRelation(long fkUserAId, long fkUserBId)
        {
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