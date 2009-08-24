// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
// ***********************************************************************

using System;
using System.IO;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace NUnit.Framework.CodeGeneration
{
    public class CodeGenSpec
    {
        private Stanza stanza = new Stanza();

        private string typeName = "void";
        private List<string> comments = new List<string>();
        private List<CodeGenItem> genSpecs = new List<CodeGenItem>();
        private List<string> defaults = new List<string>();

        private string currentRegion;

        public CodeGenSpec(Stanza stanza)
        {
            this.stanza = stanza;
            this.ProcessLines();
        }

        private void ProcessLines()
        {
            foreach (string line in stanza)
            {
                if (line.StartsWith("Type:"))
                    this.typeName = line.Substring(5).Trim();
                else if (line.StartsWith("///"))
                    this.comments.Add(line);
                else if (line.StartsWith("Gen:"))
                    AddSyntaxElement(line);
                else if (line.StartsWith("Gen3:"))
                    AddStandardSyntaxElements(line.Substring(5).Trim());
                else if (line.StartsWith("Default:"))
                    this.defaults.Add(line.Substring(8).Trim());
                else
                    IssueFormatError(line);
            }
        }

        private void AddSyntaxElement(string line)
        {
            this.genSpecs.Add(new CodeGenItem(line));
        }

        private void AddStandardSyntaxElements(string element)
        {
            int arrow = element.IndexOf("=>");
            if (arrow < 0) IssueFormatError(element);
            string leftside = element.Substring(0, arrow);
            string rightside = element.Substring(arrow + 2);

            string fullname = leftside;
            string attributes = "";
            int rbrack = leftside.LastIndexOf("]");
            if (rbrack > 0)
            {
                attributes = leftside.Substring(0, rbrack + 1);
                fullname = leftside.Substring(rbrack + 1);
            }

            string constraint = rightside;
            if (constraint.StartsWith("return "))
                constraint = constraint.Substring(7);
            if (constraint.StartsWith("new "))
                constraint = constraint.Substring(4);

            int dot = fullname.IndexOf('.');
            if (dot < 0) IssueFormatError(element);
            string name = fullname.Substring(dot + 1);

            this.typeName = constraint.Substring(0, constraint.IndexOf("("));
            this.genSpecs.Add(new CodeGenItem(
                "Gen: " + leftside + "=>" + rightside));
            this.genSpecs.Add(new CodeGenItem(
                "Gen: " + attributes + "ConstraintFactory." + name + "=>" + rightside));
            this.genSpecs.Add(new CodeGenItem(
                "Gen: " + attributes + "ConstraintExpression." + name + "=>(" + typeName + ")this.Append(" + rightside + ")"));
        }

        private void IssueFormatError(string line)
        {
            throw new ArgumentException("Invalid line in spec file" + Environment.NewLine + line);
        }

        public void Generate(IndentedTextWriter writer, string className, bool isStatic)
        {
            foreach (CodeGenItem spec in genSpecs)
            {
                if (spec.ClassName == className)
                {
                    if (currentRegion == null)
                    {
                        //currentRegion = spec.LeftPart;
                        //int dot = currentRegion.IndexOf('.');
                        //if (dot > 0) currentRegion = currentRegion.Substring(dot + 1);
                        currentRegion = spec.MethodName;
                        int lpar = currentRegion.IndexOf('(');
                        if (lpar > 0) currentRegion = currentRegion.Substring(0, lpar);

                        writer.WriteLine("#region " + currentRegion);
                        writer.WriteLine();
                    }

                    if (spec.IsGeneric)
                        writer.WriteLineNoTabs("#if NET_2_0");

                    if (spec.ClassName == "Assert")
                        GenerateAssertOverloads(writer, isStatic, spec);
                    else
                        GenerateMethod(writer, isStatic, spec);

                    if (spec.IsGeneric)
                        writer.WriteLineNoTabs("#endif");
                }
            }

            if (currentRegion != null)
            {
                writer.WriteLine("#endregion");
                writer.WriteLine();
                currentRegion = null;
            }
        }

        private void GenerateMethod(IndentedTextWriter writer, bool isStatic, CodeGenItem spec)
        {
            WriteComments(writer);
            WriteMethodDefinition(writer, isStatic, spec);
        }

        private void WriteMethodDefinition(IndentedTextWriter writer, bool isStatic, CodeGenItem spec)
        {
            if (spec.Attributes != null)
                writer.WriteLine(spec.Attributes);

            if (isStatic)
                writer.WriteLine("public static {0} {1}", typeName, spec.MethodName);
            else
                writer.WriteLine("public {0} {1}", typeName, spec.MethodName);
            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine(spec.IsProperty
                    ? "get { return " + spec.RightPart + "; }"
                    : typeName == "void"
                        ? spec.RightPart + ";"
                        : "return " + spec.RightPart + ";");
            writer.Indent--;
            writer.WriteLine("}");
            writer.WriteLine();
        }

        private void WriteComments(IndentedTextWriter writer)
        {
            foreach (string comment in comments)
                writer.WriteLine(comment);
        }

        private void GenerateAssertOverloads(IndentedTextWriter writer, bool isStatic, CodeGenItem spec)
        {
            if (!spec.LeftPart.EndsWith(")") || !spec.RightPart.EndsWith(")"))
                IssueFormatError(spec.ToString());
            string leftPart = spec.LeftPart.Substring(0, spec.LeftPart.Length - 1);
            string rightPart = spec.RightPart.Substring(0, spec.RightPart.Length - 1);

            CodeGenItem spec1 = new CodeGenItem(
                "Gen: " + leftPart + ", string message, params object[] args)=>" + rightPart + " ,message, args)");
            WriteComments(writer);
            writer.WriteLine("/// <param name=\"message\">The message to display in case of failure</param>");
            writer.WriteLine("/// <param name=\"args\">Array of objects to be used in formatting the message</param>");
            WriteMethodDefinition(writer, isStatic, spec1);

            CodeGenItem spec2 = new CodeGenItem(
                "Gen: " + leftPart + ", string message)=>" + rightPart + " ,message, null)");
            WriteComments(writer);
            writer.WriteLine("/// <param name=\"message\">The message to display in case of failure</param>");
            WriteMethodDefinition(writer, isStatic, spec2);

            CodeGenItem spec3 = new CodeGenItem(
                "Gen: " + leftPart + ")=>" + rightPart + " ,null, null)");
            WriteComments(writer);
            WriteMethodDefinition(writer, isStatic, spec3);
        }

        public List<string> Defaults
        {
            get { return defaults; }
        }
    }
}
