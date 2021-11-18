FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OpenFoodFactService.csproj", "./"]
RUN dotnet restore "OpenFoodFactService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "OpenFoodFactService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenFoodFactService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenFoodFactService.dll"]
