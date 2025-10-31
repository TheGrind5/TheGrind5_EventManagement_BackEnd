# 📋 HƯỚNG DẪN APPLY MIGRATION

**File migration:** `20251031112955_AddEventIdToOrderAndEventQuestionSupport`

**Thay đổi:**
1. ✅ Thêm cột `EventId` vào table `Order`
2. ✅ Thêm cột `OrderAnswers` vào table `Order`  
3. ✅ Tạo table `EventQuestion`
4. ✅ Tạo indexes và foreign keys

---

## 🔧 CÁCH 1: Chạy SQL Script (KHUYẾN NGHỊ)

### Bước 1: Mở SQL Server Management Studio
- Kết nối đến database của bạn

### Bước 2: Chạy SQL Script
- Mở file: `src/Scripts/ApplyEventQuestionMigration.sql`
- Copy toàn bộ nội dung
- Paste và Execute trong SSMS

**Script sẽ:**
- ✅ Tự động kiểm tra xem các thay đổi đã tồn tại chưa
- ✅ Chỉ apply nếu chưa có (safe cho production)
- ✅ Update EventId cho các Order hiện có từ OrderItems
- ✅ Đánh dấu migration đã apply trong `__EFMigrationsHistory`

---

## 🔧 CÁCH 2: Apply qua EF Core (Chỉ khi database mới)

Nếu database của bạn đã có migrations cũ và gặp conflict:

```bash
# Chạy SQL script để đánh dấu migrations cũ đã apply:
# INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
# VALUES ('20251030024709_FixDecimalPrecision', '9.0.9');
# ... (cho tất cả migrations trước 20251031112955)

# Sau đó mới apply migration mới:
dotnet ef database update 20251031112955_AddEventIdToOrderAndEventQuestionSupport
```

---

## ⚠️ LƯU Ý QUAN TRỌNG

1. **Backup database trước khi apply** (recommended)
2. **EventId default = 0**: Script sẽ update EventId từ OrderItems, nhưng nếu không có OrderItems thì sẽ = 0
3. **OrderAnswers**: Column nullable, không ảnh hưởng dữ liệu cũ

---

## ✅ KIỂM TRA SAU KHI APPLY

Sau khi chạy SQL script, kiểm tra:

```sql
-- Kiểm tra EventId column
SELECT TOP 5 OrderId, EventId, OrderAnswers FROM [Order];

-- Kiểm tra EventQuestion table
SELECT * FROM EventQuestion;

-- Kiểm tra indexes
SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('Order') AND name = 'IX_Order_EventId';
SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('EventQuestion') AND name = 'IX_EventQuestion_EventId';

-- Kiểm tra migration history
SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251031112955_AddEventIdToOrderAndEventQuestionSupport';
```

---

## 🎯 TÓM TẮT

**Recommended:** Chạy SQL script `ApplyEventQuestionMigration.sql` để apply migration an toàn.

**Alternative:** Nếu database hoàn toàn mới, có thể dùng `dotnet ef database update`.

**Status:** Backend code đã hoàn tất, chỉ cần apply migration vào database.

