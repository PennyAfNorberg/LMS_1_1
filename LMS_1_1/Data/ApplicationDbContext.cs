using LMS_1_1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

    }
}
