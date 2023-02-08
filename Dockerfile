FROM mcr.microsoft.com/dotnet/sdk:7.0 as build

WORKDIR /src

COPY *.sln .
COPY SelfTraining/*.csproj ./SelfTraining/
RUN dotnet restore

COPY SelfTraining/. ./SelfTraining/
WORKDIR /src/SelfTraining/
RUN dotnet add package Microsoft.CodeAnalysis.Analyzers --version 3.3.3
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet","SelfTraining.dll"]