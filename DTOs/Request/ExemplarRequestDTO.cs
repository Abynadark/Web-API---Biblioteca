using Biblioteca.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Biblioteca.DTOs.Request
{
    public class ExemplarRequestDTO
    {
        [Required(ErrorMessage = "O campo Id é obrigatório.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo ISBN-10 é obrigatório.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "ISBN-10 deve conter exatamente 10 dígitos e ser composto apenas por dígitos numéricos")]
        public string LivroISBN10 { get; set; }

        [JsonIgnore]
        public StatusExemplar StatusExemplar { get; set; } // ajuda a pessoa ter controle do status automaticamente
    }
}
