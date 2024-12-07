using AoL_HCI_Backend.Models;
using Newtonsoft.Json;

namespace AoL_HCI_Backend.Contracts;

public record class UpdateJobRecord
{
    [JsonProperty(nameof(Issuer))]
    public string Issuer { get; set; } = null!;
    [JsonProperty(nameof(Title))]
    public string Title { get; set; } = null!;
    [JsonProperty(nameof(DueDate))]
    public string DueDate { get; set; } = null!;

    public Job ReplaceModel(Job job)
    {
        return new Job
        {
            Id = job.Id,
            Issuer = Issuer is not null ? Issuer : job.Issuer,
            Title = Title is not null ? Title : job.Title,
            IssueDate = job.IssueDate,
            DueDate = DueDate is not null ? DueDate : job.DueDate,
        };
    }
}

public record class UpdateSubTaskRecord
{
    [JsonProperty(nameof(Title))]
    public string Title { get; set; } = null!;
    [JsonProperty(nameof(Description))]
    public string Description { get; set; } = null!;
    [JsonProperty(nameof(UserIds))]
    public List<string> UserIds { get; set; } = null!;
    [JsonProperty(nameof(DueDate))]
    public string DueDate { get; set; } = null!;

    public SubTask ReplaceModel(SubTask subTask)
    {
        return new SubTask
        {
            Id = subTask.Id,
            Title = Title is not null ? Title : subTask.Title,
            Description = Description is not null ? Description : subTask.Description,
            TaskId = subTask.TaskId,
            UserIds = UserIds is not null ? UserIds : subTask.UserIds,
            IssueDate = subTask.IssueDate,
            DueDate = DueDate is not null ? DueDate : subTask.DueDate,
            Status = subTask.Status
        };
    }
}

public record class UpdateTaskRecord
{
    [JsonProperty(nameof(Title))]
    public string Title { get; set; } = null!;
    [JsonProperty(nameof(DivisionId))]
    public string DivisionId { get; set; } = null!;
    [JsonProperty(nameof(SubTaskIds))]
    public List<string> SubTaskIds { get; set; } = null!;
    [JsonProperty(nameof(DueDate))]
    public string DueDate { get; set; } = null!;

    public Tasks ReplaceModel(Tasks task)
    {
        return new Tasks
        {
            Id = task.Id,
            Title = Title is not null ? Title : task.Title,
            DivisionId = DivisionId is not null ? DivisionId : task.DivisionId,
            JobId = task.JobId,
            IssueDate = task.IssueDate,
            DueDate = DueDate is not null ? DueDate : task.DueDate,
            Status = task.Status,
            Progress = task.Progress
        };
    }
}

public record UpdateUserRecord
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

    public User ReplaceModel(User user)
    {
        return new User
        {
            Id = user.Id,
            Name = Name is not null ? Name : user.Name,
            IdentityId = user.IdentityId,
            RoleId = RoleId is not null ? RoleId : user.RoleId,
            DivisionId = DivisionId is not null ? DivisionId : user.DivisionId,
            SubTaskIds = user.SubTaskIds
        };
    }
}