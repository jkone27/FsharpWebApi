using AutoMapper.Configuration;
using CSharpWebApiSample.AppConfiguration;
using CSharpWebApiSample.Domain;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpWebApiSample.Services
{
    public class PetsApiClient
    {
        public HttpClient Client { get; }

        public PetsApiClient(HttpClient client, IOptions<ApiClientsConfig> clientsOptions)
        {
            client.BaseAddress = clientsOptions.Value.BaseAddress;
            client.DefaultRequestHeaders.Add("accept", "application/json");
            Client = client;
        }

        public async Task<IEnumerable<PetDto>> GetPetsAync(CancellationToken token = default)
        {
            var response = await Client.GetAsync("pet/findByStatus?status=available", token);

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync
                <IEnumerable<PetDto>>(responseStream);

            return result.Take(10);
        }
    }
}
