FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS restore
ARG USER
ARG TOKEN
WORKDIR /src
RUN dotnet nuget add source https://nuget.pkg.github.com/dkfz-unite/index.json -n github -u ${USER} -p ${TOKEN} --store-password-in-clear-text
COPY ["Unite.Mutations.DataFeed.Domain/Unite.Mutations.DataFeed.Domain.csproj", "Unite.Mutations.DataFeed.Domain/"]
COPY ["Unite.Mutations.DataFeed.Web/Unite.Mutations.DataFeed.Web.csproj", "Unite.Mutations.DataFeed.Web/"]
RUN dotnet restore "Unite.Mutations.DataFeed.Domain/Unite.Mutations.DataFeed.Domain.csproj"
RUN dotnet restore "Unite.Mutations.DataFeed.Web/Unite.Mutations.DataFeed.Web.csproj"

FROM restore as build
COPY . .
WORKDIR "/src/Unite.Mutations.DataFeed.Web"
RUN dotnet build --no-restore "Unite.Mutations.DataFeed.Web.csproj" -c Release

FROM build AS publish
RUN dotnet publish --no-build "Unite.Mutations.DataFeed.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unite.Mutations.DataFeed.Web.dll"]