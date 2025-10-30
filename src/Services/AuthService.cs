using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Repositories;
using TheGrind5_EventManagement.Business;
using TheGrind5_EventManagement.Mappers;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;
        private readonly IUserMapper _userMapper;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IPasswordService passwordService,
            IUserMapper userMapper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordService = passwordService;
            _userMapper = userMapper;
        }

        public async Task<AuthDTOs.LoginResponse?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || !_passwordService.VerifyPassword(password, user.PasswordHash))
                return null;

            // Check if user is banned
            if (user.IsBanned)
            {
                throw new UnauthorizedAccessException("Tài khoản của bạn đã bị cấm");
            }

            var token = _jwtService.GenerateToken(user);
            var userDto = _userMapper.MapToUserReadDto(user);
            
            return new AuthDTOs.LoginResponse(token, DateTime.UtcNow.AddDays(7), userDto);
        }

        public async Task<AuthDTOs.UserReadDto> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var passwordHash = _passwordService.HashPassword(request.Password);
                var user = _userMapper.MapFromRegisterRequest(request, passwordHash);

                var createdUser = await _userRepository.CreateUserAsync(user);
                return _userMapper.MapToUserReadDto(createdUser);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in RegisterAsync: {ex.Message}", ex);
            }
        }

    }
}


