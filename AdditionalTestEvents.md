# Additional Test Events for OneTimeSetUp and OneTimeTearDown

## Problem Statement

For our higher-level System Tests, we need a mechanism that notified us during the execution of a test run when certain parts of the test suite are executed. Regarding the begin and the end of a test case, NUnit already already provides corresponding test events. So we can implement a custom TestEventListener and attach some handlers to it that react when the corresponding events occure.

However, we need a similar mechanism also for OneTimeSetUp and OneTimeTearDown.

## Solution Statement

In this branch we tried to implement a solution by adding additional test events for 
* Start of OneTimeSetUp 
* End of OneTimeSetUp 
* Start of OneTimeTearDown 
* End of OneTimeTearDown

We would appreciate your opinion if the provided solution is in principle a good idea or if we should tackle the problem in a completely different way.

### Description of the Implementation

#### Extending the ITestListener

First I extended `ITestListener` by the corresponding APIs,
e.g. `void OneTimeSetUpStarted(ITest test);`
> I am fully aware that the parameter type ITest is not suitable here, because it only contains information about the surrounding test fixture. However I took it as a starting point in order to simulate how event parameters will be passed in the `TestProgressReporter`.

Possible drawback:
Every class implementing `ITestListener` has to be adapted. These are in this case here:
* The EventQueue
* The QueuingEventListener
* The always empty (dummy) TestListener.cs
* The TestProgressReporter, that creates the xml formated events
* The TeamCityEventListener (seems specific, I left the implementation empty)
* The TextRunner for NUnitLite (left implementation empty for now)
* All test classes that implement `ITestListener` 

#### Finding the Right Place to Raise the Events

I ended up raising the events in the `OneTimeSetUpCommand` and `OneTimeTearDownCommand`, however I experimented with different locations:

* OneTimeSetUpCommand.cs
Felt most fitting because the event can be raised as part of the `BeforeTest` / `AfterTest` action, thats why I ended up imlementing the solution here. However I noticed that the OneTimeTearDown event in this case is raised too often in the special case that a fixture has an additional `OneTimeSetUp` in a base class. See my additions in the `TestAssemblyRunnerTests` in order to see what I mean. 
* SetUpTearDownItem.cs
Benefit from raising the event here would be that we are most close to the execution of the actual method. 
Drawback: At this level we cannot distinguish anymore between `SetUp` and `OneTimeSetUp` etc...
Please see also my comments in this class.
* CompositeWorkItem.cs
This was my first attempt, just because the method name ("MakeOneTimeSetUpCommand") indicated it. However, also here I received additional unexpected events and the methods for OneTimeSetUp and OneTimeTearDown looked a bit asymetric, so I didnt follow up the implementation at that place. 
I also stumbled over a code block that looked a little suspicious and could be responsible for the additional unexpected event. Please see also my comments in this class.
