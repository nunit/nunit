**RepeatAttribute** is used on a test method to specify that it should be
executed multiple times. If any repetition fails, the remaining ones are
not run and a failure is reported.

#### Notes:

1. If RepeatAttribute is used on a parameterized method, each individual
   test case created for that method is repeated.

2. It is not currently possible to use RepeatAttribute on a TestFixture
   or any higher level suite. Only test cases may be repeated.
   
