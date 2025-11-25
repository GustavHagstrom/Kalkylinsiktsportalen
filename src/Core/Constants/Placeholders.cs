namespace Core.Constants;
public static class Placeholders
{
    // The value of these must not ever be changed!!!!
    public static class Estimation
    {
        public static class Properties
        {
            public const string Id = "{Id}";
            public const string Name = "{Name}";
            public const string Description = "{Description}";
            public const string Customer = "{Customer}";
            public const string Place = "{Place}";
            public const string HandlingOfficer = "{HandlingOfficer}";
            public const string ConfirmationOfficer = "{ConfirmationOfficer}";
            public const string EstimationDate = "{EstimationDate}";
            public const string TenderTotal = "{TenderTotal}";
        }
        public static class Functions
        {
            /// <summary>
            /// Get recourse with passed argument value. Should throw error if none or multiple resources are returned.
            /// </summary>
            public static string GetResourceWithAccountSingle(string ? value = null) => value is null ? "AccountSingle()" : $"AccountSingle({value})";
            /// <summary>
            /// Mathematical calculation. Should be the last function to execute.
            /// </summary>
            public static string Calculate(string? value = null) => value is null ? "Calc()" : $"Calc({value})";

        }
    }
    

}
