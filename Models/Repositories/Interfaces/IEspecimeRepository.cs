using System.Collections.Generic;

namespace DiarioDeEspecime.Models.Repositories.Interfaces
{
    /// <summary>
    /// Interface para operações de repositório relacionadas a espécimes biológicos.
    /// </summary>
    public interface IEspecimeRepository
    {
        /// <summary>
        /// Adiciona um novo espécime ao repositório.
        /// </summary>
        /// <param name="especime">O espécime a ser adicionado.</param>
        void Add(Especime especime);

        /// <summary>
        /// Obtém todos os espécimes do repositório.
        /// </summary>
        /// <returns>Uma lista de espécimes.</returns>
        IEnumerable<Especime> GetAll();

        /// <summary>
        /// Obtém um espécime pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do espécime.</param>
        /// <returns>O espécime correspondente ao ID, ou null se não encontrado.</returns>
        Especime GetById(int id);

        /// <summary>
        /// Atualiza um espécime existente no repositório.
        /// </summary>
        /// <param name="especime">O espécime a ser atualizado.</param>
        void Update(Especime especime);

        /// <summary>
        /// Remove um espécime do repositório.
        /// </summary>
        /// <param name="id">O ID do espécime a ser removido.</param>
        void Remove(int id);

        /// <summary>
        /// Obtém uma lista paginada de espécimes.
        /// </summary>
        /// <param name="page">O número da página (iniciando em 1).</param>
        /// <param name="pageSize">O tamanho da página.</param>
        /// <returns>Uma lista de espécimes para a página especificada.</returns>
        IEnumerable<Especime> GetPaged(int page, int pageSize);
    }
}
