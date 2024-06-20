using Biblioteca.Enum;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Repositories
{
    public class EmprestimoRepository : IEmprestimoRepository
    {
        private readonly Context _context;

        public EmprestimoRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Emprestimo>> GetEmprestimos()
        {
            return await _context.Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Multa)
                .Include(e => e.Renovacoes)
                .ToListAsync();
        }

        public async Task<IEnumerable<Emprestimo>> GetEmprestimosPorUsuarioCPF(string cpf)
        {
            return await _context.Emprestimos
                .Where(e => e.Usuario.CPF == cpf)
                .Include(e => e.Usuario)
                .Include(e => e.Multa)
                .Include(e => e.Renovacoes)
                .ToListAsync();
        }


        public async Task<Emprestimo> GetEmprestimoPorId(int id)
        {
            var emprestimo = await _context.Emprestimos.Include(e => e.Usuario).Include(e => e.Multa).Include(e => e.Renovacoes)
                .FirstOrDefaultAsync(e => e.Id == id);

            return emprestimo;
        }

        public async Task<Emprestimo> CreateEmprestimo(Emprestimo emprestimo)
        {
            if (emprestimo == null)
                throw new ArgumentNullException(nameof(emprestimo));

            emprestimo.StatusEmprestimo = StatusEmprestimo.NaoDevolvido;

            emprestimo.DataPrevistaInicial = emprestimo.DataEmprestimo.AddDays(7);

            _context.Emprestimos.Add(emprestimo);
            return emprestimo;
        }

        public async Task<Emprestimo> UpdateEmprestimo(Emprestimo emprestimo)
        {
            if (emprestimo == null)
                throw new ArgumentNullException(nameof(emprestimo));

            _context.Entry(emprestimo).State = EntityState.Modified;
            return emprestimo;
        }

        public async Task<Emprestimo> DeleteEmprestimo(int id)
        {
            var emprestimo = await _context.Emprestimos.FindAsync(id);
            if (emprestimo == null)
                throw new ArgumentNullException(nameof(emprestimo));

            _context.Emprestimos.Remove(emprestimo);
            return emprestimo;
        }




        public async Task<IEnumerable<Emprestimo>> ConsultarEmprestimos(string? cpf, int? exemplarId, StatusEmprestimo? statusEmprestimo, string? isbn10)
        {
            IQueryable<Emprestimo> query = _context.Emprestimos.Include(e => e.Usuario).Include(e => e.Multa).Include(e => e.Renovacoes).Include(e => e.Exemplar.Livro);

            if (!string.IsNullOrEmpty(cpf))
            {
                query = query.Where(e => e.Usuario.CPF == cpf);
            }

            if (exemplarId > 0)
            {
                query = query.Where(e => e.ExemplarId == exemplarId);
            }

            if (statusEmprestimo.HasValue)
            {
                query = query.Where(e => e.StatusEmprestimo == statusEmprestimo);
            }


            if (!string.IsNullOrEmpty(isbn10))
            {
                query = query.Where(e => e.Exemplar.Livro.ISBN10 == isbn10);
            }


            return await query.ToListAsync();
        }

        public async Task<Emprestimo> UpdateEmprestimoStatus(Emprestimo emprestimo)
        {
            if (emprestimo == null)
                throw new ArgumentNullException(nameof(emprestimo));

            _context.Entry(emprestimo).Property(e => e.StatusEmprestimo).IsModified = true;

            return emprestimo;
        }

    }
}
