# Stage 1: Build the .NET application (with Node.js)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-backend

# Install Node.js v22.12.0
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_22.x | bash - && \
    apt-get install -y nodejs=22.12.0-* && \
    rm -rf /var/lib/apt/lists/*

# Install Vite globally
RUN npm install -g vite

WORKDIR /src

# Copy project files
COPY mini-ITS.Web/mini-ITS.Web.csproj ./mini-ITS.Web/

# Restore .NET dependencies
RUN dotnet restore mini-ITS.Web/mini-ITS.Web.csproj

# Copy the rest of the source code
COPY . .

# Move to the ClientApp directory, install dependencies and build the frontend
WORKDIR /src/mini-ITS.Web/ClientApp

# Set environment variable IS_DOCKER to true
ENV IS_DOCKER=true

RUN npm install && npm run build

# Return to the main project directory
WORKDIR /src

# Publish the application (including frontend compilation)
RUN dotnet publish mini-ITS.Web/mini-ITS.Web.csproj -c Release -o /app/publish

# Stage 2: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

# Copy published files from the build stage
COPY --from=build-backend /app/publish .

# Expose the port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "mini-ITS.Web.dll"]