using System.Text;
using System.Text.Json;


namespace noteApp.backend.Helpers
{
    public class CohereServices
    {
        private readonly string _apiKey = "goRDtOeOgF8LktumaaZiEKP7bwkftupkjTy53EPA";
        private readonly string _apiUrl = "https://api.cohere.com/v1/classify";

        public async Task<CohereApiResponse> GetChatResponseAsync(string userMessage)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var requestBody = new
                {
                    model = "72f6bbb2-39da-44fe-a546-39aca7bbb6c4-ft",
                    inputs = new[] { userMessage },
                };

                var jsonRequestBody = JsonSerializer.Serialize(requestBody);

                var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error en la solicitud: {response.StatusCode}");
                    return new CohereApiResponse { };
                }

                string responseContent = response.Content.ReadAsStringAsync().Result;
                CohereApiResponse objectResponse = JsonSerializer.Deserialize<CohereApiResponse>(responseContent);

                if (objectResponse != null)
                {
                    return objectResponse;
                } else
                {
                    return new CohereApiResponse { };
                }
            }
        }

        public class CohereApiResponse
        {
            public List<CohereClassification> classifications { get; set; }
        }

        public class CohereClassification
        {
            public string prediction { get; set; }
        }
    }
}
