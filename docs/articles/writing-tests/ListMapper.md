Unlike Constraint classes, `ListMapper` is used to modify the actual
value argument to `Assert.That()`. It transforms the actual value, which
must be a collection, creating a new collection to be tested against the
supplied constraint. Currently, `ListMapper` supports one transformation: creating
a collection of property values.

Normally, `ListMapper` will be used through the `List.Map()` syntax helper. The following example
shows two forms of the same assert.

```C#
string[] strings = new string[] { "a", "ab", "abc" };
int[] lengths = new int[] { 1, 2, 3 };

Assert.That(List.Map(strings).Property("Length"), 
       Is.EqualTo(lengths));
	   
Assert.That(new ListMapper(strings).Property("Length"),
       Is.EqualTo(lengths));
```

