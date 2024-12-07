using AoL_HCI_Backend.Contracts;
using AoL_HCI_Backend.Models;
using AoL_HCI_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AoL_HCI_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubTasksController(MongoDBServices database, IAuthenticationServices authentication) : ControllerBase
    {
        private readonly MongoDBServices _database = database;
        private readonly IAuthenticationServices _authentication = authentication;

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<SubTask>>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, CreateSubTaskRecord createSubTask){
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            var tokenArr = token.Split(" ");
            AuthToken newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
            if (JwtValidator.TokenIsExpired(tokenArr[1]))
            {
                var refreshedToken = await _authentication.RefreshToken(refreshToken);
                if (refreshedToken != null)
                {
                    newToken = refreshedToken;
                    if (newToken.IdToken is null) return Unauthorized("Token Expired");
                }
                else
                {
                    return Unauthorized("Token Expired");
                }
            }
            SubTask subTask = createSubTask.ToModel();
            await _database.GetCollection<SubTask>("SubTasks").InsertOneAsync(subTask);
            _database.SyncStatusSubTask(subTask);
            return CreatedAtRoute(new { id = subTask.Id }, new ReturnDataRecord<SubTask>(subTask, newToken));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ReturnListRecord<SubTask>>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, [FromQuery] string? id = null, [FromQuery] string? taskId = null, [FromQuery] string? userId = null, [FromQuery] string? dueDate = null, [FromQuery] int? status = null){
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            var tokenArr = token.Split(" ");
            AuthToken newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
            if (JwtValidator.TokenIsExpired(tokenArr[1]))
            {
                var refreshedToken = await _authentication.RefreshToken(refreshToken);
                if (refreshedToken != null)
                {
                    newToken = refreshedToken;
                    if (newToken.IdToken is null) return Unauthorized("Token Expired");
                }
                else
                {
                    return Unauthorized("Token Expired");
                }
            }
            var filterBuilder = Builders<SubTask>.Filter;
            var filter = filterBuilder.Empty;
            if(!string.IsNullOrEmpty(id)) filter &= filterBuilder.Eq(u => u.Id, id);
            if(!string.IsNullOrEmpty(taskId)) filter &= filterBuilder.Eq(u => u.TaskId, taskId);
            if(!string.IsNullOrEmpty(userId)) filter &= filterBuilder.AnyEq(u => u.UserIds, userId);
            if(!string.IsNullOrEmpty(dueDate)){
                var dueDateUnix = long.Parse(dueDate);
                filter &= filterBuilder.Lt(u => long.Parse(u.DueDate), dueDateUnix);
            }
            if(status != null) filter &= filterBuilder.Eq(u => u.Status, status);
            return Ok(new ReturnListRecord<SubTask>(await _database.GetCollection<SubTask>("SubTasks").Find(filter).ToListAsync(), newToken));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<SubTask>>> Update([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id, UpdateSubTaskRecord updateSubTask, [FromQuery] int? status){
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            var tokenArr = token.Split(" ");
            AuthToken newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
            if(JwtValidator.TokenIsExpired(tokenArr[1])){
                var refreshedToken = await _authentication.RefreshToken(refreshToken);
                if (refreshedToken!= null)
                {
                    newToken = refreshedToken;
                    if (newToken.IdToken is null) return Unauthorized("Token Expired");
                }
                else
                {
                    return Unauthorized("Token Expired");
                }
            }
            var subTask = await _database.GetCollection<SubTask>("SubTasks").Find(u => u.Id == id).FirstOrDefaultAsync();
            if(status != null){
                if(subTask == null) return NotFound("SubTask not found");
                subTask.Status = status.Value;
                await _database.GetCollection<SubTask>("SubTasks").ReplaceOneAsync(u => u.Id == id, subTask);
                _database.SyncStatusSubTask(subTask);
                return Ok(new ReturnDataRecord<SubTask>(subTask, newToken));
            }
            if(subTask == null) return NotFound("SubTask not found");
            subTask = updateSubTask.ReplaceModel(subTask);
            await _database.GetCollection<SubTask>("SubTasks").ReplaceOneAsync(u => u.Id == id, subTask);
            _database.SyncStatusSubTask(subTask);
            return new ReturnDataRecord<SubTask>(subTask, newToken);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<SubTask>>> Delete([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            var tokenArr = token.Split(" ");
            AuthToken newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
            if(JwtValidator.TokenIsExpired(tokenArr[1])){
                var refreshedToken = await _authentication.RefreshToken(refreshToken);
                if (refreshedToken!= null)
                {
                    newToken = refreshedToken;
                    if (newToken.IdToken is null) return Unauthorized("Token Expired");
                }
                else
                {
                    return Unauthorized("Token Expired");
                }
            }
            var subTask = await _database.GetCollection<SubTask>("SubTasks").Find(u => u.Id == id).FirstOrDefaultAsync();
            if(subTask == null) return NotFound("SubTask not found");
            await _database.GetCollection<SubTask>("SubTasks").DeleteOneAsync(u => u.Id == id);
            _database.SyncStatusSubTask(subTask);
            return new ReturnDataRecord<SubTask>(subTask, newToken);
        }
    }
}
