namespace OracleMES.Core.Exceptions;

public static class ErrorCodes
{

    // 보편적인 에러 코드
    public const string ValidationError = "VALIDATION_ERROR";
    public const string NotFound = "NOT_FOUND";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string DuplicateEntry = "DUPLICATE_ENTRY";
    public const string BusinessRuleViolation = "BUSINESS_RULE_VIOLATION";
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    public const string BadRequest = "BAD_REQUEST";
    

    // MES 관련 에러 코드
    public const string MachineFailure = "MACHINE_FAILURE";
    public const string InventoryShortage = "INVENTORY_SHORTAGE";
    public const string QualityControlFailure = "QUALITY_CONTROL_FAILURE";
    public const string DowntimeExceeded = "DOWNTIME_EXCEEDED";
    public const string WorkOrderDelay = "WORK_ORDER_DELAY";
    public const string EmployeeUnavailable = "EMPLOYEE_UNAVAILABLE";
    public const string DataSyncError = "DATA_SYNC_ERROR";
    public const string ConfigurationError = "CONFIGURATION_ERROR";
    public const string PermissionDenied = "PERMISSION_DENIED";
    public const string ResourceLocked = "RESOURCE_LOCKED";
    public const string OperationTimeout = "OPERATION_TIMEOUT";

}