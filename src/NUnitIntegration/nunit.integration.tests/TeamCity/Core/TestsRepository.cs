using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Common;
using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class TestsRepository : ITestsRepository, ICheckList
    {
        public IEnumerable<ICmdLineToolTest> GetCmdLineToolTests(CertType certType)
        {
            Contract.Ensures(Contract.Result<IEnumerable<ICmdLineToolTest>>() != null);

            return 
                from test in ServiceLocator.Root.GetAllServices<ICmdLineToolTest>()
                where test.CertType == certType
                select test;
        }

        public IEnumerable<ITest> GetCheckList(CertType certType)
        {
            Contract.Ensures(Contract.Result<IEnumerable<ITest>>() != null);

            return GetCmdLineToolTests(certType);
        }
    }
}
