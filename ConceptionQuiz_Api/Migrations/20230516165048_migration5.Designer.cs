﻿// <auto-generated />
using System;
using ConceptionQuiz_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ConceptionQuiz_Api.Migrations
{
    [DbContext(typeof(ConceptionQuizDbContext))]
    [Migration("20230516165048_migration5")]
    partial class migration5
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ConceptionQuiz_Api.Models.Question", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("questionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("questions");
                });

            modelBuilder.Entity("ConceptionQuiz_Api.Models.Quiz", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("dateCreation")
                        .HasColumnType("datetime2");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("duree_quiz")
                        .HasColumnType("int");

                    b.Property<int>("nbr_questions")
                        .HasColumnType("int");

                    b.Property<int>("niv_deficulte")
                        .HasColumnType("int");

                    b.Property<string>("titre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("quiz");
                });

            modelBuilder.Entity("ConceptionQuiz_Api.Models.Reponse", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("questionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("id");

                    b.HasIndex("questionId");

                    b.ToTable("Reponse");
                });

            modelBuilder.Entity("QuestionQuiz", b =>
                {
                    b.Property<string>("questionsid")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("quizid")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("questionsid", "quizid");

                    b.HasIndex("quizid");

                    b.ToTable("QuestionQuiz");
                });

            modelBuilder.Entity("ConceptionQuiz_Api.Models.Reponse", b =>
                {
                    b.HasOne("ConceptionQuiz_Api.Models.Question", "question")
                        .WithMany("reponses")
                        .HasForeignKey("questionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("question");
                });

            modelBuilder.Entity("QuestionQuiz", b =>
                {
                    b.HasOne("ConceptionQuiz_Api.Models.Question", null)
                        .WithMany()
                        .HasForeignKey("questionsid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConceptionQuiz_Api.Models.Quiz", null)
                        .WithMany()
                        .HasForeignKey("quizid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ConceptionQuiz_Api.Models.Question", b =>
                {
                    b.Navigation("reponses");
                });
#pragma warning restore 612, 618
        }
    }
}
