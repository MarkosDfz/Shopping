using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shopping.Data.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [Display(Name = "País")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres.")]
        public string Name { get; set; }

        public ICollection<State> States { get; set; }

        [Display(Name = "Provincias")]
        public int StatesNumber => States == null ? 0 : States.Count;
    }
}
