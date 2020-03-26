namespace EA.Audit.Common.Infrastructure
{
    public static class Constants
    {
        public static class ErrorMessages
        {
            public const string NoAuditApplicationExists = "Audit Application does not exist";
            public const string DuplicateRequestForAudit = "Duplicate Request for Create Audit";
            public const string DuplicateRequestForAuditApplication = "Duplicate Request for Create Audit Application";
            public const string InvalidPageNumber = "Invalid page number";
            public const string InvalidPageSize = "Invalid page size";
            public const string PublishAuditFailure = "Publish Audit Failed";
            public const string NoItemExists = "Item does not exist";
            public const string NoApplicationFound = "No Application found for ClientId";
            public const string XRequestIdIsMissing = "x-requestid is missing in Headers";
            public const string NoClientIdFound = "No ClientId found on HttpContext";
        }

        public static class SuccessMessages
        {
            public const string AuditPublishSuccess = "Audit Successfully Published";
            public const string AuditApplicationCreatedSuccess = "Audit Application Successfully Created";
        }

        public static class XRequest
        {
            public const string XRequestIdHeaderName = "x-requestid";
        }

        public static class Auth
        {
            public const string ReadAudits = "audit-api/read_audits";
            public const string CreateAudits = "audit-api/create_audit";
            public const string Admin = "audit-api/audit_admin";
        }

        public static class Redis
        {
            public const string AuditChannel = "AuditCommand";
        }
    }
}
