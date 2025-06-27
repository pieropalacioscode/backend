using AutoMapper;
using Bussnies;
using Constantes;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using IBussines;
using IBussnies;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.RequestResponse;
using Models.ResponseResponse;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UtilSecurity.UtilSecurity;

namespace Bussines
{
    public class AuthBussnies : IAuthBussines
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioBussnies _userBussnies;
        private readonly appSettings _appSettings;
        private readonly IConfiguration _configuration;
        public AuthBussnies(IMapper mapper)
        {
            _userBussnies = new UsuarioBussnies(mapper);
            _mapper = mapper;
            
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public LoginResponse login(LoginRequest request)
        {
            var res = new LoginResponse();

            // Validación básica de entrada nula
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                res.Message = "El nombre de usuario y la contraseña son requeridos.";
                res.Usuario = null;
                res.Success = false;
                return res;
            }

            // Buscar usuario
            var user = _userBussnies.GetByUserName(request.Username);
            if (user == null || string.IsNullOrWhiteSpace(user.Username))
            {
                res.Message = "Usuario y/o contraseña inválidos.";
                res.Usuario = null;
                res.Success = false;
                return res;
            }

            // Comparar usuarios ignorando mayúsculas/minúsculas
            if (!user.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase))
            {
                res.Message = "Usuario y/o contraseña inválidos.";
                res.Usuario = null;
                res.Success = false;
                return res;
            }

            // Verificar contraseña
            string newPassword = UtilCripto.encriptar_AES(request.Password);
            if (!newPassword.Equals(user.Password))
            {
                res.Message = "Usuario y/o contraseña inválidos.";
                res.Usuario = null;
                res.Success = false;
                return res;
            }

            // Si todo es correcto
            res.Usuario = user;
            res.Message = "Inicio de sesión exitoso.";
            res.Success = true;
            return res;
        }





        //public async Task<LoginResponse> LoginWithGoogle(string idToken)
        //{
        //    var response = new LoginResponse();

        //    try
        //    {
        //        // Valida el token de ID de Google
        //        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

        //        // Aquí puedes adaptar la lógica para verificar si el usuario existe en tu sistema
        //        // Si el usuario no existe, puedes crear uno nuevo o devolver un error

        //        // Supongamos que aquí tienes lógica para verificar si el usuario existe en tu sistema
        //        // Si el usuario no existe, puedes devolver un error
        //        var user = await _userBussnies.GetUserByEmail(payload.Email);
        //        if (user == null)
        //        {
        //            response.Success = false;
        //            response.Message = "Usuario no encontrado";
        //            return response;
        //        }

        //        // Si el usuario existe, crea el token JWT
        //        response.Success = true;
        //        response.Token = CreateTokenGoogle(payload);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = ex.Message;
        //    }

        //    return response;
        //}




        private string CreateTokenGoogle(GoogleJsonWebSignature.Payload payload)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, payload.Subject),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat,
        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
        ClaimValueTypes.Integer64), // ✅ Tipo correcto
        new Claim("UserId", payload.Subject),
        new Claim("Email", payload.Email)
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:TimeJWTMin"])),
                signingCredentials: signIn
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
