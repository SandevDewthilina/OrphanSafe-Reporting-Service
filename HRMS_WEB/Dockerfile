FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["HRMS_WEB/HRMS_WEB.csproj", "HRMS_WEB/"]
RUN dotnet restore "HRMS_WEB/HRMS_WEB.csproj"
COPY . .
WORKDIR "/src/HRMS_WEB"
RUN dotnet build "HRMS_WEB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HRMS_WEB.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HRMS_WEB.dll"]
