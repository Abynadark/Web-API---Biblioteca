using Biblioteca.Enum;
using Biblioteca.Models;
namespace Biblioteca.Interfaces
{
    public interface ILivroRepository
    {
        Task<IEnumerable<Livro>> GetLivros();
        Task<Livro> GetLivroPorISBN10(string isbn10);
        Task<Livro> CreateLivro(Livro livro);
        Task<Livro> UpdateLivro(Livro livro);
        Task<Livro> DeleteLivro(string isbn10);

        Task<IEnumerable<Livro>> ConsultarLivros(string? nome, string? autor, string? isbn10, string? isbn13, string? editora, string? edicao, string? idioma, string? genero, DateTime? datapublicacao, string? classificacao, int? qnt_Pagina, float? precoMinimo, float? precoMaximo, StatusLivro? statusLivro);
    }
}
