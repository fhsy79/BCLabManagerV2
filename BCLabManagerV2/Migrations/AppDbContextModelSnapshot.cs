﻿// <auto-generated />
using System;
using BCLabManager.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BCLabManager.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("BCLabManager.Model.AssetUsageRecordClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BatteryClassId");

                    b.Property<int?>("ChamberClassId");

                    b.Property<int?>("ChannelClassId");

                    b.Property<string>("ProgramName");

                    b.Property<int>("Status");

                    b.Property<string>("SubProgramName");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("Id");

                    b.HasIndex("BatteryClassId");

                    b.HasIndex("ChamberClassId");

                    b.HasIndex("ChannelClassId");

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

            modelBuilder.Entity("BCLabManager.Model.ChamberClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("HighestTemperature");

                    b.Property<double>("LowestTemperature");

                    b.Property<string>("Manufactor");

                    b.Property<string>("Name");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.ToTable("Chambers");
                });

            modelBuilder.Entity("BCLabManager.Model.ChannelClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Status");

                    b.Property<int?>("TesterId");

                    b.HasKey("Id");

                    b.HasIndex("TesterId");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("BCLabManager.Model.ProgramClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CompleteDate");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<DateTime>("RequestDate");

                    b.Property<string>("Requester");

                    b.HasKey("Id");

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("BCLabManager.Model.RawDataClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("RawDataClass");
                });

            modelBuilder.Entity("BCLabManager.Model.SubProgramClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int?>("ProgramClassId");

                    b.Property<int>("TestCount");

                    b.HasKey("Id");

                    b.HasIndex("ProgramClassId");

                    b.ToTable("SubPrograms");
                });

            modelBuilder.Entity("BCLabManager.Model.SubProgramTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("TestCount");

                    b.HasKey("Id");

                    b.ToTable("SubProgramTemplates");
                });

            modelBuilder.Entity("BCLabManager.Model.TestRecordClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AssignedBatteryId");

                    b.Property<int?>("AssignedChamberId");

                    b.Property<int?>("AssignedChannelId");

                    b.Property<string>("BatteryStr");

                    b.Property<string>("BatteryTypeStr");

                    b.Property<string>("ChamberStr");

                    b.Property<string>("ChannelStr");

                    b.Property<string>("Comment");

                    b.Property<DateTime>("EndTime");

                    b.Property<double>("NewCycle");

                    b.Property<string>("ProgramStr");

                    b.Property<int?>("RawDataId");

                    b.Property<DateTime>("StartTime");

                    b.Property<int>("Status");

                    b.Property<string>("Steps");

                    b.Property<int?>("SubProgramClassId");

                    b.Property<int?>("SubProgramClassId1");

                    b.Property<string>("SubProgramStr");

                    b.Property<string>("TesterStr");

                    b.HasKey("Id");

                    b.HasIndex("AssignedBatteryId");

                    b.HasIndex("AssignedChamberId");

                    b.HasIndex("AssignedChannelId");

                    b.HasIndex("RawDataId");

                    b.HasIndex("SubProgramClassId");

                    b.HasIndex("SubProgramClassId1");

                    b.ToTable("TestRecords");
                });

            modelBuilder.Entity("BCLabManager.Model.TesterClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Manufactor");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Testers");
                });

            modelBuilder.Entity("BCLabManager.Model.AssetUsageRecordClass", b =>
                {
                    b.HasOne("BCLabManager.Model.BatteryClass")
                        .WithMany("Records")
                        .HasForeignKey("BatteryClassId");

                    b.HasOne("BCLabManager.Model.ChamberClass")
                        .WithMany("Records")
                        .HasForeignKey("ChamberClassId");

                    b.HasOne("BCLabManager.Model.ChannelClass")
                        .WithMany("Records")
                        .HasForeignKey("ChannelClassId");
                });

            modelBuilder.Entity("BCLabManager.Model.BatteryClass", b =>
                {
                    b.HasOne("BCLabManager.Model.BatteryTypeClass", "BatteryType")
                        .WithMany()
                        .HasForeignKey("BatteryTypeId");
                });

            modelBuilder.Entity("BCLabManager.Model.ChannelClass", b =>
                {
                    b.HasOne("BCLabManager.Model.TesterClass", "Tester")
                        .WithMany()
                        .HasForeignKey("TesterId");
                });

            modelBuilder.Entity("BCLabManager.Model.SubProgramClass", b =>
                {
                    b.HasOne("BCLabManager.Model.ProgramClass")
                        .WithMany("SubPrograms")
                        .HasForeignKey("ProgramClassId");
                });

            modelBuilder.Entity("BCLabManager.Model.TestRecordClass", b =>
                {
                    b.HasOne("BCLabManager.Model.BatteryClass", "AssignedBattery")
                        .WithMany()
                        .HasForeignKey("AssignedBatteryId");

                    b.HasOne("BCLabManager.Model.ChamberClass", "AssignedChamber")
                        .WithMany()
                        .HasForeignKey("AssignedChamberId");

                    b.HasOne("BCLabManager.Model.ChannelClass", "AssignedChannel")
                        .WithMany()
                        .HasForeignKey("AssignedChannelId");

                    b.HasOne("BCLabManager.Model.RawDataClass", "RawData")
                        .WithMany()
                        .HasForeignKey("RawDataId");

                    b.HasOne("BCLabManager.Model.SubProgramClass")
                        .WithMany("FirstTestRecords")
                        .HasForeignKey("SubProgramClassId");

                    b.HasOne("BCLabManager.Model.SubProgramClass")
                        .WithMany("SecondTestRecords")
                        .HasForeignKey("SubProgramClassId1");
                });
#pragma warning restore 612, 618
        }
    }
}
