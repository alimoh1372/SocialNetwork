using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using _00_Framework.Domain;
using SocialNetwork.Domain.UserRelationAgg;

namespace SocialNetwork.Domain.UserAgg
{
    /// <summary>
    /// User Entity to handling the user operations
    /// </summary>
    public class User:EntityBase
    {
        public string Name { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }

        public DateTime BirthDay { get; private set; }
        public string Password { get; private set; }
        public string AboutMe { get; private set; }
        public string ProfilePicture { get; private set; }
        public ICollection<UserRelation> UserRelations { get; private set; }

        #region UserMethods
        /// <summary>
        /// Define A User
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="birthDay"></param>
        /// <param name="password"></param>
        /// <param name="aboutMe">A description About User</param>
        /// <param name="profilePicture"></param>
        public User(string name, string lastName, string email, DateTime birthDay, string password, string aboutMe, string profilePicture)
        {
            Name = name;
            LastName = lastName;
            Email = email;
            BirthDay = birthDay;
            Password = password;
            AboutMe = aboutMe;
            ProfilePicture = profilePicture;


        }
        /// <summary>
        /// Edit User properties that editable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lastName"></param>
        /// <param name="aboutMe"></param>
        /// <param name="profilePicture"></param>
        public void Edit(string name, string lastName, string aboutMe, string profilePicture)
        {
            Name = name;
            LastName = lastName;
            AboutMe = aboutMe;
            ProfilePicture = profilePicture;
        }
        /// <summary>
        /// To change the user password
        /// </summary>
        /// <param name="password"></param>
        public void ChangePassword(string password)
        {
            Password = password;
        }



        #endregion

        #region UserRelationMethods
        /// <summary>
        /// To request a friendship from this user to the Last Users
        /// </summary>
        /// <param name="fkUserBId"></param>
        public void RequestFriendShip(long fkUserBId)
        {
            //ToDo:The check the validating things

            //check if this Request is exist before
            
            UserRelation relation = new UserRelation(this.Id, fkUserBId);

            //add request to the relations list
            UserRelations.Add(relation);

        }

        public void AcceptFriendShip(long requestFromUserId)
        {
            //Get the request friend ship with userA= the user that request Friendship(User A)
            UserRelation relation =
                UserRelations.FirstOrDefault(x => x.FkUserAId == requestFromUserId && x.FkUserBId == this.Id);
            //if there is that relation request Accept it
            if(relation != null)
                relation.AcceptRelation();

        }


        public void DeclineFriendShip(long requestFromUserId)
        {
            //Get the request friend ship with userA= the user that request Friendship(User A)
            UserRelation relation =
                UserRelations.FirstOrDefault(x => x.FkUserAId == requestFromUserId && x.FkUserBId == this.Id);
            //if there is that relation request Accept it
            if (relation != null)
                relation.DeclineRelation();

        }


        #endregion


    }
}