using Biblioteca.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Biblioteca.DTOs.Request
{
    public class EmprestimoRequestDTO
    {

        [Required(ErrorMessage = "A Data de Emprestimo é obrigatório.")]
        [DataType(DataType.Date)]
        public DateTime DataEmprestimo { get; set; } // isso futuramente irá deixar de ser obrigatório e passará a ser gerado automaticamente pelo sistema


        [Required(ErrorMessage = "O CPF do usuário é obrigatório.")]
        [RegularExpression(@"\d{11}", ErrorMessage = "O CPF deve conter exatamente 11 dígitos.")]
        public string UsuarioCPF { get; set; }

        [Required(ErrorMessage = "O Id do exemplar é obrigatório.")]
        public int ExemplarId { get; set; }

        [JsonIgnore]
        public StatusEmprestimo StatusEmprestimo { get; set; } // ajuda a pessoa ter controle do status automaticamente

    }
}
