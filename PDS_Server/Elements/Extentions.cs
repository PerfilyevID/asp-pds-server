using System;

namespace CommonEnvironment.Elements
{
    public static class Extentions
    {
        public static bool IsExpired(this DbAccount user)
        {
            if (user.Access.Ticks > DateTime.UtcNow.Ticks)
            {
                return false;
            }
            return true;
        }

        public static dynamic ToResponse(this DbAccount user)
        {
            return new
            {
                FirstName = user.FirstName,
                LastName = user.SecondName,
                Team = user.Team,
                ImageData = user.ImageData
            };
        }
        public static dynamic ToResponse(this DbApplication application)
        {
            return new
            {
                Link = application.Link,
                Version = application.Version,
                Changelog = application.Changelog
            };
        }
    }
}
