namespace NUnit.Framework.Syntax
{
    public class DefaultConstraintSyntaxTests : SyntaxTest
    {
        public DefaultConstraintSyntaxTests()
        {
            ParseTree = "<default>";
            StaticSyntax = Is.Default;
            BuilderSyntax = Builder().Default;
        }
    }
}
