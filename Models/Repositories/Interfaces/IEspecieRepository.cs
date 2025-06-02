using System.Collections.Generic;

namespace DiarioDeEspecime.Models.Repositories.Interfaces
{
    public interface IEspecieRepository
    {
        /// <summary>
        /// Adiciona uma nova espécie ao repositório.
        /// </summary>
        /// <param name="especie">A espécie a ser adicionada.</param>
        void Add(Especie especie);

        /// <summary>
        /// Obtém todas as espécies do repositório.
        /// </summary>
        /// <returns>Uma lista de espécies.</returns>
        IEnumerable<Especie> GetAll();

        /// <summary>
        /// Obtém uma espécie pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da espécie.</param>
        /// <returns>A espécie correspondente ao ID, ou null se não encontrado.</returns>
        Especie GetById(int id);

        /// <summary>
        /// Atualiza uma espécie existente no repositório.
        /// </summary>
        /// <param name="especie">A espécie a ser atualizada.</param>
        void Update(Especie especie);

        /// <summary>
        /// Remove uma espécie do repositório.
        /// </summary>
        /// <param name="id">O ID da espécie a ser removida.</param>
        void Remove(int id);
    }
}
