-- Kiểm tra xem bảng Feedback có tồn tại không
SELECT 
    TABLE_NAME,
    TABLE_SCHEMA,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME IN ('Feedback', 'FeedbackReaction');

-- Nếu bảng tồn tại, hiển thị cấu trúc
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Feedback')
BEGIN
    SELECT 'Feedback table exists' AS Status;
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Feedback';
END
ELSE
BEGIN
    SELECT 'Feedback table does NOT exist' AS Status;
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'FeedbackReaction')
BEGIN
    SELECT 'FeedbackReaction table exists' AS Status;
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'FeedbackReaction';
END
ELSE
BEGIN
    SELECT 'FeedbackReaction table does NOT exist' AS Status;
END
