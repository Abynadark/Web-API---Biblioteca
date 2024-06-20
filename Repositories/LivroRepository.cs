using Biblioteca.Enum;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Biblioteca.Repositories
{
    public class LivroRepository : ILivroRepository
    {
        private readonly Context _context;

        public LivroRepository(Context context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Livro>> GetLivros()
        {
            return await _context.Livros.ToListAsync();
        }

        public async Task<Livro> GetLivroPorISBN10(string isbn10)
        {
            return await _context.Livros.FirstOrDefaultAsync(l => l.ISBN10 == isbn10);
        }

        public async Task<Livro> CreateLivro(Livro livro)
        {
            if(livro is null)
                throw new ArgumentNullException(nameof(livro));

            livro.StatusLivro = Enum.StatusLivro.NaoDisponivel;

            _context.Livros.Add(livro);
            

            return livro;
        }

        public async Task<Livro> UpdateLivro(Livro livro)
        {
            if(livro is null)
                throw new ArgumentNullException(nameof(livro));

            _context.Entry(livro).State = EntityState.Modified; 
            //await _context.SaveChangesAsync();

            return livro;
        }

        public async Task<Livro> DeleteLivro(string isbn10)
        {
            var livro = await _context.Livros.FirstOrDefaultAsync(l => l.ISBN10 == isbn10);

            if (livro is null)
                throw new ArgumentNullException(nameof(livro));

            _context.Livros.Remove(livro);
            //await _context.SaveChangesAsync();

            return livro;

        }

        public async Task<IEnumerable<Livro>> ConsultarLivros(string? nome, string? autor, string? isbn10, string? isbn13,string? editora, string? edicao, string? idioma, string? genero, DateTime? datapublicacao, string? classificacao, int? qnt_Pagina, float? precoMinimo, float? precoMaximo, StatusLivro? statusLivro)
        {
            IQueryable<Livro> query = _context.Livros;

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(l => l.Nome == nome);
            }

            if (!string.IsNullOrEmpty(autor))
            {
                query = query.Where(l => l.Autor == autor);
            }

            if (!string.IsNullOrEmpty(isbn10))
            {
                query = query.Where(l => l.ISBN10 == isbn10);
            }

            if (!string.IsNullOrEmpty(isbn13))
            {
                query = query.Where(l => l.ISBN13 == isbn13);
            }

            if (!string.IsNullOrEmpty(editora))
            {
                query = query.Where(l => l.Editora == editora);
            }

            if (!string.IsNullOrEmpty(edicao))
            {
                query = query.Where(l => l.Edicao == edicao);
            }

            if (!string.IsNullOrEmpty(idioma))
            {
                query = query.Where(l => l.Idioma == idioma);
            }

            if (!string.IsNullOrEmpty(genero))
            {
                query = query.Where(l => l.Genero == genero);
            }

            if (datapublicacao.HasValue)
            {
                query = query.Where(l => l.DataPublicacao >= datapublicacao.Value.Date); 
            }

            if (!string.IsNullOrEmpty(classificacao))
            {
                query = query.Where(l => l.Classificacao == classificacao);
            }

            if (qnt_Pagina >= 0)
            {
                query = query.Where(e => e.Qnt_Pagina == qnt_Pagina);
            }

            if (precoMinimo.HasValue && precoMaximo.HasValue && precoMinimo >= 0 && precoMaximo >= precoMinimo)
            {
                query = query.Where(e => e.Valor >= precoMinimo && e.Valor <= precoMaximo);
            }

            if (statusLivro.HasValue)
            {
                query = query.Where(e => e.StatusLivro == statusLivro);
            }

            return await query.ToListAsync();
        }
    }
}
