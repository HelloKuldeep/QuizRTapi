﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuizRT.Models;

namespace QuizRTapi.Migrations
{
    [DbContext(typeof(QuizRTContext))]
    [Migration("20181015062303_CreateQuiztest1DB")]
    partial class CreateQuiztest1DB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("QuizRT.Models.QuizRTTemplate", b =>
                {
                    b.Property<int>("TempId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Categ");

                    b.HasKey("TempId");

                    b.ToTable("QuizRTTemplateT");
                });
#pragma warning restore 612, 618
        }
    }
}