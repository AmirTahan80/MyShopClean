FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore MyShopClean.sln
RUN dotnet publish MyShop/MyShop.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
RUN mkdir -p /app/keys && chown -R app:app /app
COPY --from=build --chown=app:app /app/publish .
USER app
ENTRYPOINT ["dotnet", "MyShop.dll"]
