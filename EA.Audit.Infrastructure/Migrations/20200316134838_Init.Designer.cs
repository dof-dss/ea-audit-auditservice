﻿// <auto-generated />
using System;
using EA.Audit.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EA.Audit.Common.Migrations
{
    [DbContext(typeof(AuditContext))]
    [Migration("20200316134838_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("EA.Audit.Common.Idempotency.ClientRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("ClientRequests");
                });

            modelBuilder.Entity("EA.Audit.Common.Model.Admin.AuditApplication", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ApplicationID")
                        .HasColumnType("bigint");

                    b.Property<string>("ClientId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("AuditApplications");
                });

            modelBuilder.Entity("EA.Audit.Common.Model.AuditEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AuditID")
                        .HasColumnType("bigint");

                    b.Property<string>("Actor")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<long>("ActorId")
                        .HasColumnType("bigint");

                    b.Property<long?>("AuditApplicationId")
                        .HasColumnType("bigint");

                    b.Property<string>("ClientId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Properties")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Subject")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<long>("SubjectId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AuditApplicationId");

                    b.ToTable("Audits");
                });

            modelBuilder.Entity("EA.Audit.Common.Model.AuditEntity", b =>
                {
                    b.HasOne("EA.Audit.Common.Model.Admin.AuditApplication", "AuditApplication")
                        .WithMany()
                        .HasForeignKey("AuditApplicationId");
                });
#pragma warning restore 612, 618
        }
    }
}
