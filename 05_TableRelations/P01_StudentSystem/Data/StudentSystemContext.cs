namespace P01_StudentSystem.Data
{
    using Microsoft.EntityFrameworkCore;
    using P01_StudentSystem.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class StudentSystemContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Homework> HomeworkSubmissions { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureResources(modelBuilder);
            ConfigureStudents(modelBuilder);
            ConfigureCourse(modelBuilder);
            ConfigureHomeworkSubmissions(modelBuilder);
            ConfigureStudentCourses(modelBuilder);
        }

        private void ConfigureCourse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.CourseId);

                entity.HasMany(c => c.StudentsEnrolled).WithOne(se => se.Course);

                entity.HasMany(c => c.HomeworkSubmissions).WithOne(hw => hw.Course);

                entity.Property(c => c.Name).HasMaxLength(80).IsUnicode();

                entity.Property(c => c.Price).HasColumnType("MONEY");

            });
        }

        private void ConfigureHomeworkSubmissions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Homework>(entity =>
            {
                entity.HasKey(hw => hw.HomeworkId);

                entity.Property(hw => hw.Content).HasColumnType("VARCHAR(MAX)");
            });
        }

        private void ConfigureResources(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(r => r.ResourceId);

                entity.Property(r => r.Name)
                  .HasMaxLength(50)
                  .IsUnicode();
            });
        }

        private void ConfigureStudents(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentId);

                entity.HasMany(s => s.CourseEnrollments).WithOne(c => c.Student);

                entity.HasMany(s => s.HomeworkSubmissions).WithOne(h => h.Student);

                entity.Property(s => s.Name)
                    .HasMaxLength(100)
                    .IsUnicode();

                entity.Property(s => s.PhoneNumber)
                    .HasColumnType("CHAR(10)")
                    .IsUnicode(false)
                    .IsRequired(false);

            });
        }

        private void ConfigureStudentCourses(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });
        }

    }
}
