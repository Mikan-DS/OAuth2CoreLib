using Microsoft.AspNetCore.Mvc;
using OAuth2CoreLib.RequestFields;
using OAuth2CoreLib.Services;

namespace OAuth2CoreLib.Controllers
{
    [Route("oauth2/")]

    public class OAuth2Controller : Controller
    {
        private readonly IOAuth2Service oAuth2Service;

        public OAuth2Controller(IOAuth2Service oAuth2Service)
        {
            this.oAuth2Service = oAuth2Service;
        }

        [HttpPost("token")]
        public IActionResult Token(TokenRequest tokenRequest)
        {
            if (string.IsNullOrEmpty(tokenRequest.grant_type))
            {
                return BadRequest("Invalid request: grant_type is required.");
            }
            if (string.IsNullOrEmpty(tokenRequest.client_id))
            {
                return BadRequest("Invalid request: client_id is required.");

            }

            try
            {
           
                 return Ok(oAuth2Service.GenerateToken(tokenRequest));
                
                //Json(); // TODO
            }
            catch (Exception)
            {
                throw;
                return BadRequest("Undhandled exception raised");
            }

            // Шаг 1: Проверить grant_type (тип гранта) и обработать соответствующую логику авторизации
            // - Если grant_type = "authorization_code", выполнить действия для авторизации по коду авторизации
            // - Если grant_type = "password", выполнить действия для авторизации по паролю пользователя
            // - Если grant_type = "client_credentials", выполнить действия для авторизации по учетным данным клиента

            // Шаг 2: Проверить и обработать другие параметры запроса в соответствии с типом гранта:
            // - Если grant_type = "authorization_code", проверить и обработать параметры code, redirect_uri, client_id и client_secret
            // - Если grant_type = "password", проверить и обработать параметры username, password, scope и client_id
            // - Если grant_type = "client_credentials", проверить и обработать параметры scope и client_id

            // Шаг 3: Проверить валидность идентификатора клиента и его секретного ключа
            // - Проверить, соответствуют ли переданные client_id и client_secret действительным учетным данным клиента

            // Шаг 4: Создать и вернуть авторизационный токен
            // - В зависимости от grant_type, создать и вернуть соответствующий тип токена (например, access token или refresh token)

            // Шаг 5: Обработать и вернуть результат авторизации
            // - Вернуть успешный ответ с авторизационным токеном и другими необходимыми данными
            // - Вернуть ошибку, если произошла ошибка при авторизации или валидации параметров

            return Json(tokenRequest);
        }


        //[HttpPost("auth")]
        //public IActionResult Auth(AuthRequest authRequest)
        //{

        //    if (string.IsNullOrEmpty(authRequest.client_id))
        //    {
        //        return BadRequest("Invalid request: client_id is required.");

        //    }

        //    // Шаг 1: Проверить тип ответа (response_type) и выполнить соответствующую логику авторизации
        //    if (authRequest.response_type == "code")
        //    {
        //        // - Если response_type = "code", выполнить действия для получения авторизационного кода
        //        // Реализовать логику для получения авторизационного кода
        //        // ...
        //    }
        //    else if (authRequest.response_type == "token")
        //    {
        //        // - Если response_type = "token", выполнить действия для получения токена доступа
        //        // Реализовать логику для получения токена доступа
        //        // ...
        //    }
        //    else
        //    {
        //        return BadRequest("Invalid request: Invalid response_type.");
        //    }

        //    // Шаг 2: Проверить и обработать другие параметры запроса в соответствии с типом ответа:
        //    // - Если response_type = "code", проверить и обработать параметры client_id и redirect_uri
        //    // - Если response_type = "token", проверить и обработать параметры client_id и scope

        //    // Шаг 3: Проверить валидность идентификатора клиента
        //    // - Проверить, соответствует ли переданный client_id действительному идентификатору клиента

        //    // Шаг 4: Создать и вернуть авторизационный код или токен доступа
        //    // - В зависимости от response_type, создать и вернуть соответствующий тип ответа (авторизационный код или токен доступа)

        //    // Шаг 5: Обработать и вернуть результат авторизации
        //    // - Вернуть успешный ответ с авторизационным кодом или токеном доступа
        //    // - Вернуть ошибку, если произошла ошибка при авторизации или валидации параметров


        //    return Json(authRequest);
        //}


    }

}
