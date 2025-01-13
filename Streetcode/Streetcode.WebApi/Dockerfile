FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Streetcode.WebApi/Streetcode.WebApi.csproj", "Streetcode.WebApi/"]
COPY ["Streetcode.DAL/Streetcode.DAL.csproj", "Streetcode.DAL/"]
COPY ["Streetcode.BLL/Streetcode.BLL.csproj", "Streetcode.BLL/"]
COPY ["UserService.BLL/UserService.BLL.csproj", "UserService.BLL/"]
COPY ["UserService.DAL/UserService.DAL.csproj", "UserService.DAL/"]
RUN dotnet restore "Streetcode.WebApi/Streetcode.WebApi.csproj"
COPY . .
WORKDIR "/src/Streetcode.WebApi"
RUN dotnet build "Streetcode.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Streetcode.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Streetcode.WebApi.dll"]
