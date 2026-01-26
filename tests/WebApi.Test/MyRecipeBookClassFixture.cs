using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebApi.Test
{
    public class MyRecipeBookClassFixture : IClassFixture<CustomWebApplicationFactory>
    {   

        private readonly HttpClient _httpClient;
        public MyRecipeBookClassFixture(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        protected async Task<HttpResponseMessage> DoPost(string route, object request, string culture = "en")
        {
            ChangeCulture(culture);

            return await _httpClient.PostAsJsonAsync(route, request);
        }

        protected async Task<HttpResponseMessage> DoGet(string route, string token = "", string culture = "en")
        {
            ChangeCulture(culture);
            AuthorizeRequest(token);

            return await _httpClient.GetAsync(route);
        }

        protected async Task<HttpResponseMessage> DoPut(string route,  object request, string token = "", string culture = "en")
        {
            ChangeCulture(culture);
            AuthorizeRequest(token);
            return await _httpClient.PutAsJsonAsync(route, request);
        }

        private void ChangeCulture(string culture)
        {
            if (_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
            {
                _httpClient.DefaultRequestHeaders.Remove("Accept-Language");
            }
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);
        }

        private void AuthorizeRequest(string token)
        { 
            if(string.IsNullOrEmpty(token)) 
            {
                return;
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
