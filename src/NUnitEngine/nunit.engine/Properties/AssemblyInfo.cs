using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("NUnit Engine")]
[assembly: AssemblyDescription("Provides a common interface for loading, exploring and running NUnit tests")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5796938b-03c9-4b75-8b43-89a8adc4acd0")]

// This forwards the shim serializable attribute in the PCL framework to the real one so we
// can run PCL tests.
[assembly: TypeForwardedTo(typeof(SerializableAttribute))]

