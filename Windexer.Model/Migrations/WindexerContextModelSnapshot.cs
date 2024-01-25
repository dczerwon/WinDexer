﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace WinDexer.Model.Migrations
{
    [DbContext(typeof(WinDexerContext))]
    partial class WinDexerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("WinDexer.Model.Entities.IndexEntry", b =>
                {
                    b.Property<Guid>("IndexEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Extension")
                        .HasColumnType("TEXT");

                    b.Property<int>("FilesCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FoldersCount")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("IndexationDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsFolder")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastAccessTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastSeen")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastWriteTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ParentIndexEntryId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RelativePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RootFolderId")
                        .HasColumnType("TEXT");

                    b.Property<long>("Size")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("StillFound")
                        .HasColumnType("INTEGER");

                    b.HasKey("IndexEntryId");

                    b.HasIndex("ParentIndexEntryId");

                    b.HasIndex("RootFolderId");

                    b.ToTable("IndexEntry", (string)null);
                });

            modelBuilder.Entity("WinDexer.Model.Entities.RootFolder", b =>
                {
                    b.Property<Guid>("RootFolderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("IndexationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool?>("StillFound")
                        .HasColumnType("INTEGER");

                    b.HasKey("RootFolderId");

                    b.ToTable("RootFolder", (string)null);
                });

            modelBuilder.Entity("WinDexer.Model.Entities.IndexEntry", b =>
                {
                    b.HasOne("WinDexer.Model.Entities.IndexEntry", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentIndexEntryId");

                    b.HasOne("WinDexer.Model.Entities.RootFolder", "Root")
                        .WithMany("Children")
                        .HasForeignKey("RootFolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parent");

                    b.Navigation("Root");
                });

            modelBuilder.Entity("WinDexer.Model.Entities.RootFolder", b =>
                {
                    b.Navigation("Children");
                });
#pragma warning restore 612, 618
        }
    }
}