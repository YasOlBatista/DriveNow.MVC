using System.ComponentModel.DataAnnotations;

namespace DriveNow.MVC.Models
{
    public class LocacaoView
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cliente é obrigatório.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Veículo é obrigatório.")]
        public int VeiculoId { get; set; }

        [Required(ErrorMessage = "Data de retirada é obrigatória.")]
        public DateTime DataRetirada { get; set; }

        [Required(ErrorMessage = "Data de devolução é obrigatória.")]
        public DateTime DataDevolucao { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
