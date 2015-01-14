//
// ConditionType.cs
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
using System.Xml;
using Mono.Addins.Description;
using System.Collections;

namespace Mono.Addins
{
	/// <summary>
	/// A condition evaluator.
	/// </summary>
	/// <remarks>
	/// Add-ins may use conditions to register nodes in an extension point which
	/// are only visible under some contexts. For example, an add-in registering
	/// a custom menu option to the main menu of a sample text editor might want
	/// to make that option visible only for some kind of files. To allow add-ins
	/// to do this kind of check, the host application needs to define a new condition.
	/// </remarks>
	public abstract class ConditionType
	{
		internal event EventHandler Changed;
		string id;
		
		/// <summary>
		/// Evaluates the condition.
		/// </summary>
		/// <param name="conditionNode">
		/// Condition node information.
		/// </param>
		/// <returns>
		/// 'true' if the condition is satisfied.
		/// </returns>
		public abstract bool Evaluate (NodeElement conditionNode);
		
		/// <summary>
		/// Notifies that the condition has changed, and that it has to be re-evaluated.
		/// </summary>
		/// This method must be called when there is a change in the state that determines
		/// the result of the evaluation. When this method is called, all node conditions
		/// depending on it are reevaluated and the corresponding events for adding or
		/// removing extension nodes are fired.
		/// <remarks>
		/// </remarks>
		public void NotifyChanged ()
		{
			if (Changed != null)
				Changed (this, EventArgs.Empty);
		}
		
		internal string Id {
			get { return id; }
			set { id = value; }
		}
	}
	
	internal class BaseCondition
	{
		BaseCondition parent;
		
		internal BaseCondition (BaseCondition parent)
		{
			this.parent = parent;
		}
		
		public virtual bool Evaluate (ExtensionContext ctx)
		{
			return parent == null || parent.Evaluate (ctx);
		}
		
		internal virtual void GetConditionTypes (ArrayList listToFill)
		{
		}
	}
	
	internal class NullCondition: BaseCondition
	{
		public NullCondition (): base (null)
		{
		}
		
		public override bool Evaluate (ExtensionContext ctx)
		{
			return false;
		}
	}
	
	class OrCondition: BaseCondition
	{
		BaseCondition[] conditions;
		
		public OrCondition (BaseCondition[] conditions, BaseCondition parent): base (parent)
		{
			this.conditions = conditions;
		}
		
		public override bool Evaluate (ExtensionContext ctx)
		{
			if (!base.Evaluate (ctx))
				return false;
			foreach (BaseCondition cond in conditions)
				if (cond.Evaluate (ctx))
					return true;
			return false;
		}
		
		internal override void GetConditionTypes (ArrayList listToFill)
		{
			foreach (BaseCondition cond in conditions)
				cond.GetConditionTypes (listToFill);
		}
	}
	
	class AndCondition: BaseCondition
	{
		BaseCondition[] conditions;
		
		public AndCondition (BaseCondition[] conditions, BaseCondition parent): base (parent)
		{
			this.conditions = conditions;
		}
		
		public override bool Evaluate (ExtensionContext ctx)
		{
			if (!base.Evaluate (ctx))
				return false;
			foreach (BaseCondition cond in conditions)
				if (!cond.Evaluate (ctx))
					return false;
			return true;
		}
		
		internal override void GetConditionTypes (ArrayList listToFill)
		{
			foreach (BaseCondition cond in conditions)
				cond.GetConditionTypes (listToFill);
		}
	}
	
	class NotCondition: BaseCondition
	{
		BaseCondition baseCond;
		
		public NotCondition (BaseCondition baseCond, BaseCondition parent): base (parent)
		{
			this.baseCond = baseCond;
		}
		
		public override bool Evaluate (ExtensionContext ctx)
		{
			return !base.Evaluate (ctx);
		}
		
		internal override void GetConditionTypes (System.Collections.ArrayList listToFill)
		{
			baseCond.GetConditionTypes (listToFill);
		}
	}

	
	internal sealed class Condition: BaseCondition
	{
		ExtensionNodeDescription node;
		string typeId;
		AddinEngine addinEngine;
		string addin;

		internal const string SourceAddinAttribute = "__sourceAddin"; 
		
		internal Condition (AddinEngine addinEngine, ExtensionNodeDescription element, BaseCondition parent): base (parent)
		{
			this.addinEngine = addinEngine;
			typeId = element.GetAttribute ("id");
			addin = element.GetAttribute (SourceAddinAttribute);
			node = element;
		}
		
		public override bool Evaluate (ExtensionContext ctx)
		{
			if (!base.Evaluate (ctx))
				return false;

			if (!string.IsNullOrEmpty (addin)) {
				// Make sure the add-in that implements the condition is loaded
				addinEngine.LoadAddin (null, addin, true);
				addin = null; // Don't try again
			}
			
			ConditionType type = ctx.GetCondition (typeId);
			if (type == null) {
				addinEngine.ReportError ("Condition '" + typeId + "' not found in current extension context.", null, null, false);
				return false;
			}
			
			try {
				return type.Evaluate (node);
			}
			catch (Exception ex) {
				addinEngine.ReportError ("Error while evaluating condition '" + typeId + "'", null, ex, false);
				return false;
			}
		}
		
		internal override void GetConditionTypes (ArrayList listToFill)
		{
			listToFill.Add (typeId);
		}
	}
}
