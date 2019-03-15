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
        }

        private void ConfigureCourse(ModelBuilder modelBuilder)
        {

        }

        private void ConfigureHomeworkSubmissions(ModelBuilder modelBuilder)
        {

        }

        private void ConfigureResources(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(r => r.ResourceId);
            });
        }

        private void ConfigureStudents(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentId);

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

        }

    }
}
