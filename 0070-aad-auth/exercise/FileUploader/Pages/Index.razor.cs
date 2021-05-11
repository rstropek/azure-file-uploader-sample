using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Threading.Tasks;
using Microsoft.Graph;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace FileUploader.Pages
{
    public partial class Index
    {
        private string? userDisplayName;

        protected override async Task OnInitializedAsync()
        {
            // Note how we check if authenticated in code
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated ?? false)
            {
                // For demonstration purposes, get profile data from Graph API
                var httpClient = HttpClientFactory.CreateClient("GraphAPI");
                try
                {
                    // Check MS Graph Explorer (https://developer.microsoft.com/en-us/graph/graph-explorer)
                    // to learn about Graph API.
                    var dataRequest = await httpClient.GetAsync("https://graph.microsoft.com/beta/me");

                    if (dataRequest.IsSuccessStatusCode)
                    {
                        var response = await dataRequest.Content.ReadAsStringAsync();
                        var userData = JsonSerializer.Deserialize<User>(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                        userDisplayName = userData?.DisplayName;
                    }
                }
                catch (AccessTokenNotAvailableException ex)
                {
                    // Tokens are not valid - redirect the user to log in again
                    ex.Redirect();
                }
            }
        }
    }
}
