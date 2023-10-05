FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR "/src"

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

COPY ["src/Fanex.B2C.Ext.Service.Core/Fanex.B2C.Ext.Service.Core.csproj", "./Fanex.B2C.Ext.Service.Core/"]
RUN dotnet restore "Fanex.B2C.Ext.Service.Core/Fanex.B2C.Ext.Service.Core.csproj" -v n

COPY ["src/Fanex.B2C.Ext.Service.WebHost/Fanex.B2C.Ext.Service.WebHost.csproj", "./Fanex.B2C.Ext.Service.WebHost/"]
RUN dotnet restore "Fanex.B2C.Ext.Service.WebHost/Fanex.B2C.Ext.Service.WebHost.csproj" -v n

COPY "/src" .

WORKDIR "/src/Fanex.B2C.Ext.Service.WebHost"
RUN dotnet build "Fanex.B2C.Ext.Service.WebHost.csproj" -c Release -o /app/build

RUN dotnet publish "Fanex.B2C.Ext.Service.WebHost.csproj" -c Release -o /app/publish

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-alpine

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR "/app"

COPY --from=builder /app/publish .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.B2C.Ext.Service.WebHost.dll"]
