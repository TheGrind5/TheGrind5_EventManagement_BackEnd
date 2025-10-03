using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheGrind5_EventManagement.Models;
class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args); // Tạo builder -> cấu hình app 

        // Thêm các service vào container -----------------------------------------


            // Cấu hình EventDbContext với In-Memory Database
            builder.Services.AddDbContext<EventDBContext>(options =>
            {
                options.UseInMemoryDatabase("AuthDb");
                // Sử dụng In-Memory Database để dễ dàng phát triển và kiểm thử
            });

            builder.Services.AddAuthorization(); // Thêm service ủy quyền
            builder.Services.AddIdentityApiEndpoints<IdentityUser>()
                    .AddEntityFrameworkStores<EventDBContext>(); // Thêm service Identity API Endpoints để hỗ trợ xác thực và quản lý người dùng    

        builder.Services.AddControllers(); // Thêm service controller để xử lý các yêu cầu HTTP
            builder.Services.AddEndpointsApiExplorer(); // Thêm service explore endpoint API để làm việc với Swagger
            builder.Services.AddSwaggerGen(); // Thêm service Swagger để tạo tài liệu API


        // Cấu hình Swagger/OpenAPI (nếu cần) --> để dễ dàng kiểm thử API -----------------------------------------
            var app = builder.Build(); // Build app
            app.MapIdentityApi<IdentityUser>(); // Map các endpoint của Identity API

        if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(); // Kích hoạt Swagger trong môi trường phát triển
                app.UseSwaggerUI(); // Kích hoạt giao diện người dùng Swagger
             }

            app.UseHttpsRedirection(); // Thêm middleware chuyển hướng HTTP sang HTTPS
            app.UseAuthentication(); // Thêm middleware xác thực

            app.MapControllers();   // Map các controller
            app.Run();              // Chạy ứng dụng
    }
}