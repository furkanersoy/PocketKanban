
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["PocketKanbanAPI.csproj", "./"]
RUN dotnet restore "PocketKanbanAPI.csproj"
COPY . .
RUN dotnet publish "PocketKanbanAPI.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PocketKanbanAPI.dll"]