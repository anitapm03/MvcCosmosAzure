using Newtonsoft.Json;

namespace MvcCosmosAzure.Models
{
    public class Vehiculo
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Imagen { get; set; }

        //nuestra clase motor que sera clase dinamica
        public Motor Motor { get; set; }
    }
}
