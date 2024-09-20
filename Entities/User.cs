﻿namespace VerticalSliceAPI.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
