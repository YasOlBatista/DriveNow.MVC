using System.ComponentModel.DataAnnotations;

namespace DriveNow.MVC.Models
{
    public class LocacaoView
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public int VeiculoId { get; set; }

        public DateTime DataRetirada { get; set; }

        public DateTime DataDevolucao { get; set; }

        public decimal ValorTotal { get; set; }
    }
}

