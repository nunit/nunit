#pragma once

using namespace NUnit::Framework;

[TestFixture]
public ref class TestClass
{
public:
    TestClass()
    {
        _theMeaningOfLife = 42;
    }

    [Test]
    void TestMethod();

private:
    int _theMeaningOfLife;
};

