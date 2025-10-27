# Đề Xuất Tính Năng: Sân Khấu Ảo 2D

## 📋 Tổng Quan

Tính năng cho phép host event tạo và quản lý sân khấu ảo 2D cho các sự kiện, giúp người mua vé có thể chọn khu vực một cách trực quan.

## ✅ Tính Khả Thi

**Hoàn toàn khả thi** - Hệ thống hiện tại đã có:
- ✅ Cấu trúc Event với JSON fields (`EventDetails`, `TermsAndConditions`, `OrganizerInfo`)
- ✅ Cấu trúc TicketType với đầy đủ thông tin
- ✅ Frontend React với Material-UI
- ✅ Multi-step form cho tạo event

## 🎯 Yêu Cầu Chức Năng

### 1. Backend (ASP.NET Core)

#### A. Mở Rộng Model Event

**File: `src/Models/Event.cs`**

Thêm vào class `EventDetailsData`:

```csharp
public class EventDetailsData
{
    // ... existing fields ...
    
    // Mới: Thông tin sân khấu ảo
    public VenueLayoutData? VenueLayout { get; set; }
}

public class VenueLayoutData
{
    public bool HasVirtualStage { get; set; } = false;
    public int CanvasWidth { get; set; } = 1000;
    public int CanvasHeight { get; set; } = 800;
    public List<StageArea> Areas { get; set; } = new List<StageArea>();
}

public class StageArea
{
    public string Id { get; set; } // Unique ID
    public string Name { get; set; } // Ví dụ: "KẾT NỐI 1"
    public string Shape { get; set; } // "rectangle", "polygon", "circle"
    public List<Point> Coordinates { get; set; } = new List<Point>();
    public string Color { get; set; } // Hex color
    public int? TicketTypeId { get; set; } // Link to TicketType
    public bool IsStanding { get; set; } = false;
    public int? Capacity { get; set; }
    public string? Label { get; set; }
}

public class Point
{
    public double X { get; set; }
    public double Y { get; set; }
}
```

#### B. API Endpoints

**File: `src/Controllers/EventController.cs`**

Thêm các endpoints mới:

```csharp
// Lưu layout sân khấu ảo
[HttpPost("{eventId}/venue-layout")]
public async Task<IActionResult> SaveVenueLayout(int eventId, [FromBody] VenueLayoutData layout)
{
    // Validate event ownership
    // Save layout to EventDetails
    // Return success
}

// Lấy layout sân khấu ảo
[HttpGet("{eventId}/venue-layout")]
public async Task<IActionResult> GetVenueLayout(int eventId)
{
    // Get event
    // Extract VenueLayout from EventDetails
    // Return layout data
}

// Xóa layout sân khấu ảo
[HttpDelete("{eventId}/venue-layout")]
public async Task<IActionResult> DeleteVenueLayout(int eventId)
{
    // Validate event ownership
    // Remove layout from EventDetails
    // Return success
}
```

#### C. Database Migration

Không cần migration mới vì dùng JSON field `EventDetails` hiện có.

### 2. Frontend (React)

#### A. Component: StageEditor

**File: `src/components/stage/StageEditor.jsx`**

Component chính để vẽ và chỉnh sửa sân khấu:

```javascript
import React, { useState, useRef, useEffect } from 'react';
import Konva from 'konva';
import { Stage, Layer, Rect, Text, Group } from 'react-konva';

const StageEditor = ({ ticketTypes, onSave, initialLayout }) => {
  const [areas, setAreas] = useState([]);
  const [selectedArea, setSelectedArea] = useState(null);
  const [drawingMode, setDrawingMode] = useState('rectangle');
  
  // Drawing logic using Konva
  // Add, edit, delete areas
  // Color picker for each area
  // Link area to ticket type
  
  return (
    <div>
      {/* Toolbar với các công cụ: Rectangle, Polygon, Circle, Delete */}
      {/* Canvas để vẽ sân khấu */}
      {/* Properties panel để chỉnh sửa area */}
      {/* Save button */}
    </div>
  );
};
```

#### B. Component: StageViewer

**File: `src/components/stage/StageViewer.jsx`**

Component để hiển thị sân khấu cho người mua vé:

```javascript
import React from 'react';
import Konva from 'konva';
import { Stage, Layer, Rect, Text } from 'react-konva';

const StageViewer = ({ layout, ticketTypes, onAreaClick }) => {
  // Hiển thị sân khấu với các khu vực clickable
  // Màu sắc theo ticket type
  // Show area names
  // Click handler để chọn khu vực
  
  return (
    <div>
      {/* Interactive map */}
      {/* Zoom controls */}
      {/* Area click handler */}
    </div>
  );
};
```

#### C. Tích Hợp Vào CreateEventPage

**File: `src/pages/CreateEventPage.jsx`**

Thêm bước mới sau step 2 (DateTimeTicketStep):

```javascript
const steps = [
  'Thông tin cơ bản',
  'Thời gian & Loại vé',
  'Sân khấu ảo', // Bước mới
  'Cài đặt',
  'Thanh toán'
];

// Step 3: Virtual Stage (optional)
case 2:
  return (
    <VirtualStageStep
      data={step3Data}
      onChange={setStep3Data}
      ticketTypes={step2Data.ticketTypes}
    />
  );
```

#### D. Dependencies Cần Cài

```bash
npm install konva react-konva
```

### 3. Cấu Trúc Dữ Liệu

#### A. Backend - JSON Format

```json
{
  "VenueLayout": {
    "HasVirtualStage": true,
    "CanvasWidth": 1000,
    "CanvasHeight": 800,
    "Areas": [
      {
        "Id": "area_1",
        "Name": "KẾT NỐI 1",
        "Shape": "polygon",
        "Coordinates": [
          { "X": 100, "Y": 100 },
          { "X": 200, "Y": 100 },
          { "X": 200, "Y": 200 },
          { "X": 100, "Y": 200 }
        ],
        "Color": "#667eea",
        "TicketTypeId": 123,
        "IsStanding": true,
        "Capacity": 500,
        "Label": "KẾT NỐI 1"
      }
    ]
  }
}
```

#### B. Frontend - State Management

```javascript
const [stageData, setStageData] = useState({
  hasVirtualStage: false,
  canvasWidth: 1000,
  canvasHeight: 800,
  areas: []
});
```

## 🚀 Kế Hoạch Triển Khai

### Phase 1: MVP (Minimum Viable Product) - 2 tuần

**Backend:**
- [ ] Thêm `VenueLayoutData` class vào model
- [ ] Tạo API endpoints cơ bản
- [ ] Unit tests cho endpoints

**Frontend:**
- [ ] Cài đặt Konva/React-Konva
- [ ] Tạo StageEditor component cơ bản
- [ ] Tạo StageViewer component
- [ ] Tích hợp vào CreateEventPage
- [ ] UI/UX cơ bản

### Phase 2: Nâng Cao - 1 tuần

**Backend:**
- [ ] Validation cho layout data
- [ ] Thêm capacity checking
- [ ] API để get available seats

**Frontend:**
- [ ] Zoom controls
- [ ] Color picker
- [ ] Drag & drop cho areas
- [ ] Save/Load layout
- [ ] Preview mode

### Phase 3: Hoàn Thiện - 1 tuần

**Backend:**
- [ ] Performance optimization
- [ ] Caching
- [ ] Documentation

**Frontend:**
- [ ] Responsive design
- [ ] Mobile support
- [ ] Accessibility
- [ ] Testing

## 💡 Tính Năng Nâng Cao (Future)

1. **Template Library**: Thư viện templates sẵn có cho các loại venue
2. **3D View**: Hiển thị 3D cho một số venue nhất định
3. **Real-time Updates**: Cập nhật số lượng vé còn lại theo thời gian thực
4. **Augmented Reality**: Xem venue qua AR trên mobile
5. **Auto-layout**: Tự động tạo layout dựa trên thông tin venue

## 🔧 Công Nghệ Sử Dụng

### Backend
- ASP.NET Core
- Entity Framework Core
- JSON Serialization

### Frontend
- React
- Konva.js / React-Konva (2D graphics)
- Material-UI
- React-Konva canvas rendering

## 📊 Độ Phức Tạp

- **Backend**: ⭐⭐☆☆☆ (2/5) - Trung bình
- **Frontend**: ⭐⭐⭐☆☆ (3/5) - Cao hơn một chút
- **Tổng thể**: ⭐⭐⭐☆☆ (3/5) - Khả thi với timeline 4 tuần

## 🎯 Lợi Ích

1. **Trải nghiệm người dùng tốt hơn**: Người mua vé hiểu rõ vị trí
2. **Tăng tỷ lệ chuyển đổi**: Visual selection giúp quyết định nhanh hơn
3. **Giảm confusion**: Rõ ràng về giá và khu vực
4. **Competitive advantage**: Tính năng độc đáo trên thị trường
5. **Scalability**: Dễ mở rộng thêm tính năng sau

## ⚠️ Thách Thức

1. **Performance**: Với nhiều areas (>50), cần optimize rendering
2. **Browser compatibility**: Konva có thể có issues với một số browsers cũ
3. **Mobile UX**: Canvas khó tương tác trên mobile
4. **Data validation**: Cần validate chặt chẽ để tránh layout lỗi

## 📝 Kết Luận

Tính năng này **hoàn toàn khả thi** và sẽ là một điểm nổi bật cho hệ thống bán vé của bạn. Với cấu trúc hiện tại, bạn có thể triển khai một MVP trong 2 tuần và tiếp tục phát triển các tính năng nâng cao sau đó.

**Gợi ý**: Bắt đầu với Phase 1 để validate ý tưởng với users, sau đó tiếp tục với Phase 2 và 3 dựa trên feedback.

