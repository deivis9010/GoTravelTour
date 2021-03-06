﻿// <auto-generated />
using System;
using GoTravelTour.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GoTravelTour.Migrations
{
    [DbContext(typeof(GoTravelDBContext))]
    [Migration("20191105023305_migracion inicial")]
    partial class migracioninicial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GoTravelTour.Models.Cliente", b =>
                {
                    b.Property<int>("ClienteId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Ciudad");

                    b.Property<string>("Correo");

                    b.Property<double>("Descuento");

                    b.Property<string>("Estado");

                    b.Property<byte[]>("ImageContent");

                    b.Property<string>("ImageMimeType");

                    b.Property<string>("ImageName");

                    b.Property<bool>("IsActivo");

                    b.Property<bool>("IsPublic");

                    b.Property<string>("Localizador");

                    b.Property<string>("Nombre");

                    b.Property<string>("Pais");

                    b.Property<string>("Telefono");

                    b.Property<string>("TipoTrasaccion");

                    b.Property<string>("ZipCode");

                    b.HasKey("ClienteId");

                    b.ToTable("Clientes");
                });

            modelBuilder.Entity("GoTravelTour.Models.Rol", b =>
                {
                    b.Property<int>("RolId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("NombreRol");

                    b.HasKey("RolId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("GoTravelTour.Models.Usuario", b =>
                {
                    b.Property<int>("UsuarioId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ClienteId");

                    b.Property<string>("Correo");

                    b.Property<string>("Password");

                    b.Property<int?>("RolId");

                    b.Property<string>("Username");

                    b.HasKey("UsuarioId");

                    b.HasIndex("ClienteId");

                    b.HasIndex("RolId");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("GoTravelTour.Models.Usuario", b =>
                {
                    b.HasOne("GoTravelTour.Models.Cliente", "cliente")
                        .WithMany()
                        .HasForeignKey("ClienteId");

                    b.HasOne("GoTravelTour.Models.Rol", "rol")
                        .WithMany()
                        .HasForeignKey("RolId");
                });
#pragma warning restore 612, 618
        }
    }
}
