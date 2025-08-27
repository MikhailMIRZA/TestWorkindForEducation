FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TestWorkindForEducation.csproj", "."]
RUN dotnet restore "TestWorkindForEducation.csproj"
COPY . .
RUN dotnet build "TestWorkindForEducation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestWorkindForEducation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestWorkindForEducation.dll"]