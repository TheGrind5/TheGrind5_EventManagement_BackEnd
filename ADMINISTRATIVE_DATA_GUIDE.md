# Hướng Dẫn Bổ Sung Dữ Liệu Hành Chính Việt Nam 2025

## Trạng Thái Hiện Tại

### Đã Hoàn Thành
1. ✅ Model Campus trong database
2. ✅ Auto-set province theo campus  
3. ✅ Logic xử lý cho Quy Nhơn (không có quận/huyện)
4. ✅ Campus filter ở HomePage
5. ✅ Hiển thị campus ở EventDetailsPage

### Cần Bổ Sung Dữ Liệu

#### 1. Quận/Huyện (✅ Đã có dữ liệu cơ bản)
- Hà Nội: 12 quận, 1 thị xã, 17 huyện
- TP.HCM: 16 quận, 5 huyện  
- Đà Nẵng: 6 quận, 1 huyện
- Cần Thơ: 5 quận, 4 huyện
- Quy Nhơn: Không có quận/huyện (21 phường/xã trực thuộc)

#### 2. Phường/Xã (⚠️ Cần bổ sung đầy đủ)

**Hiện tại chỉ có dữ liệu mẫu cho:**
- Hà Nội: 2 quận (Ba Đình, Hoàn Kiếm) + Bắc Giang
- TP.HCM: 1 quận (Quận 1)
- Đà Nẵng: 1 quận (Hải Châu)

**Cần bổ sung cho tất cả quận/huyện còn lại**

## Nguồn Dữ Liệu Chính Thức

### Nguồn Chính Thức Để Tra Cứu

1. **Cổng Thông tin Điện tử Chính phủ**
   - URL: https://chinhphu.vn
   - Nghị quyết về sắp xếp đơn vị hành chính

2. **Tổng cục Thống kê Việt Nam**
   - URL: https://www.gso.gov.vn
   - Danh mục địa giới hành chính

3. **Bộ Nội vụ**
   - URL: https://moha.gov.vn
   - Quyết định về đơn vị hành chính

4. **Cổng Dịch vụ Công trực tuyến**
   - Website của từng tỉnh/thành phố
   - Danh mục các đơn vị hành chính

## Hướng Dẫn Bổ Sung

### Cách Thêm Dữ Liệu

1. **Tìm thông tin chính thức** từ các nguồn trên
2. **Bổ sung vào file** `EventInfoStep.jsx`:
   - Thêm districts vào `getDistrictsByProvince()`
   - Thêm wards vào `getWardsByDistrict()`

3. **Format dữ liệu**:
```javascript
'Quận/Huyện Tên': [
  'Phường/Xã 1',
  'Phường/Xã 2',
  // ...
]
```

### Lưu Ý Quan Trọng

1. **Quy Nhơn**: Đã xử lý riêng, không cần quận/huyện
2. **Hà Nội, TP.HCM**: Đã có 1 số quận, cần bổ sung các quận còn lại
3. **Đà Nẵng**: Cần bổ sung 5 quận còn lại + 1 huyện
4. **Cần Thơ**: Cần thêm dữ liệu cho tất cả 5 quận + 4 huyện

## Checklist Dữ Liệu Cần Bổ Sung

### TP. Hồ Chí Minh (16 quận, 5 huyện)
- [ ] Quận 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
- [ ] Quận Bình Tân, Bình Thạnh, Gò Vấp, Phú Nhuận, Tân Bình, Tân Phú, Thủ Đức
- [ ] Huyện Bình Chánh, Cần Giờ, Củ Chi, Hóc Môn, Nhà Bè

### Hà Nội (12 quận, 1 thị xã, 17 huyện)
- [x] Quận Ba Đình, Hoàn Kiếm
- [ ] Quận Tây Hồ, Long Biên, Cầu Giấy, Đống Đa, Hai Bà Trưng, Hoàng Mai, Thanh Xuân, Hà Đông, Nam Từ Liêm, Bắc Từ Liêm
- [ ] Thị xã Sơn Tây
- [ ] Tất cả 17 huyện

### Đà Nẵng (6 quận, 1 huyện)
- [x] Quận Hải Châu
- [ ] Quận Thanh Khê, Sơn Trà, Ngũ Hành Sơn, Liên Chiểu, Cẩm Lệ
- [ ] Huyện Hòa Vang

### Cần Thơ (5 quận, 4 huyện)
- [ ] Quận Ninh Kiều, Ô Môn, Bình Thuỷ, Cái Răng, Thốt Nốt
- [ ] Huyện Vĩnh Thạnh, Cờ Đỏ, Phong Điền, Thới Lai

### Quy Nhơn (Đã xử lý ✅)
- [x] 21 phường/xã đã có trong danh sách

## Khuyến Nghị

### Tùy chọn 1: Bổ Sung Đầy Đủ (Khuyến nghị)
- Tìm dữ liệu chính thức từ các nguồn trên
- Bổ sung tất cả quận/huyện và phường/xã
- Đảm bảo độ chính xác 100%

### Tùy chọn 2: Giữ Trạng Thái Hiện Tại
- Hiện tại đã có cấu trúc dữ liệu hoàn chỉnh
- Đã có dữ liệu mẫu cho một số quận
- User có thể tự điền phường/xã nếu chưa có trong danh sách

### Tùy chọn 3: API Tích Hợp
- Sử dụng API công của Chính phủ (nếu có)
- Tự động lấy dữ liệu hành chính
- Luôn cập nhật và chính xác

## Status

**Quan trọng**: Hiện tại hệ thống đã hoạt động tốt với logic xử lý campus và province. Dữ liệu phường/xã có thể bổ sung dần theo nhu cầu thực tế sử dụng.

