using AuthClassLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace DLS_Mandatory_Project.Components.Account
{
    public class CustomServerAuthStateProvider : ServerAuthenticationStateProvider, IDisposable
    {
        private readonly PersistentComponentState state;
        private readonly IdentityOptions options;

        private readonly PersistingComponentStateSubscription subscription;

        private Task<AuthenticationState>? authenticationStateTask;

        public CustomServerAuthStateProvider(
            PersistentComponentState persistentComponentState, 
            IOptions<IdentityOptions> optionsAccessor)
        {
            state = persistentComponentState;
            options = optionsAccessor.Value;

            AuthenticationStateChanged += OnAuthenticationStateChanged;
            subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
        }

        private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            authenticationStateTask = task;
        }
        private async Task OnPersistingAsync()
        {
            if (authenticationStateTask is null)
            {
                throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
            }

            var authenticationState = await authenticationStateTask;
            var principal = authenticationState.User;

            if (principal.Identity?.IsAuthenticated == true)
            {
                var userId = principal.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;

                if (userId != null)
                {
                    state.PersistAsJson(nameof(AuthUserInfo), new AuthUserInfo
                    {
                        UserId = userId,
                    });
                }
            }
        }        

        public void Dispose()
        {
            subscription.Dispose();
            AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }
    }
}      
