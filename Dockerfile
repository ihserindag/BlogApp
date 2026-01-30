# 1. Derleme Aşaması (Build SDK 9.0)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala ve kütüphaneleri geri yükle
COPY . .
RUN dotnet restore

# Projeyi derle (BlogApp kısmını .csproj dosyanın adıyla aynı yap)
RUN dotnet publish -c Release -o /app/publish

# 2. Çalıştırma Aşaması (Runtime 9.0)
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Coolify için gerekli port ve ortam ayarları
ENV ASPNETCORE_URLS=http://+:3000
EXPOSE 3000

# BlogApp.dll kısmını derlenen DLL adınla değiştir
ENTRYPOINT ["dotnet", "BlogApp.dll"]