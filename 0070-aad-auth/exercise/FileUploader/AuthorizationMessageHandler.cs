using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FileUploader
{
    public class GraphAPIAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public GraphAPIAuthorizationMessageHandler(IAccessTokenProvider provider,
            NavigationManager navigationManager)
            : base(provider, navigationManager)
        {
            ConfigureHandler(
                authorizedUrls: new[] { "https://graph.microsoft.com" },
                scopes: new[] { "https://graph.microsoft.com/User.Read" });
        }
    }

    public class FileUploaderAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public FileUploaderAuthorizationMessageHandler(IAccessTokenProvider provider,
            NavigationManager navigationManager)
            : base(provider, navigationManager)
        {
            ConfigureHandler(
                authorizedUrls: new[] { "https://func-ugtqqnnxh624m.azurewebsites.net",
                    "http://localhost:7071" },
                scopes: new[] { "app://b89440c8-04f2-43c5-a6e7-104b9b31b77c/Read",
                    "app://b89440c8-04f2-43c5-a6e7-104b9b31b77c/Write" });
        }
    }
}
