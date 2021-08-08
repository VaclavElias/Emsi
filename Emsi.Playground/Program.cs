using Emsi.Playground.Dtos;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Emsi.Playground
{
    public class Program
    {
        private static IConfigurationRoot Configuration { get; set; }

        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddUserSecrets<Program>();

            Configuration = builder.Build();

            await GetStatusAsync();
            await GetMetaAsync();
            await GetAllVersionsAsync();
        }

        private static async Task GetStatusAsync()
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

            StatusDto? dto;

            try
            {
                dto = await client.GetFromJsonAsync<StatusDto>($"{GetBaseUrl()}/skills/status");
            }
            catch (Exception e)
            {
                Console.WriteLine($"My message: {e.Message}");
                throw;
            }

            if (dto is not null)
            {
                Console.WriteLine(dto.Data.Healthy);
                Console.WriteLine(dto.Data.Message);
            }
        }

        private static async Task GetMetaAsync()
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

            MetaDto? dto;

            try
            {
                dto = await client.GetFromJsonAsync<MetaDto>($"{GetBaseUrl()}/skills/meta");
            }
            catch (Exception e)
            {
                Console.WriteLine($"My message: {e.Message}");
                throw;
            }

            if (dto is not null)
            {
                Console.WriteLine(dto.Data.Attribution.Title);
                Console.WriteLine(dto.Data.Attribution.Body);
                Console.WriteLine(dto.Data.LatestVersion);
            }
        }

        //GetVersionsAsync() GET
        //GetVersionAsync(string version) GET
        //GetChangesAsync(string version) GET
        //GetRelatedAsync(List ids) POST
        //this one has rate limit in place
        //GetSkillsAndTraceFromDocumentAsync(string version) from../extract/trace endpoint  POST

        private static async Task GetAllVersionsAsync()
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

            DataDto? dto;

            try
            {
                dto = await client.GetFromJsonAsync<DataDto>($"{GetBaseUrl()}/skills/versions");
            }

            catch (Exception e)
            {
                Console.WriteLine($"My message: {e.Message}");
                throw;
            }

            if (dto is not null)
            {
                Console.WriteLine("\nList of all versions available:");
                foreach (var data in dto.Data)
                    Console.WriteLine(data);
            }


        }

        private static string GetToken()
        {
            return Configuration["EmsiSettings:AccessToken"];
        }

        private static string GetBaseUrl()
        {
            return Configuration["EmsiSettings:Url"];
        }
    }
}
