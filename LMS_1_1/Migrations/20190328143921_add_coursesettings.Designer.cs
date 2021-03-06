﻿// <auto-generated />
using System;
using LMS_1_1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LMS_1_1.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190328143921_add_coursesettings")]
    partial class add_coursesettings
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LMS_1_1.Models.ActivityType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ActivityTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "E-Learningpass"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Föreläsning"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Övningstillfälle"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Inlämingsuppgift"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Annat"
                        });
                });

            modelBuilder.Entity("LMS_1_1.Models.CloneType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("CloneTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Skip weekends"
                        },
                        new
                        {
                            Id = 2,
                            Name = "don't skip weekends"
                        });
                });

            modelBuilder.Entity("LMS_1_1.Models.ColorActivity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AktivityTypeID");

                    b.Property<string>("Color")
                        .IsRequired();

                    b.Property<Guid?>("CourseId");

                    b.Property<Guid?>("LMSActivityId");

                    b.Property<string>("LMSUserId");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("LMSActivityId");

                    b.HasIndex("LMSUserId");

                    b.ToTable("ColorActivity");

                    b.HasData(
                        new
                        {
                            Id = new Guid("cdd4f0c6-d395-4194-b2f8-e69b8a01c57e"),
                            AktivityTypeID = 1,
                            Color = "#587aad"
                        },
                        new
                        {
                            Id = new Guid("724a0244-c183-446e-8a96-60ca91d8aac3"),
                            AktivityTypeID = 2,
                            Color = "#68c930"
                        },
                        new
                        {
                            Id = new Guid("e050874b-27af-4870-8c2b-04e989081a5e"),
                            AktivityTypeID = 3,
                            Color = "#c95e30"
                        },
                        new
                        {
                            Id = new Guid("e61ff4dd-7ac6-4e86-a1e8-fd84d09f46ac"),
                            AktivityTypeID = 4,
                            Color = "#f45004"
                        },
                        new
                        {
                            Id = new Guid("3583af22-9b65-4420-865c-fa2ac9cd724b"),
                            AktivityTypeID = 5,
                            Color = "#fcfaf9"
                        });
                });

            modelBuilder.Entity("LMS_1_1.Models.ColorModule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Color")
                        .IsRequired();

                    b.Property<string>("LMSUserId");

                    b.Property<Guid?>("ModuleId");

                    b.HasKey("Id");

                    b.HasIndex("LMSUserId");

                    b.HasIndex("ModuleId");

                    b.ToTable("ColorModule");

                    b.HasData(
                        new
                        {
                            Id = new Guid("8b8f0562-90d2-498d-b9b5-796f5486d5ba"),
                            Color = "#dbad95"
                        });
                });

            modelBuilder.Entity("LMS_1_1.Models.Course", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CourseImgPath");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("LMS_1_1.Models.CourseSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("CourseId");

                    b.Property<DateTime?>("Date");

                    b.Property<string>("EndLunch");

                    b.Property<string>("EndTime");

                    b.Property<string>("StartLunch");

                    b.Property<string>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("CourseSettings");

                    b.HasData(
                        new
                        {
                            Id = new Guid("3c2db520-45da-4775-827e-7bb03fa17aaf"),
                            EndLunch = "13:00:00",
                            EndTime = "17:00:00",
                            StartLunch = "12:00:00",
                            StartTime = "09:00:00"
                        });
                });

            modelBuilder.Entity("LMS_1_1.Models.CourseUser", b =>
                {
                    b.Property<Guid>("CourseId");

                    b.Property<string>("LMSUserId");

                    b.HasKey("CourseId", "LMSUserId");

                    b.HasIndex("LMSUserId");

                    b.ToTable("CourseUsers");
                });

            modelBuilder.Entity("LMS_1_1.Models.Document", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("CourseId");

                    b.Property<string>("Description");

                    b.Property<int>("DocumentTypeId");

                    b.Property<Guid?>("LMSActivityId");

                    b.Property<string>("LMSUserId");

                    b.Property<Guid?>("ModuleId");

                    b.Property<string>("Name");

                    b.Property<string>("Path");

                    b.Property<DateTime>("UploadDate");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("DocumentTypeId");

                    b.HasIndex("LMSActivityId");

                    b.HasIndex("LMSUserId");

                    b.HasIndex("ModuleId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("LMS_1_1.Models.DocumentType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("DocumentTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Course"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Activity"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Module"
                        });
                });

            modelBuilder.Entity("LMS_1_1.Models.LMSActivity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActivityTypeId");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<DateTime>("EndDate");

                    b.Property<Guid>("ModuleId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("ActivityTypeId");

                    b.HasIndex("ModuleId");

                    b.ToTable("LMSActivity");
                });

            modelBuilder.Entity("LMS_1_1.Models.LMSUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("LMS_1_1.Models.Module", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CourseId");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("Modules");
                });

            modelBuilder.Entity("LMS_1_1.Models.TokenUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("LMSUserId");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("LMSUserId");

                    b.ToTable("TokenUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("LMS_1_1.Models.ColorActivity", b =>
                {
                    b.HasOne("LMS_1_1.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId");

                    b.HasOne("LMS_1_1.Models.LMSActivity", "LMSActivity")
                        .WithMany()
                        .HasForeignKey("LMSActivityId");

                    b.HasOne("LMS_1_1.Models.LMSUser", "LMSUser")
                        .WithMany()
                        .HasForeignKey("LMSUserId");
                });

            modelBuilder.Entity("LMS_1_1.Models.ColorModule", b =>
                {
                    b.HasOne("LMS_1_1.Models.LMSUser", "LMSUser")
                        .WithMany()
                        .HasForeignKey("LMSUserId");

                    b.HasOne("LMS_1_1.Models.Module", "Module")
                        .WithMany()
                        .HasForeignKey("ModuleId");
                });

            modelBuilder.Entity("LMS_1_1.Models.CourseSettings", b =>
                {
                    b.HasOne("LMS_1_1.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId");
                });

            modelBuilder.Entity("LMS_1_1.Models.CourseUser", b =>
                {
                    b.HasOne("LMS_1_1.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS_1_1.Models.LMSUser", "LMSUser")
                        .WithMany("CourseUser")
                        .HasForeignKey("LMSUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS_1_1.Models.Document", b =>
                {
                    b.HasOne("LMS_1_1.Models.Course", "Courses")
                        .WithMany("Documents")
                        .HasForeignKey("CourseId");

                    b.HasOne("LMS_1_1.Models.DocumentType", "DocumentType")
                        .WithMany()
                        .HasForeignKey("DocumentTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS_1_1.Models.LMSActivity", "LMSActivity")
                        .WithMany("Documents")
                        .HasForeignKey("LMSActivityId");

                    b.HasOne("LMS_1_1.Models.LMSUser", "LMSUser")
                        .WithMany()
                        .HasForeignKey("LMSUserId");

                    b.HasOne("LMS_1_1.Models.Module", "Module")
                        .WithMany("Documents")
                        .HasForeignKey("ModuleId");
                });

            modelBuilder.Entity("LMS_1_1.Models.LMSActivity", b =>
                {
                    b.HasOne("LMS_1_1.Models.ActivityType", "ActivityType")
                        .WithMany()
                        .HasForeignKey("ActivityTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS_1_1.Models.Module", "Modules")
                        .WithMany("LMSActivities")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS_1_1.Models.Module", b =>
                {
                    b.HasOne("LMS_1_1.Models.Course", "Courses")
                        .WithMany("Modules")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LMS_1_1.Models.TokenUser", b =>
                {
                    b.HasOne("LMS_1_1.Models.LMSUser", "LMSUser")
                        .WithMany()
                        .HasForeignKey("LMSUserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("LMS_1_1.Models.LMSUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("LMS_1_1.Models.LMSUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LMS_1_1.Models.LMSUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("LMS_1_1.Models.LMSUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
