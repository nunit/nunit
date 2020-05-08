---
uid: nunitprojectxmlformat
---

## `<NUnitProject>`
The required root element for any NUnit project file.
 * **Containing Element:** None
 * **Contained Elements:** [`<Settings>`](#settings), [`<Config>`](#config)
 * **Attributes:**   None

## `<Settings>`
Contains global settings that apply to all configurations in the project. May be empty or absent.
 * **Containing Element:** [`<NUnitProject>`](#nunitproject)
 * **Contained Elements:** None
 * **Attributes:**
    * **activeconfig** The name of the config to use if none is specified by the user. Using `nunit-console`, you may override this through the `--config` command-line option.
    * **appbase** The application base to use in loading and running tests. Defaults to the location of the .nunit project file. If the path is relative, as it normally is, it is taken as relative to the location of the project file.
    * **processModel** Specifies how NUnit should create processes for executing test assemblies. Possible values are: Default, Single (no separate processes are created), Separate (tests are run in a single, separate process) and Multiple (each assembly is run in its own separate process). The default value is Multiple, provided there are multiple assemblies, otherwise Separate.
    * **domainUsage** Specifies how NUnit should create AppDomains within each process for running tests. Possible values are: Default, None (no domain is created), Single (a single domain is created) and Multiple (a separate domain is created for each assembly). The default is Multiple if multiple assemblies are run in the same process, Single if only one assembly is run in a process.

## `<Config>`
Describes a specific configuration of the project. This may map to traditional compiler configs like `Debug` or `Release` or may be used to specify any arbitrary collection of assemblies to be tested together. At least one configuration should be specified or the project will not be usable.
 * **Containing Element:** [`<NUnitProject>`](#nunitproject)
 * **Contained Elements:** [`<assembly>`](#assembly)
 * **Attributes:**
    * **name** The name of this configuration. (Required)
    * **appbase** The application base to use in loading and running tests under this config. Defaults to the appbase specified in the `<Settings>` element. The path should normally be relative to that global application base or to the location of project file if there is no global appbase.
    * **binpath** The probing path used to search for assemblies, consisting of a number of directory paths separated by semicolons. The directory paths should be relative to the application base and must be under it. Specifying this attribute automatically sets the binpathtype to 'manual'.
    * **binpathtype** Indicates how the probing path is determined. Possible values are: Auto (the probing path is determined from the location of the test assemblies), Manual (the path is specified by the binpath attribute) and None (no probing path is used). It is an error to specify a value other than Manual if the binpath attribute is also used.
    * **configfile** Specifies the path to a config file to be used when running tests under this configuration. The path is relative to the application base.
    * **processModel** Specifies how NUnit should create processes for executing test assemblies under this configuration. Possible values are: Default, Single (no separate processes are created), Separate (tests are run in a single, separate process) and Multiple (each assembly is run in its own separate process). The default is the value specified globally or Multiple if nothing has been specified.
    * **domainUsage** Specifies how NUnit should create AppDomains within each process for running tests under this configuration. Possible values are: Default, None (no domain is created), Single (a single domain is created) and Multiple (a separate domain is created for each assembly). The default is the value specified globally, if provided, otherwise Multiple if multiple assemblies are run in the same process, Single if only one assembly is run in a process.
    * **runtimeFramework** Specifies a runtime framework to be used in running tests. Abbreviations are the same as those accepted by the nunit-console command-line. If none is specified, tests are run under the target runtime specified when the assembly was compiled.

## `<assembly>`
Specifies a single assembly containing tests.
 * **Containing Element:** [`<Config>`](#config)
 * **Contained Elements:** None
 * **Attributes:**
    * **path** The path to the test assembly, relative to the application base for its configuration.

