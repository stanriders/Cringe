﻿// <auto-generated />
using System;
using Cringe.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cringe.Migrations.BeatmapDatabase
{
    [DbContext(typeof(BeatmapDatabaseContext))]
    [Migration("20210728111704_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.6");

            modelBuilder.Entity("Cringe.Types.Database.Beatmap", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("ApproachRate")
                        .HasColumnType("REAL");

                    b.Property<string>("Artist")
                        .HasColumnType("TEXT");

                    b.Property<int?>("BeatmapSetId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Bpm")
                        .HasColumnType("REAL");

                    b.Property<double>("CircleSize")
                        .HasColumnType("REAL");

                    b.Property<string>("Creator")
                        .HasColumnType("TEXT");

                    b.Property<string>("DifficultyName")
                        .HasColumnType("TEXT");

                    b.Property<double>("HpDrain")
                        .HasColumnType("REAL");

                    b.Property<int>("Length")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Md5")
                        .HasColumnType("TEXT");

                    b.Property<int>("Mode")
                        .HasColumnType("INTEGER");

                    b.Property<double>("OverallDifficulty")
                        .HasColumnType("REAL");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Beatmaps");
                });
#pragma warning restore 612, 618
        }
    }
}
