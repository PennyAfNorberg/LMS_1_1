using LMS_1_1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace LMS_1_1.Data
{
    public class ApplicationDbContext : IdentityDbContext<LMSUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CourseUser>()
                .HasKey(cu => new { cu.CourseId, cu.LMSUserId });


            modelBuilder.Entity<ActivityType>()
    .HasData(
        new ActivityType { Id = 1, Name = "E-Learningpass" },
        new ActivityType { Id = 2, Name = "Föreläsning" },
        new ActivityType { Id = 3, Name = "Övningstillfälle" },
        new ActivityType { Id = 4, Name = "Inlämingsuppgift" },
        new ActivityType { Id = 5, Name = "Annat" }


    );

            modelBuilder.Entity<DocumentType>()
    .HasData(
        new ActivityType { Id = 1, Name = "Course" },
        new ActivityType { Id = 2, Name = "Activity" },
        new ActivityType { Id = 3, Name = "Module" }
    );

            modelBuilder.Entity<CloneType>()
.HasData(
new ActivityType { Id = 1, Name = "Skip weekends" },
new ActivityType { Id = 2, Name = "don't skip weekends" }

);
            modelBuilder.Entity<ColorModule>()
            .HasData(
                 new ColorModule
                 {
                      Id= Guid.NewGuid(),
                     LMSUserId = null,
                     ModuleId = null,
                     Color = "#dbad95"
                 }
                );
            modelBuilder.Entity<ColorActivity>()
                .HasData(
                    new ColorActivity
                    {
                        Id = Guid.NewGuid(),
                        LMSUserId = null,
                        CourseId = null,
                        LMSActivityId = null,
                        AktivityTypeID = 1,
                        Color = "#587aad"
                    },
                    new ColorActivity
                    {
                        Id = Guid.NewGuid(),
                        LMSUserId = null,
                        CourseId = null,
                        LMSActivityId = null,
                        AktivityTypeID = 2,
                        Color = "#68c930"
                    },
                    new ColorActivity
                    {
                        Id = Guid.NewGuid(),
                        LMSUserId = null,
                        CourseId = null,
                        LMSActivityId = null,
                        AktivityTypeID = 3,
                        Color = "#c95e30"
                    },
                    new ColorActivity
                    {
                        Id = Guid.NewGuid(),
                        LMSUserId = null,
                        CourseId = null,
                        LMSActivityId = null,
                        AktivityTypeID = 4,
                        Color = "#f45004"
                    },
                    new ColorActivity
                    {
                        Id = Guid.NewGuid(),
                        LMSUserId = null,
                        CourseId = null,
                        LMSActivityId = null,
                        AktivityTypeID = 5,
                        Color = "#fcfaf9"
                    }
                );
            
            modelBuilder.Entity<CourseSettings>()
                .HasData(
                    new CourseSettings
                    {
                        Id = Guid.NewGuid(),
                        CourseId = null,
                        Date = null,
                        StartTime = "09:00:00", 
                        EndTime = "12:00:00"
                    },
                    new CourseSettings
                    {
                        Id = Guid.NewGuid(),
                        CourseId = null,
                        Date = null,
                        StartTime = "13:00:00",
                        EndTime = "17:00:00"
                    }
                );
       
        }


        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<LMSActivity> LMSActivity { get; set; }

        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<LMSUser> LMSUsers { get; set; }

        public DbSet<CourseUser> CourseUsers { get; set; }

        public DbSet<TokenUser> TokenUsers { get; set; }

        public DbSet<Document> Documents { get; set; }
        public DbSet<CloneType> CloneTypes { get; set; }

        public DbSet<DocumentType> DocumentTypes { get; set; }

        public DbSet<ColorModule> ColorModule { get; set; }

        public DbSet<ColorActivity> ColorActivity { get; set; }

        public DbSet<CourseSettings> CourseSettings { get; set; }

    }
}
