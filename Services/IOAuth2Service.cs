using Microsoft.EntityFrameworkCore;
using OAuth2CoreLib.Models;
using OAuth2CoreLib.RequestFields;

namespace OAuth2CoreLib.Services
{
    public interface IOAuth2Service
    {
        OAuthDbContext OAuthDbContext { get; }

        public string GenerateCode(AuthRequest authRequest, User user);
        public string GenerateToken(TokenRequest tokenRequest);

        public string AddUser(string user_id, string? secret);

        public User? GetAuthenticatedUser(string user_id, string? secret);

        public Client CreateOrModifyClient(string client_id, string? secret, bool? enabled, string[]? scopes);
        public User CreateOrModifyUser(string user_id, string? secret, string[]? scopes);

        public List<ResourceScope> SynchScopes<ScopeT>(IQueryable<ScopeT> currentScopes, string[] newScopes, DbSet<ScopeT> entities) where ScopeT : class, IScope;

    }
}
