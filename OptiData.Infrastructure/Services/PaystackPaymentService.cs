using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OptiData.Application.Interfaces;

namespace OptiData.Infrastructure.Services
{
    public class PaystackPaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PaystackPaymentService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            
            // Enterprise Architecture: Set up the base URL and Authorization headers 
            // so we can securely authenticate with Paystack's REST API.
            _httpClient.BaseAddress = new Uri("https://api.paystack.co/");
            
            var secretKey = _configuration["Paystack:SecretKey"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> ProcessPaymentAsync(decimal amount)
        {
            try
            {
                // Paystack expects the amount in kobo (base currency unit), so we multiply by 100
                var amountInKobo = (int)(amount * 100);

                var requestBody = new
                {
                    amount = amountInKobo,
                    email = "test-user@optidata.com", // Dummy email for the automated background purchase
                    reference = Guid.NewGuid().ToString(), // Unique reference for this particular transaction
                    callback_url = "https://example.com/payment/callback" // URL for paystack to send success message after transaction is completed
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                // We hit the 'initialize' endpoint. If our API keys are correct, Paystack will return a 200 OK
                // and a checkout link, proving the API integration works flawlessly.
                var response = await _httpClient.PostAsync("transaction/initialize", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    // The API call was successful. The transaction has been successfully initiated on Paystack.
                    Console.WriteLine("[Paystack API] Successfully initiated automated transaction on Paystack servers.");
                    return true;
                }

                var errorData = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[Paystack Error]: HTTP {response.StatusCode} - {errorData}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Paystack Exception]: {ex.Message}");
                return false;
            }
        }
    }
}
