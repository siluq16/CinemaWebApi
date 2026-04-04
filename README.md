# 🎬 Cinema Ticket Booking System (CinemaWebApi)

[![.NET Build](https://github.com/siluq16/CinemaWebApi/actions/workflows/dotnet.yml/badge.svg)](https://github.com/siluq16/CinemaWebApi/actions/workflows/dotnet.yml)

Một hệ thống đặt vé xem phim toàn diện (Full-stack), cho phép người dùng xem lịch chiếu, chọn ghế và đặt vé trực tuyến. Dự án được xây dựng với kiến trúc Client-Server, phân tách rõ ràng giữa giao diện người dùng và xử lý nghiệp vụ.

## 🚀 Tính năng nổi bật

* **Quản lý Phim & Lịch chiếu:** Hiển thị danh sách phim đang chiếu, sắp chiếu và chi tiết các suất chiếu.
* **Đặt vé & Chọn ghế trực tuyến:** Giao diện chọn ghế trực quan, cập nhật trạng thái ghế theo thời gian thực.
* **Xác thực người dùng:** Đăng ký, đăng nhập và quản lý hồ sơ cá nhân.
* **Lịch sử giao dịch:** Người dùng có thể xem lại các vé đã đặt.

## 🛠️ Công nghệ sử dụng

### Backend (CinemaWebApi)
* **Framework:** .NET 8.0 (C#)
* **Kiến trúc:** RESTful API
* **Database:** SQL Server & Entity Framework Core (Code-First)
* **Bảo mật:** JWT Authentication
* **CI/CD:** GitHub Actions (Automated Build & Test)

### Frontend
* **Framework:** [ReactJS]
* **Styling:** Tailwind CSS]

## ⚙️ Hướng dẫn cài đặt (Local Development)

### Yêu cầu hệ thống
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
* [Node.js](https://nodejs.org/) (nếu dùng framework frontend)

### Các bước chạy Backend
1. Clone repository này về máy:
   ```bash
