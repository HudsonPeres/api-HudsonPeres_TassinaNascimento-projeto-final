FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["api-HudsonPeres_TassinaNascimento-projeto-final.csproj", "."]
RUN dotnet restore


COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
EXPOSE 8081 

ENTRYPOINT ["dotnet", "api-HudsonPeres_TassinaNascimento-projeto-final.dll"]