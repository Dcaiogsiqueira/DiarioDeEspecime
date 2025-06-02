using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiarioDeEspecime.Models.Repositories.Interfaces
{
    /// <summary>
    /// Interface para manipulação de dados relacionados a projetos.
    /// </summary>
    public interface IProjetoRepository
    {
        /// <summary>
        /// Obtém todos os projetos.
        /// </summary>
        /// <returns>Uma lista de todos os projetos.</returns>
        Task<IEnumerable<Projeto>> GetAllAsync();

        /// <summary>
        /// Obtém um projeto pelo seu identificador único.
        /// </summary>
        /// <param name="id">O identificador único do projeto.</param>
        /// <returns>O projeto correspondente ao identificador, ou null se não encontrado.</returns>
        Task<Projeto> GetByIdAsync(int id);

        /// <summary>
        /// Adiciona um novo projeto ao repositório.
        /// </summary>
        /// <param name="projeto">O projeto a ser adicionado.</param>
        Task AddAsync(Projeto projeto);

        /// <summary>
        /// Atualiza um projeto existente no repositório.
        /// </summary>
        /// <param name="projeto">O projeto com os dados atualizados.</param>
        Task UpdateAsync(Projeto projeto);

        /// <summary>
        /// Remove um projeto do repositório pelo seu identificador único.
        /// </summary>
        /// <param name="id">O identificador único do projeto a ser removido.</param>
        Task DeleteAsync(int id);

        /// <summary>
        /// Obtém todos os projetos criados por um usuário específico.
        /// </summary>
        /// <param name="userId">O identificador único do usuário.</param>
        /// <returns>Uma lista de projetos criados pelo usuário.</returns>
        Task<IEnumerable<Projeto>> GetByCreatorAsync(string userId);

        /// <summary>
        /// Obtém todos os projetos nos quais um usuário está incluído (como criador, editor ou leitor).
        /// </summary>
        /// <param name="userId">O identificador único do usuário.</param>
        /// <returns>Uma lista de projetos nos quais o usuário está incluído.</returns>
        Task<IEnumerable<Projeto>> GetByParticipantAsync(string userId);
    }
}
