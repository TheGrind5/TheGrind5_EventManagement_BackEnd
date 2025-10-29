# 📊 SQL Changes - Feedback Feature

## 🔄 **Thay đổi Database**

### **Tables mới thêm:**

#### **1. Feedback Table**
```sql
CREATE TABLE Feedback (
    FeedbackId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    UserId INT NOT NULL,
    Comment NVARCHAR(2000) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    ParentFeedbackId INT NULL
)
```

**Relationships:**
- `EventId` → `Event.EventId` (Foreign Key, NO CASCADE)
- `UserId` → `User.UserID` (Foreign Key, NO CASCADE)  
- `ParentFeedbackId` → `Feedback.FeedbackId` (Self-referencing, cho Reply)

#### **2. FeedbackReaction Table**
```sql
CREATE TABLE FeedbackReaction (
    ReactionId INT IDENTITY(1,1) PRIMARY KEY,
    FeedbackId INT NOT NULL,
    UserId INT NOT NULL,
    ReactionType NVARCHAR(50) NOT NULL CHECK (ReactionType IN ('Like', 'Dislike')),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
```

**Relationships:**
- `FeedbackId` → `Feedback.FeedbackId` (Foreign Key, CASCADE DELETE)
- `UserId` → `User.UserID` (Foreign Key, NO CASCADE)
- **UNIQUE Constraint**: `(FeedbackId, UserId)` - Mỗi user chỉ có 1 reaction cho mỗi feedback

### **Indexes được tạo:**
```sql
CREATE INDEX IX_Feedback_EventId ON Feedback(EventId);
CREATE INDEX IX_Feedback_UserId ON Feedback(UserId);
CREATE INDEX IX_Feedback_ParentFeedbackId ON Feedback(ParentFeedbackId);
CREATE INDEX IX_FeedbackReaction_FeedbackId ON FeedbackReaction(FeedbackId);
CREATE INDEX IX_FeedbackReaction_UserId ON FeedbackReaction(UserId);
```

## 📝 **Cách apply SQL changes**

### **Cách 1: Chạy trực tiếp SQL Script**
1. Mở SQL Server Management Studio
2. Kết nối tới database của bạn
3. Chạy file: `src/Scripts/AddFeedbackTables.sql`

### **Cách 2: Dùng EF Core Migration**
```bash
# Dừng backend trước
# Sau đó chạy:
cd TheGrind5_EventManagement_BackEnd/src
dotnet ef migrations add AddFeedbackFeature
dotnet ef database update
```

## 🔍 **Database Relationships**

```
┌─────────────┐
│   Event     │
│  (Existing) │
└──────┬──────┘
       │ 1:N
       │
       ▼
┌─────────────────┐       ┌──────────────────┐
│    Feedback     │◄──1:N─│ FeedbackReaction │
│                 │       │                  │
│ - FeedbackId (PK│       │ - ReactionId (PK)│
│ - EventId (FK)  │       │ - FeedbackId (FK)│
│ - UserId (FK)   │       │ - UserId (FK)    │
│ - Comment       │       │ - ReactionType   │
│ - ParentId (FK) │       │ - CreatedAt      │
│ - Replies       │       │ - UNIQUE(User+FB)│
└─────────────────┘       └──────────────────┘
       │ 1:N                    │  N:1
       │                        │
       │ Self-reference         │
       │                        │
       └─────────────┬──────────┘
                     │ N:1
                     ▼
              ┌───────────┐
              │   User    │
              │ (Existing)│
              └───────────┘
```

## ✅ **Validation & Constraints**

1. **Feedback.Comment**: NVARCHAR(2000) - max 2000 characters
2. **FeedbackReaction.ReactionType**: CHECK constraint chỉ cho phép 'Like' hoặc 'Dislike'
3. **Unique**: Mỗi user chỉ có 1 reaction cho mỗi feedback
4. **Cascade Delete**: Khi xóa Feedback, tất cả Reactions sẽ tự động xóa

## 🚀 **Sau khi apply**

1. Backend sẽ tự động nhận diện 2 tables mới
2. Tất cả API endpoints sẽ hoạt động
3. Không cần restart backend (nếu đã stop để run migration)

## 📌 **Lưu ý**

- File SQL script: `src/Scripts/AddFeedbackTables.sql`
- Nếu dùng migration, cần stop backend trước
- Tất cả foreign keys đều có ON DELETE NO ACTION (trừ FeedbackReaction là CASCADE)
