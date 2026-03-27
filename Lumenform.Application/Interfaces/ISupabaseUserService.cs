namespace Lumenform.Application.Interfaces;

public interface ISupabaseUserService
{
    Task<SupabaseUserInfo?> GetUserByIdAsync(Guid userId);
    Task<Dictionary<Guid, SupabaseUserInfo>> GetUsersByIdsAsync(IEnumerable<Guid> userIds);
}

public record SupabaseUserInfo(
    string Id,
    string Email,
    string FirstName,
    string LastName
);