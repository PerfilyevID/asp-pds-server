using System;

namespace CommonEnvironment.Elements
{
    public class DbAccount : DbElement
    {
        public DbAccount() { }

        public string Login { get; set; }
        public string Password { get; set; }
        public string VerifyLink { get; set; } = null;
        public bool IsVerified { get; set; } = false;

        /// <summary>
        /// Values: "Admin", "User", "TeamOwner", "NewUser", "Expired".
        /// </summary>
        public string Role { get; set; } = "NewUser";

        // Personal
        public string ImageData { get; set; } = null;
        public string FirstName { get; set; } = null;
        public string SecondName { get; set; } = null;

        // Commercial
        public DateTime Access { get; set; } = DateTime.Now;

        // Team
        public string Team { get; set; } = null;
        public bool Owner { get; set; } = false;
    }

}
