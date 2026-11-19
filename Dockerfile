# syntax=docker/dockerfile:1

# FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_HTTP_PORTS=80
RUN apt-get update && apt-get install -y \
    tabix \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
ARG USER
WORKDIR /src
COPY ["Unite.Omics.Annotations/Unite.Omics.Annotations.csproj", "Unite.Omics.Annotations/"]
COPY ["Unite.Omics.Indices/Unite.Omics.Indices.csproj", "Unite.Omics.Indices/"]
COPY ["Unite.Omics.Feed/Unite.Omics.Feed.csproj", "Unite.Omics.Feed/"]
COPY ["Unite.Omics.Feed.Web/Unite.Omics.Feed.Web.csproj", "Unite.Omics.Feed.Web/"]
RUN --mount=type=secret,id=github_token \
    dotnet nuget add source https://nuget.pkg.github.com/dkfz-unite/index.json -n github -u ${USER} -p "$(cat /run/secrets/github_token)" --store-password-in-clear-text \
    && dotnet restore "Unite.Omics.Annotations/Unite.Omics.Annotations.csproj" \
    && dotnet restore "Unite.Omics.Indices/Unite.Omics.Indices.csproj" \
    && dotnet restore "Unite.Omics.Feed/Unite.Omics.Feed.csproj" \
    && dotnet restore "Unite.Omics.Feed.Web/Unite.Omics.Feed.Web.csproj" \
    && dotnet nuget remove source github

FROM restore AS build
COPY . .
WORKDIR "/src/Unite.Omics.Feed.Web"
RUN dotnet build --no-restore "Unite.Omics.Feed.Web.csproj" -c Release

FROM build AS publish
RUN dotnet publish --no-build "Unite.Omics.Feed.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unite.Omics.Feed.Web.dll"]
