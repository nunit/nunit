// IAssemblyReflector.cs
//
// Author:
//   Lluis Sanchez Gual <lluis@novell.com>
//
// Copyright (c) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Mono.Addins.Database
{
	/// <summary>
	/// An assembly reflector
	/// </summary>
	/// <remarks>
	/// This interface can be implemented to provide a custom method for getting information about assemblies.
	/// </remarks>
	public interface IAssemblyReflector
	{
		/// <summary>
		/// Called to initialize the assembly reflector
		/// </summary>
		/// <param name='locator'>
		/// IAssemblyLocator instance which can be used to locate referenced assemblies.
		/// </param>
		void Initialize (IAssemblyLocator locator);
		
		/// <summary>
		/// Gets a list of custom attributes
		/// </summary>
		/// <returns>
		/// The custom attributes.
		/// </returns>
		/// <param name='obj'>
		/// An assembly, class or class member
		/// </param>
		/// <param name='type'>
		/// Type of the attribute to be returned. It will always be one of the attribute types
		/// defined in Mono.Addins.
		/// </param>
		/// <param name='inherit'>
		/// 'true' if inherited attributes must be returned
		/// </param>
		object[] GetCustomAttributes (object obj, Type type, bool inherit);
		
		/// <summary>
		/// Gets a list of custom attributes
		/// </summary>
		/// <returns>
		/// The attributes.
		/// </returns>
		/// <param name='obj'>
		/// An assembly, class or class member
		/// </param>
		/// <param name='type'>
		/// Base type of the attribute to be returned
		/// </param>
		/// <param name='inherit'>
		/// 'true' if inherited attributes must be returned
		/// </param>
		List<CustomAttribute> GetRawCustomAttributes (object obj, Type type, bool inherit);
		
		/// <summary>
		/// Loads an assembly.
		/// </summary>
		/// <returns>
		/// The loaded assembly
		/// </returns>
		/// <param name='file'>
		/// Path of the assembly.
		/// </param>
		object LoadAssembly (string file);
		
		/// <summary>
		/// Loads the assembly specified in an assembly reference
		/// </summary>
		/// <returns>
		/// The assembly
		/// </returns>
		/// <param name='asmReference'>
		/// An assembly reference
		/// </param>
		object LoadAssemblyFromReference (object asmReference);
		
		/// <summary>
		/// Gets the names of all resources embedded in an assembly
		/// </summary>
		/// <returns>
		/// The names of the resources
		/// </returns>
		/// <param name='asm'>
		/// An assembly
		/// </param>
		string[] GetResourceNames (object asm);
		
		/// <summary>
		/// Gets the data stream of a resource
		/// </summary>
		/// <returns>
		/// The stream.
		/// </returns>
		/// <param name='asm'>
		/// An assembly
		/// </param>
		/// <param name='resourceName'>
		/// The name of a resource
		/// </param>
		Stream GetResourceStream (object asm, string resourceName);
		
		/// <summary>
		/// Gets all types defined in an assembly
		/// </summary>
		/// <returns>
		/// The types
		/// </returns>
		/// <param name='asm'>
		/// An assembly
		/// </param>
		IEnumerable GetAssemblyTypes (object asm);
		
		/// <summary>
		/// Gets all assembly references of an assembly
		/// </summary>
		/// <returns>
		/// A list of assembly references
		/// </returns>
		/// <param name='asm'>
		/// An assembly
		/// </param>
		IEnumerable GetAssemblyReferences (object asm);
		
		/// <summary>
		/// Looks for a type in an assembly
		/// </summary>
		/// <returns>
		/// The type.
		/// </returns>
		/// <param name='asm'>
		/// An assembly
		/// </param>
		/// <param name='typeName'>
		/// Name of the type
		/// </param>
		object GetType (object asm, string typeName);
		
		
		/// <summary>
		/// Gets a custom attribute
		/// </summary>
		/// <returns>
		/// The custom attribute.
		/// </returns>
		/// <param name='obj'>
		/// An assembly, class or class member
		/// </param>
		/// <param name='type'>
		/// Base type of the attribute to be returned. It will always be one of the attribute types
		/// defined in Mono.Addins.
		/// </param>
		/// <param name='inherit'>
		/// 'true' if inherited attributes must be returned
		/// </param>
		object GetCustomAttribute (object obj, Type type, bool inherit);
		
		/// <summary>
		/// Gets the name of a type (not including namespace)
		/// </summary>
		/// <returns>
		/// The type name.
		/// </returns>
		/// <param name='type'>
		/// A type
		/// </param>
		string GetTypeName (object type);
		
		/// <summary>
		/// Gets the full name of a type (including namespace)
		/// </summary>
		/// <returns>
		/// The full name of the type
		/// </returns>
		/// <param name='type'>
		/// A type
		/// </param>
		string GetTypeFullName (object type);
		
		/// <summary>
		/// Gets the assembly qualified name of a type
		/// </summary>
		/// <returns>
		/// The assembly qualified type name
		/// </returns>
		/// <param name='type'>
		/// A type
		/// </param>
		string GetTypeAssemblyQualifiedName (object type);
		
		/// <summary>
		/// Gets a list of all base types (including interfaces) of a type
		/// </summary>
		/// <returns>
		/// An enumeration of the full name of all base types of the type
		/// </returns>
		/// <param name='type'>
		/// A type
		/// </param>
		IEnumerable GetBaseTypeFullNameList (object type);
		
		/// <summary>
		/// Checks if a type is assignable to another type
		/// </summary>
		/// <returns>
		/// 'true' if the type is assignable
		/// </returns>
		/// <param name='baseType'>
		/// Expected base type.
		/// </param>
		/// <param name='type'>
		/// A type.
		/// </param>
		bool TypeIsAssignableFrom (object baseType, object type);
		
		/// <summary>
		/// Gets the fields of a type
		/// </summary>
		/// <returns>
		/// The fields.
		/// </returns>
		/// <param name='type'>
		/// A type
		/// </param>
		IEnumerable GetFields (object type);
		
		/// <summary>
		/// Gets the name of a field.
		/// </summary>
		/// <returns>
		/// The field name.
		/// </returns>
		/// <param name='field'>
		/// A field.
		/// </param>
		string GetFieldName (object field);
		
		/// <summary>
		/// Gets the full name of the type of a field
		/// </summary>
		/// <returns>
		/// The full type name
		/// </returns>
		/// <param name='field'>
		/// A field.
		/// </param>
		string GetFieldTypeFullName (object field);
	}
	
	/// <summary>
	/// Allows finding assemblies in the file system
	/// </summary>
	public interface IAssemblyLocator
	{
		/// <summary>
		/// Locates an assembly
		/// </summary>
		/// <returns>
		/// The full path to the assembly, or null if not found
		/// </returns>
		/// <param name='fullName'>
		/// Full name of the assembly
		/// </param>
		string GetAssemblyLocation (string fullName);
	}
	
	/// <summary>
	/// A custom attribute
	/// </summary>
	public class CustomAttribute: Dictionary<string,string>
	{
		string typeName;
		
		/// <summary>
		/// Full name of the type of the custom attribute
		/// </summary>
		public string TypeName {
			get { return typeName; }
			set { typeName = value; }
		}
	}
}
