namespace TestApp.Business.Const
{
    public static class ValidationErrors
    {
        public const string FIELD_IS_REQUIRED = "{0} field is required.";
        public const string TOO_LONG = "{0} field is too long. Max {1} characters allowed.";
        public const string NOT_A_COUNTRY_CODE = "{0} is not a valid ISO 3166 country code.";
        public const string INVALID_VALUE = "{0} field has invalid value {1}. Allowed set: {2}.";
    }
}