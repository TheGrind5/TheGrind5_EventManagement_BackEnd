namespace TheGrind5_EventManagement.Constants;

public static class VNPayConstants
{
    public const string VERSION = "2.1.0";
    public const string COMMAND = "pay";
    public const string CURRENCY = "VND";
    public const string LOCALE = "vn";
    public const string ORDER_TYPE = "other";
    public const string TIMEZONE = "SE Asia Standard Time";

    // Response Codes
    public const string RESPONSE_SUCCESS = "00";
    public const string RESPONSE_SUSPECT = "07";
    public const string RESPONSE_NOT_REGISTERED = "09";
    public const string RESPONSE_AUTH_FAILED = "10";
    public const string RESPONSE_EXPIRED = "11";
    public const string RESPONSE_BLOCKED = "12";
    public const string RESPONSE_INVALID_OTP = "13";
    public const string RESPONSE_USER_CANCELLED = "24";
    public const string RESPONSE_INSUFFICIENT = "51";
    public const string RESPONSE_EXCEED_LIMIT = "65";
    public const string RESPONSE_BANK_MAINTENANCE = "75";
    public const string RESPONSE_INVALID_PIN = "79";
    public const string RESPONSE_UNKNOWN_ERROR = "99";

    // Transaction Status
    public const string TXN_STATUS_SUCCESS = "00";
    public const string TXN_STATUS_INCOMPLETE = "01";
    public const string TXN_STATUS_ERROR = "02";
    public const string TXN_STATUS_REVERSED = "04";
    public const string TXN_STATUS_REFUND_PROCESSING = "05";
    public const string TXN_STATUS_REFUND_SENT = "06";
    public const string TXN_STATUS_FRAUD_SUSPECTED = "07";
    public const string TXN_STATUS_REFUND_REJECTED = "09";

    // Payment Status Mapping
    public const string STATUS_INITIATED = "Initiated";
    public const string STATUS_SUCCEEDED = "Succeeded";
    public const string STATUS_FAILED = "Failed";
    public const string STATUS_REFUNDED = "Refunded";
}

