using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using VCEWeb.Models;
using VCE.Shared;

namespace VCEWeb.Controllers
{
    ///---------------------------------------------------------------------------
    /// <summary>
    /// This class will contain the property of table.
    /// </summary>
    /// <remarks>
    /// Created on :- 22-09-2023
    /// Created By :- VCE
    /// Purpose :- Sepration of Authorization.
    /// </remarks>
    ///---------------------------------------------------------------------------
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Methods

        private IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("Login")]
        public virtual async Task<IActionResult> GetLogin(string Token)
        {
            string Result = "{}";

            try
            {
                Result = postdata(Token);
            }
            catch (InvalidJwtException e)
            {
                Result = "{}";
            }
            return Ok(Result);
        }

        /// <summary>
        /// Endpoint to retrieve Post table data.
        /// </summary>
        /// <param name="Data">Using JwtRegisteredClaimNames to user Autorize or not.</param>
        /// <returns>Return JSONWebToken Token and other details.</returns>
        public string postdata(string token)
        {
            string responseInString = string.Empty;
            using (var webClient = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                webClient.Headers.Add("ContentType", "application/json");

                var response = webClient.DownloadData(Config.GetTokenInfo + token);
                responseInString = System.Text.Encoding.UTF8.GetString(response);
                GoogleToken googleToken = JsonConvert.DeserializeObject<GoogleToken>(responseInString);
                var claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Sub, googleToken.user_id),
                    new Claim(JwtRegisteredClaimNames.Email, googleToken.expires_in.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                string WebToken = GenerateJSONWebToken(claims);
            }
            return responseInString;
        }

        /// <summary>
        /// Endpoint to retrieve Post table data.
        /// </summary>
        /// <param name="Data">Using To Generating JSONWebToken </param>
        /// <returns>Return JSONWebToken Token.</returns>
        private string GenerateJSONWebToken(List<Claim> userInfo)
        {
            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        } 
        #endregion
    }
}