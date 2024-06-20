using Biblioteca.Enum;

namespace Biblioteca.DTOs.Response
{
    public class MultaResponseDTO
    {
        public int EmprestimoId { get; set; }
        public int Id { get; set; }
        public DateTime InicioMulta { get; set; }
        public DateTime? TerminoMulta { get; set; }
        public int DiasAtrasados { get; set; }
        public float Valor { get; set; }
        public StatusMulta StatusMulta { get; set; } 
        
    }
}
