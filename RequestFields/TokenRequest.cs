﻿namespace OAuth2CoreLib.RequestFields
{
    public class TokenRequest
    {
        public string grant_type { get; set; }
        public string code { get; set; }
        public string redirect_uri { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string scope { get; set; }
        public string refresh_token { get; set; }
        public string state { get; set; }
        public string nonce { get; set; }
        public string response_type { get; set; }
        public string prompt { get; set; }
    }
}
