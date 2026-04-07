using System.ComponentModel.DataAnnotations;

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
        public string? Agencia { get; set; }

    }
}
