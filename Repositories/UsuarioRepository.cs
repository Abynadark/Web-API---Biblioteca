using Biblioteca.Enum;
using Biblioteca.Interfaces;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly Context _context;

        public UsuarioRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario> GetUsuarioPorCPF(string cpf)
        {
            
            return await _context.Set<Usuario>().SingleOrDefaultAsync(u => u.CPF == cpf);
            
           
        }


        public async Task<Usuario> CreateUsuario(Usuario usuario)
        {
            if (usuario is null)
                throw new ArgumentNullException(nameof(usuario));

            usuario.StatusUsuario = Enum.StatusUsuario.PodePegar3Livros;

            _context.Usuarios.Add(usuario);
           

            return usuario;
        }

        public async Task<Usuario> UpdateUsuario(Usuario usuario)
        {
            if (usuario is null)
                throw new ArgumentNullException(nameof(usuario));

            _context.Entry(usuario).State = EntityState.Modified;
           ;

            return usuario;
        }

        public async Task<Usuario> DeleteUsuario(string cpf)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.CPF == cpf);
            if (usuario is null)
                throw new ArgumentNullException(nameof(usuario));
            
            _context.Usuarios.Remove(usuario);
           
            
            return usuario;
        }

        public async Task<IEnumerable<Usuario>> GetUsuarios(string? nome, string? cpf, DateTime? dataNasc, string? email, string? celular, string? endereco, StatusUsuario? statusUsuario)
        {
            IQueryable<Usuario> query = _context.Usuarios;

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(u => u.Nome.Contains(nome));
            }

            if (!string.IsNullOrEmpty(cpf))
            {
                query = query.Where(u => u.CPF == cpf);
            }

            if (dataNasc.HasValue)
            {
                query = query.Where(u => u.DataNasc >= dataNasc.Value.Date); 
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.Email == email);
            }
            
            if (!string.IsNullOrEmpty(celular))
            {
                query = query.Where(u => u.Email == email);
            }

            if (!string.IsNullOrEmpty(endereco))
            {
                query = query.Where(u => u.Endereco.Contains(endereco));
            }

            if (statusUsuario.HasValue)
            {
                query = query.Where(u => u.StatusUsuario == statusUsuario);
            }

            return await query.ToListAsync();
            
        }
    }
}
