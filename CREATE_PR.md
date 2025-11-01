# Hướng dẫn tạo Pull Request

## Tình trạng hiện tại
- **Nhánh hiện tại**: `ThuNghiem_Thien`
- **Nhánh đích**: `master`
- **Kết quả merge test**: "Already up to date" - **KHÔNG CÓ CONFLICT** ✅

## Cách tạo Pull Request

### Cách 1: Qua GitHub Web Interface (Khuyên dùng)

1. Đảm bảo đã push nhánh lên remote:
   ```bash
   git push origin ThuNghiem_Thien
   ```

2. Truy cập GitHub repository:
   https://github.com/TheGrind5/TheGrind5_EventManagement_BackEnd

3. Click vào "Compare & pull request" khi GitHub hiển thị banner

4. Hoặc tạo PR thủ công:
   - Vào tab "Pull requests"
   - Click "New pull request"
   - Base: `master` ← Compare: `ThuNghiem_Thien`
   - Điền thông tin PR:
     - Title: `feat: Merge ThuNghiem_Thien branch into master`
     - Description: Mô tả các thay đổi chính
   - Click "Create pull request"

### Cách 2: Sử dụng GitHub CLI (nếu đã cài đặt)

```bash
gh pr create --base master --head ThuNghiem_Thien --title "feat: Merge ThuNghiem_Thien branch into master" --body "Merge nhánh ThuNghiem_Thien vào master"
```

### Cách 3: Sử dụng script PowerShell (sau khi giải phóng git log)

Chạy file `create_pr.ps1` trong thư mục này.

## Giải phóng Git Log (nếu cần)

Nếu gặp lỗi "Log file is already in use":

1. **Đóng tất cả Cursor/VS Code/IDE**
2. **Chạy script**: `reset_git_log.bat` (với quyền Administrator nếu cần)
3. **Hoặc chạy thủ công**:
   ```powershell
   # Dừng các process git
   taskkill /F /IM git.exe /T
   taskkill /F /IM git-credential-manager.exe /T
   
   # Xóa lock files
   Remove-Item .git\*.lock -Force -ErrorAction SilentlyContinue
   Remove-Item .git\logs\*.lock -Force -ErrorAction SilentlyContinue
   
   # Đợi 2 giây
   Start-Sleep -Seconds 2
   
   # Kiểm tra lại
   git status
   ```

## Thông tin commit gần nhất

Từ log file, nhánh `ThuNghiem_Thien` có các commit:
- Implement comprehensive file management system for event images
- Implement real order creation from wishlist checkout
- Cập nhật Event model, DTOs và thêm file upload avatars/events
- Sync avatar update on header; fix wallet balance parsing

