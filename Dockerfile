# Use the official SDK image for .NET
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# Set the working directory in the container
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the entire project files to the container
COPY . ./

# Build the API
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the port
EXPOSE 15000

# Command to run the API when the container starts
ENTRYPOINT ["dotnet", "miroirESPApi.dll"]
