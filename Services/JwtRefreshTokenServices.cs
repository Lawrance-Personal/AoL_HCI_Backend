using System;

namespace AoL_HCI_Backend.Services;

public interface IJwtRefreshTokenServices
{
    public Task<AuthToken?> RefreshToken(string refreshToken);
}

internal sealed class JwtRefreshTokenServices(HttpClient httpClient) : IJwtRefreshTokenServices
{
    private readonly HttpClient _httpclient = httpClient;

    public async Task<AuthToken?> RefreshToken(string refreshToken)
    {
        var response = await _httpclient.PostAsJsonAsync("", new
        {
            grant_type = "refresh_token",
            refresh_token = refreshToken
        });
        return await response.Content.ReadFromJsonAsync<AuthToken>();
    }
}
