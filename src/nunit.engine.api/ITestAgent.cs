using System;

namespace NUnit.Engine
{
    /// <summary>
    /// The ITestAgent interface is implemented by remote test agents.
    /// </summary>
    public interface ITestAgent
    {
        /// <summary>
        /// Gets the agency with which this agent is associated.
        /// </summary>
        ITestAgency Agency { get; }

        /// <summary>
        /// Gets a Guid that uniquely identifies this agent.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Starts the agent, performing any required initialization
        /// </summary>
        /// <returns>True if successful, otherwise false</returns>
        bool Start();

        /// <summary>
        /// Stops the agent, releasing any resources
        /// </summary>
        void Stop();

        /// <summary>
        ///  Creates a test runner
        /// </summary>
        ITestRunner CreateRunner();
    }
}
