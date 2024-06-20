using Biblioteca.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Interfaces
{
    public interface IRenovacaoRepository
    {
        Task<IEnumerable<Renovacao>> GetRenovacoes();
        Task<Renovacao> GetRenovacaoPorId(int id);
        Task<IEnumerable<Renovacao>> GetConsultarRenovacoes(string? CPF, int? idEmprestimo);
        Task<Renovacao> CreateRenovacao(Renovacao renovacao);
        Task<Renovacao> UpdateRenovacao(Renovacao renovacao);
        Task<Renovacao> DeleteRenovacao(int id);
        Task<Renovacao> GetUltimaRenovacaoAsync(int emprestimoId);
    }
}
