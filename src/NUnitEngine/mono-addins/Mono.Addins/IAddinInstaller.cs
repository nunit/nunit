

using System;

namespace Mono.Addins
{
	/// <summary>
	/// An add-in installation handler
	/// </summary>
	public interface IAddinInstaller
	{
		/// <summary>
		/// Installs a set of add-ins
		/// </summary>
		/// <param name="reg">
		/// Registry where to install
		/// </param>
		/// <param name="message">
		/// Message to show to the user when new add-ins have to be installed.
		/// </param>
		/// <param name="addinIds">
		/// List of IDs of the add-ins to be installed.
		/// </param>
		void InstallAddins (AddinRegistry reg, string message, string[] addinIds);
	}
}
