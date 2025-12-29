using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace server.Models;

public partial class AspContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public AspContext()
    {
    }

    public AspContext(DbContextOptions<AspContext> options)
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

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceRegistration> ServiceRegistrations { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    //public virtual DbSet<User> Users { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);
    //    modelBuilder.Entity<Appointment>(entity =>
    //    {
    //        entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA22A85BE3D");

    //        entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
    //        entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
    //        entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
    //        entity.Property(e => e.PatientId).HasColumnName("PatientID");
    //        entity.Property(e => e.Status).HasMaxLength(50);

    //        entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
    //            .HasForeignKey(d => d.DoctorId)
    //            .HasConstraintName("FK__Appointme__Docto__00200768");

    //        entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
    //            .HasForeignKey(d => d.PatientId)
    //            .HasConstraintName("FK__Appointme__Patie__01142BA1");
    //    });

    //    modelBuilder.Entity<AspNetRole>(entity =>
    //    {
    //        entity.Property(e => e.Name).HasMaxLength(256);
    //        entity.Property(e => e.NormalizedName).HasMaxLength(256);
    //    });

        //modelBuilder.Entity<AspNetUser>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC075D10B0FF");

        //    entity.Property(e => e.Email).HasMaxLength(256);
        //    entity.Property(e => e.LockoutEnabled).HasDefaultValue(true);
        //    entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
        //    entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
        //    entity.Property(e => e.UserName).HasMaxLength(256);

            //entity.HasMany(d => d.Roles).WithMany(p => p.Users)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "AspNetUserRole",
            //        r => r.HasOne<AspNetRole>().WithMany()
            //            .HasForeignKey("RoleId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK__AspNetUse__RoleI__04E4BC85"),
            //        l => l.HasOne<AspNetUser>().WithMany()
            //            .HasForeignKey("UserId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK__AspNetUse__UserI__03F0984C"),
            //        j =>
            //        {
            //            j.HasKey("UserId", "RoleId").HasName("PK__AspNetUs__AF2760AD3C07756E");
            //            j.ToTable("AspNetUserRoles");
            //            j.IndexerProperty<int>("RoleId").ValueGeneratedOnAdd();
            //        });
        //});

        //modelBuilder.Entity<AspNetUserClaim>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC0799A36B8F");

        //    entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims)
        //        .HasForeignKey(d => d.UserId)
        //        .OnDelete(DeleteBehavior.ClientSetNull)
        //        .HasConstraintName("FK__AspNetUse__UserI__02084FDA");
        //});

        //modelBuilder.Entity<AspNetUserLogin>(entity =>
        //{
        //    //entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("PK__AspNetUs__2B2C5B52BF01F412");
        //    //entity.HasKey(e => new { e.Id }).HasName("PK_")
        //    entity.Property(e => e.Id)
        //        .ValueGeneratedOnAdd()
        //        .HasColumnName("ID");

        //    entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins)
        //        .HasForeignKey(d => d.UserId)
        //        .OnDelete(DeleteBehavior.ClientSetNull)
        //        .HasConstraintName("FK__AspNetUse__UserI__02FC7413");
        //});

        //modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("Hidden_AspNetUserLogins");

        //modelBuilder.Entity<AspNetUserToken>(entity =>
        //{
        //    entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("PK__AspNetUs__8CC49841DDD2437B");

        //    entity.Property(e => e.UserId).ValueGeneratedOnAdd();

        //    entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens)
        //        .HasForeignKey(d => d.UserId)
        //        .OnDelete(DeleteBehavior.ClientSetNull)
        //        .HasConstraintName("FK__AspNetUse__UserI__05D8E0BE");
        //});

        //modelBuilder.Entity<Doctor>(entity =>
        //{
        //    entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__2DC00EDFCF4B9F3D");

        //    entity.HasIndex(e => e.UserId, "UQ__Doctors__1788CCAD24F21097").IsUnique();

        //    entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
        //    entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
        //    entity.Property(e => e.UserId).HasColumnName("UserID");

        //    entity.HasOne(d => d.Specialty).WithMany(p => p.Doctors)
        //        .HasForeignKey(d => d.SpecialtyId)
        //        .HasConstraintName("FK__Doctors__Special__06CD04F7");

        //    entity.HasOne(d => d.User).WithOne(p => p.Doctor)
        //        .HasForeignKey<Doctor>(d => d.UserId)
        //        .HasConstraintName("FK_Doctors_Users");

        //    entity.HasOne(d => d.UserNavigation).WithOne(p => p.Doctor)
        //        .HasForeignKey<Doctor>(d => d.UserId)
        //        .HasConstraintName("FK__Doctors__UserID__07C12930");
        //});

        //modelBuilder.Entity<MedicalRecord>(entity =>
        //{
        //    entity.HasKey(e => e.RecordId).HasName("PK__MedicalR__FBDF78C92C25C1ED");

        //    entity.HasIndex(e => e.AppointmentId, "UQ__MedicalR__8ECDFCA3DD997F62").IsUnique();

        //    entity.Property(e => e.RecordId).HasColumnName("RecordID");
        //    entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
        //    entity.Property(e => e.Diagnosis).HasMaxLength(255);
        //    entity.Property(e => e.Treatment).HasMaxLength(255);

        //    entity.HasOne(d => d.Appointment).WithOne(p => p.MedicalRecord)
        //        .HasForeignKey<MedicalRecord>(d => d.AppointmentId)
        //        .HasConstraintName("FK__MedicalRe__Appoi__08B54D69");
        //});

        //modelBuilder.Entity<Patient>(entity =>
        //{
        //    entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC34623F2B92A");

        //    entity.HasIndex(e => e.UserId, "UQ__Patients__1788CCAD363AD56A").IsUnique();

        //    entity.Property(e => e.PatientId).HasColumnName("PatientID");
        //    entity.Property(e => e.Address).HasMaxLength(255);
        //    entity.Property(e => e.UserId).HasColumnName("UserID");

        //    entity.HasOne(d => d.User).WithOne(p => p.Patient)
        //        .HasForeignKey<Patient>(d => d.UserId)
        //        .HasConstraintName("FK_Patients_Users");

        //    entity.HasOne(d => d.UserNavigation).WithOne(p => p.Patient)
        //        .HasForeignKey<Patient>(d => d.UserId)
        //        .HasConstraintName("FK__Patients__UserID__09A971A2");
        //});

        //modelBuilder.Entity<Payment>(entity =>
        //{
        //    entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A587634FB42");

        //    entity.HasIndex(e => e.AppointmentId, "UQ__Payments__8ECDFCA378929CBE").IsUnique();

        //    entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
        //    entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
        //    entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
        //    entity.Property(e => e.PaymentDate)
        //        .HasDefaultValueSql("(getdate())")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.Status).HasMaxLength(50);

        //    entity.HasOne(d => d.Appointment).WithOne(p => p.Payment)
        //        .HasForeignKey<Payment>(d => d.AppointmentId)
        //        .HasConstraintName("FK__Payments__Appoin__0A9D95DB");
        //});

        //modelBuilder.Entity<Prescription>(entity =>
        //{
        //    entity.HasKey(e => e.PrescriptionId).HasName("PK__Prescrip__40130812F074A970");

        //    entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");
        //    entity.Property(e => e.Dosage).HasMaxLength(255);
        //    entity.Property(e => e.Medicine).HasMaxLength(255);
        //    entity.Property(e => e.RecordId).HasColumnName("RecordID");

        //    entity.HasOne(d => d.Record).WithMany(p => p.Prescriptions)
        //        .HasForeignKey(d => d.RecordId)
        //        .HasConstraintName("FK__Prescript__Recor__0B91BA14");
        //});

        //modelBuilder.Entity<Service>(entity =>
        //{
        //    entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EACCFAECD5");

        //    entity.HasIndex(e => e.ServiceName, "UQ__Services__A42B5F99D60CFF5D").IsUnique();

        //    entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
        //    entity.Property(e => e.CreatedAt)
        //        .HasDefaultValueSql("(getdate())")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.Description).HasMaxLength(500);
        //    entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        //    entity.Property(e => e.ServiceName).HasMaxLength(255);
        //});

        //modelBuilder.Entity<ServiceRegistration>(entity =>
        //{
        //    entity.HasKey(e => e.RegistrationId).HasName("PK__ServiceR__6EF588301FBF0832");

        //    entity.Property(e => e.RegistrationId).HasColumnName("RegistrationID");
        //    entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
        //    entity.Property(e => e.PatientId).HasColumnName("PatientID");
        //    entity.Property(e => e.RegistrationDate)
        //        .HasDefaultValueSql("(getdate())")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
        //    entity.Property(e => e.Status).HasMaxLength(50);

        //    entity.HasOne(d => d.Appointment).WithMany(p => p.ServiceRegistrations)
        //        .HasForeignKey(d => d.AppointmentId)
        //        .HasConstraintName("FK__ServiceRe__Appoi__0C85DE4D");

        //    entity.HasOne(d => d.Patient).WithMany(p => p.ServiceRegistrations)
        //        .HasForeignKey(d => d.PatientId)
        //        .HasConstraintName("FK__ServiceRe__Patie__0D7A0286");

        //    entity.HasOne(d => d.Service).WithMany(p => p.ServiceRegistrations)
        //        .HasForeignKey(d => d.ServiceId)
        //        .HasConstraintName("FK__ServiceRe__Servi__0E6E26BF");
        //});

        //modelBuilder.Entity<Specialty>(entity =>
        //{
        //    entity.HasKey(e => e.SpecialtyId).HasName("PK__Specialt__D768F6485E7222BB");

        //    entity.HasIndex(e => e.Name, "UQ__Specialt__737584F60C29FDFF").IsUnique();

        //    entity.Property(e => e.SpecialtyId).HasColumnName("SpecialtyID");
        //    entity.Property(e => e.Description).HasMaxLength(500);
        //    entity.Property(e => e.Name).HasMaxLength(100);

        //    entity.HasMany(d => d.Services).WithMany(p => p.Specialties)
        //        .UsingEntity<Dictionary<string, object>>(
        //            "SpecialtyService",
        //            r => r.HasOne<Service>().WithMany()
        //                .HasForeignKey("ServiceId")
        //                .OnDelete(DeleteBehavior.ClientSetNull)
        //                .HasConstraintName("FK__Specialty__Servi__0F624AF8"),
        //            l => l.HasOne<Specialty>().WithMany()
        //                .HasForeignKey("SpecialtyId")
        //                .OnDelete(DeleteBehavior.ClientSetNull)
        //                .HasConstraintName("FK__Specialty__Speci__10566F31"),
        //            j =>
        //            {
        //                j.HasKey("SpecialtyId", "ServiceId").HasName("PK__Specialt__6B394DA887D63A4A");
        //                j.ToTable("SpecialtyService");
        //            });
        //});

        //modelBuilder.Entity<User>(entity =>
        //{
        //    entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACB84FD43E");

        //    entity.HasIndex(e => e.Email, "UQ__Users__A9D105347C7696AE").IsUnique();

        //    entity.Property(e => e.UserId).HasColumnName("UserID");
        //    entity.Property(e => e.CreatedAt)
        //        .HasDefaultValueSql("(getdate())")
        //        .HasColumnType("datetime");
        //    entity.Property(e => e.Email).HasMaxLength(100);
        //    entity.Property(e => e.FullName).HasMaxLength(100);
        //    entity.Property(e => e.PasswordHash).HasMaxLength(255);
        //    entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        //    entity.Property(e => e.Role).HasMaxLength(50);
        //});

        //OnModelCreatingPartial(modelBuilder);
    //}

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
