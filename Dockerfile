FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS restore
ARG USER
ARG TOKEN
WORKDIR /src
RUN dotnet nuget add source https://nuget.pkg.github.com/dkfz-unite/index.json -n github -u ${USER} -p ${TOKEN} --store-password-in-clear-text
COPY ["Unite.Mutations.Annotations/Unite.Mutations.Annotations.csproj", "Unite.Mutations.Annotations/"]
COPY ["Unite.Mutations.Indices/Unite.Mutations.Indices.csproj", "Unite.Mutations.Indices/"]
COPY ["Unite.Mutations.Feed/Unite.Mutations.Feed.csproj", "Unite.Mutations.Feed/"]
COPY ["Unite.Mutations.Feed.Web/Unite.Mutations.Feed.Web.csproj", "Unite.Mutations.Feed.Web/"]
RUN dotnet restore "Unite.Mutations.Annotations/Unite.Mutations.Annotations.csproj"
RUN dotnet restore "Unite.Mutations.Indices/Unite.Mutations.Indices.csproj"
RUN dotnet restore "Unite.Mutations.Feed/Unite.Mutations.Feed.csproj"
RUN dotnet restore "Unite.Mutations.Feed.Web/Unite.Mutations.Feed.Web.csproj"

FROM restore as build
COPY . .
WORKDIR "/src/Unite.Mutations.Feed.Web"
RUN dotnet build --no-restore "Unite.Mutations.Feed.Web.csproj" -c Release

FROM build AS publish
RUN dotnet publish --no-build "Unite.Mutations.Feed.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unite.Mutations.Feed.Web.dll"]