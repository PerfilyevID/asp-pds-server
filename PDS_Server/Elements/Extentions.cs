using CommonEnvironment.Elements.Revit;
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
        public static dynamic ToResponse(this DbDocument application)
        {
            return new
            {
                Id = application.Id.ToString(),
                Department = application.Department==null ? null : application.Department.ToString(),
                FoundByUser = application.FoundByUser == null ? null : application.FoundByUser.ToString(),
                FullPath = application.FullPath,
                IsCloudModel = application.IsCloudModel,
                Name = application.Name,
                NamSyncCounte = application.SyncCount,
                ServerGuid = application.ServerGuid,
                Project = application.Project == null ? null : application.Project.ToString(),
            };
        }
    }
}
