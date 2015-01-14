//
// AddinManager.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using Mono.Addins.Localization;

namespace Mono.Addins
{
	/// <summary>
	/// Provides access to add-in and extension model management operations.
	/// </summary>
	public class AddinManager
	{
		static AddinEngine sessionService;

		private AddinManager ()
		{
		}
		
		/// <summary>
		/// Initializes the add-in engine.
		/// </summary>
		/// <remarks>
		/// The add-in engine needs to be initialized before doing any add-in operation.
		/// When initialized with this method, it will look for add-ins in the global add-in registry.
		/// </remarks>
		public static void Initialize ()
		{
			// Code not shared with the other Initialize since I need to get the calling assembly
			Assembly asm = Assembly.GetEntryAssembly ();
			if (asm == null) asm = Assembly.GetCallingAssembly ();
			AddinEngine.Initialize (asm, null, null, null);
		}
		
		/// <summary>
		/// Initializes the add-in engine.
		/// </summary>
		/// <param name="configDir">
		/// Location of the add-in registry.
		/// </param>
		/// <remarks>
		/// The add-in engine needs to be initialized before doing any add-in operation.
		/// Configuration information about the add-in registry will be stored in the
		/// provided location. The add-in engine will look for add-ins in an 'addins'
		/// subdirectory of the provided directory.
		/// 
		/// When specifying a path, it is possible to use a special folder name as root.
		/// For example: [Personal]/.config/MyApp. In this case, [Personal] will be replaced
		/// by the location of the Environment.SpecialFolder.Personal folder. Any value
		/// of the Environment.SpecialFolder enumeration can be used (always between square
		/// brackets)
		/// </remarks>
		public static void Initialize (string configDir)
		{
			Assembly asm = Assembly.GetEntryAssembly ();
			if (asm == null) asm = Assembly.GetCallingAssembly ();
			AddinEngine.Initialize (asm, configDir, null, null);
		}
		
		/// <summary>
		/// Initializes the add-in engine.
		/// </summary>
		/// <param name='configDir'>
		/// Location of the add-in registry.
		/// </param>
		/// <param name='addinsDir'>
		/// Add-ins directory. If the path is relative, it is considered to be relative
		/// to the configDir directory.
		/// </param>
		/// <remarks>
		/// The add-in engine needs to be initialized before doing any add-in operation.
		/// Configuration information about the add-in registry will be stored in the
		/// provided location. The add-in engine will look for add-ins in the provided
		/// 'addinsDir' directory.
		/// 
		/// When specifying a path, it is possible to use a special folder name as root.
		/// For example: [Personal]/.config/MyApp. In this case, [Personal] will be replaced
		/// by the location of the Environment.SpecialFolder.Personal folder. Any value
		/// of the Environment.SpecialFolder enumeration can be used (always between square
		/// brackets)
		/// </remarks>
		public static void Initialize (string configDir, string addinsDir)
		{
			Assembly asm = Assembly.GetEntryAssembly ();
			if (asm == null) asm = Assembly.GetCallingAssembly ();
			AddinEngine.Initialize (asm, configDir, addinsDir, null);
		}
		
		/// <summary>
		/// Initializes the add-in engine.
		/// </summary>
		/// <param name='configDir'>
		/// Location of the add-in registry.
		/// </param>
		/// <param name='addinsDir'>
		/// Add-ins directory. If the path is relative, it is considered to be relative
		/// to the configDir directory.
		/// </param>
		/// <param name='databaseDir'>
		/// Location of the add-in database. If the path is relative, it is considered to be relative
		/// to the configDir directory.
		/// </param>
		/// <remarks>
		/// The add-in engine needs to be initialized before doing any add-in operation.
		/// Configuration information about the add-in registry will be stored in the
		/// provided location. The add-in engine will look for add-ins in the provided
		/// 'addinsDir' directory. Cached information about add-ins will be stored in
		/// the 'databaseDir' directory.
		/// 
		/// When specifying a path, it is possible to use a special folder name as root.
		/// For example: [Personal]/.config/MyApp. In this case, [Personal] will be replaced
		/// by the location of the Environment.SpecialFolder.Personal folder. Any value
		/// of the Environment.SpecialFolder enumeration can be used (always between square
		/// brackets)
		/// </remarks>
		public static void Initialize (string configDir, string addinsDir, string databaseDir)
		{
			Assembly asm = Assembly.GetEntryAssembly ();
			if (asm == null) asm = Assembly.GetCallingAssembly ();
			AddinEngine.Initialize (asm, configDir, addinsDir, databaseDir);
		}
		
		/// <summary>
		/// Finalizes an add-in engine.
		/// </summary>
		public static void Shutdown ()
		{
			AddinEngine.Shutdown ();
		}
		
		/// <summary>
		/// Sets the default localizer to be used for this add-in engine
		/// </summary>
		/// <param name="localizer">
		/// The add-in localizer
		/// </param>
		public static void InitializeDefaultLocalizer (IAddinLocalizer localizer)
		{
			AddinEngine.InitializeDefaultLocalizer (localizer);
		}
		
		internal static string StartupDirectory {
			get { return AddinEngine.StartupDirectory; }
		}
		
		/// <summary>
		/// Gets whether the add-in engine has been initialized.
		/// </summary>
		public static bool IsInitialized {
			get { return AddinEngine.IsInitialized; }
		}
		
		/// <summary>
		/// Gets the default add-in installer
		/// </summary>
		/// <remarks>
		/// The default installer is used by the CheckInstalled method to request
		/// the installation of missing add-ins.
		/// </remarks>
		public static IAddinInstaller DefaultInstaller {
			get { return AddinEngine.DefaultInstaller; }
			set { AddinEngine.DefaultInstaller = value; }
		}
		
		/// <summary>
		/// Gets the default localizer for this add-in engine
		/// </summary>
		public static AddinLocalizer DefaultLocalizer {
			get {
				return AddinEngine.DefaultLocalizer;
			}
		}
		
		/// <summary>
		/// Gets the localizer for the add-in that is invoking this property
		/// </summary>
		public static AddinLocalizer CurrentLocalizer {
			get {
				AddinEngine.CheckInitialized ();
				RuntimeAddin addin = AddinEngine.GetAddinForAssembly (Assembly.GetCallingAssembly ());
				if (addin != null)
					return addin.Localizer;
				else
					return AddinEngine.DefaultLocalizer;
			}
		}
		
		/// <summary>
		/// Gets a reference to the RuntimeAddin object for the add-in that is invoking this property
		/// </summary>
		public static RuntimeAddin CurrentAddin {
			get {
				AddinEngine.CheckInitialized ();
				return AddinEngine.GetAddinForAssembly (Assembly.GetCallingAssembly ());
			}
		}
		
		/// <summary>
		/// Gets the default add-in engine
		/// </summary>
		public static AddinEngine AddinEngine {
			get {
				if (sessionService == null)
					sessionService = new AddinEngine();
				
				return sessionService;
			}
		}
	
		/// <summary>
		/// Gets the add-in registry bound to the default add-in engine
		/// </summary>
		public static AddinRegistry Registry {
			get {
				return AddinEngine.Registry;
			}
		}
		
		/// <summary>
		/// Checks if the provided add-ins are installed, and requests the installation of those
		/// which aren't.
		/// </summary>
		/// <param name="message">
		/// Message to show to the user when new add-ins have to be installed.
		/// </param>
		/// <param name="addinIds">
		/// List of IDs of the add-ins to be checked.
		/// </param>
		/// <remarks>
		/// This method checks if the specified add-ins are installed.
		/// If some of the add-ins are not installed, it will use
		/// the installer assigned to the DefaultAddinInstaller property
		/// to install them. If the installation fails, or if DefaultAddinInstaller
		/// is not set, an exception will be thrown.
		/// </remarks>
		public static void CheckInstalled (string message, params string[] addinIds)
		{
			AddinEngine.CheckInstalled (message, addinIds);
		}
	
		/// <summary>
		/// Checks if an add-in has been loaded.
		/// </summary>
		/// <param name="id">
		/// Full identifier of the add-in.
		/// </param>
		/// <returns>
		/// True if the add-in is loaded.
		/// </returns>
		public static bool IsAddinLoaded (string id)
		{
			return AddinEngine.IsAddinLoaded (id);
		}
		
		/// <summary>
		/// Forces the loading of an add-in.
		/// </summary>
		/// <param name="statusMonitor">
		/// Status monitor to keep track of the loading process.
		/// </param>
		/// <param name="id">
		/// Full identifier of the add-in to load.
		/// </param>
		/// <remarks>
		/// This method loads all assemblies that belong to an add-in in memory.
		/// All add-ins on which the specified add-in depends will also be loaded.
		/// Notice that in general add-ins don't need to be explicitely loaded using
		/// this method, since the add-in engine will load them on demand.
		/// </remarks>
		public static void LoadAddin (IProgressStatus statusMonitor, string id)
		{
			AddinEngine.LoadAddin (statusMonitor, id);
		}
		
		/// <summary>
		/// Creates a new extension context.
		/// </summary>
		/// <returns>
		/// The new extension context.
		/// </returns>
		/// <remarks>
		/// Extension contexts can be used to query the extension model using particular condition values.
		/// </remarks>
		public static ExtensionContext CreateExtensionContext ()
		{
			return AddinEngine.CreateExtensionContext ();
		}
		
		/// <summary>
		/// Returns the extension node in a path
		/// </summary>
		/// <param name="path">
		/// Location of the node.
		/// </param>
		/// <returns>
		/// The node, or null if not found.
		/// </returns>
		public static ExtensionNode GetExtensionNode (string path)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionNode (path);
		}
		
		/// <summary>
		/// Returns the extension node in a path
		/// </summary>
		/// <param name="path">
		/// Location of the node.
		/// </param>
		/// <returns>
		/// The node, or null if not found.
		/// </returns>
		public static T GetExtensionNode<T> (string path) where T:ExtensionNode
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionNode<T> (path);
		}
		
		/// <summary>
		/// Gets extension nodes registered in a path.
		/// </summary>
		/// <param name="path">
		/// An extension path.>
		/// </param>
		/// <returns>
		/// All nodes registered in the provided path.
		/// </returns>
		public static ExtensionNodeList GetExtensionNodes (string path)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionNodes (path);
		}
		
		/// <summary>
		/// Gets extension nodes registered in a path.
		/// </summary>
		/// <param name="path">
		/// An extension path.
		/// </param>
		/// <param name="expectedNodeType">
		/// Expected node type.
		/// </param>
		/// <returns>
		/// A list of nodes
		/// </returns>
		/// <remarks>
		/// This method returns all nodes registered under the provided path.
		/// It will throw a InvalidOperationException if the type of one of
		/// the registered nodes is not assignable to the provided type.
		/// </remarks>
		public static ExtensionNodeList GetExtensionNodes (string path, Type expectedNodeType)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionNodes (path, expectedNodeType);
		}
		
		/// <summary>
		/// Gets extension nodes registered in a path.
		/// </summary>
		/// <param name="path">
		/// An extension path.
		/// </param>
		/// <returns>
		/// A list of nodes
		/// </returns>
		/// <remarks>
		/// This method returns all nodes registered under the provided path.
		/// It will throw a InvalidOperationException if the type of one of
		/// the registered nodes is not assignable to the provided type.
		/// </remarks>
		public static ExtensionNodeList<T> GetExtensionNodes<T> (string path) where T:ExtensionNode
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionNodes<T> (path);
		}

		/// <summary>
		/// Gets extension nodes for a type extension point
		/// </summary>
		/// <param name="instanceType">
		/// Type defining the extension point
		/// </param>
		/// <returns>
		/// A list of nodes
		/// </returns>
		/// <remarks>
		/// This method returns all extension nodes bound to the provided type.
		/// </remarks>
		public static ExtensionNodeList GetExtensionNodes (Type instanceType)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionNodes (instanceType);
		}

		/// <summary>
		/// Gets extension nodes for a type extension point
		/// </summary>
		/// <param name="instanceType">
		/// Type defining the extension point
		/// </param>
		/// <param name="expectedNodeType">
		/// Expected extension node type
		/// </param>
		/// <returns>
		/// A list of nodes
		/// </returns>
		/// <remarks>
		/// This method returns all nodes registered for the provided type.
		/// It will throw a InvalidOperationException if the type of one of
		/// the registered nodes is not assignable to the provided node type.
		/// </remarks>
		public static ExtensionNodeList GetExtensionNodes (Type instanceType, Type expectedNodeType)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionNodes (instanceType, expectedNodeType);
		}
		
		/// <summary>
		/// Gets extension nodes for a type extension point
		/// </summary>
		/// <param name="instanceType">
		/// Type defining the extension point
		/// </param>
		/// <returns>
		/// A list of nodes
		/// </returns>
		/// <remarks>
		/// This method returns all nodes registered for the provided type.
		/// It will throw a InvalidOperationException if the type of one of
		/// the registered nodes is not assignable to the specified node type argument.
		/// </remarks>
		public static ExtensionNodeList<T> GetExtensionNodes<T> (Type instanceType) where T: ExtensionNode
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionNodes<T> (instanceType);
		}
		
		/// <summary>
		/// Gets extension objects registered for a type extension point.
		/// </summary>
		/// <param name="instanceType">
		/// Type defining the extension point
		/// </param>
		/// <returns>
		/// A list of objects
		/// </returns>
		public static object[] GetExtensionObjects (Type instanceType)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionObjects (instanceType);
		}
		
		/// <summary>
		/// Gets extension objects registered for a type extension point.
		/// </summary>
		/// <returns>
		/// A list of objects
		/// </returns>
		/// <remarks>
		/// The type argument of this generic method is the type that defines
		/// the extension point.
		/// </remarks>
		public static T[] GetExtensionObjects<T> ()
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionObjects<T> ();
		}

		/// <summary>
		/// Gets extension objects registered for a type extension point.
		/// </summary>
		/// <param name="instanceType">
		/// Type defining the extension point
		/// </param>
		/// <param name="reuseCachedInstance">
		/// When set to True, it will return instances created in previous calls.
		/// </param>
		/// <returns>
		/// A list of extension objects.
		/// </returns>
		public static object[] GetExtensionObjects (Type instanceType, bool reuseCachedInstance)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionObjects (instanceType, reuseCachedInstance);
		}
		
		/// <summary>
		/// Gets extension objects registered for a type extension point.
		/// </summary>
		/// <param name="reuseCachedInstance">
		/// When set to True, it will return instances created in previous calls.
		/// </param>
		/// <returns>
		/// A list of extension objects.
		/// </returns>
		/// <remarks>
		/// The type argument of this generic method is the type that defines
		/// the extension point.
		/// </remarks>
		public static T[] GetExtensionObjects<T> (bool reuseCachedInstance)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionObjects<T> (reuseCachedInstance);
		}
		
		/// <summary>
		/// Gets extension objects registered in a path
		/// </summary>
		/// <param name="path">
		/// An extension path.
		/// </param>
		/// <returns>
		/// An array of objects registered in the path.
		/// </returns>
		/// <remarks>
		/// This method can only be used if all nodes in the provided extension path
		/// are of type Mono.Addins.TypeExtensionNode. The returned array is composed
		/// by all objects created by calling the TypeExtensionNode.CreateInstance()
		/// method for each node.
		/// </remarks>
		public static object[] GetExtensionObjects (string path)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionObjects (path);
		}
		
		/// <summary>
		/// Gets extension objects registered in a path.
		/// </summary>
		/// <param name="path">
		/// An extension path.
		/// </param>
		/// <param name="reuseCachedInstance">
		/// When set to True, it will return instances created in previous calls.
		/// </param>
		/// <returns>
		/// An array of objects registered in the path.
		/// </returns>
		/// <remarks>
		/// This method can only be used if all nodes in the provided extension path
		/// are of type Mono.Addins.TypeExtensionNode. The returned array is composed
		/// by all objects created by calling the TypeExtensionNode.CreateInstance()
		/// method for each node (or TypeExtensionNode.GetInstance() if
		/// reuseCachedInstance is set to true)
		/// </remarks>
		public static object[] GetExtensionObjects (string path, bool reuseCachedInstance)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionObjects (path, reuseCachedInstance);
		}
		
		/// <summary>
		/// Gets extension objects registered in a path.
		/// </summary>
		/// <param name="path">
		/// An extension path.
		/// </param>
		/// <param name="arrayElementType">
		/// Type of the return array elements.
		/// </param>
		/// <returns>
		/// An array of objects registered in the path.
		/// </returns>
		/// <remarks>
		/// This method can only be used if all nodes in the provided extension path
		/// are of type Mono.Addins.TypeExtensionNode. The returned array is composed
		/// by all objects created by calling the TypeExtensionNode.CreateInstance()
		/// method for each node.
		/// 
		/// An InvalidOperationException exception is thrown if one of the found
		/// objects is not a subclass of the provided type.
		/// </remarks>
		public static object[] GetExtensionObjects (string path, Type arrayElementType)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionObjects (path, arrayElementType);
		}
		
		/// <summary>
		/// Gets extension objects registered in a path.
		/// </summary>
		/// <param name="path">
		/// An extension path.
		/// </param>
		/// <returns>
		/// An array of objects registered in the path.
		/// </returns>
		/// <remarks>
		/// This method can only be used if all nodes in the provided extension path
		/// are of type Mono.Addins.TypeExtensionNode. The returned array is composed
		/// by all objects created by calling the TypeExtensionNode.CreateInstance()
		/// method for each node.
		/// 
		/// An InvalidOperationException exception is thrown if one of the found
		/// objects is not a subclass of the provided type.
		/// </remarks>
		public static T[] GetExtensionObjects<T> (string path)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionObjects<T> (path);
		}
		
		/// <summary>
		/// Gets extension objects registered in a path.
		/// </summary>
		/// <param name="path">
		/// An extension path.
		/// </param>
		/// <param name="arrayElementType">
		/// Type of the return array elements.
		/// </param>
		/// <param name="reuseCachedInstance">
		/// When set to True, it will return instances created in previous calls.
		/// </param>
		/// <returns>
		/// An array of objects registered in the path.
		/// </returns>
		/// <remarks>
		/// This method can only be used if all nodes in the provided extension path
		/// are of type Mono.Addins.TypeExtensionNode. The returned array is composed
		/// by all objects created by calling the TypeExtensionNode.CreateInstance()
		/// method for each node (or TypeExtensionNode.GetInstance() if
		/// reuseCachedInstance is set to true).
		/// 
		/// An InvalidOperationException exception is thrown if one of the found
		/// objects is not a subclass of the provided type.
		/// </remarks>
		public static object[] GetExtensionObjects (string path, Type arrayElementType, bool reuseCachedInstance)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionObjects (path, arrayElementType, reuseCachedInstance);
		}
		
		/// <summary>
		/// Gets extension objects registered in a path.
		/// </summary>
		/// <param name="path">
		/// An extension path.
		/// </param>
		/// <param name="reuseCachedInstance">
		/// When set to True, it will return instances created in previous calls.
		/// </param>
		/// <returns>
		/// An array of objects registered in the path.
		/// </returns>
		/// <remarks>
		/// This method can only be used if all nodes in the provided extension path
		/// are of type Mono.Addins.TypeExtensionNode. The returned array is composed
		/// by all objects created by calling the TypeExtensionNode.CreateInstance()
		/// method for each node (or TypeExtensionNode.GetInstance() if
		/// reuseCachedInstance is set to true).
		/// 
		/// An InvalidOperationException exception is thrown if one of the found
		/// objects is not a subclass of the provided type.
		/// </remarks>
		public static T[] GetExtensionObjects<T> (string path, bool reuseCachedInstance)
		{
			AddinEngine.CheckInitialized ();
			return AddinEngine.GetExtensionObjects<T> (path, reuseCachedInstance);
		}
		
		/// <summary>
		/// Extension change event.
		/// </summary>
		/// <remarks>
		/// This event is fired when any extension point in the add-in system changes.
		/// The event args object provides the path of the changed extension, although
		/// it does not provide information about what changed. Hosts subscribing to
		/// this event should get the new list of nodes using a query method such as
		/// AddinManager.GetExtensionNodes() and then update whatever needs to be updated.
		/// </remarks>
		public static event ExtensionEventHandler ExtensionChanged {
			add { AddinEngine.CheckInitialized(); AddinEngine.ExtensionChanged += value; }
			remove { AddinEngine.CheckInitialized(); AddinEngine.ExtensionChanged -= value; }
		}

		/// <summary>
		/// Register a listener of extension node changes.
		/// </summary>
		/// <param name="path">
		/// Path of the node.
		/// </param>
		/// <param name="handler">
		/// A handler method.
		/// </param>
		/// <remarks>
		/// Hosts can call this method to be subscribed to an extension change
		/// event for a specific path. The event will be fired once for every
		/// individual node change. The event arguments include the change type
		/// (Add or Remove) and the extension node added or removed.
		/// 
		/// NOTE: The handler will be called for all nodes existing in the path at the moment of registration.
		/// </remarks>
		public static void AddExtensionNodeHandler (string path, ExtensionNodeEventHandler handler)
		{
			AddinEngine.CheckInitialized ();
			AddinEngine.AddExtensionNodeHandler (path, handler);
		}
		
		/// <summary>
		/// Unregister a listener of extension node changes.
		/// </summary>
		/// <param name="path">
		/// Path of the node.
		/// </param>
		/// <param name="handler">
		/// A handler method.
		/// </param>
		/// <remarks>
		/// This method unregisters a delegate from the node change event of a path.
		/// </remarks>
		public static void RemoveExtensionNodeHandler (string path, ExtensionNodeEventHandler handler)
		{
			AddinEngine.CheckInitialized ();
			AddinEngine.RemoveExtensionNodeHandler (path, handler);
		}

		/// <summary>
		/// Register a listener of extension node changes.
		/// </summary>
		/// <param name="instanceType">
		/// Type defining the extension point
		/// </param>
		/// <param name="handler">
		/// A handler method.
		/// </param>
		/// <remarks>
		/// Hosts can call this method to be subscribed to an extension change
		/// event for a specific type extension point. The event will be fired once for every
		/// individual node change. The event arguments include the change type
		/// (Add or Remove) and the extension node added or removed.
		/// 
		/// NOTE: The handler will be called for all nodes existing in the path at the moment of registration.
		/// </remarks>
		public static void AddExtensionNodeHandler (Type instanceType, ExtensionNodeEventHandler handler)
		{
			AddinEngine.CheckInitialized ();
			AddinEngine.AddExtensionNodeHandler (instanceType, handler);
		}
		
		/// <summary>
		/// Unregister a listener of extension node changes.
		/// </summary>
		/// <param name="instanceType">
		/// Type defining the extension point
		/// </param>
		/// <param name="handler">
		/// A handler method.
		/// </param>
		public static void RemoveExtensionNodeHandler (Type instanceType, ExtensionNodeEventHandler handler)
		{
			AddinEngine.CheckInitialized ();
			AddinEngine.RemoveExtensionNodeHandler (instanceType, handler);
		}
		
		/// <summary>
		/// Add-in loading error event.
		/// </summary>
		/// <remarks>
		/// This event is fired when there is an error when loading the extension
		/// of an add-in, or any other kind of error that may happen when querying extension points.
		/// </remarks>
		public static event AddinErrorEventHandler AddinLoadError {
			add { AddinEngine.AddinLoadError += value; }
			remove { AddinEngine.AddinLoadError -= value; }
		}
		
		/// <summary>
		/// Add-in loaded event.
		/// </summary>
		/// <remarks>
		/// Fired after loading an add-in in memory.
		/// </remarks>
		public static event AddinEventHandler AddinLoaded {
			add { AddinEngine.AddinLoaded += value; }
			remove { AddinEngine.AddinLoaded -= value; }
		}
		
		/// <summary>
		/// Add-in unload event.
		/// </summary>
		/// <remarks>
		/// Fired when an add-in is unloaded from memory. It may happen an add-in is disabled or uninstalled.
		/// </remarks>
		public static event AddinEventHandler AddinUnloaded {
			add { AddinEngine.AddinUnloaded += value; }
			remove { AddinEngine.AddinUnloaded -= value; }
		}
		
		internal static bool CheckAssembliesLoaded (HashSet<string> files)
		{
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies ()) {
				if (asm is System.Reflection.Emit.AssemblyBuilder)
					continue;
				try {
					Uri u;
					if (!Uri.TryCreate (asm.CodeBase, UriKind.Absolute, out u))
						continue;
					string asmFile = u.LocalPath;
					if (files.Contains (Path.GetFullPath (asmFile)))
						return true;
				} catch {
					// Ignore
				}
			}
			return false;
		}
	}

}
