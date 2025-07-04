﻿using AutoMapper;
using Bussines;
using Google.Apis.Auth;
using IBussines;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.RequestResponse;
using Models.ResponseResponse;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthBussines _authBussnies;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthController(IMapper mapper)
        {
            _mapper = mapper;
            _authBussnies = new AuthBussnies(mapper);
        }

        [HttpGet]
        public IActionResult get()
        {
            return Ok("El servicio está escuchando");
        }



        /// <summary>
        /// Realiza el proceso de login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = "Username y password son requeridos"
                });
            }

            LoginResponse res = _authBussnies.login(request);

            if (res.Usuario == null || res.Usuario.IdUsuario == 0)
            {
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = res.Message ?? "Credenciales inválidas"
                });
            }

            // Solo aquí generas y devuelves el token
            res.Token = CreateToken(res.Usuario);
            res.RefreshToken = Guid.NewGuid().ToString();
            res.Success = true;

            return Ok(res);
        }


        //[HttpPost("google")]
        //public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthDto googleAuthDto)
        //{
        //    try
        //    {
        //        var payload = await _authBussnies.VerifyGoogleToken(googleAuthDto);

        //        if (payload == null)
        //        {
        //            return BadRequest("Invalid Google token.");
        //        }

        //        // Asumiendo que tienes un método para manejar la lógica de negocio
        //        var user = await _authBussnies.RegisterOrGetUser(payload);

        //        // Lógica para generar un token de sesión/jwt/etc.
        //        var token = _authBussnies.GenerateToken(user);

        //        return Ok(new { token });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Manejar excepciones adecuadamente
        //        return StatusCode(500, ex.Message);
        //    }
        //}


        private static string CreateToken(UsuarioResponse user)
        {
            IConfigurationBuilder configurationBuild = new ConfigurationBuilder();
            configurationBuild = configurationBuild.AddJsonFile("appsettings.json");
            IConfiguration configurationFile = configurationBuild.Build();

            int TimpoVidaToken = int.Parse(configurationFile["Jwt:TimeJWTMin"]);

            var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, configurationFile["Jwt:Subject"]),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

        // ✅ Corrección del campo iat
        new Claim(JwtRegisteredClaimNames.Iat,
            new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
            ClaimValueTypes.Integer64),

        new Claim("UserId", user.IdUsuario.ToString()),
        new Claim("DisplayName", "Jose Salazar"),
        new Claim("UserName", user.Username),
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configurationFile["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                configurationFile["Jwt:Issuer"],
                configurationFile["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(TimpoVidaToken),
                signingCredentials: signIn);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }





    }
}

