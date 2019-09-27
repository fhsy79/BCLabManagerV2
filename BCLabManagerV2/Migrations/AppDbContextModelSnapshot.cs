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

                    b.Property<int>("AssetUseCount");

                    b.Property<int?>("BatteryClassId");

                    b.Property<int?>("ChamberClassId");

                    b.Property<int?>("ChannelClassId");

                    b.Property<string>("ProgramName");

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

                    b.Property<int>("AssetUseCount");

                    b.Property<int?>("BatteryTypeId");

                    b.Property<double>("CycleCount");

                    b.Property<string>("Name");

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

                    b.Property<int>("AssetUseCount");

                    b.Property<double>("HighestTemperature");

                    b.Property<double>("LowestTemperature");

                    b.Property<string>("Manufactor");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Chambers");
                });

            modelBuilder.Entity("BCLabManager.Model.ChannelClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AssetUseCount");

                    b.Property<string>("Name");

                    b.Property<int?>("TesterId");

                    b.HasKey("Id");

                    b.HasIndex("TesterId");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("BCLabManager.Model.ChargeCurrentClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ChargeCurrents");
                });

            modelBuilder.Entity("BCLabManager.Model.ChargeTemperatureClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ChargeTemperatures");
                });

            modelBuilder.Entity("BCLabManager.Model.DischargeCurrentClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("DischargeCurrents");
                });

            modelBuilder.Entity("BCLabManager.Model.DischargeTemperatureClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("DischargeTemperatures");
                });

            modelBuilder.Entity("BCLabManager.Model.EstimateTimeRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<TimeSpan>("AverageTime");

                    b.Property<int?>("BatteryTypeId");

                    b.Property<int>("ExecutedCount");

                    b.Property<int?>("SubTemplateId");

                    b.Property<int>("TestCount");

                    b.HasKey("Id");

                    b.HasIndex("BatteryTypeId");

                    b.HasIndex("SubTemplateId");

                    b.ToTable("EstimateTimeRecords");
                });

            modelBuilder.Entity("BCLabManager.Model.ProgramClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BatteryTypeId");

                    b.Property<DateTime>("CompleteTime");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<DateTime>("RequestTime");

                    b.Property<string>("Requester");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("BatteryTypeId");

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("BCLabManager.Model.RawDataClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FileName");

                    b.Property<string>("MD5");

                    b.Property<int?>("TestRecordClassId");

                    b.HasKey("Id");

                    b.HasIndex("TestRecordClassId");

                    b.ToTable("RawDataClass");
                });

            modelBuilder.Entity("BCLabManager.Model.SubProgramClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ChargeCurrentId");

                    b.Property<int?>("ChargeTemperatureId");

                    b.Property<DateTime>("CompleteTime");

                    b.Property<int?>("DischargeCurrentId");

                    b.Property<int?>("DischargeTemperatureId");

                    b.Property<bool>("IsAbandoned");

                    b.Property<int>("Loop");

                    b.Property<int?>("ProgramClassId");

                    b.Property<DateTime>("StartTime");

                    b.Property<int>("TestCount");

                    b.HasKey("Id");

                    b.HasIndex("ChargeCurrentId");

                    b.HasIndex("ChargeTemperatureId");

                    b.HasIndex("DischargeCurrentId");

                    b.HasIndex("DischargeTemperatureId");

                    b.HasIndex("ProgramClassId");

                    b.ToTable("SubPrograms");
                });

            modelBuilder.Entity("BCLabManager.Model.SubProgramTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ChargeCurrentId");

                    b.Property<int?>("ChargeTemperatureId");

                    b.Property<int?>("DischargeCurrentId");

                    b.Property<int?>("DischargeTemperatureId");

                    b.Property<int>("TestCount");

                    b.HasKey("Id");

                    b.HasIndex("ChargeCurrentId");

                    b.HasIndex("ChargeTemperatureId");

                    b.HasIndex("DischargeCurrentId");

                    b.HasIndex("DischargeTemperatureId");

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

                    b.Property<DateTime>("CompleteTime");

                    b.Property<double>("NewCycle");

                    b.Property<string>("ProgramStr");

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

            modelBuilder.Entity("BCLabManager.Model.EstimateTimeRecord", b =>
                {
                    b.HasOne("BCLabManager.Model.BatteryTypeClass", "BatteryType")
                        .WithMany()
                        .HasForeignKey("BatteryTypeId");

                    b.HasOne("BCLabManager.Model.SubProgramTemplate", "SubTemplate")
                        .WithMany()
                        .HasForeignKey("SubTemplateId");
                });

            modelBuilder.Entity("BCLabManager.Model.ProgramClass", b =>
                {
                    b.HasOne("BCLabManager.Model.BatteryTypeClass", "BatteryType")
                        .WithMany()
                        .HasForeignKey("BatteryTypeId");
                });

            modelBuilder.Entity("BCLabManager.Model.RawDataClass", b =>
                {
                    b.HasOne("BCLabManager.Model.TestRecordClass")
                        .WithMany("RawDataList")
                        .HasForeignKey("TestRecordClassId");
                });

            modelBuilder.Entity("BCLabManager.Model.SubProgramClass", b =>
                {
                    b.HasOne("BCLabManager.Model.ChargeCurrentClass", "ChargeCurrent")
                        .WithMany()
                        .HasForeignKey("ChargeCurrentId");

                    b.HasOne("BCLabManager.Model.ChargeTemperatureClass", "ChargeTemperature")
                        .WithMany()
                        .HasForeignKey("ChargeTemperatureId");

                    b.HasOne("BCLabManager.Model.DischargeCurrentClass", "DischargeCurrent")
                        .WithMany()
                        .HasForeignKey("DischargeCurrentId");

                    b.HasOne("BCLabManager.Model.DischargeTemperatureClass", "DischargeTemperature")
                        .WithMany()
                        .HasForeignKey("DischargeTemperatureId");

                    b.HasOne("BCLabManager.Model.ProgramClass")
                        .WithMany("SubPrograms")
                        .HasForeignKey("ProgramClassId");
                });

            modelBuilder.Entity("BCLabManager.Model.SubProgramTemplate", b =>
                {
                    b.HasOne("BCLabManager.Model.ChargeCurrentClass", "ChargeCurrent")
                        .WithMany()
                        .HasForeignKey("ChargeCurrentId");

                    b.HasOne("BCLabManager.Model.ChargeTemperatureClass", "ChargeTemperature")
                        .WithMany()
                        .HasForeignKey("ChargeTemperatureId");

                    b.HasOne("BCLabManager.Model.DischargeCurrentClass", "DischargeCurrent")
                        .WithMany()
                        .HasForeignKey("DischargeCurrentId");

                    b.HasOne("BCLabManager.Model.DischargeTemperatureClass", "DischargeTemperature")
                        .WithMany()
                        .HasForeignKey("DischargeTemperatureId");
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
