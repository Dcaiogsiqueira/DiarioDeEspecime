using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DiarioDeEspecime.Models
{
    public class Usuario : IdentityUser
    {
        public ICollection<UsuarioProjeto> UsuariosProjeto { get; set; }
        public ICollection<Projeto> ProjetosCriados { get; set; }


        [Required]
        [StringLength(50, ErrorMessage = "O nome de usuário deve ter no máximo 50 caracteres.")]
        [Display(Name = "Nome de Usuário")]

        public override string UserName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "O sobrenome deve ter no máximo 50 caracteres.")]
        [Display(Name = "Sobrenome")]
        public string Sobrenome { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        [StringLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres.")]
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public override string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "A senha deve ter no máximo 50 caracteres.")]
        [Display(Name = "Senha")]
        [DataType(DataType.Password)]
        [DisplayFormat(DataFormatString = "{0:****}", ApplyFormatInEditMode = true)]
        [Compare("SenhaConfirmacao", ErrorMessage = "As senhas não coincidem.")]
        public string Senha { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "A senha deve ter no máximo 50 caracteres.")]
        [Display(Name = "Confirmação de Senha")]
        [DataType(DataType.Password)]
        [DisplayFormat(DataFormatString = "{0:****}", ApplyFormatInEditMode = true)]
        public string SenhaConfirmacao { get; set; }

        [Required]
        [StringLength(13, ErrorMessage = "o numero do celular deve conter 13 digitos")]
        [Display(Name = "Celular")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:(##) #####-####}", ApplyFormatInEditMode = true)]
        [Phone(ErrorMessage = "O número de celular não é válido.")]
        [RegularExpression(@"^\d{2} \d{5} \d{4}$", ErrorMessage = "O número de celular deve estar no formato xx xxxxx xxxx.")]
        public string Celular { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date, ErrorMessage = "A data de nascimento deve ser uma data válida.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataNascimento { get; set; }

        public int? BiomaId { get; set; }
        public Bioma Bioma { get; set; }

        public NivelAcesso NivelAcesso { get; set; }
        public bool Ativo { get; set; } = true;

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }

    public enum NivelAcesso
    {
        Administrador,
        UsuarioComum
    }
}
