NUnit provides three levels of public APIs for discovering and running tests. From highest to lowest level they are:

* **Test Engine API** - for general use by any program that needs to discover and execute tests.
* **Engine Driver API** - implemented by framework drivers to allow the engine to communicate with a particular framework.
* **Framework API** - used by the NUnit 3 framework driver to communicate with the framework.

![](https://docs.google.com/drawings/d/1eBVjjrWtiqgyIod_ld0rjtyLdeLYzXs_JMGHkhkZaJw/pub?w=361&h=434)

#### Test Engine API

The NUnit TestEngine is a separate component, introduced in NUnit 3.0, which knows how to discover and execute tests. It provides an API for both simple batch execution and more complex interaction as needed by Gui test runners. It also provides additional Engine services beyond what the framework provides. This is what we recommend for use by anyone needing to run NUnit tests programmatically.

See [[Test Engine API]] for more info.

#### Engine Driver API

The NUnit TestEngine uses drivers to communicate with test frameworks. It is possible to create a driver for running any sort of test framework, supporting any language at all. The driver API is what makes this possible. The TestEngine has support for the NUnit 3 framework built in. An extension driver for running NUnit 2 tests is also available.

The driver API is only intended to be implemented by drivers and is only used by the NUnit engine. See [[Engine Driver API]] for more info.

#### NUnit Framework API

This is a primitive API implemented by the NUnit 3 Framework. The NUnitFrameworkDriver in the engine uses this API. The API is a bit complicated to use. Since it needs to support multiple versions of the framework, it uses well-known framework class names, which are constructed via reflection. All results are returned as raw XML. 

This API is not intended for any use except by NUnit itself. See [[Framework API]] for more info.
