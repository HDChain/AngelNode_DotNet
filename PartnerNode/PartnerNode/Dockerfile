FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
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
