FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TechMoves_Formative2/TechMoves_Formative2.csproj", "TechMoves_Formative2/"]
COPY ["TechMoves_API/TechMoves_API.csproj", "TechMoves_API/"]
RUN dotnet restore "TechMoves_Formative2/TechMoves_Formative2.csproj"
COPY . .
RUN dotnet build "TechMoves_Formative2/TechMoves_Formative2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TechMoves_Formative2/TechMoves_Formative2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechMoves_Formative2.dll"]