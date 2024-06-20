using Biblioteca.Enum;
using Biblioteca.Models;

namespace Biblioteca.Interfaces
{
    public interface IMultaRepository
    {
        Task<IEnumerable<Multa>> GetMultas();
        Task<Multa> GetMultaPorId(int id);
        Task<Multa> GetMultaPorIdEmprestimo(int idEmprestimo);
        Task<Multa> CreateMulta(Multa multa);
        Task<Multa> UpdateMulta(Multa multa);
        Task<Multa> DeleteMulta(int id);

        Task<IEnumerable<Multa>> ConsultarMultas(string? usuarioCPF, int? emprestimoId, int? dias, StatusMulta? statusMulta);
    }
}
