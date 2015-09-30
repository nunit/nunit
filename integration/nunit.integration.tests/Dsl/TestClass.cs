namespace nunit.integration.tests.Dsl
{
    using System;
    using System.Collections.Generic;

    internal class TestClass
    {
        private readonly Dictionary<string, Method> _methods = new Dictionary<string, Method>(StringComparer.InvariantCultureIgnoreCase);
        private readonly List<string> _attributes = new List<string>();

        public TestClass(string namespaceName, string className)
        {
            NamespaceName = namespaceName;
            ClassName = className;
        }

        public string NamespaceName { get; private set; }

        public string ClassName { get; private set; }

        public IEnumerable<Method> Methods => _methods.Values;

        public IEnumerable<string> Attributes => _attributes;

        public Method GetOrCreateMethod(string testMethodName, string methodTemplate)
        {
            Method method;
            if (!_methods.TryGetValue(testMethodName, out method))
            {
                _methods[testMethodName] = method = new Method(testMethodName, methodTemplate);
            }

            return method;
        }

        public void AddAttribute(string attribute)
        {
            _attributes.Add(attribute);
        }
    }
}
