using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    internal interface ICase : ICaseDescription, IOutputValidator
    {
    }
}
