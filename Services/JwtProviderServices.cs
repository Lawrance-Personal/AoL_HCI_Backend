using System;

namespace AoL_HCI_Backend.Services;

public interface IJwtProviderServices
{
    public Task<AuthToken?> GenerateToken(string email, string password);
}

internal sealed class JwtProviderServices(HttpClient httpClient) : IJwtProviderServices
{
    private readonly HttpClient _httpclient = httpClient;

    public async Task<AuthToken?> GenerateToken(string email, string password)
    {
        var response = await _httpclient.PostAsJsonAsync("", new
        {
            email,
            password,
            returnSecureToken = true
        });
        return await response.Content.ReadFromJsonAsync<AuthToken>();
    }
}
