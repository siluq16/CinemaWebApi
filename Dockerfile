# 1. Sử dụng môi trường SDK để build code
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 2. Copy TOÀN BỘ thư mục gốc vào trong Docker
COPY . ./

# 3. Build thẳng vào file .csproj nằm bên trong thư mục con
# (Lưu ý: Thay chữ CinemaWebApi bằng đúng tên thư mục và tên file .csproj của bạn nếu nó khác)
RUN dotnet publish CinemaWebApi/CinemaWebApi.csproj -c Release -o out

# 4. Sử dụng môi trường Runtime nhẹ để chạy app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Cấu hình Port cho Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Chạy file thực thi
ENTRYPOINT ["dotnet", "CinemaWebApi.dll"]