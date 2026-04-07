using System.ComponentModel.DataAnnotations;

namespace DriveNow.MVC.Models
{
    public class AgenciaView
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome Fantasia é obrigatório.")]
        public string NomeFantasia { get; set; }

        [Required(ErrorMessage = "CEP é obrigatório.")]
        public string Cep { get; set; }

        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string Localidade { get; set; }
        public string Uf { get; set; }
    }
}
