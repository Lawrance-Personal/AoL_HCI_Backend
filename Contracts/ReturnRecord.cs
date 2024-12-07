using AoL_HCI_Backend.Models;
using AoL_HCI_Backend.Services;
using Newtonsoft.Json;

namespace AoL_HCI_Backend.Contracts;

public record class ReturnDataRecord<T>
{
    [JsonProperty(nameof(Data))]
    public T? Data { get; set; }
    [JsonProperty(nameof(Token))]
    public AuthToken Token { get; set; } = null!;

    public ReturnDataRecord(T data, AuthToken token)
    {
        Data = data;
        Token = token;
    }
}

public record class ReturnListRecord<T>
{
    [JsonProperty(nameof(Data))]
    public List<T> Data { get; set; } = [];
    [JsonProperty(nameof(Token))]
    public AuthToken Token { get; set; } = null!;

    public ReturnListRecord(List<T> data, AuthToken token)
    {
        Data = data;
        Token = token;
    }
}