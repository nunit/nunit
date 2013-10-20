module FsharpAsyncTestFixture

open NUnit.Framework

let [<TestCase(ExpectedResult = 1)>] ``async int success`` () =
  async { return 1 }

let [<Test>] ``async unit success`` () =
  async { return () }

let rec [<Test>] ``async tailcall success`` left =
  async {
    if left = 0 then return ()
    else return! ``async tailcall success`` (left - 1) }

let [<Test>] ``async single true assert success`` () =
  async {
    Assert.That(true, "should be obviously true")
    return () }

let [<Test>] ``async multiple asserts success`` () =
  async {
    Assert.That("a" = "b", Is.False, "because they are not the same string")
    Assert.That(1 = 1, "because we're in the default abelian group")
    return () }

let [<Test>] ``calling into async code success`` () =
  let a () = async { return () }
  let b () = async { return 1 }
  async {
    do! a ()
    return! b () }

// should fail tests:

let [<Test>] ``when throwing exception fails`` () =
  async { failwith "this was unexpected" }

let [<Test>] ``async single false assert failure`` () =
  async {
    Assert.That(false, "should be true, but is not")
    return () }