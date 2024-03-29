﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using core.Database;

#nullable disable

namespace core.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20221031224109_add amount after transaction")]
    partial class addamountaftertransaction
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("core.models.BankItem", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<decimal?>("Amount")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal?>("AmountInCategoryAfter")
                        .HasColumnType("decimal(65,30)");

                    b.Property<Guid?>("CategoryId")
                        .HasColumnType("char(36)");

                    b.Property<ulong?>("DiscordMessageId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("RawEmail")
                        .HasColumnType("longtext");

                    b.Property<string>("RawPayload")
                        .HasColumnType("longtext");

                    b.Property<string>("State")
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset?>("Timestamp")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Vendor")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("core.models.Category", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<ulong?>("DiscordMessageId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Emoji")
                        .HasColumnType("longtext");

                    b.Property<string>("FireflyId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("FireflyOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("core.models.BankItem", b =>
                {
                    b.HasOne("core.models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}
