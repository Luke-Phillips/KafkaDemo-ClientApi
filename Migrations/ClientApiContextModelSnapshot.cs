﻿// <auto-generated />
using ClientApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ClientApi.Migrations
{
    [DbContext(typeof(ClientApiContext))]
    partial class ClientApiContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("ClientApi.Models.SavedNumber", b =>
                {
                    b.Property<int>("SavedNumberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsBanned")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("SavedNumberId");

                    b.ToTable("SavedNumbers");
                });
#pragma warning restore 612, 618
        }
    }
}
