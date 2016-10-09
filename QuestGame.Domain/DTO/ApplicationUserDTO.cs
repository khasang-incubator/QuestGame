﻿namespace QuestGame.Domain.DTO
{
    public class ApplicationUserDTO
    {
        public string Id { get; set; }
        public string NickName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }

        public string Token { get; set; }
    }
}
