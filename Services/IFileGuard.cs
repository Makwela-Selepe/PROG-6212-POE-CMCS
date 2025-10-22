namespace ContractMonthlyClaimSystem.Services
{
    public interface IFileGuard
    {
        /// <summary>
        /// Validates a file name and size against allowed rules.
        /// Returns true if allowed; otherwise false and sets an error message.
        /// </summary>
        bool IsAllowed(string fileName, long size, out string? error);
    }
}
