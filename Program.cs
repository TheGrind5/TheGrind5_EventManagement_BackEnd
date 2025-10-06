using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Respositories;
using TheGrind5_EventManagement.Services;
class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args); // Tạo builder -> cấu hình app 

        // Thêm các service vào container -----------------------------------------


            // Cấu hình EventDbContext với SQL Server Database
            builder.Services.AddDbContext<EventDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                // Sử dụng SQL Server Database thật
            });

            builder.Services.AddAuthorization(); // Thêm service ủy quyền    

            builder.Services.AddControllers(); // Thêm service controller để xử lý các yêu cầu HTTP
            builder.Services.AddEndpointsApiExplorer(); // Thêm service explore endpoint API để làm việc với Swagger
            builder.Services.AddSwaggerGen(); // Thêm service Swagger để tạo tài liệu API
            
            // Cấu hình CORS để cho phép frontend gọi API
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "https://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Đăng ký Repository
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            
            // Đăng ký Services
            builder.Services.AddScoped<AuthService>();


        // Cấu hình Swagger/OpenAPI (nếu cần) --> để dễ dàng kiểm thử API -----------------------------------------
            var app = builder.Build(); // Build app
            

        if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(); // Kích hoạt Swagger trong môi trường phát triển
                app.UseSwaggerUI(); // Kích hoạt giao diện người dùng Swagger
             }

            app.UseHttpsRedirection(); // Thêm middleware chuyển hướng HTTP sang HTTPS
            app.UseCors("AllowFrontend"); // Sử dụng CORS policy
            app.UseAuthentication(); // Thêm middleware xác thực

            app.MapControllers();   // Map các controller
            app.Run();              // Chạy ứng dụng
    }
}