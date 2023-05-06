using System.Net.Http;
using System.Resources.NetStandard;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.Json;

namespace TheRoost.API.Services
{
    public class TranslateService : ITranslateService
    {
        private static string key;
        private static string endpoint;
        private static string location;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public TranslateService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            key = _configuration["AzureTranslation_Key"];
            endpoint = _configuration["AzureTranslation_Endpoint"];
            location = _configuration["AzureTranslation_Location"];
        }
        public TranslateJsonList TranslateToCZandSK(string text)
        {
            string route = "/translate?api-version=3.0&from=en&to=cs&to=sk";
            Uri requestUri = new Uri(endpoint + route);
            string textToTranslate = text;
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonContent.Create(body);
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", location);
            var output = client.PostAsync(requestUri, requestBody).Result;
            var myObject = output.Content.ReadFromJsonAsync<List<TranslateJsonList>>().Result;
            return myObject[0];
        }

        public void WriteToResourceFileSK(string key, string value)
        {
            //Check if file exists, if not skip
            if (File.Exists(@".\Resources\SharedResource.sk.resx"))
            {
                var resourceReader = new ResXResourceReader(@".\Resources\SharedResource.sk.resx");
                var node = resourceReader.GetEnumerator();
                var writer = new ResXResourceWriter(@".\Resources\SharedResource.sk.resx");

                while (node.MoveNext())
                {
                    writer.AddResource(node.Key.ToString(), node.Value.ToString());
                }
                var newNode = new ResXDataNode(key, value);
                writer.AddResource(newNode);
                writer.Generate();
                writer.Close();
            }
        }

        public void WriteToResourceFileCS(string key, string value)
        {
            //Check if file exists, if not skip
            if (File.Exists(@".\Resources\SharedResource.cs.resx"))
            {
                var resourceReader = new ResXResourceReader(@".\Resources\SharedResource.cs.resx");
                var node = resourceReader.GetEnumerator();
                var writer = new ResXResourceWriter(@".\Resources\SharedResource.cs.resx");
                while (node.MoveNext())
                {
                    writer.AddResource(node.Key.ToString(), node.Value.ToString());
                }
                var newNode = new ResXDataNode(key, value);
                writer.AddResource(newNode);
                writer.Generate();
                writer.Close();
            }
        }
    }
}
