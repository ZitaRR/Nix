FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY Nix/Nix.csproj ./src/
RUN dotnet restore ./src/Nix.csproj

COPY Nix/. ./src/

RUN dotnet publish ./src/Nix.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Nix.dll"]