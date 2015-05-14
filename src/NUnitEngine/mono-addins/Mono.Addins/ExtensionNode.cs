//
// ExtensionNode.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using Mono.Addins.Description;

namespace Mono.Addins
{
	/// <summary>
	/// A node of the extension model.
	/// </summary>
	/// <remarks>
	/// An extension node is an element registered by an add-in in an extension point.
	/// A host can get nodes registered in an extension point using methods such as
	/// AddinManager.GetExtensionNodes(string), which returns a collection of ExtensionNode objects.
	/// 
	/// ExtensionNode will normally be used as a base class of more complex extension point types.
	/// The most common subclass is Mono.Addins.TypeExtensionNode, which allows registering a class
	/// implemented in an add-in.
	/// </remarks>
	public class ExtensionNode
	{
		bool childrenLoaded;
		TreeNode treeNode;
		ExtensionNodeList childNodes;
		RuntimeAddin addin;
		string addinId;
		ExtensionNodeType nodeType;
		ModuleDescription module;
		AddinEngine addinEngine;
		event ExtensionNodeEventHandler extensionNodeChanged;
		
		/// <summary>
		/// Identifier of the node.
		/// </summary>
		/// <remarks>
		/// It is not mandatory to specify an 'id' for a node. When none is provided,
		/// the add-in manager will automatically generate an unique id for the node.
		/// The ExtensionNode.HasId property can be used to know if the 'id' has been
		/// specified by the developer or not.
		/// </remarks>
		public string Id {
			get { return treeNode != null ? treeNode.Id : string.Empty; }
		}
		
		/// <summary>
		/// Location of this node in the extension tree.
		/// </summary>
		/// <remarks>
		/// The node path is composed by the path of the extension point where it is defined,
		/// the identifiers of its parent nodes, and its own identifier.
		/// </remarks>
		public string Path {
			get { return treeNode != null ? treeNode.GetPath () : string.Empty; }
		}
		
		/// <summary>
		/// Parent node of this node.
		/// </summary>
		public ExtensionNode Parent {
			get {
				if (treeNode != null && treeNode.Parent != null)
					return treeNode.Parent.ExtensionNode;
				else
					return null;
			}
		}
		
		/// <summary>
		/// Extension context to which this node belongs
		/// </summary>
		public ExtensionContext ExtensionContext {
			get { return treeNode.Context; }
		}
		
		/// <summary>
		/// Specifies whether the extension node has as an Id or not.
		/// </summary>
		/// <remarks>
		/// It is not mandatory to specify an 'id' for a node. When none is provided,
		/// the add-in manager will automatically generate an unique id for the node.
		/// This property will return true if an 'id' was provided for the node, and
		/// false if the id was assigned by the add-in manager.
		/// </remarks>
		public bool HasId {
			get { return !Id.StartsWith (ExtensionTree.AutoIdPrefix); }
		}
		
		internal void SetTreeNode (TreeNode node)
		{
			treeNode = node;
		}
		
		internal void SetData (AddinEngine addinEngine, string plugid, ExtensionNodeType nodeType, ModuleDescription module)
		{
			this.addinEngine = addinEngine;
			this.addinId = plugid;
			this.nodeType = nodeType;
			this.module = module;
		}
		
		internal string AddinId {
			get { return addinId; }
		}
		
		internal TreeNode TreeNode {
			get { return treeNode; }
		}
		
		/// <summary>
		/// The add-in that registered this extension node.
		/// </summary>
		/// <remarks>
		/// This property provides access to the resources and types of the add-in that created this extension node.
		/// </remarks>
		public RuntimeAddin Addin {
			get {
				if (addin == null && addinId != null) {
					if (!addinEngine.IsAddinLoaded (addinId))
						addinEngine.LoadAddin (null, addinId, true);
					addin = addinEngine.GetAddin (addinId);
					if (addin != null)
						addin = addin.GetModule (module);
				}
				if (addin == null)
					throw new InvalidOperationException ("Add-in '" + addinId + "' could not be loaded.");
				return addin; 
			}
		}
		
		/// <summary>
		/// Notifies that a child node of this node has been added or removed.
		/// </summary>
		/// <remarks>
		/// The first time the event is subscribed, the handler will be called for each existing node.
		/// </remarks>
		public event ExtensionNodeEventHandler ExtensionNodeChanged {
			add {
				extensionNodeChanged += value;
				foreach (ExtensionNode node in ChildNodes) {
					try {
						value (this, new ExtensionNodeEventArgs (ExtensionChange.Add, node));
					} catch (Exception ex) {
						addinEngine.ReportError (null, node.Addin != null ? node.Addin.Id : null, ex, false);
					}
				}
			}
			remove {
				extensionNodeChanged -= value;
			}
		}
		
		/// <summary>
		/// Child nodes of this extension node.
		/// </summary>
		public ExtensionNodeList ChildNodes {
			get {
				if (childrenLoaded)
					return childNodes;
				
				try {
					if (treeNode.Children.Count == 0) {
						childNodes = ExtensionNodeList.Empty;
						return childNodes;
					}
				}
				catch (Exception ex) {
					addinEngine.ReportError (null, null, ex, false);
					childNodes = ExtensionNodeList.Empty;
					return childNodes;
				} finally {
					childrenLoaded = true;
				}

				List<ExtensionNode> list = new List<ExtensionNode> ();
				foreach (TreeNode cn in treeNode.Children) {
					
					// For each node check if it is visible for the current context.
					// If something fails while evaluating the condition, just ignore the node.
					
					try {
						if (cn.ExtensionNode != null && cn.IsEnabled)
							list.Add (cn.ExtensionNode);
					} catch (Exception ex) {
						addinEngine.ReportError (null, null, ex, false);
					}
				}
				if (list.Count > 0)
					childNodes = new ExtensionNodeList (list);
				else
					childNodes = ExtensionNodeList.Empty;
			
				return childNodes;
			}
		}
		
		/// <summary>
		/// Returns the child objects of a node.
		/// </summary>
		/// <returns>
		/// An array of child objects.
		/// </returns>
		/// <remarks>
		/// This method only works if all children of this node are of type Mono.Addins.TypeExtensionNode.
		/// The returned array is composed by all objects created by calling the
		/// TypeExtensionNode.GetInstance() method for each node.
		/// </remarks>
		public object[] GetChildObjects ()
		{
			return GetChildObjects (typeof(object), true);
		}
		
		/// <summary>
		/// Returns the child objects of a node.
		/// </summary>
		/// <param name="reuseCachedInstance">
		/// True if the method can reuse instances created in previous calls.
		/// </param>
		/// <returns>
		/// An array of child objects.
		/// </returns>
		/// <remarks>
		/// This method only works if all children of this node are of type Mono.Addins.TypeExtensionNode.
		/// The returned array is composed by all objects created by calling the TypeExtensionNode.CreateInstance()
		/// method for each node (or TypeExtensionNode.GetInstance() if reuseCachedInstance is set to true).
		/// </remarks>
		public object[] GetChildObjects (bool reuseCachedInstance)
		{
			return GetChildObjects (typeof(object), reuseCachedInstance);
		}
		
		/// <summary>
		/// Returns the child objects of a node (with type check).
		/// </summary>
		/// <param name="arrayElementType">
		/// Type of the return array elements.
		/// </param>
		/// <returns>
		/// An array of child objects.
		/// </returns>
		/// <remarks>
		/// This method only works if all children of this node are of type Mono.Addins.TypeExtensionNode.
		/// The returned array is composed by all objects created by calling the
		/// TypeExtensionNode.GetInstance(Type) method for each node.
		/// 
		/// An InvalidOperationException exception is thrown if one of the found child objects is not a
		/// subclass of the provided type.
		/// </remarks>
		public object[] GetChildObjects (Type arrayElementType)
		{
			return GetChildObjects (arrayElementType, true);
		}
		
		/// <summary>
		/// Returns the child objects of a node (casting to the specified type)
		/// </summary>
		/// <returns>
		/// An array of child objects.
		/// </returns>
		/// <remarks>
		/// This method only works if all children of this node are of type Mono.Addins.TypeExtensionNode.
		/// The returned array is composed by all objects created by calling the
		/// TypeExtensionNode.GetInstance() method for each node.
		/// </remarks>
		public T[] GetChildObjects<T> ()
		{
			return (T[]) GetChildObjectsInternal (typeof(T), true);
		}
		/// <summary>
		/// Returns the child objects of a node (with type check).
		/// </summary>
		/// <param name="arrayElementType">
		/// Type of the return array elements.
		/// </param>
		/// <param name="reuseCachedInstance">
		/// True if the method can reuse instances created in previous calls.
		/// </param>
		/// <returns>
		/// An array of child objects.
		/// </returns>
		/// <remarks>
		/// This method only works if all children of this node are of type Mono.Addins.TypeExtensionNode.
		/// The returned array is composed by all objects created by calling the TypeExtensionNode.CreateInstance(Type)
		/// method for each node (or TypeExtensionNode.GetInstance(Type) if reuseCachedInstance is set to true).
		/// 
		/// An InvalidOperationException exception will be thrown if one of the found child objects is not a subclass
		/// of the provided type.
		/// </remarks>
		public object[] GetChildObjects (Type arrayElementType, bool reuseCachedInstance)
		{
			return (object[]) GetChildObjectsInternal (arrayElementType, reuseCachedInstance);
		}
		
		/// <summary>
		/// Returns the child objects of a node (casting to the specified type).
		/// </summary>
		/// <param name="reuseCachedInstance">
		/// True if the method can reuse instances created in previous calls.
		/// </param>
		/// <returns>
		/// An array of child objects.
		/// </returns>
		/// <remarks>
		/// This method only works if all children of this node are of type Mono.Addins.TypeExtensionNode.
		/// The returned array is composed by all objects created by calling the TypeExtensionNode.CreateInstance()
		/// method for each node (or TypeExtensionNode.GetInstance() if reuseCachedInstance is set to true).
		/// </remarks>
		public T[] GetChildObjects<T> (bool reuseCachedInstance)
		{
			return (T[]) GetChildObjectsInternal (typeof(T), reuseCachedInstance);
		}
		
		Array GetChildObjectsInternal (Type arrayElementType, bool reuseCachedInstance)
		{
			ArrayList list = new ArrayList (ChildNodes.Count);
			
			for (int n=0; n<ChildNodes.Count; n++) {
				InstanceExtensionNode node = ChildNodes [n] as InstanceExtensionNode;
				if (node == null) {
					addinEngine.ReportError ("Error while getting object for node in path '" + Path + "'. Extension node is not a subclass of InstanceExtensionNode.", null, null, false);
					continue;
				}
				
				try {
					if (reuseCachedInstance)
						list.Add (node.GetInstance (arrayElementType));
					else
						list.Add (node.CreateInstance (arrayElementType));
				}
				catch (Exception ex) {
					addinEngine.ReportError ("Error while getting object for node in path '" + Path + "'.", node.AddinId, ex, false);
				}
			}
			return list.ToArray (arrayElementType);
		}
		
		/// <summary>
		/// Reads the extension node data
		/// </summary>
		/// <param name='elem'>
		/// The element containing the extension data
		/// </param>
		/// <remarks>
		/// This method can be overridden to provide a custom method for reading extension node data from an element.
		/// The default implementation reads the attributes if the element and assigns the values to the fields
		/// and properties of the extension node that have the corresponding [NodeAttribute] decoration.
		/// </remarks>
		internal protected virtual void Read (NodeElement elem)
		{
			if (nodeType == null)
				return;

			NodeAttribute[] attributes = elem.Attributes;
			ReadObject (this, attributes, nodeType.Fields);
			
			if (nodeType.CustomAttributeMember != null) {
				var att = (CustomExtensionAttribute) Activator.CreateInstance (nodeType.CustomAttributeMember.MemberType, true);
				att.ExtensionNode = this;
				ReadObject (att, attributes, nodeType.CustomAttributeFields);
				nodeType.CustomAttributeMember.SetValue (this, att);
			}
		}
		
		void ReadObject (object ob, NodeAttribute[] attributes, Dictionary<string,ExtensionNodeType.FieldData> fields)
		{
			if (fields == null)
				return;
			
			// Make a copy because we are going to remove fields that have been used
			fields = new Dictionary<string,ExtensionNodeType.FieldData> (fields);
			
			foreach (NodeAttribute at in attributes) {
				
				ExtensionNodeType.FieldData f;
				if (!fields.TryGetValue (at.name, out f))
					continue;
				
				fields.Remove (at.name);
					
				object val;
				Type memberType = f.MemberType;

				if (memberType == typeof(string)) {
					if (f.Localizable)
						val = Addin.Localizer.GetString (at.value);
					else
						val = at.value;
				}
				else if (memberType == typeof(string[])) {
					string[] ss = at.value.Split (',');
					if (ss.Length == 0 && ss[0].Length == 0)
						val = new string [0];
					else {
						for (int n=0; n<ss.Length; n++)
							ss [n] = ss[n].Trim ();
						val = ss;
					}
				}
				else if (memberType.IsEnum) {
					val = Enum.Parse (memberType, at.value);
				}
				else {
					try {
						val = Convert.ChangeType (at.Value, memberType);
					} catch (InvalidCastException) {
						throw new InvalidOperationException ("Property type not supported by [NodeAttribute]: " + f.Member.DeclaringType + "." + f.Member.Name);
					}
				}
				
				f.SetValue (ob, val);
			}
			
			if (fields.Count > 0) {
				// Check if one of the remaining fields is mandatory
				foreach (KeyValuePair<string,ExtensionNodeType.FieldData> e in fields) {
					ExtensionNodeType.FieldData f = e.Value;
					if (f.Required)
						throw new InvalidOperationException ("Required attribute '" + e.Key + "' not found.");
				}
			}
		}
		
		internal bool NotifyChildChanged ()
		{
			if (!childrenLoaded)
				return false;

			ExtensionNodeList oldList = childNodes;
			childrenLoaded = false;
			
			bool changed = false;
			
			foreach (ExtensionNode nod in oldList) {
				if (ChildNodes [nod.Id] == null) {
					changed = true;
					OnChildNodeRemoved (nod);
				}
			}
			foreach (ExtensionNode nod in ChildNodes) {
				if (oldList [nod.Id] == null) {
					changed = true;
					OnChildNodeAdded (nod);
				}
			}
			if (changed)
				OnChildrenChanged ();
			return changed;
		}
		
		/// <summary>
		/// Called when the add-in that defined this extension node is actually loaded in memory.
		/// </summary>
		internal protected virtual void OnAddinLoaded ()
		{
		}
		
		/// <summary>
		/// Called when the add-in that defined this extension node is being
		/// unloaded from memory.
		/// </summary>
		internal protected virtual void OnAddinUnloaded ()
		{
		}
		
		/// <summary>
		/// Called when the children list of this node has changed. It may be due to add-ins
		/// being loaded/unloaded, or to conditions being changed.
		/// </summary>
		protected virtual void OnChildrenChanged ()
		{
		}
		
		/// <summary>
		/// Called when a child node is added
		/// </summary>
		/// <param name="node">
		/// Added node.
		/// </param>
		protected virtual void OnChildNodeAdded (ExtensionNode node)
		{
			if (extensionNodeChanged != null)
				extensionNodeChanged (this, new ExtensionNodeEventArgs (ExtensionChange.Add, node));
		}
		
		/// <summary>
		/// Called when a child node is removed
		/// </summary>
		/// <param name="node">
		/// Removed node.
		/// </param>
		protected virtual void OnChildNodeRemoved (ExtensionNode node)
		{
			if (extensionNodeChanged != null)
				extensionNodeChanged (this, new ExtensionNodeEventArgs (ExtensionChange.Remove, node));
		}
	}
	
	/// <summary>
	/// An extension node with custom metadata
	/// </summary>
	/// <remarks>
	/// This is the default type for extension nodes bound to a custom extension attribute.
	/// </remarks>
	public class ExtensionNode<T>: ExtensionNode, IAttributedExtensionNode where T:CustomExtensionAttribute
	{
		T data;
		
		/// <summary>
		/// The custom attribute containing the extension metadata
		/// </summary>
		[NodeAttribute]
		public T Data {
			get { return data; }
			internal set { data = value; }
		}

		CustomExtensionAttribute IAttributedExtensionNode.Attribute {
			get { return data; }
		}
	}

	/// <summary>
	/// An extension node with custom metadata provided by an attribute
	/// </summary>
	/// <remarks>
	/// This interface is implemented by ExtensionNode&lt;T&gt; to provide non-generic access to the attribute instance.
	/// </remarks>
	public interface IAttributedExtensionNode
	{
		/// <summary>
		/// The custom attribute containing the extension metadata
		/// </summary>
		CustomExtensionAttribute Attribute { get; }
	}
}
