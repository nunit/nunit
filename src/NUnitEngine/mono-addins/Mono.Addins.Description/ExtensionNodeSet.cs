//
// ExtensionNodeSet.cs
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
using System.Xml;
using Mono.Addins.Serialization;
using System.Collections.Specialized;

namespace Mono.Addins.Description
{
	/// <summary>
	/// An extension node set definition.
	/// </summary>
	/// <remarks>
	/// Node sets allow grouping a set of extension node declarations and give an identifier to that group
	/// (the node set). Once a node set is declared, it can be referenced from several extension points
	/// which use the same extension node structure. Extension node sets also allow declaring recursive
	/// extension nodes, that is, extension nodes with a tree structure.
	/// </remarks>
	public class ExtensionNodeSet: ObjectDescription
	{
		string id;
		ExtensionNodeTypeCollection nodeTypes;
		NodeSetIdCollection nodeSets;
		bool missingNodeSetId;
		ExtensionNodeTypeCollection cachedAllowedTypes;
		
		internal string SourceAddinId { get; set; }
		
		internal ExtensionNodeSet (XmlElement element)
		{
			Element = element;
			id = element.GetAttribute (IdAttribute);
		}
		
		/// <summary>
		/// Copies data from another node set
		/// </summary>
		/// <param name='nset'>
		/// Node set from which to copy
		/// </param>
		public void CopyFrom (ExtensionNodeSet nset)
		{
			id = nset.id;
			NodeTypes.Clear ();
			foreach (ExtensionNodeType nt in nset.NodeTypes) {
				ExtensionNodeType cnt = new ExtensionNodeType ();
				cnt.CopyFrom (nt);
				NodeTypes.Add (cnt);
			}
			NodeSets.Clear ();
			foreach (string ns in nset.NodeSets)
				NodeSets.Add (ns);
			missingNodeSetId = nset.missingNodeSetId;
		}
		
		internal override void Verify (string location, StringCollection errors)
		{
			if (missingNodeSetId)
				errors.Add (location + "Missing id attribute in extension set reference");
			
			NodeTypes.Verify (location + "ExtensionNodeSet (" + Id + ")/", errors);
		}
		
		internal override void SaveXml (XmlElement parent)
		{
			SaveXml (parent, "ExtensionNodeSet");
		}
		
		internal virtual void SaveXml (XmlElement parent, string nodeName)
		{
			if (Element == null) {
				Element = parent.OwnerDocument.CreateElement (nodeName);
				parent.AppendChild (Element);
			}
			if (Id.Length > 0)
				Element.SetAttribute (IdAttribute, Id);
			if (nodeTypes != null)
				nodeTypes.SaveXml (Element);
			if (nodeSets != null) {
				foreach (string s in nodeSets) {
					if (Element.SelectSingleNode ("ExtensionNodeSet[@id='" + s + "']") == null) {
						XmlElement e = Element.OwnerDocument.CreateElement ("ExtensionNodeSet");
						e.SetAttribute ("id", s);
						Element.AppendChild (e);
					}
				}
				ArrayList list = new ArrayList ();
				foreach (XmlElement e in Element.SelectNodes ("ExtensionNodeSet")) {
					if (!nodeSets.Contains (e.GetAttribute ("id")))
						list.Add (e);
				}
				foreach (XmlElement e in list)
					Element.RemoveChild (e);
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.Description.ExtensionNodeSet"/> class.
		/// </summary>
		public ExtensionNodeSet ()
		{
		}
		
		/// <summary>
		/// Gets or sets the identifier of the node set.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		public string Id {
			get { return id != null ? id : string.Empty; }
			set { id = value; }
		}
			
		internal virtual string IdAttribute {
			get { return "id"; }
		}
		
		/// <summary>
		/// Gets the node types allowed in this node set.
		/// </summary>
		/// <value>
		/// The node types.
		/// </value>
		public ExtensionNodeTypeCollection NodeTypes {
			get {
				if (nodeTypes == null) {
					if (Element != null)
						InitCollections ();
					else
						nodeTypes = new ExtensionNodeTypeCollection (this);
				}
				return nodeTypes;
			}
		}
		
		/// <summary>
		/// Gets a list of other node sets included in this node set.
		/// </summary>
		/// <value>
		/// The node sets.
		/// </value>
		public NodeSetIdCollection NodeSets {
			get {
				if (nodeSets == null) {
					if (Element != null)
						InitCollections ();
					else
						nodeSets = new NodeSetIdCollection ();
				}
				return nodeSets;
			}
		}
		
		/// <summary>
		/// Gets all the allowed node types.
		/// </summary>
		/// <returns>
		/// The allowed node types.
		/// </returns>
		/// <remarks>
		/// Gets all allowed node types, including those defined in included node sets.
		/// This method only works for descriptions loaded from a registry.
		/// </remarks>
		public ExtensionNodeTypeCollection GetAllowedNodeTypes ()
		{
			if (cachedAllowedTypes == null) {
				cachedAllowedTypes = new ExtensionNodeTypeCollection ();
				GetAllowedNodeTypes (new Hashtable (), cachedAllowedTypes);
			}
		    return cachedAllowedTypes;
		}
		
		void GetAllowedNodeTypes (Hashtable visitedSets, ExtensionNodeTypeCollection col)
		{
			if (Id.Length > 0) {
				if (visitedSets.Contains (Id))
					return;
				visitedSets [Id] = Id;
			}
			
			// Gets all allowed node types, including those defined in node sets
			// It only works for descriptions generated from a registry
			
			foreach (ExtensionNodeType nt in NodeTypes)
				col.Add (nt);
			
			AddinDescription desc = ParentAddinDescription;
			if (desc == null || desc.OwnerDatabase == null)
			    return;
			
			foreach (string[] ns in NodeSets.InternalList) {
				string startAddin = ns [1];
				if (startAddin == null || startAddin.Length == 0)
					startAddin = desc.AddinId;
				ExtensionNodeSet nset = desc.OwnerDatabase.FindNodeSet (ParentAddinDescription.Domain, startAddin, ns[0]);
				if (nset != null)
					nset.GetAllowedNodeTypes (visitedSets, col);
			}
		}
		
		internal void Clear ()
		{
			Element = null;
			nodeSets = null;
			nodeTypes = null;
		}
		
		internal void SetExtensionsAddinId (string addinId)
		{
			foreach (ExtensionNodeType nt in NodeTypes) {
				nt.AddinId = addinId;
				nt.SetExtensionsAddinId (addinId);
			}
			NodeSets.SetExtensionsAddinId (addinId);
		}
		
		internal void MergeWith (string thisAddinId, ExtensionNodeSet other)
		{
			foreach (ExtensionNodeType nt in other.NodeTypes) {
				if (nt.AddinId != thisAddinId && !NodeTypes.Contains (nt))
					NodeTypes.Add (nt);
			}
			NodeSets.MergeWith (thisAddinId, other.NodeSets);
		}
		
		internal void UnmergeExternalData (string thisAddinId, Hashtable addinsToUnmerge)
		{
			// Removes extension types and extension sets coming from other add-ins.
			
			ArrayList todelete = new ArrayList ();
			foreach (ExtensionNodeType nt in NodeTypes) {
				if (nt.AddinId != thisAddinId && (addinsToUnmerge == null || addinsToUnmerge.Contains (nt.AddinId)))
					todelete.Add (nt);
			}
			foreach (ExtensionNodeType nt in todelete)
				NodeTypes.Remove (nt);
			
			NodeSets.UnmergeExternalData (thisAddinId, addinsToUnmerge);
		}
		
		void InitCollections ()
		{
			nodeTypes = new ExtensionNodeTypeCollection (this);
			nodeSets = new NodeSetIdCollection ();
			
			foreach (XmlNode n in Element.ChildNodes) {
				XmlElement nt = n as XmlElement;
				if (nt == null)
					continue;
				if (nt.LocalName == "ExtensionNode") {
					ExtensionNodeType etype = new ExtensionNodeType (nt);
					nodeTypes.Add (etype);
				}
				else if (nt.LocalName == "ExtensionNodeSet") {
					string id = nt.GetAttribute ("id");
					if (id.Length > 0)
						nodeSets.Add (id);
					else
						missingNodeSetId = true;
				}
			}
		}
		
		internal override void Write (BinaryXmlWriter writer)
		{
			writer.WriteValue ("Id", id);
			writer.WriteValue ("NodeTypes", NodeTypes);
			writer.WriteValue ("NodeSets", NodeSets.InternalList);
		}
		
		internal override void Read (BinaryXmlReader reader)
		{
			id = reader.ReadStringValue ("Id");
			nodeTypes = (ExtensionNodeTypeCollection) reader.ReadValue ("NodeTypes", new ExtensionNodeTypeCollection (this));
			reader.ReadValue ("NodeSets", NodeSets.InternalList);
		}
	}
	
	/// <summary>
	/// A collection of node set identifiers
	/// </summary>
	public class NodeSetIdCollection: IEnumerable
	{
		// A list of string[2]. Item 0 is the node set id, item 1 is the addin that defines it.
		
		ArrayList list = new ArrayList ();
		
		/// <summary>
		/// Gets the node set identifier at the specified index.
		/// </summary>
		/// <param name='n'>
		/// An index.
		/// </param>
		public string this [int n] {
			get { return ((string[])list [n])[0]; }
		}
		
		/// <summary>
		/// Gets the item count.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		public int Count {
			get { return list.Count; }
		}
		
		/// <summary>
		/// Gets the collection enumerator.
		/// </summary>
		/// <returns>
		/// The enumerator.
		/// </returns>
		public IEnumerator GetEnumerator ()
		{
			ArrayList ll = new ArrayList (list.Count);
			foreach (string[] ns in list)
				ll.Add (ns [0]);
			return ll.GetEnumerator ();
		}
		
		/// <summary>
		/// Add the specified node set identifier.
		/// </summary>
		/// <param name='nodeSetId'>
		/// Node set identifier.
		/// </param>
		public void Add (string nodeSetId)
		{
			if (!Contains (nodeSetId))
				list.Add (new string [] { nodeSetId, null });
		}
		
		/// <summary>
		/// Remove a node set identifier
		/// </summary>
		/// <param name='nodeSetId'>
		/// Node set identifier.
		/// </param>
		public void Remove (string nodeSetId)
		{
			int i = IndexOf (nodeSetId);
			if (i != -1)
				list.RemoveAt (i);
		}
		
		/// <summary>
		/// Clears the collection
		/// </summary>
		public void Clear ()
		{
			list.Clear ();
		}
		
		/// <summary>
		/// Checks if the specified identifier is present in the collection
		/// </summary>
		/// <param name='nodeSetId'>
		/// <c>true</c> if the node set identifier is present.
		/// </param>
		public bool Contains (string nodeSetId)
		{
			return IndexOf (nodeSetId) != -1;
		}

		/// <summary>
		/// Returns the index of the specified node set identifier
		/// </summary>
		/// <returns>
		/// The index.
		/// </returns>
		/// <param name='nodeSetId'>
		/// A node set identifier.
		/// </param>
		public int IndexOf (string nodeSetId)
		{
			for (int n=0; n<list.Count; n++)
				if (((string[])list [n])[0] == nodeSetId)
					return n;
			return -1;
		}
		
		internal void SetExtensionsAddinId (string id)
		{
			foreach (string[] ns in list)
				ns [1] = id;
		}
		
		internal ArrayList InternalList {
			get { return list; }
			set { list = value; }
		}
		
		internal void MergeWith (string thisAddinId, NodeSetIdCollection other)
		{
			foreach (string[] ns in other.list) {
				if (ns [1] != thisAddinId && !list.Contains (ns))
					list.Add (ns);
			}
		}
		
		internal void UnmergeExternalData (string thisAddinId, Hashtable addinsToUnmerge)
		{
			ArrayList newList = new ArrayList ();
			foreach (string[] ns in list) {
				if (ns[1] == thisAddinId || (addinsToUnmerge != null && !addinsToUnmerge.Contains (ns[1])))
					newList.Add (ns);
			}
			list = newList;
		}
	}
}
