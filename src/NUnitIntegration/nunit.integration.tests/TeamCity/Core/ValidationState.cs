namespace NUnit.Integration.Tests.TeamCity.Core
{
    // Items should be orderer from successful to failed
    internal enum ValidationState : byte
    {
        Valid,

        HasWarning,

        Unknown,
        
        NotValid
    }
}
