### DRAFT
NUnit 3.0 is intentionally changing certain design decisions made in earlier versions. We document those changes here so that developers may find them all in one place.

For details, see the individual specifications referenced under each 
design change subheading.

### Key Design Changes

#### Multiple Framework Distributions

NUnit 3.0 introduces separate framework builds for each supported runtime version, including .NET 2.0, 3.5, 4.0 and 4.5. We will continue to use the same framework assemblies for both the Microsoft and Mono implementations.

#### Merge NUnitLite

NUnitLite is a light-weight version of NUnit, requiring minimal resources and running on platforms not supported by full NUnit. NUnitLite is approximately equivalent to the framework portion of NUnit, with the addition of a rudimentary test runner.

For the NUnit 3.0 release, the NUnitLite project code has been merged with the code of NUnit itself, using conditional compilation to support a reduced feature set.

#### Framework Boundary

Historically, most xUnit frameworks incorporate the logic for loading and
running tests in the same module that includes assertions, attributes and
other types referenced by the tests. NUnit started that way but the loading
and test execution logic was extracted into a separate assembly in later
versions.

This approach had some benefit - most notably NUnit's ability to run 
tests built against older versions of the framework - but has proven
less useful as more features were added. Essentially, the surface area
that NUnit presents to a client program wanting to run tests grows 
each time a new feature is added. This has made it very difficult for
third parties to keep up with NUnit's feature growth.

For NUnit 3.0, the boundary has been moved. Each version of the framework
incorporates a test runner that knows how to load and execute the
tests supported by that version.

#### Framework Api

The internal (core) interfaces used by earlier versions of NUnit are not suitable for external use because they tend to change as features are added. The 3.0 release incorporates a new interface, supporting the same functionality as the existing interfaces, but with fewer dependencies on custom types. It is usable by both NUnit and third-party runners and will provide functions of test discovery, loading and execution.

See [Framework Api](Framework-Api.md)

#### Active Attributes

In the NUnit 2.x series, Attributes are fundamentally passive objects. They are used as markers for certain kinds of functionality, but that functionality is implemented in the nunit.core assembly. Reflection is used to identify attributes, in order to avoid a reference from the core to a specific version of the framework.

In NUnit 3.0, since the test loader is part of the framework assembly, we'll be able to identify attributes without use of reflection, which should improve load performance. Attributes will actually contain the code that carries out the necessary functionality.

See [DRAFT:Active Attributes](Active-Attributes.md)

#### Test Loading

Earlier releases of NUnit load tests in a hierarchy based on the namespace and may optionally load them as a flat list of fixtures. The test hierarchy is built as the tests are loaded and reflected in the gui display.

With NUnit 3.0, the test loader will only load fixtures and will not
create a hierarchy. It will be the responsibility of the Gui to construct
whatever display hierarchy the user chooses as a view of the tests.

This will simplify the loading of tests and is compatible with NUnitLite,
which already loads tests this way.

#### Test Execution

Currently, the sequencing of tests is the responsibility of the tests 
themselves. That is, each test suite executes its own child tests and each 
test reports its own results.

The absence of a distinct test execution object makes it difficult to
support certain features, such as high-level setup and teardown, parallel
test execution and cancellation of running tests with restart.

In NUnit 3.0, the objects representing tests will no longer have the
responsibility of running any subordinate tests and a separate test runner
will sequence through the tests to execute them in the desired order.