using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Biblioteca.DTOs.Request
{
    public class RenovacaoRequestDTO
    {
        [Required(ErrorMessage = "O campo ID de Emprestimo é obrigatório.")]
        public int EmprestimoId { get; set; }

        [Required(ErrorMessage = "O campo Data de Renovação é obrigatório.")] // isso futuramente irá deixar de ser obrigatório e passará a ser gerado automaticamente pelo sistema
        [DataType(DataType.Date)]
        public DateTime DataRenovacao { get; set; }

        [JsonIgnore]
        public DateTime NovaDataPrevista { get; set; }



    }
}
