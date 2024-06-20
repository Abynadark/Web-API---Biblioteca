using Biblioteca.Enum;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations; 

namespace Biblioteca.Models
{
    public class Exemplar
    {
        public Exemplar()
        {
            Emprestimos = new Collection<Emprestimo>();
        }


        [Key]
        public int Id { get; set; }

        public StatusExemplar StatusExemplar { get; set; } // 0 = Disponível para empréstimo, 1 = Emprestado, 2 = Livro de Consulta(ou seja, não disponível para empréstimo), 3 = Danificado

   
        public string LivroISBN10 { get; set; }
        public Livro Livro { get; set; }


        public ICollection<Emprestimo> Emprestimos { get; set; }

    }
}
