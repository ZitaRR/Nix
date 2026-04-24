FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG VERSION
ARG INFO

WORKDIR /app

RUN echo "Version=$VERSION"

COPY *.sln .
COPY Nix/Nix.csproj ./Nix/
COPY Nix.Infrastructure/Nix.Infrastructure.csproj ./Nix.Infrastructure/
COPY Nix.Domain/Nix.Domain.csproj ./Nix.Domain/
COPY Nix.Shared/Nix.Shared.csproj ./Nix.Shared/

RUN dotnet restore ./Nix/Nix.csproj

COPY . .

RUN VERSION=$(echo $VERSION | sed 's/^v//') && \
    dotnet publish ./Nix/Nix.csproj \
    -c Release \
    -o /app/publish \
    -p:Version=$VERSION \
    -p:InformationalVersion=$INFO

FROM mcr.microsoft.com/dotnet/runtime:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Nix.dll"]