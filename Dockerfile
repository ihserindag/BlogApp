# ============================================
# AŞAMA 1: BUILD (Derleme)
# ============================================
# .NET 8 SDK image'ini kullanıyoruz.
# SDK, projeyi derlemek için gerekli araçları içerir (dotnet build, dotnet publish).
# Bu aşama sadece derleme için kullanılır, son image'e dahil edilmez.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Önce sadece .csproj dosyasını kopyalıyoruz.
# Neden? Docker cache optimizasyonu. Eğer csproj değişmediyse,
# dotnet restore adımı cache'den gelir ve build hızlanır.
COPY BlogApp.csproj .
RUN dotnet restore

# Şimdi tüm kaynak kodları kopyalıyoruz.
COPY . .

# Projeyi Release modunda derleyip /app/publish klasörüne çıkartıyoruz.
# -c Release: Optimizasyonlu derleme
# -o /app/publish: Çıktı dizini
RUN dotnet publish -c Release -o /app/publish

# ============================================
# AŞAMA 2: RUNTIME (Çalıştırma)
# ============================================
# ASP.NET 8 runtime image'ini kullanıyoruz.
# Bu image SDK'dan çok daha küçüktür (~200MB vs ~800MB).
# ÖNEMLİ: Bu image ICU kütüphanesini içerir,
# bu yüzden "Exception.ToString() failed" hatası OLMAZ.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Build aşamasından derlenen dosyaları kopyalıyoruz.
COPY --from=build /app/publish .

# Ortam değişkenleri:
# ASPNETCORE_URLS: Uygulamanın dinleyeceği port (Coolify bu portu kullanır)
# ASPNETCORE_ENVIRONMENT: Production modunda çalışmasını sağlar
ENV ASPNETCORE_URLS=http://+:3000
ENV ASPNETCORE_ENVIRONMENT=Production

# Container'ın 3000 portunu dışarıya açıyoruz.
EXPOSE 3000

# Uygulama başlatma komutu.
# dotnet, BlogApp.dll dosyasını çalıştırır.
ENTRYPOINT ["dotnet", "BlogApp.dll"]
