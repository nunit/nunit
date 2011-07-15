using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Engine.Runners
{
    public class MasterTestRunner : AbstractTestRunner
    {
        private ServiceContext services;
        private ITestRunner realRunner;

        public MasterTestRunner(ServiceContext services)
        {
            this.services = services;
        }

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Load(TestPackage package)
        {
            this.TestPackage = package;

            services.ProjectService.ExpandProjectPackages(package);
            this.realRunner = services.TestRunnerFactory.MakeTestRunner(package);

            return this.realRunner.Load(package);
        }

        /// <summary>
        /// Unload any loaded TestPackage. If none is loaded,
        /// the call is ignored.
        /// </summary>
        public override void Unload()
        {
            if (this.realRunner != null)
                this.realRunner.Unload();
        }

        public override TestEngineResult[] RunDirect(ITestEventHandler listener, ITestFilter filter)
        {
            if (this.realRunner == null)
                throw new InvalidOperationException("Load must be called before Run");

            return realRunner.RunDirect(listener, filter);
        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            if (this.realRunner != null)
                this.realRunner.Dispose();
        }

        #endregion
    }
}
