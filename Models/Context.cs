using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Models;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Livro> Livros { get; set; } = null!;
    public DbSet<Emprestimo> Emprestimos { get; set; } = null!;
    public DbSet<Exemplar> Exemplares { get; set; } = null!;
    public DbSet<Multa> Multa { get; set; } = null!;
    public DbSet<Renovacao> Renovacoes { get; set; } = null!;


}