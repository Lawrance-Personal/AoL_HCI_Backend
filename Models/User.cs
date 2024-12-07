using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AoL_HCI_Backend.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("IdentityId")]
    public string IdentityId { get; set; } = null!;
    [BsonElement("RoleId")]
    public string RoleId { get; set; } = null!;
    [BsonElement("DivisionId")]
    public string? DivisionId { get; set; } = null!;
    [BsonElement("SubTaskIds")]
    public List<string> SubTaskIds { get; set; } = [];
}
