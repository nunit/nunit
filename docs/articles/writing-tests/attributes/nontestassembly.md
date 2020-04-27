This is an _assembly-level_ attribute, which may be used to specify that even though
the assembly referes to NUnit it does not contain any tests. This attribute can be
used in connection with the command line option `--skipnontestassemblies` of the
console to skip assemblies without failing.

#### Example

The following code, which might be placed in AssemblyInfo.cs, specifies that the
assembly does not contain any tests.

```csharp
  [assembly: NonTestAssembly]
```


#### See also...
 * `--skipnontestassemblies` in [[Console-Command-Line]]
