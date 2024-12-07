using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AoL_HCI_Backend.Models;

public class Tasks
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Title")]
    public string Title { get; set; } = null!;
    [BsonElement("DivisionId")]
    public string DivisionId { get; set; } = null!;
    [BsonElement("JobId")]
    public string JobId { get; set; } = null!;
    [BsonElement("IssueDate")]
    public string IssueDate { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
    [BsonElement("DueDate")]
    public string DueDate { get; set; } = null!;
    [BsonElement("Status")]
    public int Status { get; set; } = 0;
    [BsonElement("Progress")]
    public int Progress { get; set; } = 0;
}
