using System;
using AoL_HCI_Backend.Models;
using Newtonsoft.Json;

namespace AoL_HCI_Backend.Contracts;

public record class CreateDivisionRecord
{
    [JsonProperty(nameof(Name))]
    public string Name { get; set; } = null!;

    public Division ToModel()
    {
        return new Division
        {
            Name = Name
        };
    }
}

public record class CreateJobRecord
{
    [JsonProperty(nameof(Issuer))]
    public string Issuer { get; set; } = null!;
    [JsonProperty(nameof(Title))]
    public string Title { get; set; } = null!;
    [JsonProperty(nameof(DueDate))]
    public string DueDate { get; set; } = null!;

    public Job ToModel()
    {
        return new Job
        {
            Issuer = Issuer,
            Title = Title,
            DueDate = DueDate
        };
    }
}

public record CreateSubTaskRecord
{
    [JsonProperty(nameof(Title))]
    public string Title { get; set; } = null!;
    [JsonProperty(nameof(Description))]
    public string Description { get; set; } = null!;
    [JsonProperty(nameof(TaskId))]
    public string TaskId { get; set; } = null!;
    [JsonProperty(nameof(UserIds))]
    public List<string> UserIds { get; set; } = null!;
    [JsonProperty(nameof(DueDate))]
    public string DueDate { get; set; } = null!;

    public SubTask ToModel()
    {
        return new SubTask
        {
            Title = Title,
            Description = Description,
            TaskId = TaskId,
            UserIds = UserIds,
            DueDate = DueDate
        };
    }
}

public record class CreateTaskRecord
{
    [JsonProperty(nameof(Title))]
    public string Title { get; set; } = null!;
    [JsonProperty(nameof(DivisionId))]
    public string DivisionId { get; set; } = null!;
    [JsonProperty(nameof(JobId))]
    public string JobId { get; set; } = null!;
    [JsonProperty(nameof(DueDate))]
    public string DueDate { get; set; } = null!;

    public Tasks ToModel()
    {
        return new Tasks
        {
            Title = Title,
            DivisionId = DivisionId,
            JobId = JobId,
            DueDate = DueDate
        };
    }
}

public record class CreateUserRecord
{
    [JsonProperty(nameof(Name))]
    public string Name { get; set; } = null!;
    [JsonProperty(nameof(Email))]
    public string Email { get; set; } = null!;
    [JsonProperty(nameof(Password))]
    public string Password { get; set; } = null!;
    [JsonProperty(nameof(RoleId))]
    public string RoleId { get; set; } = null!;
    [JsonProperty(nameof(DivisionId))]
    public string DivisionId { get; set; } = null!;

    public User ToModel()
    {
        return new User
        {
            Name = Name,
            RoleId = RoleId,
            DivisionId = DivisionId
        };
    }
}

