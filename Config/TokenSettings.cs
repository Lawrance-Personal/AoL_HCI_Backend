using System;

namespace AoL_HCI_Backend.Config;

public class TokenSettings
{
    public string TokenUri { get; set; } = null!;
    public string RefreshTokenUri { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Issuer { get; set; } = null!;
}
