namespace Biblioteca.DTOs.Response
{
    public class RenovacaoResponseDTO
    {

        public int Id { get; set; }
        public DateTime DataRenovacao { get; set; }
        public DateTime NovaDataPrevista { get; set; }
        public int EmprestimoId { get; set; }
        
    }
}
