using AuthClassLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace DLS_Mandatory_Project.Client
{
    internal class CustomClientAuthStateProvider : AuthenticationStateProvider
    {
        private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

        private readonly Task<AuthenticationState> authenticatedStateTask = defaultUnauthenticatedTask;

        public CustomClientAuthStateProvider(PersistentComponentState state)
        {
            if (!state.TryTakeFromJson<AuthUserInfo>(nameof(AuthUserInfo), out var authUserInfo) || authUserInfo is null)
            {
                return;
            }

            Claim[] claims = [
                new Claim(ClaimTypes.NameIdentifier, authUserInfo.UserId)
                ];

            authenticatedStateTask = Task.FromResult(
                new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                    authenticationType: nameof(CustomClientAuthStateProvider)))));
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticatedStateTask;
    }
}