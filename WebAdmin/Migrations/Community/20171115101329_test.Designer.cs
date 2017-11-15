﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using MSDev.DB;
using System;

namespace WebAdmin.Migrations.Community
{
    [DbContext(typeof(CommunityContext))]
    [Migration("20171115101329_test")]
    partial class test
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("MSDev.DB.Entities.Config", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(32);

                    b.Property<string>("Type")
                        .HasMaxLength(32);

                    b.Property<string>("Value")
                        .HasMaxLength(4000);

                    b.HasKey("Id");

                    b.ToTable("Config");
                });
#pragma warning restore 612, 618
        }
    }
}
