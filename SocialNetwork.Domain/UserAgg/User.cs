using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
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

        public void Edit(string name, string lastName, string aboutMe, string profilePicture)
        {
            Name = name;
            LastName = lastName;
            AboutMe = aboutMe;
            ProfilePicture = profilePicture;
        }

        public void ChangePassword(string password)
        {
            Password = password;
        }


    }
}