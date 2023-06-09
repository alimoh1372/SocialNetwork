﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SocialNetwork.Infrastructure.EfCore;

namespace SocialNetwork.Infrastructure.EfCore.Migrations
{
    [DbContext(typeof(SocialNetworkContext))]
    [Migration("20230526142514_ChangeCreationDateToDateTimeOffset")]
    partial class ChangeCreationDateToDateTimeOffset
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SocialNetwork.Domain.MessageAgg.Message", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<long>("FkFromUserId")
                        .HasColumnType("bigint");

                    b.Property<long>("FkToUserId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<bool>("Like")
                        .HasColumnType("bit");

                    b.Property<string>("MessageContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FkToUserId");

                    b.HasIndex("FkFromUserId", "FkToUserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SocialNetwork.Domain.UserAgg.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AboutMe")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<DateTime>("BirthDay")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("ProfilePicture")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SocialNetwork.Domain.UserRelationAgg.UserRelation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Approve")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<long>("FkUserAId")
                        .HasColumnType("bigint");

                    b.Property<long>("FkUserBId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("FkUserBId");

                    b.HasIndex("FkUserAId", "FkUserBId")
                        .IsUnique();

                    b.ToTable("UserRelations");
                });

            modelBuilder.Entity("SocialNetwork.Domain.MessageAgg.Message", b =>
                {
                    b.HasOne("SocialNetwork.Domain.UserAgg.User", "FromUser")
                        .WithMany("FromMessages")
                        .HasForeignKey("FkFromUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SocialNetwork.Domain.UserAgg.User", "ToUser")
                        .WithMany("ToMessages")
                        .HasForeignKey("FkToUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("FromUser");

                    b.Navigation("ToUser");
                });

            modelBuilder.Entity("SocialNetwork.Domain.UserRelationAgg.UserRelation", b =>
                {
                    b.HasOne("SocialNetwork.Domain.UserAgg.User", "UserA")
                        .WithMany("UserARelations")
                        .HasForeignKey("FkUserAId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SocialNetwork.Domain.UserAgg.User", "UserB")
                        .WithMany("UserBRelations")
                        .HasForeignKey("FkUserBId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("UserA");

                    b.Navigation("UserB");
                });

            modelBuilder.Entity("SocialNetwork.Domain.UserAgg.User", b =>
                {
                    b.Navigation("FromMessages");

                    b.Navigation("ToMessages");

                    b.Navigation("UserARelations");

                    b.Navigation("UserBRelations");
                });
#pragma warning restore 612, 618
        }
    }
}
