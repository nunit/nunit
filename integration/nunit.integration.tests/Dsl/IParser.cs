namespace nunit.integration.tests.Dsl
{
    internal interface IParser<T>
    {
        T Parse(string text);
    }
}
