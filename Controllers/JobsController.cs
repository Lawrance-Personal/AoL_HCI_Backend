using AoL_HCI_Backend.Contracts;
using AoL_HCI_Backend.Models;
using AoL_HCI_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AoL_HCI_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController(MongoDBServices database, IAuthenticationServices authentication) : ControllerBase
    {
        private readonly MongoDBServices _database = database;
        private readonly IAuthenticationServices _authentication = authentication;

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<Job>>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, CreateJobRecord createJob){
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            var tokenArr = token.Split(" ");
            AuthToken newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
            if(JwtValidator.TokenIsExpired(tokenArr[1])){
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
            Job job = createJob.ToModel();
            await _database.GetCollection<Job>("Jobs").InsertOneAsync(job);
            return CreatedAtRoute(new {id = job.Id}, new ReturnDataRecord<Job>(job, newToken));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ReturnListRecord<Job>>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, [FromQuery] string? id = null, [FromQuery] string? dueDate = null, [FromQuery] int? status = null)
        {
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
            var filterBuilder = Builders<Job>.Filter;
            var filter = filterBuilder.Empty;
            if (!string.IsNullOrEmpty(id)) filter &= filterBuilder.Eq(u => u.Id, id);
            if (!string.IsNullOrEmpty(dueDate))
            {
                filter &= filterBuilder.Lt(u => u.DueDate, dueDate);
            }
            if (status != null) filter &= filterBuilder.Eq(u => u.Status, status);
            return Ok(new ReturnListRecord<Job>(await _database.GetCollection<Job>("Jobs").Find(filter).ToListAsync(), newToken));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<Job>>> Update([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id, UpdateJobRecord updateJob){
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
            var job = await _database.GetCollection<Job>("Jobs").Find(j => j.Id == id).FirstOrDefaultAsync();
            if (job is null) return NotFound("Job not found");
            job = updateJob.ReplaceModel(job);
            await _database.GetCollection<Job>("Jobs").ReplaceOneAsync(j => j.Id == id, job);
            return new ReturnDataRecord<Job>(job, newToken);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<Job>>> Delete([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
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
            var job = await _database.GetCollection<Job>("Jobs").Find(j => j.Id == id).FirstOrDefaultAsync();
            if (job is null) return NotFound("Job not found");
            await _database.GetCollection<Job>("Jobs").DeleteOneAsync(j => j.Id == id);
            return new ReturnDataRecord<Job>(job, newToken);
        }
    }
}
