using Serilog;

namespace EComercial.Services;
public class LoggingService
{
    public LoggingService()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Hour)
            .CreateLogger();
    }
    public void LogInfo(string message, params object[] args)
    {
        Log.Information(message, args);
    }
    public void LogWarning(string message, params object[] args)
    {
        Log.Warning(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        Log.Error(message, args);
    }

    public void LogUserLogin(string userId)
    {
        Log.Information("User {UserId} logged in successfully at {Timestamp}.", userId, DateTime.UtcNow);
    }
    public void LogUserLogout(string userId)
    {
        Log.Information("User {UserId} logged out at {Timestamp}.", userId, DateTime.UtcNow);
    }
    public void LogCreateNewUser(string userId, string email)
    {
        Log.Information("New user created with UserID: {UserId}, Email: {Email}, at {Timestamp}.", userId, email, DateTime.UtcNow);
    }

    public void LogLowStock(string productId, string productName, int remainingStock)
    {
        Log.Warning("Low stock alert for ProductID: {ProductId}, Name: {ProductName}, Remaining Stock: {Stock}, at {Timestamp}.",
            productId, productName, remainingStock, DateTime.UtcNow);
    }

    public void LogPaymentSuccess(string orderId, string userId, decimal amount)
    {
        Log.Information("Payment successful for OrderID: {OrderId}, UserID: {UserId}, Amount: {Amount}, at {Timestamp}.",
            orderId, userId, amount, DateTime.UtcNow);
    }

    public void LogPaymentFailure(string orderId, string userId, decimal amount, string reason)
    {
        Log.Error("Payment failed for OrderID: {OrderId}, UserID: {UserId}, Amount: {Amount}, Reason: {Error}, at {Timestamp}.",
            orderId, userId, amount, reason, DateTime.UtcNow);
    }
}
