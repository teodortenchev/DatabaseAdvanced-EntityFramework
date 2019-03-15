namespace P01_StudentSystem.Data
{
    using Microsoft.EntityFrameworkCore;
    using P01_StudentSystem.Data.Models;
    using P01_StudentSystem.Data.Models.Enums;
    using System;

    internal static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData
            (
                new Student
                {
                    StudentId = 1,
                    Name = "Teodor Tenchev",
                    RegisteredOn = DateTime.Now
                },
                new Student
                {
                    StudentId = 2,
                    Name = "Galina Gouleva-Tenchev",
                    RegisteredOn = DateTime.Now
                }
           );
            modelBuilder.Entity<Course>().HasData
            (
                new Course
                {
                    CourseId = 1,
                    Name = "Archeology 101",
                    Description = "Good for newbies",
                    Price = 135m
                },
                new Course
                {
                    CourseId = 2,
                    Name = "Applied Mathematics",
                    Description = "Beware the maths monster!",
                    Price = 1350m
                }
            );
            modelBuilder.Entity<Resource>().HasData
            (
                new Resource
                {
                    ResourceId = 1,
                    CourseId = 1,
                    Name = "Course schedule",
                    ResourceType = ResourceType.Document
                },
                new Resource
                {
                    ResourceId = 2,
                    CourseId = 2,
                    Name = "Introduction to imaginary numbers",
                    ResourceType = ResourceType.Presentation
                }
            );
            modelBuilder.Entity<Homework>().HasData
            (
                new Homework
                {
                    HomeworkId = 1,
                    StudentId = 2,
                    CourseId = 1,
                    Content = "http://www.google.com/",
                    ContentType  = ContentType.Pdf,
                    SubmissionTime = DateTime.Now
                }
            );
            modelBuilder.Entity<StudentCourse>().HasData
           (
               new StudentCourse
               {
                   CourseId = 1,
                   StudentId = 2
               },
               new StudentCourse
               {
                   CourseId = 2,
                   StudentId = 1
               }
           );



        }
    }
}
