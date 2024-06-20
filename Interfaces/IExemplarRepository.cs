using Biblioteca.Enum;
using Biblioteca.Models;

namespace Biblioteca.Interfaces
{
    public interface IExemplarRepository
    {
        Task<IEnumerable<Exemplar>> GetExemplares();
        Task<IEnumerable<Exemplar>> GetExemplaresISBN10eStatus(string? ISBN10, StatusExemplar? statusExemplar);
        Task<Exemplar> GetExemplarPorId(int id);
        Task<IEnumerable<Exemplar>> GetExemplaresPorISBN10(string isbn10);
        Task<Exemplar> CreateExemplar(Exemplar exemplar);
        Task<Exemplar> UpdateExemplar(Exemplar exemplar);
        Task<Exemplar> DeleteExemplar(int id);

    }
}
