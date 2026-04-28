using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveNow.MVC.Models
{
    public class VeiculoView
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Modelo é obrigatório.")]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "Placa é obrigatória.")]
        public string Placa { get; set; }

        [Required(ErrorMessage = "Valor da diária é obrigatório.")]
        public decimal ValorDiaria { get; set; }

        [Required(ErrorMessage = "Agência é obrigatória.")]
        public int AgenciaId { get; set; }
        public string? NomeAgencia { get; set; }
        public string? FotoUrl { get; set; }

        [NotMapped]
        public IFormFile FotoUpload { get; set; }

    }
}
