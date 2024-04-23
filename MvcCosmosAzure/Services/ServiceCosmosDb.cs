using Microsoft.Azure.Cosmos;
using MvcCosmosAzure.Models;

namespace MvcCosmosAzure.Services
{
    public class ServiceCosmosDb
    {

        //dentro de ccosmos trabajamos con containers
        //dentro del container estan los items
        //desde este codigo vamos a crear un container
        //debemos recibir CosmosClient
        private CosmosClient clientCosmos;
        private Container containerCosmos;

        public ServiceCosmosDb(CosmosClient clientCosmos, 
            Container containerCosmos)
        {
            this.clientCosmos = clientCosmos;
            this.containerCosmos = containerCosmos;
        }

        //VAMOS A CREAR UN METODO PARA CREAR NUESTRA BASE DE DATOS
        //Y DENTRO DE LA BASE DE DATOS NUESTRO CONTENEDOR
        public async Task CreateDatabaseAsync()
        {
            //DEBEMOS CREAR UN CONTENEDOR MEDIANTE SUS PROPIEDADES
            ContainerProperties properties =
                new ContainerProperties("containercoches", "/id");
            //CREAMOS LA BASE DE DATOS QUE CONTENDRA EL CONTAINER
            await this.clientCosmos.CreateDatabaseIfNotExistsAsync
                ("vehiculoscosmos");
            //DESPUES DE CREAR LA BASE DE DATOS, CREAMOS EL CONTAINER
            await this.clientCosmos.GetDatabase("vehiculoscosmos")
                .CreateContainerIfNotExistsAsync(properties);
        }

        //metodo para insertar items
        public async Task InsertVehiculoAsync(Vehiculo car)
        {
            //en el moento de insertar obj dentro 
            //de cosmos, debemos indicar el obj y 
            //su partition key de forma explicita
            await this.containerCosmos.CreateItemAsync<Vehiculo>
                (car, new PartitionKey(car.Id));
        }

        //metodo para recuperar todos los vehiculos 
        public async Task<List<Vehiculo>> GetVehiculosAsync()
        {
            //los datos se recuperan mediante Iterator
            //es decir, un bucle que funciona mientras
            //existan registros
            var query = 
                this.containerCosmos.GetItemQueryIterator<Vehiculo>();
            List<Vehiculo> coches = new List<Vehiculo>();

            while(query.HasMoreResults)
            {
                var results = await query.ReadNextAsync();
                //dentro de results tendremos multiples datos
                coches.AddRange(results);
            }
            return coches;
        }

        //metodo para modificar un vehiculo
        public async Task UpdateVehiculoAsync(Vehiculo car)
        {
            //vamos a usar un metodo llamado Upsert
            //que permite modificar el item
            //si encuentra el item lo modifica, 
            //si no, lo inserta
            await this.containerCosmos.UpsertItemAsync<Vehiculo>
                (car, new PartitionKey(car.Id));
        }

        //metodo para eliminar un coche
        public async Task DeleteVehiculoAsync(string id)
        {
            //para eliminar necesitamos el id y su Partition key
            await this.containerCosmos.DeleteItemAsync<Vehiculo>
                (id, new PartitionKey(id));
        }

        //metodo para buscar por id 
        public async Task<Vehiculo> FindVehiculoAsync(string id)
        {
            ItemResponse<Vehiculo> response = await
                this.containerCosmos.ReadItemAsync<Vehiculo>
                (id, new PartitionKey(id));
            return response.Resource;    
        }

        public async Task<List<Vehiculo>>
            GetVehiculosMarcaAsync(string marca)
        {
            //los filtros se concatenan
            string sql =
                "select * from c where c.Marca='" + marca + "'";

            //se usa una clase llamada query definition
            //para aplicar lso filtros
            QueryDefinition definition =
                new QueryDefinition(sql);

            var query = 
                this.containerCosmos.GetItemQueryIterator<Vehiculo>
                (definition);

            List<Vehiculo> cars = new List<Vehiculo>();
            while (query.HasMoreResults)
            {
                var results = await query.ReadNextAsync();
                cars.AddRange(results);
            }
            return cars;
        }
    }
}
