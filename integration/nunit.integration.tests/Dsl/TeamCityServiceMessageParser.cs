namespace nunit.integration.tests.Dsl
{
    using System.Collections.Generic;

    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Read;

    internal class TeamCityServiceMessageParser: IParser<IEnumerable<IServiceMessage>>
    {
        private readonly static ServiceMessageParser ServiceMessageParser = new ServiceMessageParser();

        public IEnumerable<IServiceMessage> Parse(string text)
        {
            return ServiceMessageParser.ParseServiceMessages(text);
        }
    }
}
