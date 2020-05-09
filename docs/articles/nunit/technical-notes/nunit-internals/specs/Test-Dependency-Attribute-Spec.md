### DRAFT - Not Yet Implemented
This spec describes a proposed new attribute to be used to specify dependencies between tests within a test fixture, and between different test fixtures.

Attribute

We would define a new attributes, Dependencies:

`[Dependencies (<dependency-specification> [, <dependency-specification> [, ...]])]`

where `<dependency-specification>` is:

```
Before="TestA, TestB"
After="TestC, TestD"
AfterAnySuccess="TestE, TestF"
AfterAnyFailure="TestG, TestH"
AfterAllSuccess="TestE, TestF"
AfterAllFailure="TestG, TestH"
BeforeAll
AfterAll
AfterAllSuccess
```

This attribute would apply to Test's within a single TestFixture, or to TextFixture's within a single test assembly.

Each dependency-specification is described in more detail below (in all cases, `<test-list>` should be replaced with a `<testfixture-list>` if the attribute is being applied to a TestFixture):

```
<test-list> is a string containing a comma separated list of Test's to
    include in the dependency.
<testfixture-list> is a string containing a comma separated list of
    TestFixture's to include in the dependency.


Before=<test-list>
    The designated Test or TestFixture will be run to completion (successfully or not) 
    before any of the tests specified in the list are run.

After=<test-list>
    The designated Test or TestFixture will be run after the completion (successfully
    or not) of all of the tests specified in the list.

AfterAllSuccess=<test-list>
    The Test or TestFixture will be run after the successful completion of all of
    the tests specified in the list.  If any of the tests specified in the list fail,
    the designated Test or TestFixture will not be run.

AfterAllFailure=<test-list>
    The Test or TestFixture will be run after the failure of all of the tests specified
    in the list.  If any of the tests specified in the list succeed, the designated Test
    or TestFixture will not be run. 

AfterAnySuccess=<test-list>
    The Test or TestFixture will be run after the successful completion of any of the tests
    specified in the list.  If all of the tests specified in the list fail, the designated
    Test or TestFixture will not be run.

AfterAnyFailure=<test-list>
    The Test or TestFixture will be run after the failure of any of the tests specified in
    the list.  If all of the tests specified in the list succeed, the designated Test or
    TestFixture will not be run. 
   
BeforeAll
    The Test or TestFixture will be run before all other tests or test fixtures are run.
    This dependency can be overridden by a specific Before dependency referencing this
    Test or TestFixture.

AfterAll
    The Test or TestFixture will be run after all other tests or test fixtures have been
    run (whether they have completed successfully or not).  This dependency can be
    overridden by a specific Afterxxx dependency referencing this Test or TestFixture.

AfterAllSuccess
    The Test or TestFixture will be run after all other tests or test fixtures have been
    run and completed successfully. If any Test or TestFixture fails, this Test or
    TestFixture will not be run. This dependency can be overridden by a specific
    Afterxxx dependency referencing this Test or TestFixture.
```

Note that a single Test or TestFixture may have multiple `<dependency-specification>` elements as long as they do not conflict with each other.

Any loops or contradictory references in the dependencies will be discovered, and will result in the specified dependencies being ignored, and a warning being given.  For example, if TestA has Before="TestB" and TestB has Before="TestA", both will be ignored, and a warning given. (An alternative would be to use whichever was specified first  or last).

If multiple tests or test fixtures are in the same relative position in the dependency tree, the order in which they will be run will be the default order.  For example, if multiple tests have a BeforeAll dependency, they will run in the default order, but before all other tests not having that dependency specified.


Items to consider:

*    Is the name in a `<test-list>` the name in the code, or the generated name for the test?  I would prefer to use the static name that is in the code, but there may be a case for using the generated name.
*    Should there be a simple dependency language to be used to specify the dependency rather then the static properties? e.g. `before("TestA") && afterFails ("TestB") || afterSuccess ("TestC")`
