using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using server.Models;

#nullable disable

namespace server.Migrations
{
    [DbContext(typeof(AspContext))]
    partial class ClinicManagementContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("server.Models.Appointment", b =>
                {
                    b.Property<int>("AppointmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("AppointmentID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AppointmentId"));

                    b.Property<DateTime?>("AppointmentDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("DoctorId")
                        .HasColumnType("int")
                        .HasColumnName("DoctorID");

                    b.Property<int?>("PatientId")
                        .HasColumnType("int")
                        .HasColumnName("PatientID");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("AppointmentId")
                        .HasName("PK__Appointm__8ECDFCA2D8B95A6A");

                    b.HasIndex("DoctorId");

                    b.HasIndex("PatientId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("server.Models.Doctor", b =>
                {
                    b.Property<int>("DoctorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("DoctorID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DoctorId"));

                    b.Property<byte[]>("DoctorImage")
                        .HasColumnType("varbinary(max)");

                    b.Property<int?>("ExperienceYears")
                        .HasColumnType("int");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SpecialtyId")
                        .HasColumnType("int")
                        .HasColumnName("SpecialtyID");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("DoctorId")
                        .HasName("PK__Doctors__2DC00EDF3E6EE7C7");

                    b.HasIndex("SpecialtyId");

                    b.HasIndex(new[] { "UserId" }, "UQ__Doctors__1788CCADE5A60ECA")
                        .IsUnique()
                        .HasFilter("[UserID] IS NOT NULL");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("server.Models.MedicalRecord", b =>
                {
                    b.Property<int>("RecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RecordID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RecordId"));

                    b.Property<int?>("AppointmentId")
                        .HasColumnType("int")
                        .HasColumnName("AppointmentID");

                    b.Property<string>("Diagnosis")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Treatment")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("RecordId")
                        .HasName("PK__MedicalR__FBDF78C9840459D5");

                    b.HasIndex(new[] { "AppointmentId" }, "UQ__MedicalR__8ECDFCA3BB61DEC3")
                        .IsUnique()
                        .HasFilter("[AppointmentID] IS NOT NULL");

                    b.ToTable("MedicalRecords");
                });

            modelBuilder.Entity("server.Models.Patient", b =>
                {
                    b.Property<int>("PatientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PatientID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PatientId"));

                    b.Property<string>("Address")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateOnly?>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("PatientId")
                        .HasName("PK__Patients__970EC34619C63D9E");

                    b.HasIndex(new[] { "UserId" }, "UQ__Patients__1788CCAD13D1B002")
                        .IsUnique()
                        .HasFilter("[UserID] IS NOT NULL");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("server.Models.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PaymentID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentId"));

                    b.Property<decimal?>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int?>("AppointmentId")
                        .HasColumnType("int")
                        .HasColumnName("AppointmentID");

                    b.Property<DateTime?>("PaymentDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("PaymentId")
                        .HasName("PK__Payments__9B556A58E41BFC65");

                    b.HasIndex(new[] { "AppointmentId" }, "UQ__Payments__8ECDFCA3EE667EE2")
                        .IsUnique()
                        .HasFilter("[AppointmentID] IS NOT NULL");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("server.Models.Prescription", b =>
                {
                    b.Property<int>("PrescriptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PrescriptionID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PrescriptionId"));

                    b.Property<string>("Dosage")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Medicine")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("RecordId")
                        .HasColumnType("int")
                        .HasColumnName("RecordID");

                    b.HasKey("PrescriptionId")
                        .HasName("PK__Prescrip__40130812D4F20419");

                    b.HasIndex("RecordId");

                    b.ToTable("Prescriptions");
                });

            modelBuilder.Entity("server.Models.Service", b =>
                {
                    b.Property<int>("ServiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ServiceID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ServiceId"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("ServiceName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("ServiceId")
                        .HasName("PK__Services__C51BB0EA66A5E681");

                    b.HasIndex(new[] { "ServiceName" }, "UQ__Services__A42B5F9946E0F3F3")
                        .IsUnique()
                        .HasFilter("[ServiceName] IS NOT NULL");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("server.Models.ServiceRegistration", b =>
                {
                    b.Property<int>("RegistrationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RegistrationID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RegistrationId"));

                    b.Property<int?>("AppointmentId")
                        .HasColumnType("int")
                        .HasColumnName("AppointmentID");

                    b.Property<int?>("PatientId")
                        .HasColumnType("int")
                        .HasColumnName("PatientID");

                    b.Property<DateTime?>("RegistrationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("ServiceId")
                        .HasColumnType("int")
                        .HasColumnName("ServiceID");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("RegistrationId")
                        .HasName("PK__ServiceR__6EF58830115C6A91");

                    b.HasIndex("AppointmentId");

                    b.HasIndex("PatientId");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServiceRegistrations");
                });

            modelBuilder.Entity("server.Models.Specialty", b =>
                {
                    b.Property<int>("SpecialtyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("SpecialtyID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SpecialtyId"));

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("SpecialtyId")
                        .HasName("PK__Specialt__D768F648A8F2AA6D");

                    b.HasIndex(new[] { "Name" }, "UQ__Specialt__737584F6A3F2D7BE")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Specialties");
                });

            modelBuilder.Entity("server.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FullName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Role")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserId")
                        .HasName("PK__Users__1788CCACA34D314B");

                    b.HasIndex(new[] { "Email" }, "UQ__Users__A9D10534404F7C68")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("server.Models.Appointment", b =>
                {
                    b.HasOne("server.Models.Doctor", "Doctor")
                        .WithMany("Appointments")
                        .HasForeignKey("DoctorId")
                        .HasConstraintName("FK__Appointme__Docto__4E53A1AA");

                    b.HasOne("server.Models.Patient", "Patient")
                        .WithMany("Appointments")
                        .HasForeignKey("PatientId")
                        .HasConstraintName("FK__Appointme__Patie__4D5F7D71");

                    b.Navigation("Doctor");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("server.Models.Doctor", b =>
                {
                    b.HasOne("server.Models.Specialty", "Specialty")
                        .WithMany("Doctors")
                        .HasForeignKey("SpecialtyId")
                        .HasConstraintName("FK__Doctors__Special__45BE5BA9");

                    b.HasOne("server.Models.User", "User")
                        .WithOne("Doctor")
                        .HasForeignKey("server.Models.Doctor", "UserId")
                        .HasConstraintName("FK__Doctors__UserID__44CA3770");

                    b.Navigation("Specialty");

                    b.Navigation("User");
                });

            modelBuilder.Entity("server.Models.MedicalRecord", b =>
                {
                    b.HasOne("server.Models.Appointment", "Appointment")
                        .WithOne("MedicalRecord")
                        .HasForeignKey("server.Models.MedicalRecord", "AppointmentId")
                        .HasConstraintName("FK__MedicalRe__Appoi__5224328E");

                    b.Navigation("Appointment");
                });

            modelBuilder.Entity("server.Models.Patient", b =>
                {
                    b.HasOne("server.Models.User", "User")
                        .WithOne("Patient")
                        .HasForeignKey("server.Models.Patient", "UserId")
                        .HasConstraintName("FK__Patients__UserID__498EEC8D");

                    b.Navigation("User");
                });

            modelBuilder.Entity("server.Models.Payment", b =>
                {
                    b.HasOne("server.Models.Appointment", "Appointment")
                        .WithOne("Payment")
                        .HasForeignKey("server.Models.Payment", "AppointmentId")
                        .HasConstraintName("FK__Payments__Appoin__5AB9788F");

                    b.Navigation("Appointment");
                });

            modelBuilder.Entity("server.Models.Prescription", b =>
                {
                    b.HasOne("server.Models.MedicalRecord", "Record")
                        .WithMany("Prescriptions")
                        .HasForeignKey("RecordId")
                        .HasConstraintName("FK__Prescript__Recor__55009F39");

                    b.Navigation("Record");
                });

            modelBuilder.Entity("server.Models.ServiceRegistration", b =>
                {
                    b.HasOne("server.Models.Appointment", "Appointment")
                        .WithMany("ServiceRegistrations")
                        .HasForeignKey("AppointmentId")
                        .HasConstraintName("FK__ServiceRe__Appoi__65370702");

                    b.HasOne("server.Models.Patient", "Patient")
                        .WithMany("ServiceRegistrations")
                        .HasForeignKey("PatientId")
                        .HasConstraintName("FK__ServiceRe__Patie__634EBE90");

                    b.HasOne("server.Models.Service", "Service")
                        .WithMany("ServiceRegistrations")
                        .HasForeignKey("ServiceId")
                        .HasConstraintName("FK__ServiceRe__Servi__6442E2C9");

                    b.Navigation("Appointment");

                    b.Navigation("Patient");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("server.Models.Appointment", b =>
                {
                    b.Navigation("MedicalRecord");

                    b.Navigation("Payment");

                    b.Navigation("ServiceRegistrations");
                });

            modelBuilder.Entity("server.Models.Doctor", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("server.Models.MedicalRecord", b =>
                {
                    b.Navigation("Prescriptions");
                });

            modelBuilder.Entity("server.Models.Patient", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("ServiceRegistrations");
                });

            modelBuilder.Entity("server.Models.Service", b =>
                {
                    b.Navigation("ServiceRegistrations");
                });

            modelBuilder.Entity("server.Models.Specialty", b =>
                {
                    b.Navigation("Doctors");
                });

            modelBuilder.Entity("server.Models.User", b =>
                {
                    b.Navigation("Doctor");

                    b.Navigation("Patient");
                });
#pragma warning restore 612, 618
        }
    }
}
