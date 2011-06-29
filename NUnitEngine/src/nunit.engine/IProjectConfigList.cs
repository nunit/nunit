
namespace NUnit.Engine
{
    public interface IProjectConfigList
    {
        /// <summary>
        /// Gets a count of the number of configs
        /// </summary>
        int Count { get; }

        ///// <summary>
        ///// Gets the config at the specified index.
        ///// </summary>
        //IProjectConfig this[int index] { get; }

        /// <summary>
        /// Gets the config with the specified name
        /// </summary>
        IProjectConfig this[string name] { get; }
    }
}
