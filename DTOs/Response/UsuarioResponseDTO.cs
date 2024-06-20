using Biblioteca.Enum;


namespace Biblioteca.DTOs.Response
{
    public class UsuarioResponseDTO
    {
        
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public DateTime DataNasc { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string Endereco { get; set; }
        public StatusUsuario StatusUsuario { get; set; }
        
    }
}
