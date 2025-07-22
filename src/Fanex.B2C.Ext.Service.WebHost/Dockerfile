FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

WORKDIR "/src"
COPY ["/src/Fanex.B2C.Ext.Service.Core", "./Fanex.B2C.Ext.Service.Core"]
COPY ["/src/Fanex.B2C.Ext.Service.WebHost/Fanex.B2C.Ext.Service.WebHost.csproj", "./Fanex.B2C.Ext.Service.WebHost/Fanex.B2C.Ext.Service.WebHost.csproj"]

# Restore packages
WORKDIR "/src/Fanex.B2C.Ext.Service.WebHost"
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore -v n -r linux-musl-x64 "./Fanex.B2C.Ext.Service.WebHost.csproj"

COPY "/src/Fanex.B2C.Ext.Service.WebHost" .

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-musl-x64 --self-contained false -o /app/publish

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Don't run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR "/app"

COPY --from=builder /app/publish .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.B2C.Ext.Service.WebHost.dll"]