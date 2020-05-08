**Result Writers** take the result of a test run, in NUnit 3.0 XML format, and use it to create a result file in some other format. NUnit itself provides a two result writers, one to create output in NUnit V2 format and another to write test cases to the console. The definition of a result writer extension might look something like this:

```csharp
[Extension]
[ExtensionProperty("Format", "custom")]
public class CustomResultWriterFactory : IResultWriter
{
    ...
}
```

You must provide an `ExtensionPropertyAttribute` giving the name of the format you support. Users would access your format from the nunit-console command-line by using that name in a result specification, like

```
   nunit-console test.dll --result=CustomResult.xml;format=custom
```

The `IResultWriter` interface, which you must implement, is defined as follows:

```csharp
/// <summary>
/// Common interface for objects that process and write out test results
/// </summary>
[TypeExtensionPoint(
    Description = "Supplies a writer to write the result of a test to a file using a specific format.")]
public interface IResultWriter
{
    /// <summary>
    /// Checks if the output path is writable. If the output is not
    /// writable, this method should throw an exception.
    /// </summary>
    /// <param name="outputPath"></param>
    void CheckWritability(string outputPath);

    /// <summary>
    /// Writes result to the specified output path.
    /// </summary>
    /// <param name="resultNode">XmlNode for the result</param>
    /// <param name="outputPath">Path to which it should be written</param>
    void WriteResultFile(XmlNode resultNode, string outputPath);

    /// <summary>
    /// Writes result to a TextWriter.
    /// </summary>
    /// <param name="resultNode">XmlNode for the result</param>
    /// <param name="writer">TextWriter to which it should be written</param>
    void WriteResultFile(XmlNode resultNode, TextWriter writer);
}
```

The engine calls the `CheckWritability` method at the start of the run, before executing any tests. The `WriteResultFile` method is called after the run is complete. The writer may check writability in any way desired, including writing an abbreviated output file, which it will later overwrite.