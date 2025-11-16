using AutoMapper.Configuration;
using CSharp.Contract;
using CSharpWebApiSample.AppConfiguration;
using CSharpWebApiSample.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpWebApiSample.Services
{
    public class PetsApiClient
    {
        private readonly ILogger<PetsApiClient> logger;

        public HttpClient Client { get; }

        public PetsApiClient(HttpClient client, IOptions<ApiClientsConfig> clientsOptions, ILogger<PetsApiClient> logger)
        {
            client.BaseAddress = clientsOptions.Value.BaseAddress;
            client.DefaultRequestHeaders.Add("accept", "application/json");
            Client = client;
            this.logger = logger;
        }

        public async Task<IEnumerable<PetDto>> GetPetsAync(CancellationToken token = default)
        {
            var response = await Client.GetAsync("pet/findByStatus?status=available", token);

            if(!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                logger.LogError(errorText);
                return Enumerable.Empty<PetDto>();
            }

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IEnumerable<PetDto>>();

            return result.Take(10);
        }
    }
}
