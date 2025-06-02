using System.ComponentModel.DataAnnotations;

namespace DiarioDeEspecime.Models
{

    public enum RegraProjeto { Criador, Editor, Leitor}
    public class UsuarioProjeto
    {

        public int ProjetoId { get; set; }
        public Projeto Projeto { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        [Display(Name = "Regra no Projeto")]
        public RegraProjeto Regra { get; set; }
    }
}
