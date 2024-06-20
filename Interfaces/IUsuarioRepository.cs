using Biblioteca.Enum;
using Biblioteca.Models;
using Microsoft.VisualBasic;

namespace Biblioteca.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetUsuarios();
        Task<Usuario> GetUsuarioPorCPF(string cpf);
        Task<IEnumerable<Usuario>> GetUsuarios(string? nome, string? cpf, DateTime? dataNasc, string? email, string? celular, string? endereco, StatusUsuario? statusUsuario);
        Task<Usuario> CreateUsuario(Usuario usuario);
        Task<Usuario> UpdateUsuario(Usuario usuario);
        Task<Usuario> DeleteUsuario(string cpf);
        

    }
}
