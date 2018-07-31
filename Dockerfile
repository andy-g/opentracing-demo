FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet build -c Release -o out

FROM build AS publish
WORKDIR /src
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS runtime
EXPOSE 80
WORKDIR /app
COPY --from=publish /src/out ./
ENTRYPOINT ["dotnet", "opentracing-demo.dll"]