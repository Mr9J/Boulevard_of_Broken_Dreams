﻿namespace BoulevardOfBrokenDreams.Models.DTO
{
    public class GetCurrentUserDTO
    {
        public string id { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string nickname { get; set; } = string.Empty;
        public string thumbnail { get; set; } = string.Empty;
        public string authenticationProvider { get; set; } = string.Empty;

    }
}
