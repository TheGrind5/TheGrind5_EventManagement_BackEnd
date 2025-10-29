# 🎯 Feedback Feature - Tài liệu Hướng dẫn

## 📋 **Tổng quan**

Tính năng Feedback cho phép người dùng:
- Comment trên mỗi sự kiện
- Hiển thị avatar và tên người comment
- Thả like/dislike cho feedback
- Host có thể reply feedback

## 🏗️ **Cấu trúc Code**

### **1. Models** (src/Models/)
- `Feedback.cs` - Model cho feedback
- `FeedbackReaction.cs` - Model cho like/dislike

### **2. DTOs** (src/DTOs/)
- `FeedbackDTOs.cs` - Request/Response DTOs

### **3. Repositories** (src/Repositories/)
- `FeedbackRepository.cs` - Data access layer
- `IFeedbackRepository.cs` - Interface

### **4. Services** (src/Services/)
- `FeedbackService.cs` - Business logic
- `IFeedbackService.cs` - Interface

### **5. Controllers** (src/Controllers/)
- `FeedbackController.cs` - API endpoints

## 🔌 **API Endpoints**

### **GET /api/feedback/event/{eventId}**
Lấy danh sách feedback của sự kiện
- **Auth**: Optional (có auth mới hiển thị user's reaction)
- **Response**: List feedback với replies và reactions

### **POST /api/feedback**
Tạo feedback mới
- **Auth**: Required
- **Body**:
  ```json
  {
    "eventId": 1,
    "comment": "Sự kiện rất hay!",
    "parentFeedbackId": null
  }
  ```

### **DELETE /api/feedback/{feedbackId}**
Xóa feedback
- **Auth**: Required
- **Permission**: Chỉ owner mới xóa được

### **POST /api/feedback/reaction**
Thêm/Remove reaction (Like/Dislike)
- **Auth**: Required
- **Body**:
  ```json
  {
    "feedbackId": 1,
    "reactionType": "Like" // hoặc "Dislike"
  }
  ```
- **Logic**: Nếu đã like rồi, click lại sẽ remove; click dislike sẽ đổi sang dislike

### **POST /api/feedback/{feedbackId}/reply**
Host reply feedback
- **Auth**: Required
- **Body**:
  ```json
  {
    "comment": "Cảm ơn bạn đã feedback!"
  }
  ```

## 📊 **Database Schema**

### **Feedback Table**
```
- FeedbackId (PK)
- EventId (FK)
- UserId (FK)
- Comment (nvarchar(2000))
- ParentFeedbackId (nullable FK) - Nếu có = reply
- CreatedAt
- UpdatedAt
```

### **FeedbackReaction Table**
```
- ReactionId (PK)
- FeedbackId (FK)
- UserId (FK)
- ReactionType (Like/Dislike)
- CreatedAt
- UNIQUE(FeedbackId, UserId) - Mỗi user chỉ 1 reaction
```

## 🔄 **Flow hoạt động**

### **1. User Comment**
```
User → POST /api/feedback
      ↓
Service validates event exists
      ↓
Create feedback in DB
      ↓
Return feedback với avatar & name
```

### **2. User Like/Dislike**
```
User → POST /api/feedback/reaction
      ↓
Check existing reaction
      ↓
If same → Remove (toggle off)
If different → Update
If none → Create
```

### **3. Host Reply**
```
Host → POST /api/feedback/{id}/reply
      ↓
Validate feedback exists
      ↓
Create reply with ParentFeedbackId
      ↓
Return reply
```

## 📝 **Response Format**

### **FeedbackResponse**
```json
{
  "feedbackId": 1,
  "eventId": 1,
  "userId": 5,
  "userName": "Nguyễn Văn A",
  "userAvatar": "/uploads/avatar.jpg",
  "comment": "Sự kiện rất hay!",
  "createdAt": "2025-01-01T10:00:00",
  "updatedAt": null,
  "parentFeedbackId": null,
  "replies": [
    {
      "feedbackId": 2,
      "userId": 3,
      "userName": "Host Name",
      "comment": "Cảm ơn bạn!",
      "createdAt": "2025-01-01T11:00:00",
      "stats": {
        "likeCount": 0,
        "dislikeCount": 0,
        "userReaction": null
      }
    }
  ],
  "stats": {
    "likeCount": 15,
    "dislikeCount": 2,
    "userReaction": "Like" // null nếu chưa react
  }
}
```

## ✅ **Validation Rules**

1. **Comment**: Required, max 2000 chars
2. **EventId**: Must exist
3. **Delete**: Only owner can delete
4. **Reply**: Can only reply to feedback, not reply to reply
5. **ReactionType**: Must be "Like" or "Dislike"

## 🚀 **Cần làm tiếp**

1. **Create Migration**:
   ```bash
   dotnet ef migrations add AddFeedbackFeature
   dotnet ef database update
   ```

2. **Test APIs**: Test tất cả endpoints với Postman

3. **Frontend**: Tích hợp UI cho feedback
   - Hiển thị list feedback
   - Form comment
   - Like/dislike buttons
   - Reply form (cho host)

## 📌 **Lưu ý**

- Hiện tại chưa validate host khi reply (có thể thêm later)
- Frontend cần implement UI cho tất cả chức năng
- Cần test kỹ với database thật
