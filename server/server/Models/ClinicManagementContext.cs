using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace server.Models;

public partial class ClinicManagementContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public ClinicManagementContext()
    {
    }

    public ClinicManagementContext(DbContextOptions<ClinicManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    //public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    //public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    //public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    //public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    //public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }
    public virtual DbSet<MedicalRecordDetail> MedicalRecordDetails { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceRegistration> ServiceRegistrations { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<ContactMessages> ContactMessages { get; set;}
    public virtual DbSet<DoctorReviewDetail> DoctorReviewDetails { get; set; }
    public virtual DbSet<ServiceReviewDetail> ServiceReviewDetails { get; set; }

    // public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA2D8B95A6A");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.AppointmentTime).HasMaxLength(50);
            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Appointme__Docto__4E53A1AA");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__Appointme__Patie__4D5F7D71");

            entity.HasOne(d => d.Service).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_Appointments_Services");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__2DC00EDF3E6EE7C7");

            entity.HasIndex(e => e.UserId, "UQ__Doctors__1788CCADE5A60ECA").IsUnique();

            entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .HasConstraintName("FK__Doctors__Special__45BE5BA9");

            entity.HasOne(d => d.User).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Doctors_AspNetUsers");
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__MedicalR__FBDF78C9840459D5");

            entity.HasIndex(e => e.AppointmentId, "UQ__MedicalR__8ECDFCA3BB61DEC3").IsUnique();

            entity.Property(e => e.RecordId).HasColumnName("RecordID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.Diagnosis).HasMaxLength(255);
            entity.Property(e => e.Treatment).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Appointment).WithOne(p => p.MedicalRecord)
                .HasForeignKey<MedicalRecord>(d => d.AppointmentId)
                .HasConstraintName("FK__MedicalRe__Appoi__5224328E");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC34619C63D9E");

            entity.HasIndex(e => e.UserId, "UQ__Patients__1788CCAD13D1B002").IsUnique();

            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithOne(p => p.Patient)
                .HasForeignKey<Patient>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Patients_AspNetUsers");
        });

        modelBuilder.Entity<ContactMessages>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__3214EC0764207BF0 ");

            entity.Property(e => e.PatientId).HasColumnName("PatientId");
            entity.Property(e => e.Messages).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Patient)
                .WithOne(p => p.ContactMessages)
                .HasForeignKey<ContactMessages>(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Messages__Patien__5FD33367");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A58E41BFC65");

            entity.HasIndex(e => e.AppointmentId, "UQ__Payments__8ECDFCA3EE667EE2").IsUnique();

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Appointment).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.AppointmentId)
                .HasConstraintName("FK__Payments__Appoin__5AB9788F");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.PrescriptionId).HasName("PK__Prescrip__40130812D4F20419");

            entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");
            entity.Property(e => e.Dosage).HasMaxLength(255);
            entity.Property(e => e.Medicine).HasMaxLength(255);
            entity.Property(e => e.RecordId).HasColumnName("RecordID");

            entity.HasOne(d => d.Record).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.RecordId)
                .HasConstraintName("FK__Prescript__Recor__55009F39");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EA66A5E681");

            entity.HasIndex(e => e.ServiceName, "UQ__Services__A42B5F9946E0F3F3").IsUnique();

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("float");
            entity.Property(e => e.ServiceName).HasMaxLength(255);
        });

        modelBuilder.Entity<ServiceRegistration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__ServiceR__6EF58830115C6A91");

            entity.Property(e => e.RegistrationId).HasColumnName("RegistrationID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Appointment).WithMany(p => p.ServiceRegistrations)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("FK__ServiceRe__Appoi__65370702");

            entity.HasOne(d => d.Patient).WithMany(p => p.ServiceRegistrations)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK__ServiceRe__Patie__634EBE90");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceRegistrations)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__ServiceRe__Servi__6442E2C9");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.SpecialtyId).HasName("PK__Specialt__D768F648A8F2AA6D");

            entity.HasIndex(e => e.Name, "UQ__Specialt__737584F6A3F2D7BE").IsUnique();

            entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasMany(d => d.Services).WithMany(p => p.Specialties)
                .UsingEntity<Dictionary<string, object>>(
                    "SpecialtyService",
                    r => r.HasOne<Service>().WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Specialty__Servi__09746778"),
                    l => l.HasOne<Specialty>().WithMany()
                        .HasForeignKey("SpecialtyId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Specialty__Speci__0880433F"),
                    j =>
                    {
                        j.HasKey("SpecialtyId", "ServiceId").HasName("PK__Specialt__6B394DA853777237");
                        j.ToTable("SpecialtyService");
                    });
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");

            entity.HasKey(e => e.Id).HasName("PK_AspNetUsers");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.LockoutEnabled).HasDefaultValue(true);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);
                        entity.Property(e => e.FullName).HasMaxLength(256);
            entity.Property(e => e.PhoneNumber).HasMaxLength(256);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
        });

        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            modelBuilder.Entity<ApplicationRole>().ToTable("AspNetRoles");

            entity.HasKey(e => e.Id).HasName("PK_AspNetRoles");
            entity.Property(e => e.Id).HasColumnName("Id"); // Đặt đúng tên cột trong DB
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.MedicineId).HasName("PK__Medicine__4F2128F09B53519C");

            entity.Property(e => e.MedicineId).HasColumnName("MedicineID");
            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("float");
        });

        modelBuilder.Entity<MedicalRecordDetail>(entity =>
        {
            entity.HasKey(e => new { e.ReCordId, e.MedicineId });

            entity.ToTable("MedicalRecordDetail");

            entity.Property(e => e.MedicineId).HasColumnName("MedicineID");
            entity.Property(e => e.ReCordId).HasColumnName("ReCordID");

            entity.HasOne(d => d.Medicine).WithMany()
                .HasForeignKey(d => d.MedicineId)
                .HasConstraintName("FK__MedicalRe__Medic__07220AB2");

            entity.HasOne(d => d.ReCord).WithMany(p => p.MedicalRecordDetails)
                .HasForeignKey(d => d.ReCordId)
                .HasConstraintName("FK__MedicalRe__ReCor__08162EEB");
        });

        modelBuilder.Entity<Review>()
            .HasOne(r => r.DoctorReviewDetail)
            .WithOne(d => d.Review)
            .HasForeignKey<DoctorReviewDetail>(d => d.ReviewId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.ServiceReviewDetail)
            .WithOne(s => s.Review)
            .HasForeignKey<ServiceReviewDetail>(s => s.ReviewId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.MedicalRecord)
            .WithOne(m => m.Review)
            .HasForeignKey<Review>(r => r.PrescriptionId);
            
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}