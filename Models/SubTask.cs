using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AoL_HCI_Backend.Models;

public class SubTask
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Title")]
    public string Title { get; set; } = null!;
    [BsonElement("Description")]
    public string Description { get; set; } = null!;
    [BsonElement("TaskId")]
    public string TaskId { get; set; } = null!;
    [BsonElement("UserIds")]
    public List<string> UserIds { get; set; } = [];
    [BsonElement("IssueDate")]
    public string IssueDate { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
    [BsonElement("DueDate")]
    public string DueDate { get; set; } = null!;
    [BsonElement("Status")]
    public int Status { get; set; } = 0;
}
