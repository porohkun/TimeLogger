﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TimeLogger.DataAccess.Data;

#nullable disable

namespace TimeLogger.DataAccess.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230924174851_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("ActivityTag", b =>
                {
                    b.Property<long>("ActivitiesId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TagsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ActivitiesId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("ActivityTag");
                });

            modelBuilder.Entity("TimeLogger.Domain.Data.Activity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("TimeLogger.Domain.Data.Period", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("End")
                        .HasColumnType("TEXT");

                    b.Property<long>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Start")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Periods");
                });

            modelBuilder.Entity("TimeLogger.Domain.Data.Tag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("ActivityTag", b =>
                {
                    b.HasOne("TimeLogger.Domain.Data.Activity", null)
                        .WithMany()
                        .HasForeignKey("ActivitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TimeLogger.Domain.Data.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TimeLogger.Domain.Data.Period", b =>
                {
                    b.HasOne("TimeLogger.Domain.Data.Activity", "Owner")
                        .WithMany("Periods")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("TimeLogger.Domain.Data.Activity", b =>
                {
                    b.Navigation("Periods");
                });
#pragma warning restore 612, 618
        }
    }
}
