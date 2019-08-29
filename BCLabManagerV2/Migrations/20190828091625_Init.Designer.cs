﻿// <auto-generated />
using BCLabManager.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BCLabManager.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20190828091625_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

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
#pragma warning restore 612, 618
        }
    }
}
