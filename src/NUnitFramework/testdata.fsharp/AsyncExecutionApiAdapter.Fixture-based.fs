// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

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
