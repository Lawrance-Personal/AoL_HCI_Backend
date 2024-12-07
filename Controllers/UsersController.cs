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
    public class UsersController(MongoDBServices database, IAuthenticationServices authentication) : ControllerBase
    {
        private readonly MongoDBServices _database = database;
        private readonly IAuthenticationServices _authentication = authentication;

        [HttpPost]
        public async Task<ActionResult<ReturnDataRecord<User>>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, CreateUserRecord createUser){
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
            User user = createUser.ToModel();
            var IdentityId = AuthenticationServices.Register(createUser.Email, createUser.Password).Result;
            if(string.IsNullOrEmpty(IdentityId)) return BadRequest("Email Address Already Exist");
            user.IdentityId = IdentityId;
            await _database.GetCollection<User>("Users").InsertOneAsync(user);
            return CreatedAtRoute(new {id = user.Id}, new ReturnDataRecord<User>(user, newToken));
        }
        [HttpPost("login")]
        public async Task<ActionResult<ReturnDataRecord<User>>> Login(Credential credential){
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            var token = await _authentication.Login(credential.Email, credential.Password);
            if(token == null || token.IdToken is null) return BadRequest("Invalid Credential");
            return Ok(new ReturnDataRecord<User>(await _database.GetCollection<User>("Users").Find(u => u.IdentityId == token.IdentityId).FirstOrDefaultAsync(), token));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ReturnListRecord<User>>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, [FromQuery] string? id = null, [FromQuery] string? divisionId = null, [FromQuery] string? roleId = null, [FromQuery] string? subTaskId = null)
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
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.Empty;
            if (!string.IsNullOrEmpty(id)) filter &= filterBuilder.Eq(u => u.Id, id);
            if (!string.IsNullOrEmpty(divisionId)) filter &= filterBuilder.Eq(u => u.DivisionId, divisionId);
            if (!string.IsNullOrEmpty(roleId)) filter &= filterBuilder.Eq(u => u.RoleId, roleId);
            if (!string.IsNullOrEmpty(subTaskId)) filter &= filterBuilder.AnyEq(u => u.SubTaskIds, subTaskId);
            return Ok(new ReturnListRecord<User>(await _database.GetCollection<User>("Users").Find(filter).ToListAsync(), newToken));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<User>>> Update([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id, UpdateUserRecord updateUser){
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
            var user = await _database.GetCollection<User>("Users").Find(u => u.Id == id).FirstOrDefaultAsync();
            if(user == null) return NotFound("User Not Found");
            user = updateUser.ReplaceModel(user);
            await _database.GetCollection<User>("Users").ReplaceOneAsync(u => u.Id == id, user);
            if(updateUser.Email != null) AuthenticationServices.UpdateEmail(user.IdentityId, updateUser.Email);
            if(updateUser.Password != null) AuthenticationServices.UpdatePassword(user.IdentityId, updateUser.Password);
            return new ReturnDataRecord<User>(user, newToken);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<User>>> Delete([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
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
            var user = await _database.GetCollection<User>("Users").Find(u => u.Id == id).FirstOrDefaultAsync();
            if(user == null) return NotFound("User Not Found");
            await _database.GetCollection<User>("Users").DeleteOneAsync(u => u.Id == id);
            AuthenticationServices.Unregister(user.IdentityId);
            return new ReturnDataRecord<User>(user, newToken);
        }
    }
}
