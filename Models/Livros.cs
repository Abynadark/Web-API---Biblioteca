using System;
using System.ComponentModel.DataAnnotations; // É como se fosse uma biblioteca com diferentes métodos de validação

namespace Biblioteca.Models
{
    public class Livro
    {
        public Guid Id { get; set; }

        [MaxLength(100)] // Restrição de 100 caracteres para o nome
        public string? Nome { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter exatamente 11 dígitos")] // Restrição de formato para CPF
        public string? CPF { get; set; }

        [EmailAddress(ErrorMessage = "O email fornecido não é válido")] // Restrição de formato para email
        public string? Email { get; set; }

        [Range(0, 2, ErrorMessage = "O status deve ser 0, 1 ou 2")] // Só aceita números de 0 a 2
        public int Status { get; set; }
    }
}
