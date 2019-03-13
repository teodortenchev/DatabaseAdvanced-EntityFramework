﻿namespace P01_HospitalDatabase.Data
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using P01_HospitalDatabase.Data.Models;

    public class HospitalContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }

        public DbSet<Diagnose> Diagnoses { get; set; }

        public DbSet<Medicament> Medicaments { get; set; }

        public DbSet<Visitation> Visitations { get; set; }

        public DbSet<PatientMedicament> PatientsMedicaments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigurePatientEntity(modelBuilder);

            ConfigureDoctorEntity(modelBuilder);

            ConfigureVisitationEntity(modelBuilder);

            ConfigureDiagnoseEntity(modelBuilder);

            ConfigureMedicamentEntity(modelBuilder);

            ConfigurePatientMedicamentEntity(modelBuilder);

        }

        private void ConfigureDoctorEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(d => d.DoctorId);

                entity.Property(d => d.Name).IsRequired().HasMaxLength(100).IsUnicode();

                entity.Property(d => d.Specialty).IsRequired().HasMaxLength(100).IsUnicode();

                entity.HasMany(d => d.Visitations).WithOne(v => v.Doctor);


            });
        }

        private void ConfigurePatientMedicamentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatientMedicament>(entity =>
            {
                entity.HasKey(pm => new { pm.MedicamentId, pm.PatientId });

                entity.HasOne(pm => pm.Patient)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(pm => pm.PatientId);

                entity.HasOne(pm => pm.Medicament)
                    .WithMany(m => m.Prescriptions)
                    .HasForeignKey(pm => pm.MedicamentId);

            });
        }

        private void ConfigureMedicamentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Medicament>(entity =>
            {
                entity.HasKey(m => m.MedicamentId);

                entity.Property(m => m.Name).HasMaxLength(50).IsUnicode();
            });
        }

        private void ConfigureDiagnoseEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Diagnose>(entity =>
            {
                entity.HasKey(d => d.DiagnoseId);

                entity.Property(d => d.Name).HasMaxLength(50).IsUnicode().IsRequired();

                entity.Property(d => d.Comments).HasMaxLength(250).IsUnicode().IsRequired();
            });
        }

        private void ConfigureVisitationEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Visitation>(entity =>
            {
                entity.HasKey(v => v.VisitationId);

                entity.Property(v => v.Comments)
                    .HasMaxLength(250)
                    .IsUnicode();
            });
        }

        private void ConfigurePatientEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(p => p.PatientId);

                entity.HasMany(p => p.Visitations).WithOne(v => v.Patient);

                entity.HasMany(p => p.Diagnoses).WithOne(d => d.Patient);



                entity.Property(p => p.FirstName)
                    .HasMaxLength(50)
                    .IsRequired()
                    .IsUnicode();

                entity.Property(p => p.LastName)
                    .HasMaxLength(50)
                    .IsRequired()
                    .IsUnicode();

                entity.Property(p => p.Address)
                    .HasMaxLength(250)
                    .IsUnicode();

                entity.Property(p => p.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(p => p.HasInsurance)
                    .IsRequired();





            });

        }

    }


}
