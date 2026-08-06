namespace GuardianCommunication.Shared.Definition
{
    public enum AuthorizationTypeEnumeration
    {
        NoAuth = 0,
        ApiKeyHeader = 1,
        ApiKeyQueryString = 2,
        BearerToken = 3,
        BasicAuth = 4,
        Digest = 5,
    }
}
