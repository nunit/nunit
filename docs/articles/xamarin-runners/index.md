The NUnit Xamarin Runners provide NUnit 3 test runners for Xamarin and mobile devices. See [Testing Xamarin projects using NUnit 3](http://www.alteridem.net/2015/12/21/testing-xamarin-projects-using-nunit-3/) for more general information.

## Options

Runner options are set inside a `TestOptions` object. For example:

```C#
var nunit = new NUnit.Runner.App();

nunit.Options = new TestOptions
            {
                AutoRun = true
            };
```

The following options are available: 

|   Option            | Version added | Type                            | Usage| 
|---------------------|---------------|---------------------------------|------|
| AutoRun             | 3.0           | Boolean                         | If True, the tests will run automatically when the app starts, otherwise you must run them manually.   |
| TerminateAfterExecution | 3.6.1     | Boolean                         | If True, app will exit cleanly after test execution.   |
| TcpWriterParameters | 3.6.1         | [TcpWriterInfo](#tcpwriterinfo) | Provide a TCP listener host and port, sends result as XML to the listening server. Takes a `TcpWriterInfo` - see [below](#tcpwriterinfo). |
| CreateXmlResultFile | 3.6.1         | Boolean                         | If True, create a xml file containing results.  |
| ResultFilePath      | 3.6.1         | String                          | Specify file path to save xml result file.      |

### TcpWriterInfo
`TcpWriterInfo` takes three parameters: hostname, port, and an optional timeout in seconds (default 10).

```C#
TcpWriterParameters = new TcpWriterInfo("192.168.0.108", 13000, 10);
```