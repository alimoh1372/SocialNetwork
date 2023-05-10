﻿using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.Domain.UserAgg;

namespace SocialNetwork.Infrastructure.EfCore.Mapping
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(500);
            builder.Property(x => x.BirthDay).IsRequired();
            builder.Property(x => x.Password).IsRequired().HasMaxLength(30);
            builder.Property(x => x.AboutMe).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.ProfilePicture).IsRequired().HasMaxLength(2000);
            


            //Define a self referencing many to many with UserRelation entity

            builder.HasMany(x => x.UserARelations)
                .WithOne(x => x.UserA)
                .HasForeignKey(x => x.FkUserAId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.UserBRelations)
                .WithOne(x => x.UserB)
                .HasForeignKey(x => x.FkUserBId)
                .OnDelete(DeleteBehavior.Restrict);


            //Define an index to email 
            builder.HasIndex(x => x.Email).IsUnique();
        }
    }
}