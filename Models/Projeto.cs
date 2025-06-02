using System.ComponentModel.DataAnnotations;

namespace DiarioDeEspecime.Models
{
    public class Projeto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O nome do projeto deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome do Projeto")]
        public string Nome { get; set; }

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Data de Início")]
        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }

        [Display(Name = "Data de Fim")]
        [DataType(DataType.Date)]
        public DateTime? DataFim { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "O status deve ter no máximo 50 caracteres.")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required]
        public string CriadorId { get; set; }
        public Usuario Criador { get; set; }

        public ICollection<UsuarioProjeto> UsuariosProjeto { get; set; }
        public ICollection<Especime> Especimes { get; set; }
        public ICollection<Especie> Especies { get; set; }
    }
}
