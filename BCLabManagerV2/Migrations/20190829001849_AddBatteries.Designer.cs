﻿// <auto-generated />
using System;
using BCLabManager.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BCLabManager.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20190829001849_AddBatteries")]
    partial class AddBatteries
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("BCLabManager.Model.AssetUsageRecordClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BatteryClassId");

                    b.Property<string>("ProgramName");

                    b.Property<int>("Status");

                    b.Property<string>("SubProgramName");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("Id");

                    b.HasIndex("BatteryClassId");

                    b.ToTable("AssetUsageRecordClass");
                });

            modelBuilder.Entity("BCLabManager.Model.BatteryClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BatteryTypeId");

                    b.Property<double>("CycleCount");

                    b.Property<string>("Name");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("BatteryTypeId");

                    b.ToTable("Batteries");
                });

            modelBuilder.Entity("BCLabManager.Model.BatteryTypeClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CutoffDischargeVoltage");

                    b.Property<int>("LimitedChargeVoltage");

                    b.Property<string>("Manufactor");

                    b.Property<string>("Material");

                    b.Property<string>("Name");

                    b.Property<int>("NominalVoltage");

                    b.Property<int>("RatedCapacity");

                    b.Property<int>("TypicalCapacity");

                    b.HasKey("Id");

                    b.ToTable("BatteryTypes");
                });

            modelBuilder.Entity("BCLabManager.Model.AssetUsageRecordClass", b =>
                {
                    b.HasOne("BCLabManager.Model.BatteryClass")
                        .WithMany("Records")
                        .HasForeignKey("BatteryClassId");
                });

            modelBuilder.Entity("BCLabManager.Model.BatteryClass", b =>
                {
                    b.HasOne("BCLabManager.Model.BatteryTypeClass", "BatteryType")
                        .WithMany()
                        .HasForeignKey("BatteryTypeId");
                });
#pragma warning restore 612, 618
        }
    }
}
