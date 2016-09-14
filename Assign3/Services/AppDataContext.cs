using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Assign3.Services.Entities;

namespace Assign3.Services
{
    public class AppDataContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseTemplate> CoursesTemplate { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentLinker> StudentLinker { get; set; }
        public DbSet<WaitingList> WaitingList { get; set; }

        public AppDataContext(DbContextOptions<AppDataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}

