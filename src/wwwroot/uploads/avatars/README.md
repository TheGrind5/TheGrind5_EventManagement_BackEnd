# Avatar Upload Directory

Thư mục này chứa các file avatar được upload từ người dùng.

## Cấu trúc file:
- Format: `avatar_{userId}_{timestamp}.{extension}`
- Ví dụ: `avatar_123_20251016113145.jpg`

## Bảo mật:
- Chỉ accept file ảnh (image/*)
- Kích thước tối đa: 5MB
- File được lưu với tên unique để tránh conflict

## URL Access:
- Static files được serve qua: `/uploads/avatars/{filename}`
- Frontend sẽ convert thành absolute URL: `http://localhost:5000/uploads/avatars/{filename}`


