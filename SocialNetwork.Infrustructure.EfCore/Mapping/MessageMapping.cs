﻿using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.Domain.MessageAgg;

namespace SocialNetwork.Infrastructure.EfCore.Mapping
{
    public class MessageMapping:IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");
            builder.HasKey(x => x.Id);
            //Design properties
            builder.Property(x => x.MessageContent).IsRequired();


            //Design self reference many to many 

            builder.HasOne(x => x.FromUser)
                .WithMany(x => x.FromMessages)
                .HasForeignKey(x => x.FkFromUserId);

            builder.HasOne(x => x.ToUser)
                .WithMany(x => x.ToMessages)
                .HasForeignKey(x => x.FkToUserId);


            //design index for user a and b
            builder.HasIndex(x => new {x.FkFromUserId, x.FkToUserId});




        }
    }
}