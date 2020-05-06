The NUnit 3.0 framework can run tests in parallel within an assembly. This is a completely separate facility from [[Engine Parallel Test Execution]], although it is possible to use both in the same test run.

By default, no parallel execution takes place. Attributes are used to indicate which tests may run in parallel and how they relate to other tests.


#### Platform Support

Parallel execution is supported by the NUnit framework on desktop .NET runtimes and .NET Standard 2.0. It is not supported in the .NET Standard 1.6 build, although the attributes are recognized without error in order to allow use in projects that build against multiple targets.

#### ParallelizableAttribute

This attribute is used to indicate whether the test and/or its descendants may be run in parallel with other tests. The constructor takes an optional `ParallelScope` enumeration argument (see below), which defaults to `ParallelScope.Self`. The attribute may be used at the assembly, class or method level and the word "test" in the rest of this description refers to the suite or test case that corresponds to the item on which the attribute appears.

One Named Property is supported:
  * `Scope = ParallelScope` for setting `ParallelScope` using property syntax

#### NonParallelizableAttribute

This Attribute is used to indicate that the test as well as its descendants may __not__ be run in parallel with other tests. Although `[NonParallelizable]` is completely equivalent to `[Parallelizable(ParallelScope.None)]`, we recommend that you use the former for clarity.

##### ParallelScope Enumeration

This is a `[Flags]` type enumeration used to specify which tests may run in parallel. It applies to the test upon which it appears and any subordinate tests. The following values are available for use:
  * `ParallelScope.Self` indicates that the test itself may be run in parallel with other tests. This is the default for the `ParallelizableAttribute` and is the only value permitted on a test method.
  * `ParallelScope.Children` indicates that the descendants of the test may be run in parallel with respect to one another.
  * `ParallelScope.Fixtures` indicates that test fixtures that are the descendants of the test may be run in parallel with one another.`

**Note:** Additional values of the enumerator are used internally. They do not show up in the Intellisense and are not documented here. The value `ParallelScope.None`, which was used before the creation of the `NonParallelizableAttribute` is still accepted for the purpose of backward compatibility.

##### Specifying Parallelism at Multiple Test Levels

`[Parallelizable]` or `[NonParallelizable]` may be specified on multiple levels of the tests, with lower-level specifications overriding higher ones to a certain degree. Thus, if the assembly has `[NonParallelizable]` either by use of the attribute or by default, classes with `[Parallelizable]` may be run in parallel as may their children if an appropriate scope is used.

It is important to note that a parallel or non-parallel specification only applies at that level where it appears and below. It cannot override the settings on higher-level tests. In this way, parallelism is not absolute but is relative to other tests at the same level in the tree. The following are a few examples of how this works:

1. **Non-parallel class with parallel methods:** The methods only run in parallel with one another, not with the test methods of any other classes.

2. **Parallel class with non-parallel methods:** The methods run sequentially, usually on the same thread that ran the class one-time setup, but may actually be running in parallel with other, unrelated methods from other classes.

3. **Non-parallel SetUpFixture with parallel test fixtures:** The entire group of fixtures runs separately from any fixtures outside the group. Within the group, multiple fixtures run in parallel.

4. **Parallel SetUpFixture with non-parallel test fixtures:** The group runs in parallel with other fixtures and groups. Within the group, only one fixture at a time may execute.

5. **Parallel SetUpFixture with non-parallel test fixtures containing parallel test cases:** This is just one example of a more complex setup. The fixtures themselves run as described in (4) but the cases within each fixture run in parallel with one another.

Once you understand the principles, you can construct complex hierarchies of parallel and non-parallel tests.

#### LevelOfParallelismAttribute

This is an **assembly-level** attribute, which may be used to specify the level of parallelism, that is, the maximum number of worker threads executing tests in this assembly. It may be overridden using a command-line option in the console runner. If it is not specified, NUnit uses a default value based on the number of processors available or a specified minimum, whichever is greater.

#### Parallel Execution Internals

We use multiple queues organized into "shifts". A `WorkShift` consists of one or more queues of work items, which may be active at the same time. As the name suggests, no two shifts are active simultaneously. NUnit runs one `WorkShift` until all available work is complete and then switches to the next shift. When there is no work for any shift, the run is complete.

There are three shifts, listed here with their associated queues...

|     Shift              |    Queues              |  Workers  |  Usage    |
|------------------------|------------------------|-----------|-----------|
| Parallel Shift         | Parallel Queue         |    LoP*   | Parallelizable tests run in the MTA |
|                        | Parallel STA Queue     |     1     | Parallelizable tests run in the STA |
| Non-Parallel Shift     | Non-Parallel Queue     |     1     | Non-parallelizable tests run in the MTA |
| Non-Parallel STA Shift | Non-Parallel STA Queue |     1     | Non-parallelizable tests run in the STA |

_* Depends on Level of Parallelism_

For efficiency, each queue is created when the first test is added to it. At the time of creation, all workers for that queue are also created and initialized.

Whenever a non-parallel fixture begins execution, an entirely new set of queues is created so that the child tests of that fixture may be run without any conflict from other tests that are already in the main set of queues.

If the command line specifies zero workers, all use of the dispatcher and its queues is bypassed and tests are run sequentially on a single thread.
