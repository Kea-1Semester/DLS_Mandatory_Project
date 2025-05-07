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
