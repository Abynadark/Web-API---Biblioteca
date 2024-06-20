using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;
namespace Biblioteca.Repositories
{
    public class RenovacaoRepository : IRenovacaoRepository
    {
        private readonly Context _context;

        public RenovacaoRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Renovacao>> GetRenovacoes()
        {
            return await _context.Renovacoes.Include(r => r.Emprestimo).ToListAsync();
        }

        public async Task<IEnumerable<Renovacao>> GetConsultarRenovacoes(string? CPF, int? idEmprestimo)
        {
            IQueryable<Renovacao> query = _context.Renovacoes.Include(r => r.Emprestimo)
                                                  .ThenInclude(e => e.Usuario);
           
            if (!string.IsNullOrEmpty(CPF))
            {
                query = query.Where(r => r.Emprestimo.Usuario.CPF == CPF);
            }

            if (idEmprestimo.HasValue && idEmprestimo > 0)
            {
                query = query.Where(r => r.EmprestimoId == idEmprestimo);
            }

            return await query.ToListAsync();
        }

        public async Task<Renovacao> GetRenovacaoPorId(int id)
        {
            return await _context.Renovacoes.Include(r => r.Emprestimo).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Renovacao> CreateRenovacao(Renovacao renovacao)
        {
            if (renovacao == null)
                throw new ArgumentNullException(nameof(renovacao));

            renovacao.NovaDataPrevista = renovacao.DataRenovacao.AddDays(7);

            _context.Renovacoes.Add(renovacao);
            return renovacao;
        }

        public async Task<Renovacao> UpdateRenovacao(Renovacao renovacao)
        {
            if (renovacao == null)
                throw new ArgumentNullException(nameof(renovacao));

            _context.Entry(renovacao).State = EntityState.Modified;
            return renovacao;
        }

        public async Task<Renovacao> DeleteRenovacao(int id)
        {
            var renovacao = await _context.Renovacoes.FindAsync(id);
            if (renovacao == null)
                throw new ArgumentNullException(nameof(renovacao));

            _context.Renovacoes.Remove(renovacao);
            return renovacao;
        }

        public async Task<Renovacao> GetUltimaRenovacaoAsync(int emprestimoId)
        {
            var ultimaRenovacao = await _context.Renovacoes
                .Where(r => r.EmprestimoId == emprestimoId)
                .OrderByDescending(r => r.DataRenovacao)
                .FirstOrDefaultAsync();

            return ultimaRenovacao;
        }

    }
}
