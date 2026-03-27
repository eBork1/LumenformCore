using System.Text.Json;
using Lumenform.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Lumenform.Infrastructure.Services;

public class SupabaseUserService : ISupabaseUserService
{
    private readonly HttpClient _httpClient;
    private readonly string _supabaseUrl;
    private readonly string _serviceRoleKey;

    public SupabaseUserService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _supabaseUrl = configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase URL not configured");
        _serviceRoleKey = configuration["Supabase:ServiceRoleKey"] ?? throw new InvalidOperationException("Supabase Service Role Key not configured");
    }

    public async Task<SupabaseUserInfo?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_supabaseUrl}/auth/v1/admin/users/{userId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _serviceRoleKey);
            request.Headers.Add("apikey", _serviceRoleKey);

            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var userData = JsonSerializer.Deserialize<JsonElement>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
            
            var email = userData.GetProperty("email").GetString() ?? "Unknown";
            var userMetadata = userData.TryGetProperty("user_metadata", out var metadata) ? metadata : (JsonElement?)null;
            
            var firstName = userMetadata?.TryGetProperty("first_name", out var fn) == true ? fn.GetString() ?? "" : "";
            var lastName = userMetadata?.TryGetProperty("last_name", out var ln) == true ? ln.GetString() ?? "" : "";
            
            // Fallback to email prefix if no name
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                firstName = email.Split('@')[0];
            }

            return new SupabaseUserInfo(
                userData.GetProperty("id").GetString() ?? userId.ToString(),
                email,
                firstName,
                lastName
            );
        }
        catch
        {
            return null;
        }
    }

    public async Task<Dictionary<Guid, SupabaseUserInfo>> GetUsersByIdsAsync(IEnumerable<Guid> userIds)
    {
        var users = new Dictionary<Guid, SupabaseUserInfo>();
        
        var tasks = userIds.Select(async id =>
        {
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                users[id] = user;
            }
        });

        await Task.WhenAll(tasks);
        return users;
    }
}