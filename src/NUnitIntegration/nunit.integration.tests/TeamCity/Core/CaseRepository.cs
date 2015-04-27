using System.Collections.Generic;
using System.Linq;

using NUnit.Integration.Tests.TeamCity.Core.Common;
using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class CaseRepository : ICaseRepository, ICheckList
    {
        public IEnumerable<ICase> GetCases(CertType certType)
        {
            Contract.Ensures(Contract.Result<IEnumerable<ICase>>() != null);

            return 
                from test in ServiceLocator.Root.GetAllServices<ICase>()
                where test.CertType == certType
                select test;
        }

        public IEnumerable<ICaseDescription> GetCheckList(CertType certType)
        {
            Contract.Ensures(Contract.Result<IEnumerable<ICaseDescription>>() != null);

            return GetCases(certType);
        }
    }
}
