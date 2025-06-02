using System.ComponentModel.DataAnnotations;

namespace DiarioDeEspecime.Models
{
    public class Especime
    {
        [Key]
        [Display(Name = "ID do Espécime")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Espécie")]
        public int EspecieId { get; set; }
        public Especie Especie { get; set; }

        [Required]
        [Display(Name = "Sexo")]
        public string Sexo { get; set; } // Masculino, Feminino, Indeterminado

        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        [Display(Name = "Altitude (m)")]
        public double? Altitude { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O local deve ter no máximo 100 caracteres.")]
        [Display(Name = "Local do Encontro")]
        public string LocalEncontro { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O nome do coletor deve ter no máximo 100 caracteres.")]
        [Display(Name = "Coletor")]
        public string Coletor { get; set; }

        [Display(Name = "Data e Hora do Encontro")]
        public DateTime? DataHoraEncontro { get; set; }

        [StringLength(100)]
        [Display(Name = "Condições Climáticas")]
        public string CondicoesClimaticas { get; set; }

        [Display(Name = "Peso (kg)")]
        public double? Peso { get; set; }

        [Display(Name = "Comprimento (cm)")]
        public double? Comprimento { get; set; }

        [StringLength(500)]
        [Display(Name = "Comportamento Observado")]
        public string Comportamento { get; set; }

        [StringLength(500)]
        [Display(Name = "Análise Física")]
        public string AnaliseFisica { get; set; }

        [StringLength(200)]
        [Display(Name = "Observações Adicionais")]
        public string Observacoes { get; set; }

        [StringLength(200)]
        [Display(Name = "Foto do Espécime")]
        public string FotoUrl { get; set; }

        public string CriadorId { get; set; } 
        public Usuario Criador { get; set; } 

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public int ProjetoId { get; set; }
        public Projeto Projeto { get; set; }

    }
}
