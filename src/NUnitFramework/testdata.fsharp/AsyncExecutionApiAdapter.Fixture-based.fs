// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

module NUnit.TestData.FSharp.AsyncExecutionApiAdapter

open System
open System.Threading.Tasks
open NUnit.Framework

type TaskReturningTestMethodFixture(asyncUserCode: Func<Task>) =
    [<Test>]
    member this.TestMethod() = async {
        do! Async.AwaitTask(asyncUserCode.Invoke())
    }

type TaskReturningSetUpFixture(asyncUserCode: Func<Task>) =
    [<SetUp>]
    member this.SetUp() = async {
        do! Async.AwaitTask(asyncUserCode.Invoke())
    }

    [<Test>]
    member this.DummyTest() = ()

type TaskReturningTearDownFixture(asyncUserCode: Func<Task>) =
    [<TearDown>]
    member this.TearDown() = async {
        do! Async.AwaitTask(asyncUserCode.Invoke())
    }

    [<Test>]
    member this.DummyTest() = ()

type TaskReturningOneTimeSetUpFixture(asyncUserCode: Func<Task>) =
    [<OneTimeSetUp>]
    member this.OneTimeSetUp() = async {
        do! Async.AwaitTask(asyncUserCode.Invoke())
    }

    [<Test>]
    member this.DummyTest() = ()

type TaskReturningOneTimeTearDownFixture(asyncUserCode: Func<Task>) =
    [<OneTimeTearDown>]
    member this.OneTimeTearDown() = async {
        do! Async.AwaitTask(asyncUserCode.Invoke())
    }

    [<Test>]
    member this.DummyTest() = ()
