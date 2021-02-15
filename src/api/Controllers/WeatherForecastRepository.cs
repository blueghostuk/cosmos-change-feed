using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    public class WeatherForecastRepository : IHostedService
    {
        private const string _databaseName = "weatherDb";
        private const string _containerName = "weatherContainer";
        private readonly CosmosClient _cosmosClient;
        private readonly ILogger<WeatherForecastRepository> _logger;

        public WeatherForecastRepository(string connectionString, ILogger<WeatherForecastRepository> logger)
        {
            this._cosmosClient = new CosmosClient(connectionString);
            this._logger = logger;
        }

        public async Task<WeatherForecast> AddAsync(WeatherForecast weatherForecast, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            weatherForecast.Id = id;
            var container = await this.GetContainerAsync(cancellationToken);
            var response = await container.CreateItemAsync(weatherForecast, new PartitionKey(weatherForecast.Location), cancellationToken: cancellationToken);
            _logger.LogInformation($"CreateItemAsync cost: {response.RequestCharge}");
            return response;
        }

        public async Task<IEnumerable<WeatherForecast>> Get(CancellationToken cancellationToken)
        {
            var container = await this.GetContainerAsync(cancellationToken);
            var iterator = container.GetItemQueryIterator<WeatherForecast>();
            var response = await iterator.ReadNextAsync(cancellationToken);
            _logger.LogInformation($"GetItemQueryIterator cost: {response.RequestCharge}");
            return response;
        }

        public async Task UpdateAsync(WeatherForecast weatherForecast, CancellationToken cancellationToken)
        {
            var container = await this.GetContainerAsync(cancellationToken);
            var response = await container.UpsertItemAsync(weatherForecast, cancellationToken: cancellationToken);
            _logger.LogInformation($"UpsertItemAsync cost: {response.RequestCharge}");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await GetContainerAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task<Container> GetContainerAsync(CancellationToken cancellationToken)
        {
            var databaseResponse = await this._cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName, cancellationToken: cancellationToken);
            var database = databaseResponse.Database;
            _logger.LogInformation($"CreateDatabaseIfNotExistsAsync cost: {databaseResponse.RequestCharge}");

            var response = await database.CreateContainerIfNotExistsAsync(_containerName, $"/{nameof(WeatherForecast.Location)}", cancellationToken: cancellationToken);
            _logger.LogInformation($"CreateContainerIfNotExistsAsync cost: {response.RequestCharge}");
            return response.Container;
        }
    }
}