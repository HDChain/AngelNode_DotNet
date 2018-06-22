FROM microsoft/aspnetcore

EXPOSE 80
COPY /PartnerNode /publish

WORKDIR /publish

ENTRYPOINT ["dotnet", "PartnerNode.dll"]