// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

module NUnit.TestData.FSharp.AsyncExecutionApiAdapter

open NUnit.Framework

type TaskReturningTestMethodFixture(asyncUserCode: AsyncTestDelegate) =
    [<Test>]
    member this.TestMethod() = async {
        do! Async.AwaitTask(asyncUserCode.Invoke())
    }

type TaskReturningSetUpFixture(asyncUserCode: AsyncTestDelegate) =
    [<SetUp>]
    member this.SetUp() = async {
        do! Async.AwaitTask(asyncUserCode.Invoke())
    }

    [<Test>]
    member this.DummyTest() = ()

type TaskReturningTearDownFixture(asyncUserCode: AsyncTestDelegate) =
    [<TearDown>]
    member this.TearDown() = async {
        do! Async.AwaitTask(asyncUserCode.Invoke())
    }

    [<Test>]
    member this.DummyTest() = ()

type TaskReturningOneTimeSetUpFixture(asyncUserCode: AsyncTestDelegate) =
    [<OneTimeSetUp>]
    member this.OneTimeSetUp() = async {
        do! Async.AwaitTask(asyncUserCode.Invoke())
    }

    [<Test>]
    member this.DummyTest() = ()

type TaskReturningOneTimeTearDownFixture(asyncUserCode: AsyncTestDelegate) =
    [<OneTimeTearDown>]
    member this.OneTimeTearDown() = async {
        do! Async.AwaitTask(asyncUserCode.Invoke())
    }

    [<Test>]
    member this.DummyTest() = ()
