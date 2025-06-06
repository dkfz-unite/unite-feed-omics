# FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
ARG USER
ARG TOKEN
WORKDIR /src
RUN dotnet nuget add source https://nuget.pkg.github.com/dkfz-unite/index.json -n github -u ${USER} -p ${TOKEN} --store-password-in-clear-text
COPY ["Unite.Omics.Annotations/Unite.Omics.Annotations.csproj", "Unite.Omics.Annotations/"]
COPY ["Unite.Omics.Indices/Unite.Omics.Indices.csproj", "Unite.Omics.Indices/"]
COPY ["Unite.Omics.Feed/Unite.Omics.Feed.csproj", "Unite.Omics.Feed/"]
COPY ["Unite.Omics.Feed.Web/Unite.Omics.Feed.Web.csproj", "Unite.Omics.Feed.Web/"]
RUN dotnet restore "Unite.Omics.Annotations/Unite.Omics.Annotations.csproj"
RUN dotnet restore "Unite.Omics.Indices/Unite.Omics.Indices.csproj"
RUN dotnet restore "Unite.Omics.Feed/Unite.Omics.Feed.csproj"
RUN dotnet restore "Unite.Omics.Feed.Web/Unite.Omics.Feed.Web.csproj"

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
