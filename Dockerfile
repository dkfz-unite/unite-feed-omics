FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
ARG USER
ARG TOKEN
WORKDIR /src
RUN dotnet nuget add source https://nuget.pkg.github.com/dkfz-unite/index.json -n github -u ${USER} -p ${TOKEN} --store-password-in-clear-text
COPY ["Unite.Genome.Annotations/Unite.Genome.Annotations.csproj", "Unite.Genome.Annotations/"]
COPY ["Unite.Genome.Indices/Unite.Genome.Indices.csproj", "Unite.Genome.Indices/"]
COPY ["Unite.Genome.Feed/Unite.Genome.Feed.csproj", "Unite.Genome.Feed/"]
COPY ["Unite.Genome.Feed.Web/Unite.Genome.Feed.Web.csproj", "Unite.Genome.Feed.Web/"]
RUN dotnet restore "Unite.Genome.Annotations/Unite.Genome.Annotations.csproj"
RUN dotnet restore "Unite.Genome.Indices/Unite.Genome.Indices.csproj"
RUN dotnet restore "Unite.Genome.Feed/Unite.Genome.Feed.csproj"
RUN dotnet restore "Unite.Genome.Feed.Web/Unite.Genome.Feed.Web.csproj"

FROM restore as build
COPY . .
WORKDIR "/src/Unite.Genome.Feed.Web"
RUN dotnet build --no-restore "Unite.Genome.Feed.Web.csproj" -c Release

FROM build AS publish
RUN dotnet publish --no-build "Unite.Genome.Feed.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unite.Genome.Feed.Web.dll"]