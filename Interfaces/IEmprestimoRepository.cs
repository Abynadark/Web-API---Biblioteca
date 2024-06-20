using Biblioteca.Enum;
using Biblioteca.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biblioteca.Interfaces
{
    public interface IEmprestimoRepository
    {
        Task<IEnumerable<Emprestimo>> GetEmprestimos();
        Task<IEnumerable<Emprestimo>> GetEmprestimosPorUsuarioCPF(string cpf);
        Task<Emprestimo> GetEmprestimoPorId(int id);
        Task<Emprestimo> CreateEmprestimo(Emprestimo emprestimo);
        Task<Emprestimo> UpdateEmprestimo(Emprestimo emprestimo);
        Task<Emprestimo> DeleteEmprestimo(int id);
        Task<IEnumerable<Emprestimo>> ConsultarEmprestimos(string? cpf, int? exemplarId, StatusEmprestimo? statusEmprestimo, string? isbn10);
        Task<Emprestimo> UpdateEmprestimoStatus(Emprestimo emprestimo);

    }
}
