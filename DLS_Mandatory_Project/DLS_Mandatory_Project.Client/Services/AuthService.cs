using AuthClassLibrary;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Json;

namespace DLS_Mandatory_Project.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> Login(LogIn logIn)
        {
            var response = await _httpClient.PostAsJsonAsync("/Auth/Login", logIn);

            if (!response.IsSuccessStatusCode)
                return false;

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResult>();

            if (authResponse is not null)
            {
                Console.WriteLine(authResponse.Token);
                await _localStorage.SetItemAsync("authToken", authResponse.Token);
                ((CustomClientAuthStateProvider)_authStateProvider).UpdateAuthState(authResponse.Token);
                return true;
            }

            return false;
        }
    }
}
