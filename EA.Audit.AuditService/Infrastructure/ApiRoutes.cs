namespace EA.Audit.AuditService.Infrastructure
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root + "/" + Version;

        public static class Audits
        {
            public const string GetAll = Base + "/audits";
            public const string Get = Base + "/audits/{id}";
            public const string Create = Base + "/audits";
            public const string Search = Base + "/audits/search";
        }

        public static class AuditApplications
        {
            public const string GetAll = Base + "/applications";
            public const string Get = Base + "/applications/{id}";
            public const string Create = Base + "/applications";
        }

        public static class AuditLevels
        {
            public const string GetAll = Base + "/auditlevels";
            public const string Get = Base + "/auditlevels/{id}";
            public const string Create = Base + "/auditlevels";
        }

        public static class AuditTypes
        {
            public const string GetAll = Base + "/audittypes";
            public const string Get = Base + "/audittypes/{id}";
            public const string Create = Base + "/audittypes";
        }


    }

}