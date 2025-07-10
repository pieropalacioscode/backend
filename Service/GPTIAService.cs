using IService;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class GPTIAservice :IGPTIAservice
    {

        private readonly HttpClient _httpClient;
        private readonly string apiKey = "OPENAI_API_KEY";

        public GPTIAservice()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<string> ObtenerDatosLibroDesdeGPT(string isbn)
        {
            string prompt = $@"
Eres un asistente experto en libros. A partir del ISBN '{isbn}', responde en el siguiente formato JSON:

{{
  ""libro"": {{
    ""titulo"": """",
    ""isbn"": """",
    ""descripcion"": """",
    ""estado"": true
  }},
  ""autor"": {{
    ""nombre"": """",
    ""apellido"": """"
  }}
}}

Si no encuentras información del libro, responde exactamente así: {{ ""error"": ""Libro no encontrado"" }}.";

            var payload = new
            {
                model = "gpt-3.5-turbo", // puedes usar "gpt-3.5-turbo" si quieres ahorrar
                messages = new[]
                {
                new {
                    role = "user",
                    content = prompt
                }
            },
                temperature = 0.2
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("chat/completions", content);
            var result = await response.Content.ReadAsStringAsync();

            var jsonResult = JObject.Parse(result);
            var text = jsonResult["choices"]?[0]?["message"]?["content"]?.ToString();
            // 👇 Validación para evitar null o cadena vacía
            if (string.IsNullOrWhiteSpace(text))
            {
                return "{ \"error\": \"Respuesta vacía desde OpenAI\" }";
            }
            Console.WriteLine("Respuesta cruda de GPT:");
            Console.WriteLine(text);
            return text;
        }
    }
}
