FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY CatalogoService.Domain/CatalogoService.Domain.csproj CatalogoService.Domain/
COPY CatalogoService.Application/CatalogoService.Application.csproj CatalogoService.Application/
COPY CatalogoService.Infrastructure/CatalogoService.Infrastructure.csproj CatalogoService.Infrastructure/
COPY CatalogoService.API/CatalogoService.API.csproj CatalogoService.API/

RUN dotnet restore CatalogoService.API/CatalogoService.API.csproj

COPY CatalogoService.Domain/ CatalogoService.Domain/
COPY CatalogoService.Application/ CatalogoService.Application/
COPY CatalogoService.Infrastructure/ CatalogoService.Infrastructure/
COPY CatalogoService.API/ CatalogoService.API/

RUN dotnet publish CatalogoService.API/CatalogoService.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

EXPOSE 8080

ENTRYPOINT ["dotnet", "CatalogoService.API.dll"]
