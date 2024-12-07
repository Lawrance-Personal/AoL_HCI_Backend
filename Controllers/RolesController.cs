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
    public class RolesController(MongoDBServices database, IAuthenticationServices authentication) : ControllerBase
    {
        private readonly MongoDBServices _database = database;
        private readonly IAuthenticationServices _authentication = authentication;

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ReturnListRecord<Role>>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, [FromQuery] string? id = null)
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
            var filterBuilder = Builders<Role>.Filter;
            var filter = filterBuilder.Empty;
            if (!string.IsNullOrEmpty(id)) filter &= filterBuilder.Eq(u => u.Id, id);
            return Ok(new ReturnListRecord<Role>(await _database.GetCollection<Role>("Roles").Find(filter).ToListAsync(), newToken));
        }
    }
}
