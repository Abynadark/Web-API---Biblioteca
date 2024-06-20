﻿// <auto-generated />
using System;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Biblioteca.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Biblioteca.Models.Emprestimo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("DataDevolucao")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DataEmprestimo")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DataPrevistaInicial")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ExemplarId")
                        .HasColumnType("int");

                    b.Property<int>("StatusEmprestimo")
                        .HasColumnType("int");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ExemplarId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Emprestimos");
                });

            modelBuilder.Entity("Biblioteca.Models.Exemplar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("LivroISBN10")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("LivroId")
                        .HasColumnType("int");

                    b.Property<int>("StatusExemplar")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LivroId");

                    b.ToTable("Exemplares");
                });

            modelBuilder.Entity("Biblioteca.Models.Livro", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Autor")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Classificacao")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("DataPublicacao")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Edicao")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Editora")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Genero")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ISBN10")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ISBN13")
                        .HasColumnType("longtext");

                    b.Property<string>("Idioma")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Qnt_Pagina")
                        .HasColumnType("int");

                    b.Property<int>("StatusLivro")
                        .HasColumnType("int");

                    b.Property<float>("Valor")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Livros");
                });

            modelBuilder.Entity("Biblioteca.Models.Multa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DiasAtrasados")
                        .HasColumnType("int");

                    b.Property<int>("EmprestimoId")
                        .HasColumnType("int");

                    b.Property<DateTime>("InicioMulta")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("StatusMulta")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TerminoMulta")
                        .HasColumnType("datetime(6)");

                    b.Property<float>("Valor")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("EmprestimoId")
                        .IsUnique();

                    b.ToTable("Multa");
                });

            modelBuilder.Entity("Biblioteca.Models.Renovacao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DataRenovacao")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("EmprestimoId")
                        .HasColumnType("int");

                    b.Property<DateTime>("NovaDataPrevista")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("EmprestimoId");

                    b.ToTable("Renovacoes");
                });

            modelBuilder.Entity("Biblioteca.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CPF")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Celular")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("DataNasc")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Endereco")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.Property<int>("StatusUsuario")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("Biblioteca.Models.Emprestimo", b =>
                {
                    b.HasOne("Biblioteca.Models.Exemplar", "Exemplar")
                        .WithMany("Emprestimos")
                        .HasForeignKey("ExemplarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Biblioteca.Models.Usuario", "Usuario")
                        .WithMany("Emprestimos")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exemplar");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Biblioteca.Models.Exemplar", b =>
                {
                    b.HasOne("Biblioteca.Models.Livro", "Livro")
                        .WithMany("Exemplares")
                        .HasForeignKey("LivroId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Livro");
                });

            modelBuilder.Entity("Biblioteca.Models.Multa", b =>
                {
                    b.HasOne("Biblioteca.Models.Emprestimo", "Emprestimo")
                        .WithOne("Multa")
                        .HasForeignKey("Biblioteca.Models.Multa", "EmprestimoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Emprestimo");
                });

            modelBuilder.Entity("Biblioteca.Models.Renovacao", b =>
                {
                    b.HasOne("Biblioteca.Models.Emprestimo", "Emprestimo")
                        .WithMany("Renovacoes")
                        .HasForeignKey("EmprestimoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Emprestimo");
                });

            modelBuilder.Entity("Biblioteca.Models.Emprestimo", b =>
                {
                    b.Navigation("Multa");

                    b.Navigation("Renovacoes");
                });

            modelBuilder.Entity("Biblioteca.Models.Exemplar", b =>
                {
                    b.Navigation("Emprestimos");
                });

            modelBuilder.Entity("Biblioteca.Models.Livro", b =>
                {
                    b.Navigation("Exemplares");
                });

            modelBuilder.Entity("Biblioteca.Models.Usuario", b =>
                {
                    b.Navigation("Emprestimos");
                });
#pragma warning restore 612, 618
        }
    }
}
