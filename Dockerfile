FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG VERSION
ARG INFO

WORKDIR /app

RUN echo "Version=$VERSION"

COPY Nix/Nix.csproj ./src/
RUN dotnet restore ./src/Nix.csproj

COPY Nix/. ./src/

RUN VERSION=$(echo $VERSION | sed 's/^v//') && \
    dotnet publish ./src/Nix.csproj \
    -c Release \
    -o /app/publish \
    -p:Version=$VERSION \
    -p:InformationalVersion=$VERSION-$INFO

FROM mcr.microsoft.com/dotnet/runtime:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Nix.dll"]