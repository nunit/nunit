> **NOTE:** This page is a specification that was used as a starting point for creating the feature in NUnit. It needs to be reviewed and revised in order to accurately reflect what was actually built. If you take it with a grain of salt, it may still be helpful to you as documentation. This notice will be removed when the page is brought up to date.

> **NOTE:** Partially implemented feature

NUnit 2.6.4 supports parameterized test fixtures using constructor arguments specified on the `TestFixtureAttribute`. This is similar to the way that `TestCaseAttribute` handles parameterized methods. However, methods in 2.6.4 benefit from a very rich set of attributes in addition, allowing the data to be kept separately from the test and permitting use of data types that cannot appear as arguments to an Attribute constructor in .NET.

For NUnit 3.0, we would like to create a similarly rich set of attributes for specifying how TestFixture instances should be created. This spec will outline the features we plan to support. Type names are placeholders and may be changed as the work proceeds.

##### TestFixtureSourceAttribute

This will work similarly to `TestCaseSourceAttribute` and will supply the constructor arguments for the fixture. The two types will probably be unified under a common base.

##### TestFixtureData

This will work similarly to `TestCaseData` with the addition of a number of features that are needed for fixtures. In particular, the Type will need to support a TypeArgs property and possibly a separate set of arguments for use with the OneTimeSetUp method.

##### ValuesAttribute
##### ValueSourceAttribute
##### RangeAttribute
##### RandomAttribute
These attributes may be used on a TestFixture constructor with arguments. Their effect will be similar to use on method arguments, causing the fixture to be constructed a number of times.

##### CombinatorialAttribute
##### PairwiseAttribute
##### SequentialAttribute

These attributes will need to be modified to work on test fixtures as well as methods. They specify how individual argument values are to be combined to for a set of arguments for constructing the fixture.