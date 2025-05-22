# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8084
EXPOSE 8085


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY AuthService/AuthService.csproj AuthService/
COPY DLS_Mandatory_Project/AuthClassLibrary/AuthClassLibrary.csproj AuthClassLibrary/
RUN dotnet restore "AuthService/AuthService.csproj"

COPY . .
WORKDIR "/src/AuthService"
RUN dotnet build "AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AuthService.dll"]

# # new lines added to copy the wait-for-it.sh script and set permissions

# # Add wait-for-it.sh script
# COPY scripts/wait-for-it.sh /app/wait-for-it.sh

# # Switch to root to set permissions to run the wait-for-it script
# USER root

# # Debug step to verify the file exists
# RUN ls -l /app

# # Make the script executable
# RUN chmod +x /app/wait-for-it.sh


# # Install 'dos2unix' to convert Windows line endings (CRLF) to Unix (LF)
# # Install 'netcat' (nc), which is required by wait-for-it.sh to check service availability
# # Convert the script's line endings, make it executable, then clean up to keep the image lean
#     #  instalation of dos2unix and netcat steps
#     ## Convert CRLF to LF in the script  
#     ## Make the script executable
#     ## Remove dos2unix after use to reduce image size
#     ## Clean up unused dependencies
#     ## Clear package cache
# RUN apt-get update && apt-get install -y dos2unix netcat-openbsd \
#     && dos2unix /app/wait-for-it.sh \            
#     && chmod +x /app/wait-for-it.sh \            
#     && apt-get remove -y dos2unix \              
#     && apt-get autoremove -y \                   
#     && apt-get clean                            
     

# # Switch back to non-root user to run entrypoint script
# USER $APP_UID


# # Set the default command to run the wait-for-it script with a timeout of 60 seconds
# ENTRYPOINT ["/app/wait-for-it.sh", "rabbitmq:5672", "-t", "60", "--", "dotnet", "AuthService.dll"]


