using System;
using AoL_HCI_Backend.Models;
using MongoDB.Driver;

namespace AoL_HCI_Backend.Services;

public class MongoDBServices
{
    private readonly IMongoDatabase _database;

    public MongoDBServices(string ConnectionString, string DatabaseName)
    {
        var client = new MongoClient(ConnectionString);
        _database = client.GetDatabase(DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string CollectionName)
    {
        return _database.GetCollection<T>(CollectionName);
    }

    public async void SyncStatusTask(Tasks task)
    {
        var tasksCollection = GetCollection<Tasks>("Tasks");
        var jobsCollection = GetCollection<Job>("Jobs");
        var job = await jobsCollection.Find(j => j.Id == task.JobId).FirstOrDefaultAsync();
        if (job != null)
        {
            var tasks = await tasksCollection.Find(t => t.JobId == job.Id).ToListAsync();
            UpdateJobStatusAndProgress(job, tasks);
            await jobsCollection.ReplaceOneAsync(j => j.Id == job.Id, job);
        }
    }

    public async void SyncStatusSubTask(SubTask subTask)
    {
        var tasksCollection = GetCollection<Tasks>("Tasks");
        var jobsCollection = GetCollection<Job>("Jobs");
        var subtaskCollection = GetCollection<SubTask>("Subtasks");
        if (subTask != null)
        {
            var parentTask = await tasksCollection.Find(t => t.Id == subTask.TaskId).FirstOrDefaultAsync();
            if (parentTask != null)
            {
                var subtasks = await subtaskCollection.Find(s => s.TaskId == parentTask.Id).ToListAsync();
                UpdateTaskStatusAndProgress(parentTask, subtasks);
                await tasksCollection.ReplaceOneAsync(t => t.Id == parentTask.Id, parentTask);

                var job = await jobsCollection.Find(j => j.Id == parentTask.JobId).FirstOrDefaultAsync();
                if (job != null)
                {
                    var tasks = await tasksCollection.Find(t => t.JobId == job.Id).ToListAsync();
                    UpdateJobStatusAndProgress(job, tasks);
                    await jobsCollection.ReplaceOneAsync(j => j.Id == job.Id, job);
                }
            }
        }
    }

    private void UpdateJobStatusAndProgress(Job job, List<Tasks> tasks)
    {
        job.Progress = tasks.Count == 0 ? 0 : (int)(tasks.Count(t => t.Status == 2) / (double)tasks.Count);
        if(tasks.Count == 0) job.Status = 0;
        else job.Status = tasks.All(t => t.Status == 0) ? 0 : tasks.All(t => t.Status == 2) ? 2 : 1;
    }

    private void UpdateTaskStatusAndProgress(Tasks task, List<SubTask> subtasks)
    {
        task.Progress = subtasks.Count == 0 ? 0 : (int)(subtasks.Count(s => s.Status == 2) / (double)subtasks.Count);
        if(subtasks.Count == 0) task.Status = 0;
        else task.Status = subtasks.All(s => s.Status == 0) ? 0 : subtasks.All(s => s.Status == 2) ? 2 : 1;
    }
}
