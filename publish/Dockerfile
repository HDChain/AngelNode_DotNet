FROM microsoft/dotnet:2.1-aspnetcore-runtime 

EXPOSE 80
COPY /partnernode /publish

WORKDIR /publish

ENTRYPOINT ["dotnet", "PartnerNode.dll"]