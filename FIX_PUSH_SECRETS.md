# Cách xử lý Push bị chặn do Secrets

## Vấn đề
Commit cũ `c8a0f03` trong lịch sử vẫn chứa OAuth credentials trong `appsettings.json`. 
GitHub Push Protection quét toàn bộ lịch sử khi push, không chỉ commit mới.

## Giải pháp nhanh nhất

Click vào các link sau để cho phép secret tạm thời (chỉ cho commit cũ):

1. [Allow Google OAuth Client ID](https://github.com/TheGrind5/TheGrind5_EventManagement_BackEnd/security/secret-scanning/unblock-secret/34pgTe84CXzfllWN19yX1kq92i4)

2. [Allow Google OAuth Client Secret](https://github.com/TheGrind5/TheGrind5_EventManagement_BackEnd/security/secret-scanning/unblock-secret/34pgTkKUemp6E7fszAMTFPCm7t2)

3. [Allow Google OAuth Refresh Token](https://github.com/TheGrind5/TheGrind5_EventManagement_BackEnd/security/secret-scanning/unblock-secret/34pgTeaou3AsdRJBzH9t9MIyfg1)

Sau khi click "Allow secret" cho cả 3 secrets, chạy lại:
```bash
git push origin ThuNghiem_Thien
```

## Lưu ý
- Đây chỉ là giải pháp tạm thời cho commit cũ
- Từ giờ trở đi, OAuth credentials đã được tách sang `secrets.json` (không commit)
- `appsettings.json` mới không còn secrets nên có thể commit bình thường

