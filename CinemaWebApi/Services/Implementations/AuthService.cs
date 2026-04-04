using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Helpers; 
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces; 
using CinemaWebApi.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CinemaWebApi.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo; 
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepo, IConfiguration configuration, IEmailService emailService)
        {
            _userRepo = userRepo;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("Email hoặc mật khẩu không chính xác.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new Exception("Email hoặc mật khẩu không chính xác.");
            }

            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Role = user.Role,
                Token = token
            };
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            if (user == null) return true;

            string rawToken = SecurityHelper.GenerateRandomToken();

            string hashedToken = SecurityHelper.HashToken(rawToken);

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = hashedToken,
                ExpiresAt = DateTime.Now.AddMinutes(15),
                CreatedAt = DateTime.Now
            };

            await _userRepo.AddPasswordResetTokenAsync(resetToken);
            await _userRepo.SaveChangesAsync();

            string resetLink = $"http://localhost:3000/reset-password?token={rawToken}";

            string emailBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px; border-radius: 10px;'>
                    <h2 style='color: #E50914; text-align: center;'>ĐẶT LẠI MẬT KHẨU</h2>
                    <p>Xin chào <strong>{user.FullName}</strong>,</p>
                    <p>Hệ thống nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Vui lòng click vào nút bên dưới để tạo mật khẩu mới:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}' style='padding: 12px 25px; background-color: #E50914; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>ĐẶT LẠI MẬT KHẨU</a>
                    </div>
                    <p style='color: #555; font-size: 13px;'><i>Link này sẽ hết hạn sau 15 phút. Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email và bảo mật tài khoản của bạn.</i></p>
                </div>";

            await _emailService.SendEmailAsync(user.Email, "Hỗ trợ đặt lại mật khẩu - Cinema", emailBody);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            string hashedInputToken = SecurityHelper.HashToken(request.Token);

            var resetRecord = await _userRepo.GetValidResetTokenAsync(hashedInputToken);

            if (resetRecord == null)
                throw new Exception("Đường dẫn đặt lại mật khẩu không hợp lệ, đã hết hạn hoặc đã được sử dụng.");

            var user = await _userRepo.GetByIdAsync(resetRecord.UserId);
            if (user == null)
                throw new Exception("Không tìm thấy tài khoản người dùng.");

            user.PasswordHash = HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.Now;

            resetRecord.UsedAt = DateTime.Now;

            _userRepo.UpdateUser(user);
            _userRepo.UpdatePasswordResetToken(resetRecord);

            await _userRepo.SaveChangesAsync();
            return true;
        }

        public async Task<LoginResponse> GoogleLoginAsync(GoogleLoginRequest request)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _configuration["Authentication:Google:ClientId"]! }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
            }
            catch (Exception)
            {
                throw new Exception("Mã xác thực Google không hợp lệ hoặc đã hết hạn.");
            }

            var oauthAccount = await _userRepo.GetOAuthAccountAsync("google", payload.Subject);
            User? user;

            if (oauthAccount != null)
            {
                user = await _userRepo.GetByIdAsync(oauthAccount.UserId);
                if (user == null || !user.IsActive) throw new Exception("Tài khoản đã bị khóa.");
            }
            else
            {
                user = await _userRepo.GetByEmailAsync(payload.Email);

                var newOauth = new OauthAccount
                {
                    Provider = "google",
                    ProviderUserId = payload.Subject,
                    LinkedAt = DateTime.Now
                };

                if (user != null)
                {
                    newOauth.UserId = user.Id;
                    await _userRepo.AddOAuthAccountAsync(newOauth);
                    await _userRepo.SaveChangesAsync();
                }
                else
                {
                    user = new User
                    {
                        FullName = payload.Name,
                        Email = payload.Email,
                        AvatarUrl = payload.Picture, // Lấy luôn ảnh đại diện từ Google
                        Role = "customer",
                        IsVerified = true, // Đăng nhập Google thì chắc chắn email xịn
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    await _userRepo.CreateUserWithOAuthAsync(user, newOauth);
                }
            }

            // 4. Trả về chuẩn JWT của hệ thống chúng ta (Khách hàng dùng Web sẽ không thấy sự khác biệt)
            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Role = user.Role,
                Token = token
            };
        }


        // ==========================================
        // HÀM PRIVATE: NHÀO NẶN RA CHUỖI JWT (Giữ nguyên)
        // ==========================================
        private string GenerateJwtToken(Models.User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}