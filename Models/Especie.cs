using System.ComponentModel.DataAnnotations;

namespace DiarioDeEspecime.Models
{
    public class Especie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O nome científico deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome Científico")]
        public string NomeCientifico { get; set; }

        [StringLength(50)]
        [Display(Name = "Reino")]
        public string Reino { get; set; }

        [StringLength(50)]
        [Display(Name = "Filo")]
        public string Filo { get; set; }

        [StringLength(50)]
        [Display(Name = "Classe")]
        public string Classe { get; set; }

        [StringLength(50)]
        [Display(Name = "Ordem")]
        public string Ordem { get; set; }

        [StringLength(50)]
        [Display(Name = "Família")]
        public string Familia { get; set; }

        [StringLength(50)]
        [Display(Name = "Gênero")]
        public string Genero { get; set; }

        [StringLength(100)]
        [Display(Name = "Nome da Espécie")]
        public string NomeEspecie { get; set; }

        [StringLength(500)]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [StringLength(500)]
        [Display(Name = "Distribuição")]
        public string Distribuicao { get; set; }

        public string FotoUrlSquare { get; set; }
        public string FotoUrlSmall { get; set; }
        public string FotoUrlMedium { get; set; }
        public string FotoUrlLarge { get; set; }
        public string FotoUrlOriginal { get; set; }

        public string FotoAutor { get; set; }

        public string CriadorId { get; set; }
        public Usuario Criador { get; set; }

        public List<Especime> Especimes { get; set; }
    }

}
