using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace TheGrind5_EventManagement.Services
{
    public class EventSeedService
    {
        private readonly EventDBContext _context;

        public EventSeedService(EventDBContext context)
        {
            _context = context;
        }

        public async Task SeedSampleEventsAsync()
        {
            // Kiểm tra xem đã có events chưa
            var existingEvents = await _context.Events.AnyAsync();
            if (existingEvents)
            {
                return; // Đã có events rồi, không cần tạo lại
            }

            // Lấy admin user làm host
            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin@test.com");
            if (adminUser == null)
            {
                return; // Không có admin user
            }

            // Tạo sample events
            var events = new List<Event>
            {
                new Event
                {
                    HostId = adminUser.UserId,
                    Title = "WATERSOME 2025",
                    Description = "Sự kiện âm nhạc và giải trí lớn nhất năm 2025 tại TP.HCM. Với sự tham gia của nhiều nghệ sĩ nổi tiếng và các hoạt động thú vị.",
                    StartTime = new DateTime(2025, 11, 15, 15, 0, 0),
                    EndTime = new DateTime(2025, 11, 16, 22, 0, 0),
                    Location = "VAN PHUC CITY",
                    Category = "Music",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                },
                new Event
                {
                    HostId = adminUser.UserId,
                    Title = "EXSH Concert",
                    Description = "EM XINH SAY HI CONCERT ĐÊM 2 - Buổi biểu diễn đặc biệt với những ca khúc hit nhất.",
                    StartTime = new DateTime(2025, 10, 11, 19, 0, 0),
                    EndTime = new DateTime(2025, 10, 11, 22, 0, 0),
                    Location = "SÂN VẬN ĐỘNG QUỐC GIA MỸ ĐÌNH",
                    Category = "Music",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                },
                new Event
                {
                    HostId = adminUser.UserId,
                    Title = "TRANG TRANG A",
                    Description = "Biểu diễn nghệ thuật đặc sắc với những màn trình diễn độc đáo và ấn tượng.",
                    StartTime = new DateTime(2025, 10, 30, 19, 30, 0),
                    EndTime = new DateTime(2025, 10, 30, 21, 30, 0),
                    Location = "CAPITAL THEATRE",
                    Category = "Art",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.Events.AddRange(events);
            await _context.SaveChangesAsync();

            // Tạo ticket types cho mỗi event
            var savedEvents = await _context.Events.ToListAsync();
            var ticketTypes = new List<TicketType>();

            foreach (var evt in savedEvents)
            {
                ticketTypes.Add(new TicketType
                {
                    EventId = evt.EventId,
                    TypeName = "Vé thường",
                    Price = 500000,
                    Quantity = 200,
                    MinOrder = 1,
                    MaxOrder = 5,
                    SaleStart = DateTime.UtcNow,
                    SaleEnd = evt.StartTime.AddDays(-1),
                    Status = "Active"
                });

                ticketTypes.Add(new TicketType
                {
                    EventId = evt.EventId,
                    TypeName = "Vé VIP",
                    Price = 1000000,
                    Quantity = 50,
                    MinOrder = 1,
                    MaxOrder = 3,
                    SaleStart = DateTime.UtcNow,
                    SaleEnd = evt.StartTime.AddDays(-1),
                    Status = "Active"
                });
            }

            _context.TicketTypes.AddRange(ticketTypes);
            await _context.SaveChangesAsync();
        }
    }
}
