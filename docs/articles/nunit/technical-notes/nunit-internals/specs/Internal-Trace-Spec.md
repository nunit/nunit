### DRAFT
NUnit traps text output directed to the Console, Trace or a logging subsystem. This makes it difficult to use text output when debugging the framework itself, particularly when working on the classes that redirect output. For that reason, NUnit incorporates a simple internal tracing mechanism for use by developers and for debugging problems in the field.

### User Stories

##### An NUnit User... 

  * enables NUnit's internal trace in order to collect information about an apparent bug or unexplained error.

##### An NUnit Developer...

  * makes use of the internal trace as a part of the development cycle.

### Design

NUnit provides a simple internal trace facility using the **InternalTrace** class. The following public methods are provided for creating output:

```csharp
public static void Error(string message, params object[] args)
public static void Warning(string message, params object[] args)
public static void Info(string message, params object[] args)
public static void Debug(string message, params object[] args)
```

By default, all output through **InternalTrace** is sent to the console, which means it is mixed with other console output. Note that the trace mechanism bypasses NUnit's redirection of output and displays on the actual Console. Of course, if NUnit is being run without a console - from the Gui, for example - the output will be lost.

#### Redirecting Output

The **InternalTrace.Open()** method allows the programmer to specify a file to which the output will be written. Any absolute or relative path may be used as an argument. Relative paths are interpreted as relative to the location of the assembly being tested. A programmer working on NUnit may call this method at any point in the code. **InternalTrace.Close()** is also provided.

<note>
Since the path is saved as a static property, it must be specified separately within each AppDomain. Writing to the same file from two different AppDomains is not supported.
</note>


#### Specifying Verbosity

Verbosity is specified using the nested enum InternalTrace.TraceLevel. Although the values currently match those of System.Diagnostics.TraceLevel, they have no actual relation to that class. Additional values may be added in the future.

```csharp
public class InternalTrace
{
    public enum TraceLevel
    {
        Off,
        Error,
        Warning,
        Info,
        Debug
    }
    ...
}
```

A programmer working on NUnit may set the TraceLevel at any point in the code by setting the **InternalTrace.Level** property.

### Unresolved Issues

  * How should the destination and verbosity level of internal trace be specified by a user?
