namespace TemplateBlazorWasmHostedNet8.Shared.Services;

public class TokenService(
        IOptionsSnapshot<ConfigurationServiceOptions> _optionsSnapshot
    )
{
    public string GenerateTokenJwt(int id, string userName, string userNomeCompleto, bool IsAdmin)
    {
        //getting the secret key
        string jwtKey = _optionsSnapshot.Value.ApiKeyJwt;
        var key = Encoding.ASCII.GetBytes(jwtKey);

        //create claims
        var claimId = new Claim(ClaimTypes.NameIdentifier, id.ToString());
        var claimUser = new Claim(ClaimTypes.Name, userName);
        var claimRole = new Claim(ClaimTypes.Role, IsAdmin ? "Admin" : "Default");
        var claimUserNomeCompleto = new Claim(EnumTokenService.ClaimNomeCompleto.ToString(), userNomeCompleto);

        //create claimsIdentity
        var claimsIdentity = new ClaimsIdentity(new[] {
            claimId,
            claimUser,
            claimRole,
            claimUserNomeCompleto
        });

        int tokenJwtExpiresInMinutes = _optionsSnapshot.Value.TokenJwtExpiresInMinutes;

        // generate token that is valid for X days
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddMinutes(tokenJwtExpiresInMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        //creating a token handler
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        //returning the token back
        return tokenHandler.WriteToken(token);
    }

    public (bool? IsTokenValid, bool? IsToUpdateToken, string? ErrorMessage) ValidateTokenJWT(string tokenJWTToBeValided)
    {
        try
        {
            tokenJWTToBeValided = tokenJWTToBeValided.Trim();
            string jwtKey = _optionsSnapshot.Value.ApiKeyJwt;
            var key = Encoding.ASCII.GetBytes(jwtKey);

            //preparing the validation parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            //validating the token
            var principle = tokenHandler.ValidateToken(tokenJWTToBeValided, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = (JwtSecurityToken)securityToken;

            if (jwtSecurityToken != null
                && jwtSecurityToken.ValidTo > DateTime.Now
                && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return (IsTokenValid: true, IsToUpdateToken: false, ErrorMessage: null);
            }
            else
            {
                return (IsTokenValid: false, IsToUpdateToken: false, ErrorMessage: "Token não validado!");
            }
        }
        catch (Exception ex)
        {
            // idx10223: lifetime validation failed. The token is expired. 
            if (ex.Message.Contains("IDX10223"))
            { 
                return (IsTokenValid: false, IsToUpdateToken: true, ErrorMessage: null);
            }
            else
            {
                return (IsTokenValid: false, IsToUpdateToken: false, ErrorMessage: ex.Message);
            }
        }
    }

    #region Método para Pega uma Claim do Token JWT usando o nome da Claim desejada
    public string? GetClaimFromTokenJWTByClaimName(string tokenJwt, string claimName)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(tokenJwt);
        var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
        return claimValue;
    }
    #endregion

    #region Método para Extrair todas as Claims do Token JWT
    public List<Claim>? GetClaimsFromTokenJWT(string tokenJwt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(tokenJwt);
        var claimsWithThemValues = securityToken.Claims;
        return claimsWithThemValues.ToList();
    }
    #endregion

    public string GenerateRefreshTokenJWT()
    {
        var bytes = new byte[32];
        using var rng = new RNGCryptoServiceProvider();
        rng.GetBytes(bytes);

        // and if you need it as a string...
        string hash1 = BitConverter.ToString(bytes);

        // or maybe...
        string hash2 = BitConverter.ToString(bytes).Replace("-", "").ToLower();
        return hash1 + hash2;
    }
}
