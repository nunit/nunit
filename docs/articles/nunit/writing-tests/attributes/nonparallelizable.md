This attribute is used to indicate that the test on which it appears may not be run in parallel with any other tests. The attribute takes no arguments and may be used at the assembly, class or method level.

When used at the assembly level, its only effect is that execution begins on the non-parallel queue. Test suites, fixtures and test cases will continue to run on the same thread unless a fixture or method is marked with the [Parallelizable Attribute](Parallelizable.md).

When used on a test fixture or method, that test will be queued on the non-parallel queue and will not run while other tests marked as Parallelizable are being run.

#### Platform Support

Parallel execution is not supported by all builds of the NUnit Framework, although the attributes are recognized without error in order to allow use in projects that build against multiple targets. Currently, only the .NET Standard 1.6 build does not support parallelization.

#### See also...
 * [Parallelizable Attribute](Parallelizable.md)
 * [LevelOfParallelism Attribute](LevelOfParallelism.md)

