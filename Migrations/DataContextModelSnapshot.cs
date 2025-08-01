﻿// <auto-generated />
using System;
using ASP_P26.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ASP_P26.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ASP_P26.Data.Entities.AccessToken", b =>
                {
                    b.Property<string>("Jti")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Aud")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Exp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Iat")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Iss")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nbf")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Sub")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Jti");

                    b.HasIndex("Sub");

                    b.ToTable("AccessTokens");
                });

            modelBuilder.Entity("ASP_P26.Data.Entities.UserAccess", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Dk")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserAccesses");
                });

            modelBuilder.Entity("ASP_P26.Data.Entities.UserData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Birthdate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RegisteredAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ASP_P26.Data.Entities.UserRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("CanCreate")
                        .HasColumnType("bit");

                    b.Property<bool>("CanDelete")
                        .HasColumnType("bit");

                    b.Property<bool>("CanRead")
                        .HasColumnType("bit");

                    b.Property<bool>("CanUpdate")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            Id = "SelfRegistered",
                            CanCreate = false,
                            CanDelete = false,
                            CanRead = false,
                            CanUpdate = false,
                            Description = "Самостійно зареєстрований користувач"
                        },
                        new
                        {
                            Id = "Employee",
                            CanCreate = true,
                            CanDelete = false,
                            CanRead = true,
                            CanUpdate = false,
                            Description = "Співробітник компанії"
                        },
                        new
                        {
                            Id = "Moderator",
                            CanCreate = false,
                            CanDelete = true,
                            CanRead = true,
                            CanUpdate = true,
                            Description = "Редактор контенту"
                        },
                        new
                        {
                            Id = "Administrator",
                            CanCreate = true,
                            CanDelete = true,
                            CanRead = true,
                            CanUpdate = true,
                            Description = "Системний адміністратор"
                        });
                });

            modelBuilder.Entity("ASP_P26.Data.Entities.AccessToken", b =>
                {
                    b.HasOne("ASP_P26.Data.Entities.UserAccess", "UserAccess")
                        .WithMany()
                        .HasForeignKey("Sub")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserAccess");
                });

            modelBuilder.Entity("ASP_P26.Data.Entities.UserAccess", b =>
                {
                    b.HasOne("ASP_P26.Data.Entities.UserRole", "UserRole")
                        .WithMany("UserAccesses")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ASP_P26.Data.Entities.UserData", "UserData")
                        .WithMany("UserAccesses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserData");

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("ASP_P26.Data.Entities.UserData", b =>
                {
                    b.Navigation("UserAccesses");
                });

            modelBuilder.Entity("ASP_P26.Data.Entities.UserRole", b =>
                {
                    b.Navigation("UserAccesses");
                });
#pragma warning restore 612, 618
        }
    }
}
