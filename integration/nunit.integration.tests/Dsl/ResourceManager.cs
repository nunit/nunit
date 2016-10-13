namespace nunit.integration.tests.Dsl
{
    using System;
    using System.IO;

    internal class ResourceManager
    {
        public string GetContentFromResource(string resourceName)
        {
            string testFileSrc;
            using (var testStream = typeof(Compiler).Assembly.GetManifestResourceStream(resourceName))
            {
                if (testStream == null)
                {
                    throw new InvalidOperationException($"Resource '{resourceName}' was not found");
                }

                using (var sr = new StreamReader(testStream))
                {
                    testFileSrc = sr.ReadToEnd();
                }
            }
            return testFileSrc;
        }
    }
}
