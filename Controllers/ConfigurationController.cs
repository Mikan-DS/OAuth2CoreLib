using Microsoft.AspNetCore.Mvc;

namespace OAuth2CoreLib.Controllers
{
    [Route(".well-known/")]
    public class ConfigurationController : Controller
    {
        [HttpGet("openid-configuration")]
        public IActionResult OpenIdConfiguration()
        {
            string host_name = $"https://{Request.Host.Value}";
            return Json(new
            {
                issuer = host_name,
                authorization_endpoint = $"{host_name}/oauth2/auth",
                token_endpoint = $"{host_name}/oauth2/token"
                //TODO: подгружать нужные данные
            });
        }
    }
}
