namespace ContractMonthlyClaimSystem.Services
{
    public interface IFileGuard
    {
   
        bool IsAllowed(string fileName, long size, out string? error);
    }
}
