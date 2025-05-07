## Token Validation in Web API
To validate the token in your Web API, you can leverage the built-in middleware provided by ASP.NET Core. Below is an example of how token validation is configured and works:

### Configuration in `Program.cs`

The token validation is configured using the `AddAuthentication` and `AddJwtBearer` methods. Here's how it works:

    ```C#
    # Progam.cs
    ...
    var configuration = builder.Configuration;
    var jwt = configuration.GetSection("JWT");


    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Token"]!)),
                RequireExpirationTime = false, //TODO: Set to true for production
            };
        });

    ```
### Example `appsettings.json` for Development
For development purposes, you can add the following configuration to your `appsettings.json` file:

    ```json
    {
        "JWT": {
            "Token": "qsPGoCZpXEMaqXxcNGFOQMBeKHfRqx^irLmBb5i&XUZ7nwF%NHN0fBJg$J%ORVmXGs*oG0fOvKsM3h8r2#OXFC3D*CcI*CdrtAqJ",
            "Issuer": "JwtTestIssuer",
            "Audience": "JwtTestAudience",
            "DurationInMinutes": 60
        }
    }
    ```

### Notes for Development

- Use a simple and secure key for development, but ensure you replace it with a strong key in production.
- The `Issuer` and `Audience` should match the values used in your application during development.
- Avoid hardcoding sensitive information in your codebase. Use environment variables or secret management tools for production.

By including this configuration in your `appsettings.json`, you can easily test token validation during development.

### Authorization in Controllers
When implementing authorization in your controllers, you can use the `[Authorize]` attribute to restrict access to authenticated users. Additionally, you can specify roles using `[Authorize(Roles = "...")]` to restrict access to users with specific roles.

#### Examples:

1. **Restrict Access to Authenticated Users**:
   ```csharp
   [Authorize]
   public async Task<IActionResult> GetUserData()
   {
       // Logic for authenticated users
   }
   ```

   2. **Restrict Access to Specific Roles**:
   ```csharp
   [Authorize(Roles = "Admin")]
   public async Task<IActionResult> DeleteUser(Guid userId)
   {
       // Logic for admin users
   }
   ```

3. **Allow Anonymous Access**:
   For actions that should be accessible without authentication, use `[AllowAnonymous]`:
   ```csharp
   [AllowAnonymous]
   public IActionResult PublicEndpoint()
   {
       // Logic for public access
   }
   ```

#### Notes:
- Use `[Authorize]` at the controller level if all actions require authentication.
- Apply `[Authorize(Roles = "...")]` to specific actions if role-based access is needed.
- Ensure your role names match the roles defined in your authentication system.
- For development, you can test with simplified roles, but ensure proper role management in production.