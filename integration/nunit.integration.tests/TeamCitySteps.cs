namespace nunit.integration.tests
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using JetBrains.TeamCity.ServiceMessages;

    using Dsl;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    [Binding]
    public class TeamCitySteps
    {
        [Given(@"I want to use (.+) type of TeamCity integration")]
        public void UseIntegrationType(string teamCityIntegration)
        {
            var ctx = ScenarioContext.Current.GetTestContext();
            var configuration = ctx.GetOrCreateNUnitConfiguration();
            switch (teamCityIntegration.ConvertToTeamCityIntegration())
            {
                case TeamCityIntegration.CmdArguments:
                    configuration.AddArg(new CmdArg(DataType.TeamCity));
                    break;

                case TeamCityIntegration.EnvVariable:
                    configuration.AddEnvVariable(new EnvVariable(DataType.TeamCity));
                    break;

                default:
                    throw new NotSupportedException(teamCityIntegration);
            }
        }

        [Then(@"the output should contain TeamCity service messages:")]
        public void ResultShouldContainServiceMessage(Table data)
        {
            var ctx = ScenarioContext.Current.GetTestContext();            
            var messages = new TeamCityServiceMessageParser().Parse(ctx.TestSession.Output).ToList();
            Assert.AreEqual(data.RowCount, messages.Count, $"{ctx}\nExpected service messages are {data.RowCount} but actual is {messages.Count}");

            var invalidMessages = (
                from item in data.Rows.Zip(messages, (row, message) => new { row, message })
                where !VerifyServiceMessage(item.row, item.message)
                select item).ToList();

            if (invalidMessages.Any())
            {
                var details = string.Join("\n", invalidMessages.Select(i => CreateErrorMessage(i.row, i.message)));
                Assert.Fail($"See {ctx}\n{details}");
            }
        }

        private static bool VerifyServiceMessage(TableRow row, IServiceMessage serviceMessage)
        {
            var messageNameRegex = new Regex(row[""]);
            if (!messageNameRegex.IsMatch(serviceMessage.Name))
            {
                return false;
            }

            foreach (var key in row.Keys)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                string rowValue;
                if (!row.TryGetValue(key, out rowValue))
                {
                    continue;
                }

                var serviceMessageValue = serviceMessage.GetValue(key) ?? "";
                var rowValueRegex = new Regex(rowValue ?? "");
                if (!rowValueRegex.IsMatch(serviceMessageValue))
                {
                    return false;
                }
            }

            return true;
        }

        private static string CreateErrorMessage(TableRow row, IServiceMessage serviceMessage)
        {
            var rowInfo = string.Join(", ", from key in row.Keys select $"{key} = {row[key]}");
            var serviceMessageInfo = string.Join(", ", from key in serviceMessage.Keys select $"{key} = {serviceMessage.GetValue(key)}");            

            return $"Expected service message should has:\n{rowInfo}\nbut it has:\n{serviceMessageInfo}";
        }
    }
}