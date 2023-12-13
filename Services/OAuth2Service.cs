using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OAuth2CoreLib.Exceptions;
using OAuth2CoreLib.Models;
using OAuth2CoreLib.RequestFields;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace OAuth2CoreLib.Services
{
    public class OAuth2Service : IOAuth2Service
    {
        public readonly OAuthDbContext oAuthDbContext;
        private readonly IConfiguration _configuration;
        public OAuth2Service(
                OAuthDbContext oAuthDbContext,
                IConfiguration configuration

            )
        {

            this.oAuthDbContext = oAuthDbContext;
            _configuration = configuration;

        }

        public OAuthDbContext OAuthDbContext => oAuthDbContext;

        [Obsolete("Существует более общий метод")]
        public string AddUser(string user_id, string? secret)
        {
            if (string.IsNullOrEmpty(user_id))
            {
                return "user_id is required";
            }
            try
            {
                User user = new User() { UserId = user_id, Secret = secret };

                OAuthDbContext.Users.Add(user);
                OAuthDbContext.SaveChanges();
            }
            catch (Exception e)
            {

                return e.Message;
            }
            return String.Empty;
        }

        public string GenerateCode(AuthRequest authRequest, User user)
        {

            Client? client = OAuthDbContext.Clients.Include(e => e.Scopes).ThenInclude(cs => cs.ResourceScope).FirstOrDefault(c => c.ClientId == authRequest.client_id);
            if (client == null)
            {
                throw new OAuthException("Client not found");
            }

            string cryptoKey = GenerateCryptoKey(32);
            while (OAuthDbContext.AuthorizationCodes.FirstOrDefault(ac => ac.Code == cryptoKey) != null)
            {
                cryptoKey = GenerateCryptoKey(32);
            }

            AuthorizationCode code = new()
            {
                Code = cryptoKey,
                User = user,
                Activated = false
            };

            List<AuthorizationCodeScope> scopes = new List<AuthorizationCodeScope>();

            if (authRequest.scope != null)
            {
                string[] requestedScopes = authRequest.scope.Split(' ');
                ResourceScope[] allowedScopes = client.Scopes.Select(s => s.ResourceScope).ToArray();
                ResourceScope[] allowedUserScopes;
                try
                {
                    allowedUserScopes = user.Scopes.Select(s => s.ResourceScope).ToArray();
                }
                catch (Exception)
                {

                    throw new OAuthException("Client scopes exception");
                }
                
                foreach (string scope in requestedScopes)
                {
                    ResourceScope? resourceScope = allowedScopes.FirstOrDefault(a => a.Scope == scope);

                    if (resourceScope == null)
                    {
                        throw new OAuthException("Non available scope for current client");
                    }

                    if (allowedUserScopes.Any(s => s.Scope == scope))
                    {// Очищаем к только тем областям доступа, которыми обладает пользователь
                        scopes.Add(new AuthorizationCodeScope()
                        {
                            AuthorizationCode = code,
                            ResourceScope = resourceScope
                        });
                    }

                }
            }
            user.LastAccessed = DateTime.UtcNow;
            client.LastAccessed = DateTime.UtcNow;
            OAuthDbContext.AuthorizationCodes.Add(code);
            OAuthDbContext.AuthorizationCodeScopes.AddRange(scopes);

            OAuthDbContext.SaveChanges(true);

            return cryptoKey;
        }

        public string GenerateToken(TokenRequest tokenRequest)
        {
            if (string.IsNullOrEmpty(tokenRequest.grant_type))
            {
                throw new OAuthException("grant_type is required.");
            }
            if (string.IsNullOrEmpty(tokenRequest.client_id))
            {
                throw new OAuthException("client_id is required.");

            }

            tokenRequest.client_secret ??= String.Empty;
            Client? client = OAuthDbContext.Clients.FirstOrDefault(c => c.ClientId == tokenRequest.client_id && c.Secret == tokenRequest.client_secret);
            if (client == null)
            {
                throw new OAuthException("Client not found");
            }


            AuthorizationCode? code = OAuthDbContext.AuthorizationCodes
                .Include(c => c.User)
                .Include(c => c.Scopes)
                .ThenInclude(c => c.ResourceScope)
                .FirstOrDefault(a => a.Code == tokenRequest.code);

            if (code == null || code.Activated)
            {
                throw new OAuthException("Code is wrong or old");
            }

            string[] scopes = code.Scopes.Select(s => s.ResourceScope.Scope).ToArray();

            // Создание JWT токена
            var tokenHandler = new JwtSecurityTokenHandler();

            List<Claim> claims = new List<Claim>()
            {
                new Claim("client_id", client.ClientId),
                new Claim("scopes", string.Join(' ', scopes)),
            };

            if (code.User != null)
            {
                claims.Add(
                    new Claim("user_id", code.User.UserId)
                    );
            }

            X509Certificate2 certificate = new X509Certificate2(_configuration["Certificate:private"], _configuration["Certificate:private_key"]);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                SigningCredentials = new X509SigningCredentials(certificate),
                Issuer = Environment.GetEnvironmentVariable("ASPNETCORE_URLS"),
                Expires = DateTime.UtcNow.AddHours(12)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            code.Activated = true;
            client.LastAccessed = DateTime.UtcNow;
            oAuthDbContext.SaveChanges();

            return tokenHandler.WriteToken(token);
        }

        public User? GetAuthenticatedUser(string user_id, string? secret)
        {
            return OAuthDbContext.Users.Include(u => u.Scopes).ThenInclude(s => s.ResourceScope).FirstOrDefault(u => u.UserId == user_id && u.Secret==secret);
        }

        public string GenerateCryptoKey(int size=32)
        {
            var rand = RandomNumberGenerator.Create();
            byte[] bytes = new byte[size];
            rand.GetBytes(bytes);
            var code = Base64UrlEncoder.Encode(bytes);
            return code;
        }

        public Client CreateOrModifyClient(string client_id, string? secret, bool? enabled, string[]? scope)
        {
            Client client = OAuthDbContext.Clients.FirstOrDefault(c => c.ClientId == client_id) ?? new Client()
            {
                ClientId = client_id,
                Secret = String.Empty,
                Enabled = (bool)(enabled != null ? enabled : true),
            };
            if (scope != null)
            {
                var clientScopes = OAuthDbContext.ClientScopes.Where(cs => cs.Client == client);
                OAuthDbContext.ClientScopes.AddRange(
                    SynchScopes(clientScopes, scope, OAuthDbContext.ClientScopes).Select(s => new ClientScope() { Client = client, ResourceScope = s })
                    );
                client.Updated = DateTime.UtcNow;
            }

            if (enabled != null)
            {
                client.Enabled = (bool)enabled;
                client.Updated = DateTime.UtcNow;
            }

            if (secret != null)
            { // Секрет может быть и пустым
                client.Secret = (string)secret;
                client.Updated = DateTime.UtcNow;
            }
            OAuthDbContext.SaveChanges();
            return client;
        }

        public User CreateOrModifyUser(string user_id, string? secret, bool? enabled, string[]? scope)
        {
            User user = OAuthDbContext.Users.FirstOrDefault(u => u.UserId == user_id) ?? new User()
            {
                UserId = user_id,
                Secret = String.Empty,
            };
            if (scope != null)
            {
                var usersScopes = OAuthDbContext.UserAllowedScopes.Where(cs => cs.User == user);
                OAuthDbContext.UserAllowedScopes.AddRange(
                    SynchScopes(usersScopes, scope, OAuthDbContext.UserAllowedScopes).Select(s => new UserAllowedScope() { User = user, ResourceScope = s })
                    );
                user.Updated = DateTime.UtcNow;
            }

            if (enabled != null)
            {
                user.Enabled = (bool)enabled;
                user.Updated = DateTime.UtcNow;
            }

            if (secret != null)
            { // Секрет может быть и пустым
                user.Secret = (string)secret;
                user.Updated = DateTime.UtcNow;
            }
            OAuthDbContext.SaveChanges();
            return user;
        }

        public List<ResourceScope> SynchScopes<ScopeT>(IQueryable<ScopeT> currentScopes, string[] newScopes, DbSet<ScopeT> entities) where ScopeT : class, IScope
        {
            IQueryable<ScopeT> scopesToRemove = currentScopes.Where(cs => !newScopes.Contains(cs.ResourceScope.Scope));
            IEnumerable<string> scopesToAdd = newScopes.Where(scope => !currentScopes.Any(cs => cs.ResourceScope.Scope == scope));

            foreach (var scope in scopesToRemove)
            {
                entities.Remove(scope);
            }
            var allScopes = OAuthDbContext.ResourceScopes;
            List<ResourceScope> addedScopes = new List<ResourceScope>();
            foreach (var scope in scopesToAdd)
            {
                ResourceScope? resourceScope = allScopes.FirstOrDefault(s => s.Scope == scope);
                resourceScope ??= new ResourceScope()
                {
                    Scope = scope
                };
                addedScopes.Add(resourceScope);
            }
            return addedScopes;
        }
    }
}
