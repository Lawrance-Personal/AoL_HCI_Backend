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
    public class DivisionsController(MongoDBServices database, IAuthenticationServices authentication) : ControllerBase
    {
        private readonly MongoDBServices _database = database;
        private readonly IAuthenticationServices _authentication = authentication;

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<Division>>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, CreateDivisionRecord createDivision){
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
            Division division = createDivision.ToModel();
            await _database.GetCollection<Division>("Divisions").InsertOneAsync(division);
            return CreatedAtRoute(new {id = division.Id}, new ReturnDataRecord<Division>(division, newToken));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ReturnListRecord<Division>>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, [FromQuery] string? id = null)
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
            var filterBuilder = Builders<Division>.Filter;
            var filter = filterBuilder.Empty;
            if (!string.IsNullOrEmpty(id)) filter &= filterBuilder.Eq(u => u.Id, id);
            return Ok(new ReturnListRecord<Division>(await _database.GetCollection<Division>("Divisions").Find(filter).ToListAsync(), newToken));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<Division>>> Delete([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
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
            var division = await _database.GetCollection<Division>("Divisions").Find(u => u.Id == id).FirstOrDefaultAsync();
            if(division is null) return NotFound();
            await _database.GetCollection<Division>("Divisions").DeleteOneAsync(u => u.Id == id);
            return new ReturnDataRecord<Division>(division, newToken);
        }
    }
}
