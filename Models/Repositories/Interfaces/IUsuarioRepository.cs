using DiarioDeEspecime.Models;

namespace DiarioDeEspecime.Repositories
{
    /// <summary>
    /// Interface para operações de acesso a dados relacionadas ao usuário.
    /// </summary>
    public interface IUsuarioRepository
    {
        /// <summary>
        /// Obtém todos os usuários cadastrados.
        /// </summary>
        /// <returns>Uma coleção de usuários.</returns>
        IEnumerable<Usuario> GetAll();

        /// <summary>
        /// Obtém um usuário pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do usuário (string).</param>
        /// <returns>O usuário correspondente ou null se não encontrado.</returns>
        Usuario GetById(string id);

        /// <summary>
        /// Adiciona um novo usuário.
        /// </summary>
        /// <param name="usuario">Usuário a ser adicionado.</param>
        void Add(Usuario usuario);

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        /// <param name="usuario">Usuário a ser atualizado.</param>
        void Update(Usuario usuario);

        /// <summary>
        /// Remove um usuário pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do usuário a ser removido (string).</param>
        void Delete(string id);

        /// <summary>
        /// Salva as alterações pendentes no repositório.
        /// </summary>
        void Save();
    }
}
