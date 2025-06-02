using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace DiarioDeEspecime.Controllers
{
    public class TaxonomyController : Controller
    {
        private static readonly HttpClient client = new HttpClient();

        public IActionResult Buscar()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> BuscarPorNome(string nome)
        {
            try
            {
                // URL de busca com os parâmetros fornecidos
                string searchUrl = $"https://api.inaturalist.org/v1/search?q={Uri.EscapeDataString(nome)}&sources=taxa&include_taxon_ancestors=true&locale=pt-br&preferred_place_id=6878";
                var response = await client.GetStringAsync(searchUrl);
                var json = JObject.Parse(response);

                // Obtém o primeiro resultado do tipo "Taxon"
                var taxa = json["results"]?
                    .FirstOrDefault(r => r["type"]?.ToString() == "Taxon")?["record"];
                if (taxa == null)
                    return Json(new { success = false });

                // Extrai o nome comum preferido (preferred_common_name)
                string preferredCommonName = taxa["preferred_common_name"]?.ToString() ?? "Nome comum não disponível";

                // Extrai outros dados taxonômicos
                var ancestors = taxa["ancestors"] as JArray ?? new JArray();
                var allTaxa = ancestors.ToList();
                allTaxa.Add(taxa);

                string GetRank(string rank)
                {
                    var found = allTaxa.FirstOrDefault(a => a["rank"]?.ToString() == rank);
                    return found?["name"]?.ToString();
                }

                // Busca a primeira foto e os tamanhos disponíveis
                var taxonPhotos = taxa["taxon_photos"] as JArray;
                JObject photo = null;
                string author = null;
                if (taxonPhotos != null && taxonPhotos.Count > 0)
                {
                    photo = taxonPhotos[0]?["photo"] as JObject;
                    author = photo?["attribution"]?.ToString();
                }

                return Json(new
                {
                    success = true,
                    nomeEspecie = preferredCommonName, // Nome comum preferido
                    reino = GetRank("kingdom"),
                    filo = GetRank("phylum"),
                    classe = GetRank("class"),
                    ordem = GetRank("order"),
                    familia = GetRank("family"),
                    genero = GetRank("genus"),
                    especie = GetRank("species"),
                    nomeCientifico = taxa["name"]?.ToString(),
                    fotos = photo == null ? null : new
                    {
                        square = photo["square_url"]?.ToString(),
                        small = photo["small_url"]?.ToString(),
                        medium = photo["medium_url"]?.ToString(),
                        large = photo["large_url"]?.ToString(),
                        original = photo["original_url"]?.ToString()
                    },
                    autorImagem = author
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}