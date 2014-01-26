// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

#if !NUNITLITE
using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework.Extensibility;

namespace NUnit.Framework.Internal.Extensibility
{
	/// <summary>
	/// Summary description for AddinRegistry.
	/// </summary>
	public class AddinRegistry : IAddinRegistry
    {
        #region Instance Fields
        private ArrayList addins = new ArrayList();
		#endregion

		#region IAddinRegistry Members

        /// <summary>
        /// Registers an addin
        /// </summary>
        /// <param name="addin">The addin to be registered</param>
		public void Register(Addin addin)
		{
			addins.Add( addin );
		}

        /// <summary>
        /// Gets a list of all addins as Addin objects
        /// </summary>
		public  IList Addins
		{
			get
			{
				return addins;
			}
		}

        /// <summary>
        /// Returns true if an addin of a given name is registered
        /// </summary>
        /// <param name="name">The name of the addin</param>
        /// <returns>
        /// True if an addin of that name is registered, otherwise false
        /// </returns>
        public bool IsAddinRegistered(string name)
        {
            return FindAddinByName(name) != null;
        }

        /// <summary>
        /// Sets the load status of an addin
        /// </summary>
        /// <param name="name">The name of the addin</param>
        /// <param name="status">The status to be set</param>
        /// <param name="message">An optional message explaining the status</param>
		public void SetStatus( string name, AddinStatus status, string message )
		{
            Addin addin = FindAddinByName(name);
            if (addin != null)
            {
                addin.Status = status;
                addin.Message = message;
            }
        }

        private Addin FindAddinByName(string name)
        {
            foreach (Addin addin in addins)
                if (addin.Name == name)
                    return addin;

            return null;
        }
		#endregion
    }
}
#endif
