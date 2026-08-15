using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OptiData.Application.Interfaces;

namespace OptiData.Infrastructure.Services
{
    public class OpenAIAssistantService : IAssistantService
    {   
        // built in c# tool designed to send and recieve data to and from a web address 
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        
        public OpenAIAssistantService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Pull the secure API key from the appsettings.json file
            _apiKey = configuration["OpenAI:ApiKey"];
        }

        public async Task<string> AskQuestionAsync(string userQuestion)
        {
            var url = "https://api.openai.com/v1/chat/completions";

            // We construct the strict "Data Bundle Expert" persona here
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new 
                    { 
                        role = "system", 
                        content = "You are a friendly Data Bundle Expert for the OptiData application. You must explicitly introduce yourself as such. Your goal is to help users understand how OptiData optimizes their data purchases. IMPORTANT CONTEXT: OptiData does not track real-world internet usage. Instead, we use a custom background worker to constantly simulate data consumption. Our Machine Learning engine then analyzes this simulated data to predict exactly how much data the user needs to buy next. If a user asks about how we get their data, you must explain this simulation process naturally and clearly. Do not use robotic, repetitive phrasing. Just make sure they understand the data is simulated by a background worker to train our prediction models. You must absolutely refuse to answer any general life questions, coding questions, or unrelated topics." 
                    },
                    new 
                    { 
                        role = "user", 
                        content = userQuestion 
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            
            // Attach the secure API key to the request
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync(url, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                
                // Parse the JSON response from OpenAI to extract just the text answer
                using var jsonDocument = JsonDocument.Parse(responseString);
                var answer = jsonDocument.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return answer;
            }

            return "I'm sorry, my connection to the OpenAI servers failed. Please try again later.";
        }
    }
}
