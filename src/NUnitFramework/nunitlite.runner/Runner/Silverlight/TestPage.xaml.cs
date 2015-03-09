#if SILVERLIGHT
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using NUnit.Common;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnitLite.Runner.Silverlight
{
    /// <summary>
    /// TestPage is the display page for the test results
    /// </summary>
    public partial class TestPage : UserControl
    {
        private Assembly _callingAssembly;
        private TextUI _textUI;
        private TextRunner _textRunner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestPage"/> class.
        /// </summary>
        public TestPage()
        {
            InitializeComponent();

            _callingAssembly = Assembly.GetCallingAssembly();
            _textUI = new TextUI(new TextBlockWriter(this.ScratchArea));
            _textRunner = new TextRunner(_textUI);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _textUI.DisplayHeader();

            _textUI.DisplayTestFiles(new string[] { AssemblyHelper.GetAssemblyName(_callingAssembly).Name });

            _textUI.DisplayRuntimeEnvironment();

            Dispatcher.BeginInvoke(() => ExecuteTests());
        }

        #region Helper Methods

        private void ExecuteTests()
        {
            _textRunner.Execute(_callingAssembly);

            ResultSummary summary = _textRunner.Summary;

            this.Total.Text = summary.TestCount.ToString();
            this.Failures.Text = summary.FailureCount.ToString();
            this.Errors.Text = summary.ErrorCount.ToString();
            var notRunTotal = summary.SkipCount + summary.InvalidCount + summary.IgnoreCount;
            this.NotRun.Text = notRunTotal.ToString();
            this.Passed.Text = summary.PassCount.ToString();
            this.Inconclusive.Text = summary.InconclusiveCount.ToString();

            this.Notice.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}
#endif
