using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AoL_HCI_Backend.Models;

public class Job
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Issuer")]
    public string Issuer { get; set; } = null!;
    [BsonElement("Title")]
    public string Title { get; set; } = null!;
    [BsonElement("IssueDate")]
    public string IssueDate { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
    [BsonElement("DueDate")]
    public string DueDate { get; set; } = null!;
    [BsonElement("Status")]
    public int Status { get; set; } = 0;
    [BsonElement("Progress")]
    public int Progress { get; set; } = 0;
}