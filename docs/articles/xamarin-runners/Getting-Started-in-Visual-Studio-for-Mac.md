If you're using Visual Studio for Mac the NUnit templates extension can't be used. This guide assumes that you have a solution with either a PCL or a Shared project and a number of platform specific projects. It doesn't matter if you're using Xamarin.Forms or Xamarin.iOS/Xamarin.Android directly. Your solution might look something like this:

```
Project Solution
...Project (Contains Shared Code)
...Project.iOS
...Project.Android
```

### Shared Test Project

First, create a new PCL that holds your shared testing code, so the test code doesn't end up in production builds. Right Click on the Project Solution and Add a new project using the Cross-Platform Shared Library Template. Use Project.Tests as a name. Afterwards, your solution should look like this:

```
Project Solution
...Project (Contains Shared Code)
...Project.iOS
...Project.Android
...Project.Tests (Holds your testing code)
```

Edit the references of the newly created test project so that it contains a reference to the standard PCL. Afterwards, add a NuGet dependency on NUnit by right-clicking on Project.Tests and selection Add -> Add NuGet Package. **Note:** Ensure you reference the same version of NUnit as the version of nunit.xamarin you are using. e.g. If you are using nunit.xamarin 3.0.1, add the v3.0.1 NUnit NuGet package. Afterwards, your test project is ready.

### Platform runners

In order to run the tests it's necessary to create a project for each platform you'd like to support (iOS, Android and so on). Do so using the standard Xamarin templates for new platform projects. It's probably sensible to use a naming scheme like Project.Tests.iOS for the individual test projects do keep your solution structured.

```
Project Solution
...Project (Contains Shared Code)
...Project.iOS
...Project.Android
...Project.Tests (Holds your testing code)
...Project.Tests.iOS
...Project.Tests.Android
```

The NUnit.Xamarin runners are built using Xamarin.Forms, so you need to add NUnit, NUnit.Xamarin and Xamarin.Forms as NuGet dependencies to the newly created projects. It's also necessary to add a reference to the shared test project containing the tests.

If you've followed the steps above, you can now replace the AppDelegate.cs or MainActivity.cs with the code shown below. Since your tests are not in the same assembly as the runner it's a good idea to create a canary test class in the Shared Test Projects that you can use to reference the test assembly explicitly. In the example below, the name of this class is MyTest.cs.

### Android

**MainActivity.cs**

```csharp
[Activity(Label = "NUnit 3", MainLauncher = true, Theme = "@android:style/Theme.Holo.Light", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

        // This will load all tests within the current project
        var nunit = new NUnit.Runner.App();

        // If you want to add tests in another assembly
        //nunit.AddTestAssembly(typeof(MyTests).Assembly);

        // Do you want to automatically run tests when the app starts?
        nunit.Options = new TestOptions
            {
                AutoRun = true
            };

        LoadApplication(nunit);
    }
}
```
### iOS

**AppDelegate.cs**

```csharp
[Register("AppDelegate")]
public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
{
    //
    // This method is invoked when the application has loaded and is ready to run. In this
    // method you should instantiate the window, load the UI into it and then make the window
    // visible.
    //
    // You have 17 seconds to return from this method, or iOS will terminate your application.
    //
    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        global::Xamarin.Forms.Forms.Init();

        // This will load all tests within the current project
        var nunit = new NUnit.Runner.App();

        // If you want to add tests in another assembly
        //nunit.AddTestAssembly(typeof(MyTests).Assembly);

        // Do you want to automatically run tests when the app starts?
        nunit.Options = new TestOptions
            {
                AutoRun = true
            };

        LoadApplication(nunit);

        return base.FinishedLaunching(app, options);
    }
}
```
