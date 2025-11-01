# Hướng dẫn Triển khai A.I Suggestion với Hugging Face API

## Tổng quan

Hệ thống A.I Suggestion cung cấp 4 chức năng AI chính cho người dùng của TheGrind5 Event Management:

1. **Event Recommendation** - Gợi ý sự kiện cá nhân hóa cho Customer
2. **AI Chatbot Q&A** - Trả lời câu hỏi tự động cho tất cả người dùng
3. **Pricing Suggestion** - Gợi ý giá vé tối ưu cho Host
4. **Content Generation** - Tạo nội dung mô tả sự kiện cho Host

## Kiến trúc Backend

### 1. Models và DTOs

**File:** `src/Models/AISuggestion.cs`
- Model lưu lịch sử AI requests/responses
- Foreign key tới User
- SuggestionType: EventRecommendation, ChatbotQA, PricingSuggestion, ContentGeneration

**File:** `src/DTOs/AISuggestionDTOs.cs`
- Request DTOs: EventRecommendationRequest, ChatbotRequest, PricingSuggestionRequest, ContentGenerationRequest
- Response DTOs: EventRecommendationResponse, ChatbotResponse, PricingSuggestionResponse, ContentGenerationResponse
- Helper DTOs: RecommendedEvent, PriceRange, AISuggestionHistoryResponse

### 2. Services

**HuggingFaceService** (`src/Services/HuggingFaceService.cs`)
- Tích hợp Hugging Face Inference API
- Methods:
  - `GenerateTextAsync(prompt, maxLength)`: Tạo text generation
  - `GetEmbeddingsAsync(text)`: Lấy embeddings cho similarity search
  - `GenerateWithContextAsync(prompt, context)`: Tạo text với context
- Fallback responses khi API không available

**AIRecommendationService** (`src/Services/AIRecommendationService.cs`)
- Phân tích lịch sử mua vé (Orders) và Wishlist
- Tính similarity scores dựa trên categories
- Trả về top 5 events được gợi ý
- Generate AI reasoning cho recommendations

**AIChatbotService** (`src/Services/AIChatbotService.cs`)
- Context-aware responses (biết user đang xem event nào)
- Phân loại câu hỏi: Event info, Payment/Refund, Ticket, Wallet, General
- RAG (Retrieval-Augmented Generation) với database
- Fallback responses cho câu hỏi không liên quan

**AIPricingService** (`src/Services/AIPricingService.cs`)
- Phân tích giá vé của các events tương tự
- Tính toán price ranges: Early Bird, Regular, VIP
- Generate pricing analysis và recommendations

**AIContentGenerationService** (`src/Services/AIContentGenerationService.cs`)
- Generate event descriptions
- Tạo introduction hấp dẫn
- Sinh terms and conditions chuẩn
- Customize theo EventType và Category

### 3. Repository

**AISuggestionRepository** (`src/Repositories/AISuggestionRepository.cs`)
- CRUD operations cho AISuggestion
- Lưu lịch sử AI requests/responses
- Query history by user và type

### 4. Controller

**AISuggestionController** (`src/Controllers/AISuggestionController.cs`)
- `POST /api/aisuggestion/recommend-events` - Get event recommendations (Customer only)
- `POST /api/aisuggestion/chatbot` - Ask chatbot (All roles)
- `POST /api/aisuggestion/suggest-pricing` - Get pricing suggestions (Host only)
- `POST /api/aisuggestion/generate-content` - Generate content (Host only)
- `GET /api/aisuggestion/history` - Get user's AI history (All roles)

All endpoints require authentication.

### 5. Database

**Migration:** `20251031232328_AddAISuggestionTable`

```sql
CREATE TABLE AISuggestion (
    SuggestionId INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    SuggestionType NVARCHAR(50) NOT NULL,
    RequestData NVARCHAR(MAX),
    ResponseData NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES User(UserID) ON DELETE NO ACTION
);
```

## Setup và Configuration

### 1. Hugging Face API Key

1. Đăng ký tài khoản miễn phí tại https://huggingface.co
2. Tạo API token tại https://huggingface.co/settings/tokens
3. Thêm vào `appsettings.json`:

```json
{
  "HuggingFace": {
    "ApiKey": "YOUR_HUGGINGFACE_API_KEY",
    "BaseUrl": "https://api-inference.huggingface.co/models/",
    "TextGenerationModel": "mistralai/Mistral-7B-Instruct-v0.2",
    "EmbeddingModel": "sentence-transformers/all-MiniLM-L6-v2"
  }
}
```

### 2. Models sử dụng

- **Text Generation**: `mistralai/Mistral-7B-Instruct-v0.2`
  - Mistral 7B là model text generation mạnh, miễn phí
  - Phù hợp cho chatbot, content generation
  
- **Embeddings**: `sentence-transformers/all-MiniLM-L6-v2`
  - Model tạo embeddings 384-dimensional
  - Dùng cho similarity search trong recommendation

### 3. Rate Limiting

Hugging Face Free tier có giới hạn:
- 1,000 requests/hour cho text generation
- 10,000 requests/hour cho embeddings

**Giải pháp:**
- Implement caching cho responses
- Cache embeddings của events
- Fallback responses khi rate limit

## API Endpoints

### 1. Event Recommendations

**Endpoint:** `POST /api/aisuggestion/recommend-events`  
**Role:** Customer  
**Auth:** Required

**Response:**
```json
{
  "success": true,
  "message": "Gợi ý sự kiện thành công",
  "data": {
    "events": [
      {
        "eventId": 1,
        "title": "Sự kiện mẫu",
        "description": "...",
        "startTime": "2025-01-01T10:00:00Z",
        "endTime": "2025-01-01T18:00:00Z",
        "location": "Hà Nội",
        "category": "Business",
        "eventMode": "Offline",
        "minPrice": 500000,
        "imageUrl": "https://...",
        "similarityScore": 0.85,
        "reason": "Phù hợp với sở thích của bạn về Business"
      }
    ],
    "reasoning": "Dựa trên lịch sử tham dự sự kiện về Business..."
  }
}
```

### 2. AI Chatbot

**Endpoint:** `POST /api/aisuggestion/chatbot`  
**Role:** All  
**Auth:** Required

**Request:**
```json
{
  "question": "Làm thế nào để mua vé?",
  "eventId": 1  // Optional
}
```

**Response:**
```json
{
  "success": true,
  "message": "Chatbot đã trả lời",
  "data": {
    "answer": "Bạn có thể mua vé bằng cách...",
    "relatedLinks": ["/events/1", "/my-tickets"],
    "confidence": "High"
  }
}
```

### 3. Pricing Suggestions

**Endpoint:** `POST /api/aisuggestion/suggest-pricing`  
**Role:** Host  
**Auth:** Required

**Request:**
```json
{
  "category": "Business",
  "startTime": "2025-02-01T10:00:00Z",
  "location": "Hà Nội"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Gợi ý giá vé thành công",
  "data": {
    "suggestedPrices": [
      {
        "ticketType": "Early Bird",
        "minPrice": 400000,
        "maxPrice": 440000,
        "recommendedPrice": 420000,
        "reasoning": "Giá ưu đãi cho người đăng ký sớm..."
      },
      {
        "ticketType": "Regular",
        "minPrice": 490000,
        "maxPrice": 550000,
        "recommendedPrice": 500000,
        "reasoning": "Giá vé tiêu chuẩn..."
      }
    ],
    "analysis": "Dựa trên phân tích 10 sự kiện Business...",
    "recommendation": "Chúng tôi khuyến nghị bắt đầu với gói Regular..."
  }
}
```

### 4. Content Generation

**Endpoint:** `POST /api/aisuggestion/generate-content`  
**Role:** Host  
**Auth:** Required

**Request:**
```json
{
  "eventType": "Conference",
  "title": "Tech Conference 2025",
  "category": "Technology"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Tạo nội dung thành công",
  "data": {
    "description": "Tech Conference 2025 là sự kiện lớn về công nghệ...",
    "introduction": "Khám phá những xu hướng công nghệ mới nhất...",
    "termsAndConditions": "Vé đã bán sẽ không được hoàn lại...",
    "specialExperience": "Trải nghiệm công nghệ và đổi mới..."
  }
}
```

### 5. AI History

**Endpoint:** `GET /api/aisuggestion/history`  
**Role:** All  
**Auth:** Required

**Response:**
```json
{
  "success": true,
  "message": "Lấy lịch sử thành công",
  "data": [
    {
      "suggestionId": 1,
      "suggestionType": "ChatbotQA",
      "requestData": "{\"question\":\"...\"}",
      "createdAt": "2025-01-01T10:00:00Z"
    }
  ]
}
```

## Frontend Integration

### Service: `src/services/aiSuggestionService.js`

```javascript
import { aiSuggestionAPI } from './services/aiSuggestionService';

// Get recommendations
const recommendations = await aiSuggestionAPI.getEventRecommendations();

// Ask chatbot
const response = await aiSuggestionAPI.askChatbot("How to buy tickets?", eventId);

// Get pricing suggestions
const pricing = await aiSuggestionAPI.getSuggestedPricing({
  category: "Business",
  startTime: new Date(),
  location: "Hà Nội"
});

// Generate content
const content = await aiSuggestionAPI.generateContent({
  eventType: "Conference",
  title: "My Event",
  category: "Technology"
});

// Get history
const history = await aiSuggestionAPI.getHistory();
```

## Caching Strategy

### 1. Embeddings Cache
- Cache embeddings của events để giảm API calls
- Recompute embeddings khi event metadata thay đổi
- TTL: 24 hours

### 2. AI Responses Cache
- Cache common AI responses
- Key: hash(prompt + context)
- TTL: 1 hour

### 3. Recommendations Cache
- Cache event recommendations per user
- Key: userId + timestamp (hour)
- TTL: 1 hour

## Error Handling

### API Failures
- Retry logic: 3 attempts with exponential backoff
- Fallback responses khi API unavailable
- Logging errors for monitoring

### Rate Limiting
- Detect rate limit errors (HTTP 429)
- Exponential backoff
- Use cached responses when possible

### Invalid Requests
- Validation errors trả về 400 Bad Request
- Error messages rõ ràng cho user

## Monitoring và Logging

### Metrics cần theo dõi
- Số lượng AI requests/endpoint
- Success rate của AI responses
- API latency
- Rate limit hits
- Cache hit rate

### Logs
- Tất cả AI requests/responses được log
- User ID, SuggestionType, Timestamp
- Performance metrics
- Error details

## Testing

### Unit Tests
- Test HuggingFaceService với mock responses
- Test AI services với sample data
- Test fallback mechanisms

### Integration Tests
- Test full API endpoints
- Test authentication/authorization
- Test database operations

### Manual Testing
- Test với HuggingFace API thật
- Test caching behavior
- Test rate limiting

## Deployment

### Development
1. Cấu hình HuggingFace API key trong appsettings.Development.json
2. Run migration: `dotnet ef database update`
3. Test endpoints với Swagger

### Production
1. Cấu hình HuggingFace API key trong appsettings.json hoặc secrets
2. Run migration
3. Monitor AI usage và costs
4. Set up alerts cho rate limiting

## Troubleshooting

### Issue: API timeout
**Solution:** Tăng HttpClient timeout, implement retry logic

### Issue: Rate limiting
**Solution:** Implement caching, use fallback responses, consider upgrading API tier

### Issue: Invalid responses
**Solution:** Check prompt formatting, model compatibility, implement validation

### Issue: Slow recommendations
**Solution:** Cache embeddings, optimize database queries, consider precomputed recommendations

## Future Improvements

1. **Advanced Recommendation**: Collaborative filtering, content-based filtering
2. **Multi-language Support**: Support multiple languages cho chatbot
3. **Sentiment Analysis**: Analyze user feedback
4. **A/B Testing**: Test different AI models/prompts
5. **Custom Models**: Fine-tune models on domain-specific data
6. **Real-time Chat**: WebSocket support cho chatbot
7. **Visual Recommendations**: Image-based event recommendations

## Security Considerations

1. **API Key Security**: Store API keys in secrets, never commit to git
2. **Input Validation**: Validate all user inputs
3. **Rate Limiting**: Implement client-side và server-side rate limiting
4. **Privacy**: Logs không chứa sensitive data
5. **Audit Trail**: Track all AI requests for compliance

## Costs

### Hugging Face Free Tier
- Text Generation: 1,000 requests/hour
- Embeddings: 10,000 requests/hour

### Paid Tiers
- Standard: $9/month
- Pro: $20/month
- Enterprise: Custom pricing

## References

- Hugging Face API Docs: https://huggingface.co/docs/api-inference
- Mistral 7B Model: https://huggingface.co/mistralai/Mistral-7B-Instruct-v0.2
- Sentence Transformers: https://huggingface.co/sentence-transformers
- TheGrind5 Event Management Docs: /Diagram/SYSTEM_DESCRIPTION.txt

