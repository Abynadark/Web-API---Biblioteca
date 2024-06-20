using Biblioteca.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Biblioteca.DTOs.Request
{
    public class MultaRequestDTO
    {

        [Required(ErrorMessage = "O campo 'InicioMulta' é obrigatório.")]
        [DataType(DataType.Date, ErrorMessage = "O campo 'Inicio da Multa' deve ser uma data válida.")]
        public DateTime InicioMulta { get; set; }

        [JsonIgnore]
        //[Required(ErrorMessage = "O campo 'TerminoMulta' é obrigatório.")]
        //[DataType(DataType.Date, ErrorMessage = "O campo 'Termino da Multa' deve ser uma data válida.")]
        public DateTime? TerminoMulta { get; set; }

        [Required(ErrorMessage = "O campo 'DiasAtrasados' é obrigatório.")]
        public int DiasAtrasados { get; set; }

        [Required(ErrorMessage = "O campo 'Valor' é obrigatório.")]
        public float Valor { get; set; }

        [Required(ErrorMessage = "O campo 'Status' é obrigatório.")]

        [JsonIgnore]
        public StatusMulta StatusMulta { get; set; } // 0 = Pendente, 1 = Paga

        // Chave Estrangeira
        [Required(ErrorMessage = "O campo 'EmprestimoId' é obrigatório.")]
        public int EmprestimoId { get; set; }
    }
}
