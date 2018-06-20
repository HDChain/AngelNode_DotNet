FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY PartnerNode/PartnerNode.csproj PartnerNode/
RUN dotnet restore PartnerNode/PartnerNode.csproj
COPY . .
WORKDIR /src/PartnerNode
RUN dotnet build PartnerNode.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish PartnerNode.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PartnerNode.dll"]
