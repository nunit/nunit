namespace nunit.integration.tests.Dsl
{
    internal class Method
    {
        public Method(string name, string template)
        {
            Name = name;
            Template = template;
        }

        public string Name { get; private set; }

        public string Template { get; private set; }
    }
}
