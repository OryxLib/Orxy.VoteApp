﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Oryx.VoteApp.Server.Models;
using System;

namespace Oryx.VoteApp.Server.Migrations
{
    [DbContext(typeof(VoteAppDbContext))]
    [Migration("20180527234128_bossVoteBossId")]
    partial class bossVoteBossId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Oryx.VoteApp.Server.Models.AdminUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateTime");

                    b.Property<string>("Passworkd");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("AdminUser");
                });

            modelBuilder.Entity("Oryx.VoteApp.Server.Models.ExcelUserInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("ExcelUserInfo");
                });

            modelBuilder.Entity("Oryx.VoteApp.Server.Models.UserInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Password");

                    b.Property<string>("UserName");

                    b.Property<string>("WxAvatarUrl");

                    b.Property<string>("WxCity");

                    b.Property<string>("WxCountry");

                    b.Property<string>("WxNickName");

                    b.Property<string>("WxOpenId");

                    b.HasKey("Id");

                    b.ToTable("UserInfo");
                });

            modelBuilder.Entity("Oryx.VoteApp.Server.Models.VoteBuff", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BossId");

                    b.Property<int>("VoteId");

                    b.Property<string>("VoteOption");

                    b.HasKey("Id");

                    b.ToTable("VoteBuff");
                });

            modelBuilder.Entity("Oryx.VoteApp.Server.Models.VoteInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CloseSeconds");

                    b.Property<DateTime>("CreateTime");

                    b.Property<double>("EnableStartTime");

                    b.Property<DateTime>("EnableTime");

                    b.Property<bool>("Expired");

                    b.Property<bool>("IsEnable");

                    b.Property<bool>("IsOpen");

                    b.Property<string>("Name");

                    b.Property<bool>("ShouldLogin");

                    b.Property<string>("VoteType");

                    b.HasKey("Id");

                    b.ToTable("VoteInfo");
                });

            modelBuilder.Entity("Oryx.VoteApp.Server.Models.VoteLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateTime");

                    b.Property<string>("Latitude");

                    b.Property<string>("Longtitude");

                    b.Property<string>("UserId");

                    b.Property<string>("UserKey");

                    b.Property<int>("VoteId");

                    b.Property<string>("VoteOption");

                    b.HasKey("Id");

                    b.ToTable("VoteLog");
                });

            modelBuilder.Entity("Oryx.VoteApp.Server.Models.VoteOptions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Key");

                    b.Property<string>("Type");

                    b.Property<string>("Value");

                    b.Property<int?>("VoteInfoId");

                    b.HasKey("Id");

                    b.HasIndex("VoteInfoId");

                    b.ToTable("VoteOption");
                });

            modelBuilder.Entity("Oryx.VoteApp.Server.Models.WxqLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Avarta");

                    b.Property<string>("Msg");

                    b.Property<int>("MsgStatus");

                    b.Property<string>("MsgType");

                    b.Property<string>("NickName");

                    b.HasKey("Id");

                    b.ToTable("WxqLog");
                });

            modelBuilder.Entity("Oryx.VoteApp.Server.Models.VoteOptions", b =>
                {
                    b.HasOne("Oryx.VoteApp.Server.Models.VoteInfo", "VoteInfo")
                        .WithMany("VoteOptions")
                        .HasForeignKey("VoteInfoId");
                });
#pragma warning restore 612, 618
        }
    }
}
