using System.Security.Cryptography;
using System.Text;
using TheGrind5_EventManagement.Constants;
using TheGrind5_EventManagement.DTOs;

namespace TheGrind5_EventManagement.Helpers;

public static class VNPayHelper
{
    /// <summary>
    /// Tạo HMAC SHA512 hash từ query string
    /// </summary>
    public static string CreateHash(string queryString, string secretKey)
    {
        var bytes = Encoding.UTF8.GetBytes(secretKey);
        var keyBytes = Encoding.UTF8.GetBytes(queryString);

        using (var hmac = new HMACSHA512(bytes))
        {
            var hash = hmac.ComputeHash(keyBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    /// <summary>
    /// Sắp xếp parameters theo alphabet và build query string
    /// </summary>
    public static string SortAndBuildQueryString(Dictionary<string, string> parameters)
    {
        var sortedParams = parameters
            .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}");

        return string.Join("&", sortedParams);
    }

    /// <summary>
    /// Tạo query string từ VNPay parameters
    /// </summary>
    public static string BuildQueryString(Dictionary<string, string> parameters, string secretKey)
    {
        var sortedQuery = SortAndBuildQueryString(parameters);
        var hash = CreateHash(sortedQuery, secretKey);
        return sortedQuery + "&vnp_SecureHash=" + hash;
    }

    /// <summary>
    /// Validate VNPay signature
    /// </summary>
    public static bool ValidateSignature(VNPayWebhookData data, string secretKey)
    {
        var parameters = new Dictionary<string, string>();

        // Add all fields to dictionary
        if (!string.IsNullOrEmpty(data.vnp_TmnCode))
            parameters.Add("vnp_TmnCode", data.vnp_TmnCode);
        if (data.vnp_Amount > 0)
            parameters.Add("vnp_Amount", data.vnp_Amount.ToString());
        if (!string.IsNullOrEmpty(data.vnp_BankCode))
            parameters.Add("vnp_BankCode", data.vnp_BankCode);
        if (!string.IsNullOrEmpty(data.vnp_BankTranNo))
            parameters.Add("vnp_BankTranNo", data.vnp_BankTranNo);
        if (!string.IsNullOrEmpty(data.vnp_CardType))
            parameters.Add("vnp_CardType", data.vnp_CardType);
        if (!string.IsNullOrEmpty(data.vnp_PayDate))
            parameters.Add("vnp_PayDate", data.vnp_PayDate);
        if (!string.IsNullOrEmpty(data.vnp_OrderInfo))
            parameters.Add("vnp_OrderInfo", data.vnp_OrderInfo);
        if (!string.IsNullOrEmpty(data.vnp_TransactionNo))
            parameters.Add("vnp_TransactionNo", data.vnp_TransactionNo);
        if (!string.IsNullOrEmpty(data.vnp_ResponseCode))
            parameters.Add("vnp_ResponseCode", data.vnp_ResponseCode);
        if (!string.IsNullOrEmpty(data.vnp_TransactionStatus))
            parameters.Add("vnp_TransactionStatus", data.vnp_TransactionStatus);
        if (!string.IsNullOrEmpty(data.vnp_TxnRef))
            parameters.Add("vnp_TxnRef", data.vnp_TxnRef);
        if (!string.IsNullOrEmpty(data.vnp_CreateDate))
            parameters.Add("vnp_CreateDate", data.vnp_CreateDate);
        if (!string.IsNullOrEmpty(data.vnp_IpAddr))
            parameters.Add("vnp_IpAddr", data.vnp_IpAddr);
        if (!string.IsNullOrEmpty(data.vnp_CurrCode))
            parameters.Add("vnp_CurrCode", data.vnp_CurrCode);

        // Sort and build query string (excluding vnp_SecureHash and vnp_SecureHashType)
        var sortedQuery = SortAndBuildQueryString(parameters);
        
        // Create hash
        var calculatedHash = CreateHash(sortedQuery, secretKey);
        
        // Compare with received hash
        return calculatedHash.Equals(data.vnp_SecureHash, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Convert amount to VNPay format (multiply by 100)
    /// </summary>
    public static long ConvertAmountToVnPayFormat(decimal amount)
    {
        return (long)(amount * 100);
    }

    /// <summary>
    /// Convert amount from VNPay format (divide by 100)
    /// </summary>
    public static decimal ConvertAmountFromVnPayFormat(long amount)
    {
        return amount / 100m;
    }

    /// <summary>
    /// Extract OrderId from TxnRef
    /// Format: ORDER_{OrderId}_{Timestamp}
    /// </summary>
    public static int? ExtractOrderIdFromTxnRef(string txnRef)
    {
        if (string.IsNullOrEmpty(txnRef) || !txnRef.StartsWith("ORDER_"))
            return null;

        var parts = txnRef.Split('_');
        if (parts.Length < 2)
            return null;

        if (int.TryParse(parts[1], out var orderId))
            return orderId;

        return null;
    }

    /// <summary>
    /// Get current VNPay date format (yyyyMMddHHmmss)
    /// </summary>
    public static string GetVnPayDateFormat()
    {
        return DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss");
    }

    /// <summary>
    /// Generate TxnRef from OrderId
    /// </summary>
    public static string GenerateTxnRef(int orderId)
    {
        var timestamp = GetVnPayDateFormat();
        return $"ORDER_{orderId}_{timestamp}";
    }

    /// <summary>
    /// Check if transaction is success based on ResponseCode and TransactionStatus
    /// </summary>
    public static bool IsTransactionSuccess(string responseCode, string transactionStatus)
    {
        return responseCode == VNPayConstants.RESPONSE_SUCCESS && 
               transactionStatus == VNPayConstants.TXN_STATUS_SUCCESS;
    }
}

