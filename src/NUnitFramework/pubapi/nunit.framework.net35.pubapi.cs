// Name:       nunit.framework
// Public key: ACQAAASAAACUAAAABgIAAAAkAABSU0ExAAQAAAEAAQAx7qNwsZhL+m0ep2DhymBlzuQaGiecojSTP+l3oJYiLA4U+eWhfVaJMFxtfxIGqFpTxIygEAgHmdbu72HJir0Ydngn3AXa6mtvvS6GhBDZvuXpcqAE3daS3sj6QEukWR6EeozzXeIcLTcjvI13Wma1lK3rlnU3cp/ipEa1SM1Xpg==

namespace NUnit
{
    public static class FrameworkPackageSettings
    {
        public const string DebugTests = "DebugTests";

        public const string DefaultTestNamePattern = "DefaultTestNamePattern";

        public const string DefaultTimeout = "DefaultTimeout";

        public const string InternalTraceLevel = "InternalTraceLevel";

        public const string InternalTraceWriter = "InternalTraceWriter";

        public const string LOAD = "LOAD";

        public const string NumberOfTestWorkers = "NumberOfTestWorkers";

        public const string PauseBeforeRun = "PauseBeforeRun";

        public const string RandomSeed = "RandomSeed";

        public const string RunOnMainThread = "RunOnMainThread";

        public const string StopOnError = "StopOnError";

        public const string SynchronousEvents = "SynchronousEvents";

        public const string TestParameters = "TestParameters";

        public const string TestParametersDictionary = "TestParametersDictionary";

        public const string WorkDirectory = "WorkDirectory";
    }
}
namespace NUnit.Compatibility
{
    public static class AdditionalTypeExtensions
    {
        public static bool IsCastableFrom(this System.Type to, System.Type from);

        public static bool ParametersMatch(this System.Reflection.ParameterInfo[] pinfos, System.Type[] ptypes);
    }

    public static class AssemblyExtensions
    {
        public static T GetCustomAttribute<T>(this System.Reflection.Assembly assembly) where T : System.Attribute;
    }

    public static class AttributeHelper
    {
        public static System.Attribute[] GetCustomAttributes(object actual, System.Type attributeType, bool inherit);
    }

    public class LongLivedMarshalByRefObject : System.MarshalByRefObject
    {
        public LongLivedMarshalByRefObject();

        [System.Security.SecurityCritical]
        public override object InitializeLifetimeService();
    }

    public static class MemberInfoExtensions
    {
        public static System.Collections.Generic.IEnumerable<T> GetAttributes<T>(this System.Reflection.MemberInfo info, bool inherit) where T : class;

        public static System.Collections.Generic.IEnumerable<T> GetAttributes<T>(this System.Reflection.ParameterInfo info, bool inherit) where T : class;

        public static System.Collections.Generic.IEnumerable<T> GetAttributes<T>(this System.Reflection.Assembly asm) where T : class;
    }

    public static class MethodInfoExtensions
    {
        public static System.Delegate CreateDelegate(this System.Reflection.MethodInfo method, System.Type type);
    }

    public class NUnitNullType
    {
        public NUnitNullType();
    }

    public static class TypeExtensions
    {
        public static System.Type GetTypeInfo(this System.Type type);
    }
}
namespace NUnit.Framework
{
    [System.Flags]
    public enum ActionTargets : int
    {
        Default = 0,
        Test = 1,
        Suite = 2
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ApartmentAttribute : PropertyAttribute
    {
        public ApartmentAttribute(System.Threading.ApartmentState apartmentState);
    }

    public class Assert
    {
        public static void AreEqual(double expected, double actual, double delta, string message, params object[] args);

        public static void AreEqual(double expected, double actual, double delta);

        public static void AreEqual(double expected, double? actual, double delta, string message, params object[] args);

        public static void AreEqual(double expected, double? actual, double delta);

        public static void AreEqual(object expected, object actual, string message, params object[] args);

        public static void AreEqual(object expected, object actual);

        public static void AreNotEqual(object expected, object actual, string message, params object[] args);

        public static void AreNotEqual(object expected, object actual);

        public static void AreNotSame(object expected, object actual, string message, params object[] args);

        public static void AreNotSame(object expected, object actual);

        public static void AreSame(object expected, object actual, string message, params object[] args);

        public static void AreSame(object expected, object actual);

        protected static void AssertDoublesAreEqual(double expected, double actual, double delta, string message, object[] args);

        public static void ByVal(object actual, NUnit.Framework.Constraints.IResolveConstraint expression);

        public static void ByVal(object actual, NUnit.Framework.Constraints.IResolveConstraint expression, string message, params object[] args);

        public static System.Exception Catch(TestDelegate code, string message, params object[] args);

        public static System.Exception Catch(TestDelegate code);

        public static System.Exception Catch(System.Type expectedExceptionType, TestDelegate code, string message, params object[] args);

        public static System.Exception Catch(System.Type expectedExceptionType, TestDelegate code);

        public static TActual Catch<TActual>(TestDelegate code, string message, params object[] args) where TActual : System.Exception;

        public static TActual Catch<TActual>(TestDelegate code) where TActual : System.Exception;

        public static void Contains(object expected, System.Collections.ICollection actual, string message, params object[] args);

        public static void Contains(object expected, System.Collections.ICollection actual);

        public static void DoesNotThrow(TestDelegate code, string message, params object[] args);

        public static void DoesNotThrow(TestDelegate code);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool Equals(object a, object b);

        public static void Fail(string message, params object[] args);

        public static void Fail(string message);

        public static void Fail();

        public static void False(bool? condition, string message, params object[] args);

        public static void False(bool condition, string message, params object[] args);

        public static void False(bool? condition);

        public static void False(bool condition);

        public static void Greater(int arg1, int arg2, string message, params object[] args);

        public static void Greater(int arg1, int arg2);

        [System.CLSCompliant(false)]
        public static void Greater(uint arg1, uint arg2, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void Greater(uint arg1, uint arg2);

        public static void Greater(long arg1, long arg2, string message, params object[] args);

        public static void Greater(long arg1, long arg2);

        [System.CLSCompliant(false)]
        public static void Greater(ulong arg1, ulong arg2, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void Greater(ulong arg1, ulong arg2);

        public static void Greater(decimal arg1, decimal arg2, string message, params object[] args);

        public static void Greater(decimal arg1, decimal arg2);

        public static void Greater(double arg1, double arg2, string message, params object[] args);

        public static void Greater(double arg1, double arg2);

        public static void Greater(float arg1, float arg2, string message, params object[] args);

        public static void Greater(float arg1, float arg2);

        public static void Greater(System.IComparable arg1, System.IComparable arg2, string message, params object[] args);

        public static void Greater(System.IComparable arg1, System.IComparable arg2);

        public static void GreaterOrEqual(int arg1, int arg2, string message, params object[] args);

        public static void GreaterOrEqual(int arg1, int arg2);

        [System.CLSCompliant(false)]
        public static void GreaterOrEqual(uint arg1, uint arg2, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void GreaterOrEqual(uint arg1, uint arg2);

        public static void GreaterOrEqual(long arg1, long arg2, string message, params object[] args);

        public static void GreaterOrEqual(long arg1, long arg2);

        [System.CLSCompliant(false)]
        public static void GreaterOrEqual(ulong arg1, ulong arg2, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void GreaterOrEqual(ulong arg1, ulong arg2);

        public static void GreaterOrEqual(decimal arg1, decimal arg2, string message, params object[] args);

        public static void GreaterOrEqual(decimal arg1, decimal arg2);

        public static void GreaterOrEqual(double arg1, double arg2, string message, params object[] args);

        public static void GreaterOrEqual(double arg1, double arg2);

        public static void GreaterOrEqual(float arg1, float arg2, string message, params object[] args);

        public static void GreaterOrEqual(float arg1, float arg2);

        public static void GreaterOrEqual(System.IComparable arg1, System.IComparable arg2, string message, params object[] args);

        public static void GreaterOrEqual(System.IComparable arg1, System.IComparable arg2);

        public static void Ignore(string message, params object[] args);

        public static void Ignore(string message);

        public static void Ignore();

        public static void Inconclusive(string message, params object[] args);

        public static void Inconclusive(string message);

        public static void Inconclusive();

        public static void IsAssignableFrom(System.Type expected, object actual, string message, params object[] args);

        public static void IsAssignableFrom(System.Type expected, object actual);

        public static void IsAssignableFrom<TExpected>(object actual, string message, params object[] args);

        public static void IsAssignableFrom<TExpected>(object actual);

        public static void IsEmpty(string aString, string message, params object[] args);

        public static void IsEmpty(string aString);

        public static void IsEmpty(System.Collections.IEnumerable collection, string message, params object[] args);

        public static void IsEmpty(System.Collections.IEnumerable collection);

        public static void IsFalse(bool? condition, string message, params object[] args);

        public static void IsFalse(bool condition, string message, params object[] args);

        public static void IsFalse(bool? condition);

        public static void IsFalse(bool condition);

        public static void IsInstanceOf(System.Type expected, object actual, string message, params object[] args);

        public static void IsInstanceOf(System.Type expected, object actual);

        public static void IsInstanceOf<TExpected>(object actual, string message, params object[] args);

        public static void IsInstanceOf<TExpected>(object actual);

        public static void IsNaN(double aDouble, string message, params object[] args);

        public static void IsNaN(double aDouble);

        public static void IsNaN(double? aDouble, string message, params object[] args);

        public static void IsNaN(double? aDouble);

        public static void IsNotAssignableFrom(System.Type expected, object actual, string message, params object[] args);

        public static void IsNotAssignableFrom(System.Type expected, object actual);

        public static void IsNotAssignableFrom<TExpected>(object actual, string message, params object[] args);

        public static void IsNotAssignableFrom<TExpected>(object actual);

        public static void IsNotEmpty(string aString, string message, params object[] args);

        public static void IsNotEmpty(string aString);

        public static void IsNotEmpty(System.Collections.IEnumerable collection, string message, params object[] args);

        public static void IsNotEmpty(System.Collections.IEnumerable collection);

        public static void IsNotInstanceOf(System.Type expected, object actual, string message, params object[] args);

        public static void IsNotInstanceOf(System.Type expected, object actual);

        public static void IsNotInstanceOf<TExpected>(object actual, string message, params object[] args);

        public static void IsNotInstanceOf<TExpected>(object actual);

        public static void IsNotNull(object anObject, string message, params object[] args);

        public static void IsNotNull(object anObject);

        public static void IsNull(object anObject, string message, params object[] args);

        public static void IsNull(object anObject);

        public static void IsTrue(bool? condition, string message, params object[] args);

        public static void IsTrue(bool condition, string message, params object[] args);

        public static void IsTrue(bool? condition);

        public static void IsTrue(bool condition);

        public static void Less(int arg1, int arg2, string message, params object[] args);

        public static void Less(int arg1, int arg2);

        [System.CLSCompliant(false)]
        public static void Less(uint arg1, uint arg2, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void Less(uint arg1, uint arg2);

        public static void Less(long arg1, long arg2, string message, params object[] args);

        public static void Less(long arg1, long arg2);

        [System.CLSCompliant(false)]
        public static void Less(ulong arg1, ulong arg2, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void Less(ulong arg1, ulong arg2);

        public static void Less(decimal arg1, decimal arg2, string message, params object[] args);

        public static void Less(decimal arg1, decimal arg2);

        public static void Less(double arg1, double arg2, string message, params object[] args);

        public static void Less(double arg1, double arg2);

        public static void Less(float arg1, float arg2, string message, params object[] args);

        public static void Less(float arg1, float arg2);

        public static void Less(System.IComparable arg1, System.IComparable arg2, string message, params object[] args);

        public static void Less(System.IComparable arg1, System.IComparable arg2);

        public static void LessOrEqual(int arg1, int arg2, string message, params object[] args);

        public static void LessOrEqual(int arg1, int arg2);

        [System.CLSCompliant(false)]
        public static void LessOrEqual(uint arg1, uint arg2, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void LessOrEqual(uint arg1, uint arg2);

        public static void LessOrEqual(long arg1, long arg2, string message, params object[] args);

        public static void LessOrEqual(long arg1, long arg2);

        [System.CLSCompliant(false)]
        public static void LessOrEqual(ulong arg1, ulong arg2, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void LessOrEqual(ulong arg1, ulong arg2);

        public static void LessOrEqual(decimal arg1, decimal arg2, string message, params object[] args);

        public static void LessOrEqual(decimal arg1, decimal arg2);

        public static void LessOrEqual(double arg1, double arg2, string message, params object[] args);

        public static void LessOrEqual(double arg1, double arg2);

        public static void LessOrEqual(float arg1, float arg2, string message, params object[] args);

        public static void LessOrEqual(float arg1, float arg2);

        public static void LessOrEqual(System.IComparable arg1, System.IComparable arg2, string message, params object[] args);

        public static void LessOrEqual(System.IComparable arg1, System.IComparable arg2);

        public static void Multiple(TestDelegate testDelegate);

        public static void Negative(int actual);

        public static void Negative(int actual, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void Negative(uint actual);

        [System.CLSCompliant(false)]
        public static void Negative(uint actual, string message, params object[] args);

        public static void Negative(long actual);

        public static void Negative(long actual, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void Negative(ulong actual);

        [System.CLSCompliant(false)]
        public static void Negative(ulong actual, string message, params object[] args);

        public static void Negative(decimal actual);

        public static void Negative(decimal actual, string message, params object[] args);

        public static void Negative(double actual);

        public static void Negative(double actual, string message, params object[] args);

        public static void Negative(float actual);

        public static void Negative(float actual, string message, params object[] args);

        public static void NotNull(object anObject, string message, params object[] args);

        public static void NotNull(object anObject);

        public static void NotZero(int actual);

        public static void NotZero(int actual, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void NotZero(uint actual);

        [System.CLSCompliant(false)]
        public static void NotZero(uint actual, string message, params object[] args);

        public static void NotZero(long actual);

        public static void NotZero(long actual, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void NotZero(ulong actual);

        [System.CLSCompliant(false)]
        public static void NotZero(ulong actual, string message, params object[] args);

        public static void NotZero(decimal actual);

        public static void NotZero(decimal actual, string message, params object[] args);

        public static void NotZero(double actual);

        public static void NotZero(double actual, string message, params object[] args);

        public static void NotZero(float actual);

        public static void NotZero(float actual, string message, params object[] args);

        public static void Null(object anObject, string message, params object[] args);

        public static void Null(object anObject);

        public static void Pass(string message, params object[] args);

        public static void Pass(string message);

        public static void Pass();

        public static void Positive(int actual);

        public static void Positive(int actual, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void Positive(uint actual);

        [System.CLSCompliant(false)]
        public static void Positive(uint actual, string message, params object[] args);

        public static void Positive(long actual);

        public static void Positive(long actual, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void Positive(ulong actual);

        [System.CLSCompliant(false)]
        public static void Positive(ulong actual, string message, params object[] args);

        public static void Positive(decimal actual);

        public static void Positive(decimal actual, string message, params object[] args);

        public static void Positive(double actual);

        public static void Positive(double actual, string message, params object[] args);

        public static void Positive(float actual);

        public static void Positive(float actual, string message, params object[] args);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void ReferenceEquals(object a, object b);

        public static void That(bool condition, string message, params object[] args);

        public static void That(bool condition);

        public static void That(bool condition, System.Func<string> getExceptionMessage);

        public static void That(System.Func<bool> condition, string message, params object[] args);

        public static void That(System.Func<bool> condition);

        public static void That(System.Func<bool> condition, System.Func<string> getExceptionMessage);

        public static void That<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr);

        public static void That<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr, string message, params object[] args);

        public static void That<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr, System.Func<string> getExceptionMessage);

        public static void That(TestDelegate code, NUnit.Framework.Constraints.IResolveConstraint constraint);

        public static void That(TestDelegate code, NUnit.Framework.Constraints.IResolveConstraint constraint, string message, params object[] args);

        public static void That(TestDelegate code, NUnit.Framework.Constraints.IResolveConstraint constraint, System.Func<string> getExceptionMessage);

        public static void That<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression);

        public static void That<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression, string message, params object[] args);

        public static void That<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression, System.Func<string> getExceptionMessage);

        public static System.Exception Throws(NUnit.Framework.Constraints.IResolveConstraint expression, TestDelegate code, string message, params object[] args);

        public static System.Exception Throws(NUnit.Framework.Constraints.IResolveConstraint expression, TestDelegate code);

        public static System.Exception Throws(System.Type expectedExceptionType, TestDelegate code, string message, params object[] args);

        public static System.Exception Throws(System.Type expectedExceptionType, TestDelegate code);

        public static TActual Throws<TActual>(TestDelegate code, string message, params object[] args) where TActual : System.Exception;

        public static TActual Throws<TActual>(TestDelegate code) where TActual : System.Exception;

        public static void True(bool? condition, string message, params object[] args);

        public static void True(bool condition, string message, params object[] args);

        public static void True(bool? condition);

        public static void True(bool condition);

        public static void Warn(string message, params object[] args);

        public static void Warn(string message);

        public static void Zero(int actual);

        public static void Zero(int actual, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void Zero(uint actual);

        [System.CLSCompliant(false)]
        public static void Zero(uint actual, string message, params object[] args);

        public static void Zero(long actual);

        public static void Zero(long actual, string message, params object[] args);

        [System.CLSCompliant(false)]
        public static void Zero(ulong actual);

        [System.CLSCompliant(false)]
        public static void Zero(ulong actual, string message, params object[] args);

        public static void Zero(decimal actual);

        public static void Zero(decimal actual, string message, params object[] args);

        public static void Zero(double actual);

        public static void Zero(double actual, string message, params object[] args);

        public static void Zero(float actual);

        public static void Zero(float actual, string message, params object[] args);

        protected Assert();
    }

    public class AssertionException : ResultStateException
    {
        public override NUnit.Framework.Interfaces.ResultState ResultState { get; }

        public AssertionException(string message);

        public AssertionException(string message, System.Exception inner);

        protected AssertionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
    }

    [System.Obsolete("The AssertionHelper class will be removed in a coming release. Consider using the NUnit.StaticExpect NuGet package as a replacement.")]
    public class AssertionHelper
    {
        public NUnit.Framework.Constraints.ConstraintExpression All { get; }

        public NUnit.Framework.Constraints.BinarySerializableConstraint BinarySerializable { get; }

        public NUnit.Framework.Constraints.ResolvableConstraintExpression Count { get; }

        public NUnit.Framework.Constraints.EmptyConstraint Empty { get; }

        public NUnit.Framework.Constraints.FalseConstraint False { get; }

        public NUnit.Framework.Constraints.ResolvableConstraintExpression InnerException { get; }

        public NUnit.Framework.Constraints.ResolvableConstraintExpression Length { get; }

        public NUnit.Framework.Constraints.ResolvableConstraintExpression Message { get; }

        public NUnit.Framework.Constraints.NaNConstraint NaN { get; }

        public NUnit.Framework.Constraints.LessThanConstraint Negative { get; }

        public NUnit.Framework.Constraints.ConstraintExpression No { get; }

        public NUnit.Framework.Constraints.ConstraintExpression None { get; }

        public NUnit.Framework.Constraints.ConstraintExpression Not { get; }

        public NUnit.Framework.Constraints.NullConstraint Null { get; }

        public NUnit.Framework.Constraints.CollectionOrderedConstraint Ordered { get; }

        public NUnit.Framework.Constraints.GreaterThanConstraint Positive { get; }

        public NUnit.Framework.Constraints.ConstraintExpression Some { get; }

        public NUnit.Framework.Constraints.TrueConstraint True { get; }

        public NUnit.Framework.Constraints.UniqueItemsConstraint Unique { get; }

        public NUnit.Framework.Constraints.XmlSerializableConstraint XmlSerializable { get; }

        public NUnit.Framework.Constraints.EqualConstraint Zero { get; }

        public static NUnit.Framework.Constraints.ItemsConstraintExpression Exactly(int expectedCount);

        public static void Expect<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression);

        public static void Expect<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression, string message, params object[] args);

        public AssertionHelper();

        public NUnit.Framework.Constraints.AssignableFromConstraint AssignableFrom(System.Type expectedType);

        public NUnit.Framework.Constraints.AssignableFromConstraint AssignableFrom<TExpected>();

        public NUnit.Framework.Constraints.AssignableToConstraint AssignableTo(System.Type expectedType);

        public NUnit.Framework.Constraints.AssignableToConstraint AssignableTo<TExpected>();

        public NUnit.Framework.Constraints.GreaterThanOrEqualConstraint AtLeast(object expected);

        public NUnit.Framework.Constraints.LessThanOrEqualConstraint AtMost(object expected);

        public NUnit.Framework.Constraints.ResolvableConstraintExpression Attribute(System.Type expectedType);

        public NUnit.Framework.Constraints.ResolvableConstraintExpression Attribute<TExpected>();

        public NUnit.Framework.Constraints.SomeItemsConstraint Contains(object expected);

        public NUnit.Framework.Constraints.ContainsConstraint Contains(string expected);

        [System.Obsolete("Deprecated, use Contains")]
        public NUnit.Framework.Constraints.SubstringConstraint ContainsSubstring(string expected);

        [System.Obsolete("Deprecated, use Does.Not.Contain")]
        public NUnit.Framework.Constraints.SubstringConstraint DoesNotContain(string expected);

        [System.Obsolete("Deprecated, use Does.Not.EndWith")]
        public NUnit.Framework.Constraints.EndsWithConstraint DoesNotEndWith(string expected);

        [System.Obsolete("Deprecated, use Does.Not.Match")]
        public NUnit.Framework.Constraints.RegexConstraint DoesNotMatch(string pattern);

        [System.Obsolete("Deprecated, use Does.Not.StartWith")]
        public NUnit.Framework.Constraints.StartsWithConstraint DoesNotStartWith(string expected);

        public NUnit.Framework.Constraints.EndsWithConstraint EndsWith(string expected);

        public NUnit.Framework.Constraints.EndsWithConstraint EndWith(string expected);

        public NUnit.Framework.Constraints.EqualConstraint EqualTo(object expected);

        public NUnit.Framework.Constraints.CollectionEquivalentConstraint EquivalentTo(System.Collections.IEnumerable expected);

        public void Expect(bool condition, string message, params object[] args);

        public void Expect(bool condition);

        public void Expect<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr);

        public void Expect<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr, string message, params object[] args);

        public void Expect(TestDelegate code, NUnit.Framework.Constraints.IResolveConstraint constraint);

        public NUnit.Framework.Constraints.GreaterThanConstraint GreaterThan(object expected);

        public NUnit.Framework.Constraints.GreaterThanOrEqualConstraint GreaterThanOrEqualTo(object expected);

        public NUnit.Framework.Constraints.RangeConstraint InRange(object from, object to);

        public NUnit.Framework.Constraints.InstanceOfTypeConstraint InstanceOf(System.Type expectedType);

        public NUnit.Framework.Constraints.InstanceOfTypeConstraint InstanceOf<TExpected>();

        public NUnit.Framework.Constraints.LessThanConstraint LessThan(object expected);

        public NUnit.Framework.Constraints.LessThanOrEqualConstraint LessThanOrEqualTo(object expected);

        public ListMapper Map(System.Collections.ICollection original);

        public NUnit.Framework.Constraints.RegexConstraint Match(string pattern);

        public NUnit.Framework.Constraints.RegexConstraint Matches(string pattern);

        public NUnit.Framework.Constraints.SomeItemsConstraint Member(object expected);

        public NUnit.Framework.Constraints.ResolvableConstraintExpression Property(string name);

        public NUnit.Framework.Constraints.SameAsConstraint SameAs(object expected);

        public NUnit.Framework.Constraints.SamePathConstraint SamePath(string expected);

        public NUnit.Framework.Constraints.SamePathOrUnderConstraint SamePathOrUnder(string expected);

        public NUnit.Framework.Constraints.StartsWithConstraint StartsWith(string expected);

        public NUnit.Framework.Constraints.StartsWithConstraint StartWith(string expected);

        [System.Obsolete("Deprecated, use Contains")]
        public NUnit.Framework.Constraints.SubstringConstraint StringContaining(string expected);

        [System.Obsolete("Deprecated, use Does.EndWith or EndsWith")]
        public NUnit.Framework.Constraints.EndsWithConstraint StringEnding(string expected);

        [System.Obsolete("Deprecated, use Does.Match or Matches")]
        public NUnit.Framework.Constraints.RegexConstraint StringMatching(string pattern);

        [System.Obsolete("Deprecated, use Does.StartWith or StartsWith")]
        public NUnit.Framework.Constraints.StartsWithConstraint StringStarting(string expected);

        public NUnit.Framework.Constraints.SubPathConstraint SubPathOf(string expected);

        public NUnit.Framework.Constraints.CollectionSubsetConstraint SubsetOf(System.Collections.IEnumerable expected);

        public NUnit.Framework.Constraints.CollectionSupersetConstraint SupersetOf(System.Collections.IEnumerable expected);

        public NUnit.Framework.Constraints.ExactTypeConstraint TypeOf(System.Type expectedType);

        public NUnit.Framework.Constraints.ExactTypeConstraint TypeOf<TExpected>();
    }

    public class Assume
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool Equals(object a, object b);

        public static void ReferenceEquals(object a, object b);

        public static void That<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr);

        public static void That<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr, string message, params object[] args);

        public static void That<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr, System.Func<string> getExceptionMessage);

        public static void That(bool condition, string message, params object[] args);

        public static void That(bool condition);

        public static void That(bool condition, System.Func<string> getExceptionMessage);

        public static void That(System.Func<bool> condition, string message, params object[] args);

        public static void That(System.Func<bool> condition);

        public static void That(System.Func<bool> condition, System.Func<string> getExceptionMessage);

        public static void That(TestDelegate code, NUnit.Framework.Constraints.IResolveConstraint constraint);

        public static void That<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression);

        public static void That<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression, string message, params object[] args);

        public static void That<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression, System.Func<string> getExceptionMessage);

        public Assume();
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class AuthorAttribute : PropertyAttribute
    {
        public AuthorAttribute(string name);

        public AuthorAttribute(string name, string email);
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CategoryAttribute : NUnitAttribute, NUnit.Framework.Interfaces.IApplyToTest
    {
        protected string categoryName;

        public string Name { get; }

        public CategoryAttribute(string name);

        protected CategoryAttribute();

        public void ApplyToTest(NUnit.Framework.Internal.Test test);
    }

    public class CollectionAssert
    {
        public static void AllItemsAreInstancesOfType(System.Collections.IEnumerable collection, System.Type expectedType);

        public static void AllItemsAreInstancesOfType(System.Collections.IEnumerable collection, System.Type expectedType, string message, params object[] args);

        public static void AllItemsAreNotNull(System.Collections.IEnumerable collection);

        public static void AllItemsAreNotNull(System.Collections.IEnumerable collection, string message, params object[] args);

        public static void AllItemsAreUnique(System.Collections.IEnumerable collection);

        public static void AllItemsAreUnique(System.Collections.IEnumerable collection, string message, params object[] args);

        public static void AreEqual(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual);

        public static void AreEqual(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual, System.Collections.IComparer comparer);

        public static void AreEqual(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual, string message, params object[] args);

        public static void AreEqual(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual, System.Collections.IComparer comparer, string message, params object[] args);

        public static void AreEquivalent(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual);

        public static void AreEquivalent(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual, string message, params object[] args);

        public static void AreNotEqual(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual);

        public static void AreNotEqual(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual, System.Collections.IComparer comparer);

        public static void AreNotEqual(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual, string message, params object[] args);

        public static void AreNotEqual(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual, System.Collections.IComparer comparer, string message, params object[] args);

        public static void AreNotEquivalent(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual);

        public static void AreNotEquivalent(System.Collections.IEnumerable expected, System.Collections.IEnumerable actual, string message, params object[] args);

        public static void Contains(System.Collections.IEnumerable collection, object actual);

        public static void Contains(System.Collections.IEnumerable collection, object actual, string message, params object[] args);

        public static void DoesNotContain(System.Collections.IEnumerable collection, object actual);

        public static void DoesNotContain(System.Collections.IEnumerable collection, object actual, string message, params object[] args);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool Equals(object a, object b);

        public static void IsEmpty(System.Collections.IEnumerable collection, string message, params object[] args);

        public static void IsEmpty(System.Collections.IEnumerable collection);

        public static void IsNotEmpty(System.Collections.IEnumerable collection, string message, params object[] args);

        public static void IsNotEmpty(System.Collections.IEnumerable collection);

        public static void IsNotSubsetOf(System.Collections.IEnumerable subset, System.Collections.IEnumerable superset);

        public static void IsNotSubsetOf(System.Collections.IEnumerable subset, System.Collections.IEnumerable superset, string message, params object[] args);

        public static void IsNotSupersetOf(System.Collections.IEnumerable superset, System.Collections.IEnumerable subset);

        public static void IsNotSupersetOf(System.Collections.IEnumerable superset, System.Collections.IEnumerable subset, string message, params object[] args);

        public static void IsOrdered(System.Collections.IEnumerable collection, string message, params object[] args);

        public static void IsOrdered(System.Collections.IEnumerable collection);

        public static void IsOrdered(System.Collections.IEnumerable collection, System.Collections.IComparer comparer, string message, params object[] args);

        public static void IsOrdered(System.Collections.IEnumerable collection, System.Collections.IComparer comparer);

        public static void IsSubsetOf(System.Collections.IEnumerable subset, System.Collections.IEnumerable superset);

        public static void IsSubsetOf(System.Collections.IEnumerable subset, System.Collections.IEnumerable superset, string message, params object[] args);

        public static void IsSupersetOf(System.Collections.IEnumerable superset, System.Collections.IEnumerable subset);

        public static void IsSupersetOf(System.Collections.IEnumerable superset, System.Collections.IEnumerable subset, string message, params object[] args);

        public static void ReferenceEquals(object a, object b);

        public CollectionAssert();
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CombinatorialAttribute : CombiningStrategyAttribute
    {
        public CombinatorialAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class CombiningStrategyAttribute : NUnitAttribute, NUnit.Framework.Interfaces.ITestBuilder, NUnit.Framework.Interfaces.IApplyToTest
    {
        protected CombiningStrategyAttribute(NUnit.Framework.Interfaces.ICombiningStrategy strategy, NUnit.Framework.Interfaces.IParameterDataProvider provider);

        protected CombiningStrategyAttribute(object strategy, object provider);

        public void ApplyToTest(NUnit.Framework.Internal.Test test);

        public System.Collections.Generic.IEnumerable<NUnit.Framework.Internal.TestMethod> BuildFrom(NUnit.Framework.Interfaces.IMethodInfo method, NUnit.Framework.Internal.Test suite);
    }

    public class Contains
    {
        public static NUnit.Framework.Constraints.SomeItemsConstraint Item(object expected);

        public static NUnit.Framework.Constraints.DictionaryContainsKeyConstraint Key(object expected);

        public static NUnit.Framework.Constraints.SubstringConstraint Substring(string expected);

        public static NUnit.Framework.Constraints.DictionaryContainsValueConstraint Value(object expected);

        public Contains();
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CultureAttribute : IncludeExcludeAttribute, NUnit.Framework.Interfaces.IApplyToTest
    {
        public CultureAttribute();

        public CultureAttribute(string cultures);

        public void ApplyToTest(NUnit.Framework.Internal.Test test);

        public bool IsCultureSupported(string culture);

        public bool IsCultureSupported(string[] cultures);
    }

    public abstract class DataAttribute : NUnitAttribute
    {
        public DataAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DatapointAttribute : NUnitAttribute
    {
        public DatapointAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DatapointsAttribute : DatapointSourceAttribute
    {
        public DatapointsAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DatapointSourceAttribute : NUnitAttribute
    {
        public DatapointSourceAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DefaultFloatingPointToleranceAttribute : NUnitAttribute, NUnit.Framework.Interfaces.IApplyToContext
    {
        public DefaultFloatingPointToleranceAttribute(double amount);

        public void ApplyToContext(NUnit.Framework.Internal.TestExecutionContext context);
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class DescriptionAttribute : PropertyAttribute
    {
        public DescriptionAttribute(string description);
    }

    public static class DirectoryAssert
    {
        public static void AreEqual(System.IO.DirectoryInfo expected, System.IO.DirectoryInfo actual, string message, params object[] args);

        public static void AreEqual(System.IO.DirectoryInfo expected, System.IO.DirectoryInfo actual);

        public static void AreNotEqual(System.IO.DirectoryInfo expected, System.IO.DirectoryInfo actual, string message, params object[] args);

        public static void AreNotEqual(System.IO.DirectoryInfo expected, System.IO.DirectoryInfo actual);

        public static void DoesNotExist(System.IO.DirectoryInfo actual, string message, params object[] args);

        public static void DoesNotExist(System.IO.DirectoryInfo actual);

        public static void DoesNotExist(string actual, string message, params object[] args);

        public static void DoesNotExist(string actual);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool Equals(object a, object b);

        public static void Exists(System.IO.DirectoryInfo actual, string message, params object[] args);

        public static void Exists(System.IO.DirectoryInfo actual);

        public static void Exists(string actual, string message, params object[] args);

        public static void Exists(string actual);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void ReferenceEquals(object a, object b);
    }

    public static class Does
    {
        public static NUnit.Framework.Constraints.FileOrDirectoryExistsConstraint Exist { get; }

        public static NUnit.Framework.Constraints.ConstraintExpression Not { get; }

        public static NUnit.Framework.Constraints.SomeItemsConstraint Contain(object expected);

        public static NUnit.Framework.Constraints.ContainsConstraint Contain(string expected);

        public static NUnit.Framework.Constraints.DictionaryContainsKeyConstraint ContainKey(object expected);

        public static NUnit.Framework.Constraints.DictionaryContainsValueConstraint ContainValue(object expected);

        public static NUnit.Framework.Constraints.EndsWithConstraint EndWith(string expected);

        public static NUnit.Framework.Constraints.RegexConstraint Match(string pattern);

        public static NUnit.Framework.Constraints.StartsWithConstraint StartWith(string expected);
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExplicitAttribute : NUnitAttribute, NUnit.Framework.Interfaces.IApplyToTest
    {
        public ExplicitAttribute();

        public ExplicitAttribute(string reason);

        public void ApplyToTest(NUnit.Framework.Internal.Test test);
    }

    public static class FileAssert
    {
        public static void AreEqual(System.IO.Stream expected, System.IO.Stream actual, string message, params object[] args);

        public static void AreEqual(System.IO.Stream expected, System.IO.Stream actual);

        public static void AreEqual(System.IO.FileInfo expected, System.IO.FileInfo actual, string message, params object[] args);

        public static void AreEqual(System.IO.FileInfo expected, System.IO.FileInfo actual);

        public static void AreEqual(string expected, string actual, string message, params object[] args);

        public static void AreEqual(string expected, string actual);

        public static void AreNotEqual(System.IO.Stream expected, System.IO.Stream actual, string message, params object[] args);

        public static void AreNotEqual(System.IO.Stream expected, System.IO.Stream actual);

        public static void AreNotEqual(System.IO.FileInfo expected, System.IO.FileInfo actual, string message, params object[] args);

        public static void AreNotEqual(System.IO.FileInfo expected, System.IO.FileInfo actual);

        public static void AreNotEqual(string expected, string actual, string message, params object[] args);

        public static void AreNotEqual(string expected, string actual);

        public static void DoesNotExist(System.IO.FileInfo actual, string message, params object[] args);

        public static void DoesNotExist(System.IO.FileInfo actual);

        public static void DoesNotExist(string actual, string message, params object[] args);

        public static void DoesNotExist(string actual);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool Equals(object a, object b);

        public static void Exists(System.IO.FileInfo actual, string message, params object[] args);

        public static void Exists(System.IO.FileInfo actual);

        public static void Exists(string actual, string message, params object[] args);

        public static void Exists(string actual);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void ReferenceEquals(object a, object b);
    }

    public class Has
    {
        public static NUnit.Framework.Constraints.ConstraintExpression All { get; }

        public static NUnit.Framework.Constraints.ResolvableConstraintExpression Count { get; }

        public static NUnit.Framework.Constraints.ResolvableConstraintExpression InnerException { get; }

        public static NUnit.Framework.Constraints.ResolvableConstraintExpression Length { get; }

        public static NUnit.Framework.Constraints.ResolvableConstraintExpression Message { get; }

        public static NUnit.Framework.Constraints.ConstraintExpression No { get; }

        public static NUnit.Framework.Constraints.ConstraintExpression None { get; }

        public static NUnit.Framework.Constraints.ItemsConstraintExpression One { get; }

        public static NUnit.Framework.Constraints.ConstraintExpression Some { get; }

        public static NUnit.Framework.Constraints.ResolvableConstraintExpression Attribute(System.Type expectedType);

        public static NUnit.Framework.Constraints.ResolvableConstraintExpression Attribute<T>();

        public static NUnit.Framework.Constraints.ItemsConstraintExpression Exactly(int expectedCount);

        public static NUnit.Framework.Constraints.SomeItemsConstraint Member(object expected);

        public static NUnit.Framework.Constraints.ResolvableConstraintExpression Property(string name);

        public Has();
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class IgnoreAttribute : NUnitAttribute, NUnit.Framework.Interfaces.IApplyToTest
    {
        public string Until { get; set; }

        public IgnoreAttribute(string reason);

        public void ApplyToTest(NUnit.Framework.Internal.Test test);
    }

    public class IgnoreException : ResultStateException
    {
        public override NUnit.Framework.Interfaces.ResultState ResultState { get; }

        public IgnoreException(string message);

        public IgnoreException(string message, System.Exception inner);

        protected IgnoreException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
    }

    public abstract class IncludeExcludeAttribute : NUnitAttribute
    {
        public string Exclude { get; set; }

        public string Include { get; set; }

        public string Reason { get; set; }

        public IncludeExcludeAttribute();

        public IncludeExcludeAttribute(string include);
    }

    public class InconclusiveException : ResultStateException
    {
        public override NUnit.Framework.Interfaces.ResultState ResultState { get; }

        public InconclusiveException(string message);

        public InconclusiveException(string message, System.Exception inner);

        protected InconclusiveException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
    }

    public class Is
    {
        public static NUnit.Framework.Constraints.ConstraintExpression All { get; }

        public static NUnit.Framework.Constraints.BinarySerializableConstraint BinarySerializable { get; }

        public static NUnit.Framework.Constraints.EmptyConstraint Empty { get; }

        public static NUnit.Framework.Constraints.FalseConstraint False { get; }

        public static NUnit.Framework.Constraints.NaNConstraint NaN { get; }

        public static NUnit.Framework.Constraints.LessThanConstraint Negative { get; }

        public static NUnit.Framework.Constraints.ConstraintExpression Not { get; }

        public static NUnit.Framework.Constraints.NullConstraint Null { get; }

        public static NUnit.Framework.Constraints.CollectionOrderedConstraint Ordered { get; }

        public static NUnit.Framework.Constraints.GreaterThanConstraint Positive { get; }

        public static NUnit.Framework.Constraints.TrueConstraint True { get; }

        public static NUnit.Framework.Constraints.UniqueItemsConstraint Unique { get; }

        public static NUnit.Framework.Constraints.XmlSerializableConstraint XmlSerializable { get; }

        public static NUnit.Framework.Constraints.EqualConstraint Zero { get; }

        public static NUnit.Framework.Constraints.AssignableFromConstraint AssignableFrom(System.Type expectedType);

        public static NUnit.Framework.Constraints.AssignableFromConstraint AssignableFrom<TExpected>();

        public static NUnit.Framework.Constraints.AssignableToConstraint AssignableTo(System.Type expectedType);

        public static NUnit.Framework.Constraints.AssignableToConstraint AssignableTo<TExpected>();

        public static NUnit.Framework.Constraints.GreaterThanOrEqualConstraint AtLeast(object expected);

        public static NUnit.Framework.Constraints.LessThanOrEqualConstraint AtMost(object expected);

        public static NUnit.Framework.Constraints.EqualConstraint EqualTo(object expected);

        public static NUnit.Framework.Constraints.CollectionEquivalentConstraint EquivalentTo(System.Collections.IEnumerable expected);

        public static NUnit.Framework.Constraints.GreaterThanConstraint GreaterThan(object expected);

        public static NUnit.Framework.Constraints.GreaterThanOrEqualConstraint GreaterThanOrEqualTo(object expected);

        public static NUnit.Framework.Constraints.RangeConstraint InRange(object from, object to);

        public static NUnit.Framework.Constraints.InstanceOfTypeConstraint InstanceOf(System.Type expectedType);

        public static NUnit.Framework.Constraints.InstanceOfTypeConstraint InstanceOf<TExpected>();

        public static NUnit.Framework.Constraints.LessThanConstraint LessThan(object expected);

        public static NUnit.Framework.Constraints.LessThanOrEqualConstraint LessThanOrEqualTo(object expected);

        public static NUnit.Framework.Constraints.SameAsConstraint SameAs(object expected);

        public static NUnit.Framework.Constraints.SamePathConstraint SamePath(string expected);

        public static NUnit.Framework.Constraints.SamePathOrUnderConstraint SamePathOrUnder(string expected);

        public static NUnit.Framework.Constraints.SubPathConstraint SubPathOf(string expected);

        public static NUnit.Framework.Constraints.CollectionSubsetConstraint SubsetOf(System.Collections.IEnumerable expected);

        public static NUnit.Framework.Constraints.CollectionSupersetConstraint SupersetOf(System.Collections.IEnumerable expected);

        public static NUnit.Framework.Constraints.ExactTypeConstraint TypeOf(System.Type expectedType);

        public static NUnit.Framework.Constraints.ExactTypeConstraint TypeOf<TExpected>();

        public Is();
    }

    public interface ITestAction
    {
        ActionTargets Targets { get; }

        void AfterTest(NUnit.Framework.Interfaces.ITest test);

        void BeforeTest(NUnit.Framework.Interfaces.ITest test);
    }

    public class Iz : Is
    {
        public Iz();
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class LevelOfParallelismAttribute : PropertyAttribute
    {
        public LevelOfParallelismAttribute(int level);
    }

    public class List
    {
        public static ListMapper Map(System.Collections.ICollection actual);

        public List();
    }

    public class ListMapper
    {
        public ListMapper(System.Collections.ICollection original);

        public System.Collections.ICollection Property(string name);
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MaxTimeAttribute : PropertyAttribute, NUnit.Framework.Interfaces.IWrapSetUpTearDown, NUnit.Framework.Interfaces.ICommandWrapper
    {
        public MaxTimeAttribute(int milliseconds);
    }

    public class MultipleAssertException : ResultStateException
    {
        public override NUnit.Framework.Interfaces.ResultState ResultState { get; }

        public MultipleAssertException();

        public MultipleAssertException(string message, System.Exception inner);

        protected MultipleAssertException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class NonParallelizableAttribute : ParallelizableAttribute
    {
        public NonParallelizableAttribute();
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class NonTestAssemblyAttribute : NUnitAttribute
    {
        public NonTestAssemblyAttribute();
    }

    public abstract class NUnitAttribute : System.Attribute
    {
        public NUnitAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OneTimeSetUpAttribute : NUnitAttribute
    {
        public OneTimeSetUpAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OneTimeTearDownAttribute : NUnitAttribute
    {
        public OneTimeTearDownAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OrderAttribute : NUnitAttribute, NUnit.Framework.Interfaces.IApplyToTest
    {
        public readonly int Order;

        public OrderAttribute(int order);

        public void ApplyToTest(NUnit.Framework.Internal.Test test);
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PairwiseAttribute : CombiningStrategyAttribute
    {
        public PairwiseAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ParallelizableAttribute : PropertyAttribute, NUnit.Framework.Interfaces.IApplyToContext
    {
        public ParallelScope Scope { get; }

        public ParallelizableAttribute();

        public ParallelizableAttribute(ParallelScope scope);

        public void ApplyToContext(NUnit.Framework.Internal.TestExecutionContext context);

        public override void ApplyToTest(NUnit.Framework.Internal.Test test);
    }

    [System.Flags]
    public enum ParallelScope : int
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        Default = 0,
        Self = 1,

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        None = 2,

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        ItemMask = 3,
        Children = 256,
        All = 257,
        Fixtures = 512,

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        ContextMask = 768
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PlatformAttribute : IncludeExcludeAttribute, NUnit.Framework.Interfaces.IApplyToTest
    {
        public PlatformAttribute();

        public PlatformAttribute(string platforms);

        public void ApplyToTest(NUnit.Framework.Internal.Test test);
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PropertyAttribute : NUnitAttribute, NUnit.Framework.Interfaces.IApplyToTest
    {
        public NUnit.Framework.Interfaces.IPropertyBag Properties { get; }

        public PropertyAttribute(string propertyName, string propertyValue);

        public PropertyAttribute(string propertyName, int propertyValue);

        public PropertyAttribute(string propertyName, double propertyValue);

        protected PropertyAttribute();

        protected PropertyAttribute(object propertyValue);

        public virtual void ApplyToTest(NUnit.Framework.Internal.Test test);
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class RandomAttribute : DataAttribute, NUnit.Framework.Interfaces.IParameterDataSource
    {
        public bool Distinct { get; set; }

        public RandomAttribute(int count);

        public RandomAttribute(int min, int max, int count);

        [System.CLSCompliant(false)]
        public RandomAttribute(uint min, uint max, int count);

        public RandomAttribute(long min, long max, int count);

        [System.CLSCompliant(false)]
        public RandomAttribute(ulong min, ulong max, int count);

        public RandomAttribute(short min, short max, int count);

        [System.CLSCompliant(false)]
        public RandomAttribute(ushort min, ushort max, int count);

        public RandomAttribute(double min, double max, int count);

        public RandomAttribute(float min, float max, int count);

        public RandomAttribute(byte min, byte max, int count);

        [System.CLSCompliant(false)]
        public RandomAttribute(sbyte min, sbyte max, int count);

        public System.Collections.IEnumerable GetData(NUnit.Framework.Interfaces.IParameterInfo parameter);
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public class RangeAttribute : DataAttribute, NUnit.Framework.Interfaces.IParameterDataSource
    {
        public RangeAttribute(int from, int to);

        public RangeAttribute(int from, int to, int step);

        [System.CLSCompliant(false)]
        public RangeAttribute(uint from, uint to);

        [System.CLSCompliant(false)]
        public RangeAttribute(uint from, uint to, uint step);

        public RangeAttribute(long from, long to);

        public RangeAttribute(long from, long to, long step);

        [System.CLSCompliant(false)]
        public RangeAttribute(ulong from, ulong to);

        [System.CLSCompliant(false)]
        public RangeAttribute(ulong from, ulong to, ulong step);

        public RangeAttribute(double from, double to, double step);

        public RangeAttribute(float from, float to, float step);

        public System.Collections.IEnumerable GetData(NUnit.Framework.Interfaces.IParameterInfo parameter);
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RepeatAttribute : PropertyAttribute, NUnit.Framework.Interfaces.IWrapSetUpTearDown, NUnit.Framework.Interfaces.ICommandWrapper
    {
        public RepeatAttribute(int count);

        public NUnit.Framework.Internal.Commands.TestCommand Wrap(NUnit.Framework.Internal.Commands.TestCommand command);

        public class RepeatedTestCommand : NUnit.Framework.Internal.Commands.DelegatingTestCommand
        {
            public RepeatedTestCommand(NUnit.Framework.Internal.Commands.TestCommand innerCommand, int repeatCount);

            public override NUnit.Framework.Internal.TestResult Execute(NUnit.Framework.Internal.TestExecutionContext context);
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequiresThreadAttribute : PropertyAttribute, NUnit.Framework.Interfaces.IApplyToTest
    {
        public RequiresThreadAttribute();

        public RequiresThreadAttribute(System.Threading.ApartmentState apartment);
    }

    public abstract class ResultStateException : System.Exception
    {
        public abstract NUnit.Framework.Interfaces.ResultState ResultState { get; }

        public ResultStateException(string message);

        public ResultStateException(string message, System.Exception inner);

        protected ResultStateException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RetryAttribute : PropertyAttribute, NUnit.Framework.Interfaces.IWrapSetUpTearDown, NUnit.Framework.Interfaces.ICommandWrapper
    {
        public RetryAttribute(int tryCount);

        public NUnit.Framework.Internal.Commands.TestCommand Wrap(NUnit.Framework.Internal.Commands.TestCommand command);

        public class RetryCommand : NUnit.Framework.Internal.Commands.DelegatingTestCommand
        {
            public RetryCommand(NUnit.Framework.Internal.Commands.TestCommand innerCommand, int tryCount);

            public override NUnit.Framework.Internal.TestResult Execute(NUnit.Framework.Internal.TestExecutionContext context);
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SequentialAttribute : CombiningStrategyAttribute
    {
        public SequentialAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SetCultureAttribute : PropertyAttribute, NUnit.Framework.Interfaces.IApplyToContext
    {
        public SetCultureAttribute(string culture);
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SetUICultureAttribute : PropertyAttribute, NUnit.Framework.Interfaces.IApplyToContext
    {
        public SetUICultureAttribute(string culture);
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SetUpAttribute : NUnitAttribute
    {
        public SetUpAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SetUpFixtureAttribute : NUnitAttribute, NUnit.Framework.Interfaces.IFixtureBuilder
    {
        public SetUpFixtureAttribute();

        public System.Collections.Generic.IEnumerable<NUnit.Framework.Internal.TestSuite> BuildFrom(NUnit.Framework.Interfaces.ITypeInfo typeInfo);
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SingleThreadedAttribute : NUnitAttribute, NUnit.Framework.Interfaces.IApplyToContext
    {
        public SingleThreadedAttribute();

        public void ApplyToContext(NUnit.Framework.Internal.TestExecutionContext context);
    }

    public class StringAssert
    {
        public static void AreEqualIgnoringCase(string expected, string actual, string message, params object[] args);

        public static void AreEqualIgnoringCase(string expected, string actual);

        public static void AreNotEqualIgnoringCase(string expected, string actual, string message, params object[] args);

        public static void AreNotEqualIgnoringCase(string expected, string actual);

        public static void Contains(string expected, string actual, string message, params object[] args);

        public static void Contains(string expected, string actual);

        public static void DoesNotContain(string expected, string actual, string message, params object[] args);

        public static void DoesNotContain(string expected, string actual);

        public static void DoesNotEndWith(string expected, string actual, string message, params object[] args);

        public static void DoesNotEndWith(string expected, string actual);

        public static void DoesNotMatch(string pattern, string actual, string message, params object[] args);

        public static void DoesNotMatch(string pattern, string actual);

        public static void DoesNotStartWith(string expected, string actual, string message, params object[] args);

        public static void DoesNotStartWith(string expected, string actual);

        public static void EndsWith(string expected, string actual, string message, params object[] args);

        public static void EndsWith(string expected, string actual);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool Equals(object a, object b);

        public static void IsMatch(string pattern, string actual, string message, params object[] args);

        public static void IsMatch(string pattern, string actual);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void ReferenceEquals(object a, object b);

        public static void StartsWith(string expected, string actual, string message, params object[] args);

        public static void StartsWith(string expected, string actual);

        public StringAssert();
    }

    public class SuccessException : ResultStateException
    {
        public override NUnit.Framework.Interfaces.ResultState ResultState { get; }

        public SuccessException(string message);

        public SuccessException(string message, System.Exception inner);

        protected SuccessException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TearDownAttribute : NUnitAttribute
    {
        public TearDownAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method | System.AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public abstract class TestActionAttribute : System.Attribute, ITestAction
    {
        public virtual ActionTargets Targets { get; }

        protected TestActionAttribute();

        public virtual void AfterTest(NUnit.Framework.Interfaces.ITest test);

        public virtual void BeforeTest(NUnit.Framework.Interfaces.ITest test);
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class TestAssemblyDirectoryResolveAttribute : NUnitAttribute
    {
        public TestAssemblyDirectoryResolveAttribute();
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestAttribute : NUnitAttribute, NUnit.Framework.Interfaces.ISimpleTestBuilder, NUnit.Framework.Interfaces.IApplyToTest, NUnit.Framework.Interfaces.IImplyFixture
    {
        public string Author { get; set; }

        public string Description { get; set; }

        public object ExpectedResult { get; set; }

        public System.Type TestOf { get; set; }

        public TestAttribute();

        public void ApplyToTest(NUnit.Framework.Internal.Test test);

        public NUnit.Framework.Internal.TestMethod BuildFrom(NUnit.Framework.Interfaces.IMethodInfo method, NUnit.Framework.Internal.Test suite);
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TestCaseAttribute : NUnitAttribute, NUnit.Framework.Interfaces.ITestBuilder, NUnit.Framework.Interfaces.ITestCaseData, NUnit.Framework.Interfaces.ITestData, NUnit.Framework.Interfaces.IImplyFixture
    {
        public object[] Arguments { get; }

        public string Author { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string ExcludePlatform { get; set; }

        public object ExpectedResult { get; set; }

        public bool Explicit { get; set; }

        public bool HasExpectedResult { get; }

        public string Ignore { get; set; }

        public string IgnoreReason { get; set; }

        public string IncludePlatform { get; set; }

        public NUnit.Framework.Interfaces.IPropertyBag Properties { get; }

        public string Reason { get; set; }

        public NUnit.Framework.Interfaces.RunState RunState { get; }

        public string TestName { get; set; }

        public System.Type TestOf { get; set; }

        public TestCaseAttribute(params object[] arguments);

        public TestCaseAttribute(object arg);

        public TestCaseAttribute(object arg1, object arg2);

        public TestCaseAttribute(object arg1, object arg2, object arg3);

        public System.Collections.Generic.IEnumerable<NUnit.Framework.Internal.TestMethod> BuildFrom(NUnit.Framework.Interfaces.IMethodInfo method, NUnit.Framework.Internal.Test suite);
    }

    public class TestCaseData : NUnit.Framework.Internal.TestCaseParameters
    {
        public TestCaseData(params object[] args);

        public TestCaseData(object arg);

        public TestCaseData(object arg1, object arg2);

        public TestCaseData(object arg1, object arg2, object arg3);

        public TestCaseData Explicit();

        public TestCaseData Explicit(string reason);

        public TestCaseData Ignore(string reason);

        public TestCaseData Returns(object result);

        public TestCaseData SetCategory(string category);

        public TestCaseData SetDescription(string description);

        public TestCaseData SetName(string name);

        public TestCaseData SetProperty(string propName, string propValue);

        public TestCaseData SetProperty(string propName, int propValue);

        public TestCaseData SetProperty(string propName, double propValue);
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TestCaseSourceAttribute : NUnitAttribute, NUnit.Framework.Interfaces.ITestBuilder, NUnit.Framework.Interfaces.IImplyFixture
    {
        public string Category { get; set; }

        public object[] MethodParams { get; }

        public string SourceName { get; }

        public System.Type SourceType { get; }

        public TestCaseSourceAttribute(string sourceName);

        public TestCaseSourceAttribute(System.Type sourceType, string sourceName, object[] methodParams);

        public TestCaseSourceAttribute(System.Type sourceType, string sourceName);

        public TestCaseSourceAttribute(string sourceName, object[] methodParams);

        public TestCaseSourceAttribute(System.Type sourceType);

        public System.Collections.Generic.IEnumerable<NUnit.Framework.Internal.TestMethod> BuildFrom(NUnit.Framework.Interfaces.IMethodInfo method, NUnit.Framework.Internal.Test suite);
    }

    public class TestContext
    {
        public static readonly TestParameters Parameters;

        public static readonly System.IO.TextWriter Progress;

        public static System.IO.TextWriter Error;

        public static TestContext CurrentContext { get; }

        public static System.IO.TextWriter Out { get; }

        public int AssertCount { get; }

        public int CurrentRepeatCount { get; }

        public NUnit.Framework.Internal.Randomizer Random { get; }

        public TestContext.ResultAdapter Result { get; }

        public TestContext.TestAdapter Test { get; }

        public string TestDirectory { get; }

        public string WorkDirectory { get; }

        public string WorkerId { get; }

        public static void AddFormatter(NUnit.Framework.Constraints.ValueFormatterFactory formatterFactory);

        public static void AddFormatter<TSUPPORTED>(NUnit.Framework.Constraints.ValueFormatter formatter);

        public static void AddTestAttachment(string filePath, string description = null);

        public static void Write(bool value);

        public static void Write(char value);

        public static void Write(char[] value);

        public static void Write(double value);

        public static void Write(int value);

        public static void Write(long value);

        public static void Write(decimal value);

        public static void Write(object value);

        public static void Write(float value);

        public static void Write(string value);

        [System.CLSCompliant(false)]
        public static void Write(uint value);

        [System.CLSCompliant(false)]
        public static void Write(ulong value);

        public static void Write(string format, object arg1);

        public static void Write(string format, object arg1, object arg2);

        public static void Write(string format, object arg1, object arg2, object arg3);

        public static void Write(string format, params object[] args);

        public static void WriteLine();

        public static void WriteLine(bool value);

        public static void WriteLine(char value);

        public static void WriteLine(char[] value);

        public static void WriteLine(double value);

        public static void WriteLine(int value);

        public static void WriteLine(long value);

        public static void WriteLine(decimal value);

        public static void WriteLine(object value);

        public static void WriteLine(float value);

        public static void WriteLine(string value);

        [System.CLSCompliant(false)]
        public static void WriteLine(uint value);

        [System.CLSCompliant(false)]
        public static void WriteLine(ulong value);

        public static void WriteLine(string format, object arg1);

        public static void WriteLine(string format, object arg1, object arg2);

        public static void WriteLine(string format, object arg1, object arg2, object arg3);

        public static void WriteLine(string format, params object[] args);

        public TestContext(NUnit.Framework.Internal.TestExecutionContext testExecutionContext);

        public class ResultAdapter
        {
            public System.Collections.Generic.IEnumerable<NUnit.Framework.Interfaces.AssertionResult> Assertions { get; }

            public int FailCount { get; }

            public int InconclusiveCount { get; }

            public string Message { get; }

            public NUnit.Framework.Interfaces.ResultState Outcome { get; }

            public int PassCount { get; }

            public int SkipCount { get; }

            public virtual string StackTrace { get; }

            public int WarningCount { get; }

            public ResultAdapter(NUnit.Framework.Internal.TestResult result);
        }

        public class TestAdapter
        {
            public object[] Arguments { get; }

            public string ClassName { get; }

            public string FullName { get; }

            public string ID { get; }

            public string MethodName { get; }

            public string Name { get; }

            public NUnit.Framework.Interfaces.IPropertyBag Properties { get; }

            public TestAdapter(NUnit.Framework.Internal.Test test);
        }
    }

    public delegate void TestDelegate();

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TestFixtureAttribute : NUnitAttribute, NUnit.Framework.Interfaces.IFixtureBuilder, NUnit.Framework.Interfaces.ITestFixtureData, NUnit.Framework.Interfaces.ITestData
    {
        public object[] Arguments { get; }

        public string Author { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public bool Explicit { get; set; }

        public string Ignore { get; set; }

        public string IgnoreReason { get; set; }

        public NUnit.Framework.Interfaces.IPropertyBag Properties { get; }

        public string Reason { get; set; }

        public NUnit.Framework.Interfaces.RunState RunState { get; }

        public string TestName { get; set; }

        public System.Type TestOf { get; set; }

        public System.Type[] TypeArgs { get; set; }

        public TestFixtureAttribute();

        public TestFixtureAttribute(params object[] arguments);

        public System.Collections.Generic.IEnumerable<NUnit.Framework.Internal.TestSuite> BuildFrom(NUnit.Framework.Interfaces.ITypeInfo typeInfo);
    }

    public class TestFixtureData : NUnit.Framework.Internal.TestFixtureParameters
    {
        public TestFixtureData(params object[] args);

        public TestFixtureData(object arg);

        public TestFixtureData(object arg1, object arg2);

        public TestFixtureData(object arg1, object arg2, object arg3);

        public TestFixtureData Explicit();

        public TestFixtureData Explicit(string reason);

        public TestFixtureData Ignore(string reason);
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class TestFixtureSourceAttribute : NUnitAttribute, NUnit.Framework.Interfaces.IFixtureBuilder
    {
        public const string MUST_BE_STATIC = "The sourceName specified on a TestCaseSourceAttribute must refer to a static field, property or method.";

        public string Category { get; set; }

        public string SourceName { get; }

        public System.Type SourceType { get; }

        public TestFixtureSourceAttribute(string sourceName);

        public TestFixtureSourceAttribute(System.Type sourceType, string sourceName);

        public TestFixtureSourceAttribute(System.Type sourceType);

        public System.Collections.Generic.IEnumerable<NUnit.Framework.Internal.TestSuite> BuildFrom(NUnit.Framework.Interfaces.ITypeInfo typeInfo);

        public System.Collections.Generic.IEnumerable<NUnit.Framework.Interfaces.ITestFixtureData> GetParametersFor(System.Type sourceType);
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TestOfAttribute : PropertyAttribute
    {
        public TestOfAttribute(System.Type type);

        public TestOfAttribute(string typeName);
    }

    public class TestParameters
    {
        public int Count { get; }

        public string this[string name] { get; }

        public System.Collections.Generic.ICollection<string> Names { get; }

        public TestParameters();

        public bool Exists(string name);

        public string Get(string name);

        public string Get(string name, string defaultValue);

        public T Get<T>(string name, T defaultValue);
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TheoryAttribute : CombiningStrategyAttribute, NUnit.Framework.Interfaces.ITestBuilder, NUnit.Framework.Interfaces.IImplyFixture
    {
        public TheoryAttribute();
    }

    public class Throws
    {
        public static NUnit.Framework.Constraints.ExactTypeConstraint ArgumentException { get; }

        public static NUnit.Framework.Constraints.ExactTypeConstraint ArgumentNullException { get; }

        public static NUnit.Framework.Constraints.ResolvableConstraintExpression Exception { get; }

        public static NUnit.Framework.Constraints.ResolvableConstraintExpression InnerException { get; }

        public static NUnit.Framework.Constraints.ExactTypeConstraint InvalidOperationException { get; }

        public static NUnit.Framework.Constraints.ThrowsNothingConstraint Nothing { get; }

        public static NUnit.Framework.Constraints.ExactTypeConstraint TargetInvocationException { get; }

        public static NUnit.Framework.Constraints.InstanceOfTypeConstraint InstanceOf(System.Type expectedType);

        public static NUnit.Framework.Constraints.InstanceOfTypeConstraint InstanceOf<TExpected>() where TExpected : System.Exception;

        public static NUnit.Framework.Constraints.ExactTypeConstraint TypeOf(System.Type expectedType);

        public static NUnit.Framework.Constraints.ExactTypeConstraint TypeOf<TExpected>() where TExpected : System.Exception;

        public Throws();
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TimeoutAttribute : PropertyAttribute, NUnit.Framework.Interfaces.IApplyToContext
    {
        public TimeoutAttribute(int timeout);
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ValuesAttribute : DataAttribute, NUnit.Framework.Interfaces.IParameterDataSource
    {
        protected object[] data;

        public ValuesAttribute();

        public ValuesAttribute(object arg1);

        public ValuesAttribute(object arg1, object arg2);

        public ValuesAttribute(object arg1, object arg2, object arg3);

        public ValuesAttribute(params object[] args);

        public System.Collections.IEnumerable GetData(NUnit.Framework.Interfaces.IParameterInfo parameter);
    }

    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public class ValueSourceAttribute : DataAttribute, NUnit.Framework.Interfaces.IParameterDataSource
    {
        public string SourceName { get; }

        public System.Type SourceType { get; }

        public ValueSourceAttribute(string sourceName);

        public ValueSourceAttribute(System.Type sourceType, string sourceName);

        public System.Collections.IEnumerable GetData(NUnit.Framework.Interfaces.IParameterInfo parameter);
    }

    public class Warn
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool Equals(object a, object b);

        public static void If<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr);

        public static void If<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr, string message, params object[] args);

        public static void If<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr, System.Func<string> getExceptionMessage);

        public static void If(bool condition, string message, params object[] args);

        public static void If(bool condition);

        public static void If(bool condition, System.Func<string> getExceptionMessage);

        public static void If(System.Func<bool> condition, string message, params object[] args);

        public static void If(System.Func<bool> condition);

        public static void If(System.Func<bool> condition, System.Func<string> getExceptionMessage);

        public static void If<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression);

        public static void If<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression, string message, params object[] args);

        public static void If<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression, System.Func<string> getExceptionMessage);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void ReferenceEquals(object a, object b);

        public static void Unless<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr);

        public static void Unless<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr, string message, params object[] args);

        public static void Unless<TActual>(NUnit.Framework.Constraints.ActualValueDelegate<TActual> del, NUnit.Framework.Constraints.IResolveConstraint expr, System.Func<string> getExceptionMessage);

        public static void Unless(bool condition, string message, params object[] args);

        public static void Unless(bool condition);

        public static void Unless(bool condition, System.Func<string> getExceptionMessage);

        public static void Unless(System.Func<bool> condition, string message, params object[] args);

        public static void Unless(System.Func<bool> condition);

        public static void Unless(System.Func<bool> condition, System.Func<string> getExceptionMessage);

        public static void Unless(TestDelegate code, NUnit.Framework.Constraints.IResolveConstraint constraint);

        public static void Unless<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression);

        public static void Unless<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression, string message, params object[] args);

        public static void Unless<TActual>(TActual actual, NUnit.Framework.Constraints.IResolveConstraint expression, System.Func<string> getExceptionMessage);

        public Warn();
    }
}
namespace NUnit.Framework.Api
{
    public class DefaultTestAssemblyBuilder : ITestAssemblyBuilder
    {
        public DefaultTestAssemblyBuilder();

        public NUnit.Framework.Interfaces.ITest Build(System.Reflection.Assembly assembly, System.Collections.Generic.IDictionary<string, object> options);

        public NUnit.Framework.Interfaces.ITest Build(string assemblyName, System.Collections.Generic.IDictionary<string, object> options);
    }

    public class FrameworkController : NUnit.Compatibility.LongLivedMarshalByRefObject
    {
        public System.Reflection.Assembly Assembly { get; }

        public string AssemblyNameOrPath { get; }

        public ITestAssemblyBuilder Builder { get; }

        public ITestAssemblyRunner Runner { get; }

        public static NUnit.Framework.Interfaces.TNode InsertEnvironmentElement(NUnit.Framework.Interfaces.TNode targetNode);

        public static NUnit.Framework.Interfaces.TNode InsertSettingsElement(NUnit.Framework.Interfaces.TNode targetNode, System.Collections.Generic.IDictionary<string, object> settings);

        public FrameworkController(string assemblyNameOrPath, string idPrefix, System.Collections.IDictionary settings);

        public FrameworkController(System.Reflection.Assembly assembly, string idPrefix, System.Collections.IDictionary settings);

        public FrameworkController(string assemblyNameOrPath, string idPrefix, System.Collections.IDictionary settings, string runnerType, string builderType);

        public FrameworkController(System.Reflection.Assembly assembly, string idPrefix, System.Collections.IDictionary settings, string runnerType, string builderType);

        public int CountTests(string filter);

        public string ExploreTests(string filter);

        public string LoadTests();

        public string RunTests(string filter);

        public string RunTests(System.Action<string> callback, string filter);

        public void StopRun(bool force);

        public class CountTestsAction : FrameworkController.FrameworkControllerAction
        {
            public CountTestsAction(FrameworkController controller, string filter, object handler);
        }

        public class ExploreTestsAction : FrameworkController.FrameworkControllerAction
        {
            public ExploreTestsAction(FrameworkController controller, string filter, object handler);
        }

        public abstract class FrameworkControllerAction : NUnit.Compatibility.LongLivedMarshalByRefObject
        {
            protected FrameworkControllerAction();
        }

        public class LoadTestsAction : FrameworkController.FrameworkControllerAction
        {
            public LoadTestsAction(FrameworkController controller, object handler);
        }

        public class RunAsyncAction : FrameworkController.FrameworkControllerAction
        {
            public RunAsyncAction(FrameworkController controller, string filter, object handler);
        }

        public class RunTestsAction : FrameworkController.FrameworkControllerAction
        {
            public RunTestsAction(FrameworkController controller, string filter, object handler);
        }

        public class StopRunAction : FrameworkController.FrameworkControllerAction
        {
            public StopRunAction(FrameworkController controller, bool force, object handler);
        }
    }

    public interface ITestAssemblyBuilder
    {
        NUnit.Framework.Interfaces.ITest Build(System.Reflection.Assembly assembly, System.Collections.Generic.IDictionary<string, object> options);

        NUnit.Framework.Interfaces.ITest Build(string assemblyName, System.Collections.Generic.IDictionary<string, object> options);
    }

    public interface ITestAssemblyRunner
    {
        bool IsTestComplete { get; }

        bool IsTestLoaded { get; }

        bool IsTestRunning { get; }

        NUnit.Framework.Interfaces.ITest LoadedTest { get; }

        NUnit.Framework.Interfaces.ITestResult Result { get; }

        int CountTestCases(NUnit.Framework.Interfaces.ITestFilter filter);

        NUnit.Framework.Interfaces.ITest ExploreTests(NUnit.Framework.Interfaces.ITestFilter filter);

        NUnit.Framework.Interfaces.ITest Load(string assemblyName, System.Collections.Generic.IDictionary<string, object> settings);

        NUnit.Framework.Interfaces.ITest Load(System.Reflection.Assembly assembly, System.Collections.Generic.IDictionary<string, object> settings);

        NUnit.Framework.Interfaces.ITestResult Run(NUnit.Framework.Interfaces.ITestListener listener, NUnit.Framework.Interfaces.ITestFilter filter);

        void RunAsync(NUnit.Framework.Interfaces.ITestListener listener, NUnit.Framework.Interfaces.ITestFilter filter);

        void StopRun(bool force);

        bool WaitForCompletion(int timeout);
    }

    public class NUnitTestAssemblyRunner : ITestAssemblyRunner
    {
        public static int DefaultLevelOfParallelism { get; }

        public bool IsTestComplete { get; }

        public bool IsTestLoaded { get; }

        public bool IsTestRunning { get; }

        public NUnit.Framework.Interfaces.ITest LoadedTest { get; }

        public NUnit.Framework.Interfaces.ITestResult Result { get; }

        public NUnitTestAssemblyRunner(ITestAssemblyBuilder builder);

        public int CountTestCases(NUnit.Framework.Interfaces.ITestFilter filter);

        public NUnit.Framework.Interfaces.ITest ExploreTests(NUnit.Framework.Interfaces.ITestFilter filter);

        public NUnit.Framework.Interfaces.ITest Load(string assemblyName, System.Collections.Generic.IDictionary<string, object> settings);

        public NUnit.Framework.Interfaces.ITest Load(System.Reflection.Assembly assembly, System.Collections.Generic.IDictionary<string, object> settings);

        public NUnit.Framework.Interfaces.ITestResult Run(NUnit.Framework.Interfaces.ITestListener listener, NUnit.Framework.Interfaces.ITestFilter filter);

        public void RunAsync(NUnit.Framework.Interfaces.ITestListener listener, NUnit.Framework.Interfaces.ITestFilter filter);

        public void StopRun(bool force);

        public bool WaitForCompletion(int timeout);
    }
}
namespace NUnit.Framework.Constraints
{
    public delegate TActual ActualValueDelegate<TActual>();

    public class AllItemsConstraint : PrefixConstraint
    {
        public override string DisplayName { get; }

        public AllItemsConstraint(IConstraint itemConstraint);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class AllOperator : CollectionOperator
    {
        public AllOperator();

        public override IConstraint ApplyPrefix(IConstraint constraint);
    }

    public class AndConstraint : BinaryConstraint
    {
        public override string Description { get; }

        public AndConstraint(IConstraint left, IConstraint right);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class AndOperator : BinaryOperator
    {
        public AndOperator();

        public override IConstraint ApplyOperator(IConstraint left, IConstraint right);
    }

    public class AssignableFromConstraint : TypeConstraint
    {
        public AssignableFromConstraint(System.Type type);

        protected override bool Matches(object actual);
    }

    public class AssignableToConstraint : TypeConstraint
    {
        public AssignableToConstraint(System.Type type);

        protected override bool Matches(object actual);
    }

    public class AttributeConstraint : PrefixConstraint
    {
        public AttributeConstraint(System.Type type, IConstraint baseConstraint);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        protected override string GetStringRepresentation();
    }

    public class AttributeExistsConstraint : Constraint
    {
        public override string Description { get; }

        public AttributeExistsConstraint(System.Type type);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class AttributeOperator : SelfResolvingOperator
    {
        public AttributeOperator(System.Type type);

        public override void Reduce(ConstraintBuilder.ConstraintStack stack);
    }

    public abstract class BinaryConstraint : Constraint
    {
        protected IConstraint Left;

        protected IConstraint Right;

        protected BinaryConstraint(IConstraint left, IConstraint right);
    }

    public abstract class BinaryOperator : ConstraintOperator
    {
        public override int LeftPrecedence { get; }

        public override int RightPrecedence { get; }

        protected BinaryOperator();

        public abstract IConstraint ApplyOperator(IConstraint left, IConstraint right);

        public override void Reduce(ConstraintBuilder.ConstraintStack stack);
    }

    public class BinarySerializableConstraint : Constraint
    {
        public override string Description { get; }

        public BinarySerializableConstraint();

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        protected override string GetStringRepresentation();
    }

    public abstract class CollectionConstraint : Constraint
    {
        protected static bool IsEmpty(System.Collections.IEnumerable enumerable);

        protected CollectionConstraint();

        protected CollectionConstraint(object arg);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        protected abstract bool Matches(System.Collections.IEnumerable collection);
    }

    public class CollectionContainsConstraint : CollectionItemsEqualConstraint
    {
        public override string Description { get; }

        public override string DisplayName { get; }

        protected object Expected { get; }

        [System.Obsolete("Deprecated, use 'new SomeItemsConstraint(new EqualConstraint(expected))' or 'Has.Some.EqualTo(expected)' instead.")]
        public CollectionContainsConstraint(object expected);

        protected override bool Matches(System.Collections.IEnumerable actual);

        public CollectionContainsConstraint Using<TCollectionType, TMemberType>(System.Func<TCollectionType, TMemberType, bool> comparison);
    }

    public class CollectionEquivalentConstraint : CollectionItemsEqualConstraint
    {
        public override string Description { get; }

        public override string DisplayName { get; }

        public CollectionEquivalentConstraint(System.Collections.IEnumerable expected);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        protected override bool Matches(System.Collections.IEnumerable actual);

        public CollectionEquivalentConstraint Using<TActual, TExpected>(System.Func<TActual, TExpected, bool> comparison);
    }

    public class CollectionEquivalentConstraintResult : ConstraintResult
    {
        public CollectionEquivalentConstraintResult(CollectionEquivalentConstraint constraint, CollectionTally.CollectionTallyResult tallyResult, object actual, bool isSuccess);

        public override void WriteMessageTo(MessageWriter writer);
    }

    public abstract class CollectionItemsEqualConstraint : CollectionConstraint
    {
        public CollectionItemsEqualConstraint IgnoreCase { get; }

        protected bool IgnoringCase { get; }

        protected bool UsingExternalComparer { get; }

        protected CollectionItemsEqualConstraint();

        protected CollectionItemsEqualConstraint(object arg);

        protected bool ItemsEqual(object x, object y);

        protected CollectionTally Tally(System.Collections.IEnumerable c);

        public CollectionItemsEqualConstraint Using(System.Collections.IComparer comparer);

        public CollectionItemsEqualConstraint Using<T>(System.Collections.Generic.IComparer<T> comparer);

        public CollectionItemsEqualConstraint Using<T>(System.Comparison<T> comparer);

        public CollectionItemsEqualConstraint Using(System.Collections.IEqualityComparer comparer);

        public CollectionItemsEqualConstraint Using<T>(System.Collections.Generic.IEqualityComparer<T> comparer);

        public CollectionItemsEqualConstraint Using<T>(System.Func<T, T, bool> comparer);
    }

    public abstract class CollectionOperator : PrefixOperator
    {
        protected CollectionOperator();
    }

    public class CollectionOrderedConstraint : CollectionConstraint
    {
        public CollectionOrderedConstraint Ascending { get; }

        public CollectionOrderedConstraint Descending { get; }

        public override string Description { get; }

        public override string DisplayName { get; }

        public CollectionOrderedConstraint Then { get; }

        public CollectionOrderedConstraint();

        public CollectionOrderedConstraint By(string propertyName);

        protected override string GetStringRepresentation();

        protected override bool Matches(System.Collections.IEnumerable actual);

        public CollectionOrderedConstraint Using(System.Collections.IComparer comparer);

        public CollectionOrderedConstraint Using<T>(System.Collections.Generic.IComparer<T> comparer);

        public CollectionOrderedConstraint Using<T>(System.Comparison<T> comparer);
    }

    public class CollectionSubsetConstraint : CollectionItemsEqualConstraint
    {
        public override string Description { get; }

        public override string DisplayName { get; }

        public CollectionSubsetConstraint(System.Collections.IEnumerable expected);

        protected override bool Matches(System.Collections.IEnumerable actual);

        public CollectionSubsetConstraint Using<TSubsetType, TSupersetType>(System.Func<TSubsetType, TSupersetType, bool> comparison);
    }

    public class CollectionSupersetConstraint : CollectionItemsEqualConstraint
    {
        public override string Description { get; }

        public override string DisplayName { get; }

        public CollectionSupersetConstraint(System.Collections.IEnumerable expected);

        protected override bool Matches(System.Collections.IEnumerable actual);

        public CollectionSupersetConstraint Using<TSupersetType, TSubsetType>(System.Func<TSupersetType, TSubsetType, bool> comparison);
    }

    public class CollectionTally
    {
        public CollectionTally.CollectionTallyResult Result { get; }

        public CollectionTally(NUnitEqualityComparer comparer, System.Collections.IEnumerable c);

        public void TryRemove(object o);

        public void TryRemove(System.Collections.IEnumerable c);

        public class CollectionTallyResult
        {
            public System.Collections.Generic.List<object> ExtraItems { get; set; }

            public System.Collections.Generic.List<object> MissingItems { get; set; }

            public CollectionTallyResult();
        }
    }

    public abstract class ComparisonAdapter
    {
        public static ComparisonAdapter Default { get; }

        public static ComparisonAdapter For(System.Collections.IComparer comparer);

        public static ComparisonAdapter For<T>(System.Collections.Generic.IComparer<T> comparer);

        public static ComparisonAdapter For<T>(System.Comparison<T> comparer);

        protected ComparisonAdapter();

        public abstract int Compare(object expected, object actual);
    }

    public abstract class ComparisonConstraint : Constraint
    {
        public ComparisonConstraint Percent { get; }

        protected ComparisonConstraint(object expected);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        protected abstract bool PerformComparison(ComparisonAdapter comparer, object actual, object expected, Tolerance tolerance);

        public ComparisonConstraint Using(System.Collections.IComparer comparer);

        public ComparisonConstraint Using<T>(System.Collections.Generic.IComparer<T> comparer);

        public ComparisonConstraint Using<T>(System.Comparison<T> comparer);

        public ComparisonConstraint Within(object amount);
    }

    public abstract class Constraint : IConstraint, IResolveConstraint
    {
        public ConstraintExpression And { get; }

        public object[] Arguments { get; }

        public ConstraintBuilder Builder { get; set; }

        public virtual string Description { get; protected set; }

        public virtual string DisplayName { get; }

        public ConstraintExpression Or { get; }

        public ConstraintExpression With { get; }

        protected Constraint(params object[] args);

        public DelayedConstraint.WithRawDelayInterval After(int delay);

        public DelayedConstraint After(int delayInMilliseconds, int pollingInterval);

        public abstract ConstraintResult ApplyTo<TActual>(TActual actual);

        public virtual ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del);

        public virtual ConstraintResult ApplyTo<TActual>(ref TActual actual);

        protected virtual string GetStringRepresentation();

        protected virtual object GetTestObject<TActual>(ActualValueDelegate<TActual> del);

        public override string ToString();

        public static Constraint operator &(Constraint left, Constraint right);

        public static Constraint operator |(Constraint left, Constraint right);

        public static Constraint operator !(Constraint constraint);
    }

    public class ConstraintBuilder : IResolveConstraint
    {
        public ConstraintBuilder();

        public void Append(ConstraintOperator op);

        public void Append(Constraint constraint);

        public IConstraint Resolve();

        public class ConstraintStack
        {
            public bool Empty { get; }

            public ConstraintStack(ConstraintBuilder builder);

            public IConstraint Pop();

            public void Push(IConstraint constraint);
        }

        public class OperatorStack
        {
            public bool Empty { get; }

            public ConstraintOperator Top { get; }

            public OperatorStack(ConstraintBuilder builder);

            public ConstraintOperator Pop();

            public void Push(ConstraintOperator op);
        }
    }

    public class ConstraintExpression
    {
        protected ConstraintBuilder builder;

        public ConstraintExpression All { get; }

        public BinarySerializableConstraint BinarySerializable { get; }

        public ResolvableConstraintExpression Count { get; }

        public EmptyConstraint Empty { get; }

        public Constraint Exist { get; }

        public FalseConstraint False { get; }

        public ResolvableConstraintExpression InnerException { get; }

        public ResolvableConstraintExpression Length { get; }

        public ResolvableConstraintExpression Message { get; }

        public NaNConstraint NaN { get; }

        public LessThanConstraint Negative { get; }

        public ConstraintExpression No { get; }

        public ConstraintExpression None { get; }

        public ConstraintExpression Not { get; }

        public NullConstraint Null { get; }

        public ItemsConstraintExpression One { get; }

        public CollectionOrderedConstraint Ordered { get; }

        public GreaterThanConstraint Positive { get; }

        public ConstraintExpression Some { get; }

        public TrueConstraint True { get; }

        public UniqueItemsConstraint Unique { get; }

        public ConstraintExpression With { get; }

        public XmlSerializableConstraint XmlSerializable { get; }

        public EqualConstraint Zero { get; }

        public ConstraintExpression();

        public ConstraintExpression(ConstraintBuilder builder);

        public ConstraintExpression Append(ConstraintOperator op);

        public ResolvableConstraintExpression Append(SelfResolvingOperator op);

        public Constraint Append(Constraint constraint);

        public AssignableFromConstraint AssignableFrom(System.Type expectedType);

        public AssignableFromConstraint AssignableFrom<TExpected>();

        public AssignableToConstraint AssignableTo(System.Type expectedType);

        public AssignableToConstraint AssignableTo<TExpected>();

        public GreaterThanOrEqualConstraint AtLeast(object expected);

        public LessThanOrEqualConstraint AtMost(object expected);

        public ResolvableConstraintExpression Attribute(System.Type expectedType);

        public ResolvableConstraintExpression Attribute<TExpected>();

        public SomeItemsConstraint Contain(object expected);

        public ContainsConstraint Contain(string expected);

        public DictionaryContainsKeyConstraint ContainKey(object expected);

        public SomeItemsConstraint Contains(object expected);

        public ContainsConstraint Contains(string expected);

        [System.Obsolete("Deprecated, use Contains")]
        public SubstringConstraint ContainsSubstring(string expected);

        public DictionaryContainsValueConstraint ContainValue(object expected);

        public EndsWithConstraint EndsWith(string expected);

        public EndsWithConstraint EndWith(string expected);

        public EqualConstraint EqualTo(object expected);

        public CollectionEquivalentConstraint EquivalentTo(System.Collections.IEnumerable expected);

        public ItemsConstraintExpression Exactly(int expectedCount);

        public GreaterThanConstraint GreaterThan(object expected);

        public GreaterThanOrEqualConstraint GreaterThanOrEqualTo(object expected);

        public RangeConstraint InRange(object from, object to);

        public InstanceOfTypeConstraint InstanceOf(System.Type expectedType);

        public InstanceOfTypeConstraint InstanceOf<TExpected>();

        public LessThanConstraint LessThan(object expected);

        public LessThanOrEqualConstraint LessThanOrEqualTo(object expected);

        public RegexConstraint Match(string pattern);

        public Constraint Matches(IResolveConstraint constraint);

        public Constraint Matches<TActual>(System.Predicate<TActual> predicate);

        public RegexConstraint Matches(string pattern);

        public SomeItemsConstraint Member(object expected);

        public ResolvableConstraintExpression Property(string name);

        public SameAsConstraint SameAs(object expected);

        public SamePathConstraint SamePath(string expected);

        public SamePathOrUnderConstraint SamePathOrUnder(string expected);

        public StartsWithConstraint StartsWith(string expected);

        public StartsWithConstraint StartWith(string expected);

        [System.Obsolete("Deprecated, use Contains")]
        public SubstringConstraint StringContaining(string expected);

        [System.Obsolete("Deprecated, use Does.EndWith or EndsWith")]
        public EndsWithConstraint StringEnding(string expected);

        [System.Obsolete("Deprecated, use Does.Match or Matches")]
        public RegexConstraint StringMatching(string pattern);

        [System.Obsolete("Deprecated, use Does.StartWith or StartsWith")]
        public StartsWithConstraint StringStarting(string expected);

        public SubPathConstraint SubPathOf(string expected);

        public CollectionSubsetConstraint SubsetOf(System.Collections.IEnumerable expected);

        public CollectionSupersetConstraint SupersetOf(System.Collections.IEnumerable expected);

        public override string ToString();

        public ExactTypeConstraint TypeOf(System.Type expectedType);

        public ExactTypeConstraint TypeOf<TExpected>();
    }

    public abstract class ConstraintOperator
    {
        protected int left_precedence;

        protected int right_precedence;

        public object LeftContext { get; set; }

        public virtual int LeftPrecedence { get; }

        public object RightContext { get; set; }

        public virtual int RightPrecedence { get; }

        protected ConstraintOperator();

        public abstract void Reduce(ConstraintBuilder.ConstraintStack stack);
    }

    public class ConstraintResult
    {
        public object ActualValue { get; }

        public string Description { get; }

        public virtual bool IsSuccess { get; }

        public string Name { get; }

        public ConstraintStatus Status { get; set; }

        public ConstraintResult(IConstraint constraint, object actualValue);

        public ConstraintResult(IConstraint constraint, object actualValue, ConstraintStatus status);

        public ConstraintResult(IConstraint constraint, object actualValue, bool isSuccess);

        public virtual void WriteActualValueTo(MessageWriter writer);

        public virtual void WriteMessageTo(MessageWriter writer);
    }

    public enum ConstraintStatus : int
    {
        Unknown = 0,
        Success = 1,
        Failure = 2,
        Error = 3
    }

    public class ContainsConstraint : Constraint
    {
        public override string Description { get; }

        public ContainsConstraint IgnoreCase { get; }

        public ContainsConstraint(object expected);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class DelayedConstraint : PrefixConstraint
    {
        protected Interval DelayInterval { get; set; }

        public override string Description { get; }

        protected Interval PollingInterval { get; set; }

        public DelayedConstraint(IConstraint baseConstraint, int delayInMilliseconds);

        public DelayedConstraint(IConstraint baseConstraint, int delayInMilliseconds, int pollingIntervalInMilliseconds);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del);

        public override ConstraintResult ApplyTo<TActual>(ref TActual actual);

        protected override string GetStringRepresentation();

        public class WithDimensionedDelayInterval : DelayedConstraint
        {
            public WithDimensionedDelayInterval(DelayedConstraint parent);

            public DelayedConstraint.WithRawPollingInterval PollEvery(int milliSeconds);
        }

        public class WithRawDelayInterval : DelayedConstraint
        {
            public DelayedConstraint.WithDimensionedDelayInterval MilliSeconds { get; }

            public DelayedConstraint.WithDimensionedDelayInterval Minutes { get; }

            public DelayedConstraint.WithDimensionedDelayInterval Seconds { get; }

            public WithRawDelayInterval(DelayedConstraint parent);

            public DelayedConstraint.WithRawPollingInterval PollEvery(int milliSeconds);
        }

        public class WithRawPollingInterval : DelayedConstraint
        {
            public DelayedConstraint MilliSeconds { get; }

            public DelayedConstraint Minutes { get; }

            public DelayedConstraint Seconds { get; }

            public WithRawPollingInterval(DelayedConstraint parent);
        }
    }

    public class DictionaryContainsKeyConstraint : CollectionItemsEqualConstraint
    {
        public override string Description { get; }

        public override string DisplayName { get; }

        protected object Expected { get; }

        public DictionaryContainsKeyConstraint(object expected);

        protected override bool Matches(System.Collections.IEnumerable actual);

        public DictionaryContainsKeyConstraint Using<TCollectionType, TMemberType>(System.Func<TCollectionType, TMemberType, bool> comparison);
    }

    public class DictionaryContainsValueConstraint : CollectionItemsEqualConstraint
    {
        public override string Description { get; }

        public override string DisplayName { get; }

        protected object Expected { get; }

        public DictionaryContainsValueConstraint(object expected);

        protected override bool Matches(System.Collections.IEnumerable actual);

        public DictionaryContainsValueConstraint Using<TCollectionType, TMemberType>(System.Func<TCollectionType, TMemberType, bool> comparison);
    }

    public class EmptyCollectionConstraint : CollectionConstraint
    {
        public override string Description { get; }

        public EmptyCollectionConstraint();

        protected override bool Matches(System.Collections.IEnumerable collection);
    }

    public class EmptyConstraint : Constraint
    {
        public override string Description { get; }

        public EmptyConstraint();

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class EmptyDirectoryConstraint : Constraint
    {
        public override string Description { get; }

        public EmptyDirectoryConstraint();

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class EmptyStringConstraint : StringConstraint
    {
        public override string Description { get; }

        public EmptyStringConstraint();

        protected override bool Matches(string actual);
    }

    public class EndsWithConstraint : StringConstraint
    {
        public EndsWithConstraint(string expected);

        protected override bool Matches(string actual);
    }

    public class EqualConstraint : Constraint
    {
        public EqualConstraint AsCollection { get; }

        public bool CaseInsensitive { get; }

        public bool ClipStrings { get; }

        public EqualConstraint Days { get; }

        public override string Description { get; }

        public System.Collections.Generic.IList<NUnitEqualityComparer.FailurePoint> FailurePoints { get; }

        public EqualConstraint Hours { get; }

        public EqualConstraint IgnoreCase { get; }

        public EqualConstraint Milliseconds { get; }

        public EqualConstraint Minutes { get; }

        public EqualConstraint NoClip { get; }

        public EqualConstraint Percent { get; }

        public EqualConstraint Seconds { get; }

        public EqualConstraint Ticks { get; }

        public Tolerance Tolerance { get; }

        public EqualConstraint Ulps { get; }

        public EqualConstraint WithSameOffset { get; }

        public EqualConstraint(object expected);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        public EqualConstraint Using(System.Collections.IComparer comparer);

        public EqualConstraint Using<T>(System.Collections.Generic.IComparer<T> comparer);

        public EqualConstraint Using<T>(System.Func<T, T, bool> comparer);

        public EqualConstraint Using<T>(System.Comparison<T> comparer);

        public EqualConstraint Using(System.Collections.IEqualityComparer comparer);

        public EqualConstraint Using<T>(System.Collections.Generic.IEqualityComparer<T> comparer);

        public EqualConstraint Using<TCollectionType, TMemberType>(System.Func<TCollectionType, TMemberType, bool> comparison);

        public EqualConstraint Within(object amount);
    }

    public class EqualConstraintResult : ConstraintResult
    {
        public EqualConstraintResult(EqualConstraint constraint, object actual, bool hasSucceeded);

        public override void WriteMessageTo(MessageWriter writer);
    }

    public abstract class EqualityAdapter
    {
        public static EqualityAdapter For(System.Collections.IComparer comparer);

        public static EqualityAdapter For(System.Collections.IEqualityComparer comparer);

        public static EqualityAdapter For<TExpected, TActual>(System.Func<TExpected, TActual, bool> comparison);

        public static EqualityAdapter For<T>(System.Collections.Generic.IEqualityComparer<T> comparer);

        public static EqualityAdapter For<T>(System.Collections.Generic.IComparer<T> comparer);

        public static EqualityAdapter For<T>(System.Comparison<T> comparer);

        protected EqualityAdapter();

        public abstract bool AreEqual(object x, object y);

        public virtual bool CanCompare(object x, object y);
    }

    public class ExactCountConstraint : Constraint
    {
        public override string Description { get; }

        public ExactCountConstraint(int expectedCount);

        public ExactCountConstraint(int expectedCount, IConstraint itemConstraint);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class ExactCountOperator : SelfResolvingOperator
    {
        public ExactCountOperator(int expectedCount);

        public override void Reduce(ConstraintBuilder.ConstraintStack stack);
    }

    public class ExactTypeConstraint : TypeConstraint
    {
        public override string DisplayName { get; }

        public ExactTypeConstraint(System.Type type);

        protected override bool Matches(object actual);
    }

    public class ExceptionTypeConstraint : ExactTypeConstraint
    {
        public ExceptionTypeConstraint(System.Type type);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class FalseConstraint : Constraint
    {
        public FalseConstraint();

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    [System.Obsolete("FileExistsConstraint is deprecated, please use FileOrDirectoryExistsConstraint instead.")]
    public class FileExistsConstraint : FileOrDirectoryExistsConstraint
    {
        public override string Description { get; }

        public FileExistsConstraint();
    }

    public class FileOrDirectoryExistsConstraint : Constraint
    {
        public override string Description { get; }

        public FileOrDirectoryExistsConstraint IgnoreDirectories { get; }

        public FileOrDirectoryExistsConstraint IgnoreFiles { get; }

        public FileOrDirectoryExistsConstraint();

        public FileOrDirectoryExistsConstraint(bool ignoreDirectories);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class FloatingPointNumerics
    {
        public static bool AreAlmostEqualUlps(float left, float right, int maxUlps);

        public static bool AreAlmostEqualUlps(double left, double right, long maxUlps);

        public static double ReinterpretAsDouble(long value);

        public static float ReinterpretAsFloat(int value);

        public static int ReinterpretAsInt(float value);

        public static long ReinterpretAsLong(double value);
    }

    public class GreaterThanConstraint : ComparisonConstraint
    {
        public GreaterThanConstraint(object expected);

        protected override bool PerformComparison(ComparisonAdapter comparer, object actual, object expected, Tolerance tolerance);
    }

    public class GreaterThanOrEqualConstraint : ComparisonConstraint
    {
        public GreaterThanOrEqualConstraint(object expected);

        protected override bool PerformComparison(ComparisonAdapter comparer, object actual, object expected, Tolerance tolerance);
    }

    public interface IConstraint : IResolveConstraint
    {
        object[] Arguments { get; }

        ConstraintBuilder Builder { get; set; }

        string Description { get; }

        string DisplayName { get; }

        ConstraintResult ApplyTo<TActual>(TActual actual);

        ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del);

        ConstraintResult ApplyTo<TActual>(ref TActual actual);
    }

    public class InstanceOfTypeConstraint : TypeConstraint
    {
        public override string DisplayName { get; }

        public InstanceOfTypeConstraint(System.Type type);

        protected override bool Matches(object actual);
    }

    public class Interval
    {
        public System.TimeSpan AsTimeSpan { get; }

        public Interval InMilliseconds { get; }

        public Interval InMinutes { get; }

        public Interval InSeconds { get; }

        public bool IsNotZero { get; }

        public Interval(int value);

        public override string ToString();
    }

    public interface IResolveConstraint
    {
        IConstraint Resolve();
    }

    public sealed class ItemsConstraintExpression : ConstraintExpression
    {
        public ResolvableConstraintExpression Items { get; }

        public ItemsConstraintExpression();

        public ItemsConstraintExpression(ConstraintBuilder builder);
    }

    public class LessThanConstraint : ComparisonConstraint
    {
        public LessThanConstraint(object expected);

        protected override bool PerformComparison(ComparisonAdapter comparer, object actual, object expected, Tolerance tolerance);
    }

    public class LessThanOrEqualConstraint : ComparisonConstraint
    {
        public LessThanOrEqualConstraint(object expected);

        protected override bool PerformComparison(ComparisonAdapter comparer, object actual, object expected, Tolerance tolerance);
    }

    public abstract class MessageWriter : System.IO.StringWriter
    {
        public abstract int MaxLineLength { get; set; }

        protected MessageWriter();

        public abstract void DisplayDifferences(ConstraintResult result);

        public abstract void DisplayDifferences(object expected, object actual);

        public abstract void DisplayDifferences(object expected, object actual, Tolerance tolerance);

        public abstract void DisplayStringDifferences(string expected, string actual, int mismatch, bool ignoreCase, bool clipping);

        public abstract void WriteActualValue(object actual);

        public abstract void WriteCollectionElements(System.Collections.IEnumerable collection, long start, int max);

        public void WriteMessageLine(string message, params object[] args);

        public abstract void WriteMessageLine(int level, string message, params object[] args);

        public abstract void WriteValue(object val);
    }

    public class NaNConstraint : Constraint
    {
        public override string Description { get; }

        public NaNConstraint();

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class NoItemConstraint : PrefixConstraint
    {
        public override string DisplayName { get; }

        public NoItemConstraint(IConstraint itemConstraint);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class NoneOperator : CollectionOperator
    {
        public NoneOperator();

        public override IConstraint ApplyPrefix(IConstraint constraint);
    }

    public class NotConstraint : PrefixConstraint
    {
        public NotConstraint(IConstraint baseConstraint);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class NotOperator : PrefixOperator
    {
        public NotOperator();

        public override IConstraint ApplyPrefix(IConstraint constraint);
    }

    public class NullConstraint : Constraint
    {
        public NullConstraint();

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public static class Numerics
    {
        public static bool AreEqual(object expected, object actual, ref Tolerance tolerance);

        public static int Compare(object expected, object actual);

        public static bool IsFixedPointNumeric(object obj);

        public static bool IsFloatingPointNumeric(object obj);

        public static bool IsNumericType(object obj);
    }

    public class NUnitComparer : System.Collections.IComparer
    {
        public static NUnitComparer Default { get; }

        public NUnitComparer();

        public int Compare(object x, object y);
    }

    public class NUnitEqualityComparer
    {
        public static NUnitEqualityComparer Default { get; }

        public bool CompareAsCollection { get; set; }

        public System.Collections.Generic.IList<EqualityAdapter> ExternalComparers { get; }

        public System.Collections.Generic.IList<NUnitEqualityComparer.FailurePoint> FailurePoints { get; }

        public bool IgnoreCase { get; set; }

        public bool WithSameOffset { get; set; }

        public bool AreEqual(object x, object y, ref Tolerance tolerance, bool topLevelComparison = true);

        public class FailurePoint
        {
            public bool ActualHasData;

            public object ActualValue;

            public bool ExpectedHasData;

            public object ExpectedValue;

            public long Position;

            public FailurePoint();
        }
    }

    public class OrConstraint : BinaryConstraint
    {
        public override string Description { get; }

        public OrConstraint(IConstraint left, IConstraint right);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class OrOperator : BinaryOperator
    {
        public OrOperator();

        public override IConstraint ApplyOperator(IConstraint left, IConstraint right);
    }

    public abstract class PathConstraint : StringConstraint
    {
        public PathConstraint RespectCase { get; }

        protected PathConstraint(string expected);

        protected string Canonicalize(string path);

        protected override string GetStringRepresentation();

        protected bool IsSubPath(string path1, string path2);
    }

    public class PredicateConstraint<T> : Constraint
    {
        public override string Description { get; }

        public PredicateConstraint(System.Predicate<T> predicate);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public abstract class PrefixConstraint : Constraint
    {
        protected IConstraint BaseConstraint { get; set; }

        public override string Description { get; }

        protected string DescriptionPrefix { get; set; }

        protected PrefixConstraint(IResolveConstraint baseConstraint);
    }

    public abstract class PrefixOperator : ConstraintOperator
    {
        protected PrefixOperator();

        public abstract IConstraint ApplyPrefix(IConstraint constraint);

        public override void Reduce(ConstraintBuilder.ConstraintStack stack);
    }

    public class PropertyConstraint : PrefixConstraint
    {
        public PropertyConstraint(string name, IConstraint baseConstraint);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        protected override string GetStringRepresentation();
    }

    public class PropertyExistsConstraint : Constraint
    {
        public override string Description { get; }

        public PropertyExistsConstraint(string name);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        protected override string GetStringRepresentation();
    }

    public class PropOperator : SelfResolvingOperator
    {
        public string Name { get; }

        public PropOperator(string name);

        public override void Reduce(ConstraintBuilder.ConstraintStack stack);
    }

    public class RangeConstraint : Constraint
    {
        public override string Description { get; }

        public RangeConstraint(object from, object to);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        public RangeConstraint Using(System.Collections.IComparer comparer);

        public RangeConstraint Using<T>(System.Collections.Generic.IComparer<T> comparer);

        public RangeConstraint Using<T>(System.Comparison<T> comparer);
    }

    public class RegexConstraint : StringConstraint
    {
        public RegexConstraint(string pattern);

        protected override bool Matches(string actual);
    }

    public class ResolvableConstraintExpression : ConstraintExpression, IResolveConstraint
    {
        public ConstraintExpression And { get; }

        public ConstraintExpression Or { get; }

        public ResolvableConstraintExpression();

        public ResolvableConstraintExpression(ConstraintBuilder builder);
    }

    public class ReusableConstraint : IResolveConstraint
    {
        public ReusableConstraint(IResolveConstraint c);

        public IConstraint Resolve();

        public override string ToString();

        public static implicit operator ReusableConstraint(Constraint c);
    }

    public class SameAsConstraint : Constraint
    {
        public override string Description { get; }

        public SameAsConstraint(object expected);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public class SamePathConstraint : PathConstraint
    {
        public override string Description { get; }

        public SamePathConstraint(string expected);

        protected override bool Matches(string actual);
    }

    public class SamePathOrUnderConstraint : PathConstraint
    {
        public override string Description { get; }

        public SamePathOrUnderConstraint(string expected);

        protected override bool Matches(string actual);
    }

    public abstract class SelfResolvingOperator : ConstraintOperator
    {
        protected SelfResolvingOperator();
    }

    public class SomeItemsConstraint : PrefixConstraint
    {
        public override string DisplayName { get; }

        public SomeItemsConstraint(IConstraint itemConstraint);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        public SomeItemsConstraint Using<TCollectionType, TMemberType>(System.Func<TCollectionType, TMemberType, bool> comparison);

        public SomeItemsConstraint Using(System.Collections.IComparer comparer);

        public SomeItemsConstraint Using<T>(System.Collections.Generic.IComparer<T> comparer);

        public SomeItemsConstraint Using<T>(System.Comparison<T> comparer);

        public SomeItemsConstraint Using(System.Collections.IEqualityComparer comparer);

        public SomeItemsConstraint Using<T>(System.Collections.Generic.IEqualityComparer<T> comparer);
    }

    public class SomeOperator : CollectionOperator
    {
        public SomeOperator();

        public override IConstraint ApplyPrefix(IConstraint constraint);
    }

    public class StartsWithConstraint : StringConstraint
    {
        public StartsWithConstraint(string expected);

        protected override bool Matches(string actual);
    }

    public abstract class StringConstraint : Constraint
    {
        protected bool caseInsensitive;

        protected string descriptionText;

        protected string expected;

        public override string Description { get; }

        public virtual StringConstraint IgnoreCase { get; }

        protected StringConstraint();

        protected StringConstraint(string expected);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        protected abstract bool Matches(string actual);
    }

    public class SubPathConstraint : PathConstraint
    {
        public override string Description { get; }

        public SubPathConstraint(string expected);

        protected override bool Matches(string actual);
    }

    public class SubstringConstraint : StringConstraint
    {
        public override StringConstraint IgnoreCase { get; }

        public SubstringConstraint(string expected);

        protected override bool Matches(string actual);

        public SubstringConstraint Using(System.StringComparison comparisonType);
    }

    public class ThrowsConstraint : PrefixConstraint
    {
        public System.Exception ActualException { get; }

        public override string Description { get; }

        public ThrowsConstraint(IConstraint baseConstraint);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del);
    }

    public class ThrowsExceptionConstraint : Constraint
    {
        public override string Description { get; }

        public ThrowsExceptionConstraint();

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del);
    }

    public class ThrowsNothingConstraint : Constraint
    {
        public override string Description { get; }

        public ThrowsNothingConstraint();

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del);
    }

    public class ThrowsOperator : SelfResolvingOperator
    {
        public ThrowsOperator();

        public override void Reduce(ConstraintBuilder.ConstraintStack stack);
    }

    public class Tolerance
    {
        public static Tolerance Default { get; }

        public static Tolerance Exact { get; }

        public object Amount { get; }

        public Tolerance Days { get; }

        public Tolerance Hours { get; }

        public bool IsUnsetOrDefault { get; }

        public Tolerance Milliseconds { get; }

        public Tolerance Minutes { get; }

        public ToleranceMode Mode { get; }

        public Tolerance Percent { get; }

        public Tolerance Seconds { get; }

        public Tolerance Ticks { get; }

        public Tolerance Ulps { get; }

        public Tolerance(object amount);

        public Tolerance.Range ApplyToValue(object value);

        public class Range
        {
            public readonly object LowerBound;

            public readonly object UpperBound;

            public Range(object lowerBound, object upperBound);
        }
    }

    public enum ToleranceMode : int
    {
        Unset = 0,
        Linear = 1,
        Percent = 2,
        Ulps = 3
    }

    public class TrueConstraint : Constraint
    {
        public TrueConstraint();

        public override ConstraintResult ApplyTo<TActual>(TActual actual);
    }

    public abstract class TypeConstraint : Constraint
    {
        protected System.Type actualType;

        protected System.Type expectedType;

        protected TypeConstraint(System.Type type, string descriptionPrefix);

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        protected abstract bool Matches(object actual);
    }

    public class UniqueItemsConstraint : CollectionItemsEqualConstraint
    {
        public override string Description { get; }

        public UniqueItemsConstraint();

        protected override bool Matches(System.Collections.IEnumerable actual);
    }

    public delegate string ValueFormatter(object val);

    public delegate ValueFormatter ValueFormatterFactory(ValueFormatter next);

    public class WithOperator : PrefixOperator
    {
        public WithOperator();

        public override IConstraint ApplyPrefix(IConstraint constraint);
    }

    public class XmlSerializableConstraint : Constraint
    {
        public override string Description { get; }

        public XmlSerializableConstraint();

        public override ConstraintResult ApplyTo<TActual>(TActual actual);

        protected override string GetStringRepresentation();
    }
}
namespace NUnit.Framework.Interfaces
{
    public class AssertionResult
    {
        public string Message { get; }

        public string StackTrace { get; }

        public AssertionStatus Status { get; }

        public AssertionResult(AssertionStatus status, string message, string stackTrace);

        public override bool Equals(object obj);

        public override int GetHashCode();

        public override string ToString();
    }

    public enum AssertionStatus : int
    {
        Inconclusive = 0,
        Passed = 1,
        Warning = 2,
        Failed = 3,
        Error = 4
    }

    public class AttributeDictionary : System.Collections.Generic.Dictionary<string, string>
    {
        public string this[string key] { get; }

        public AttributeDictionary();
    }

    public enum FailureSite : int
    {
        Test = 0,
        SetUp = 1,
        TearDown = 2,
        Parent = 3,
        Child = 4
    }

    public interface IApplyToContext
    {
        void ApplyToContext(NUnit.Framework.Internal.TestExecutionContext context);
    }

    public interface IApplyToTest
    {
        void ApplyToTest(NUnit.Framework.Internal.Test test);
    }

    public interface ICombiningStrategy
    {
        System.Collections.Generic.IEnumerable<ITestCaseData> GetTestCases(System.Collections.IEnumerable[] sources);
    }

    public interface ICommandWrapper
    {
        NUnit.Framework.Internal.Commands.TestCommand Wrap(NUnit.Framework.Internal.Commands.TestCommand command);
    }

    public interface IFixtureBuilder
    {
        System.Collections.Generic.IEnumerable<NUnit.Framework.Internal.TestSuite> BuildFrom(ITypeInfo typeInfo);
    }

    public interface IImplyFixture
    {
    }

    public interface IMethodInfo : IReflectionInfo
    {
        bool ContainsGenericParameters { get; }

        bool IsAbstract { get; }

        bool IsGenericMethod { get; }

        bool IsGenericMethodDefinition { get; }

        bool IsPublic { get; }

        System.Reflection.MethodInfo MethodInfo { get; }

        string Name { get; }

        ITypeInfo ReturnType { get; }

        ITypeInfo TypeInfo { get; }

        System.Type[] GetGenericArguments();

        IParameterInfo[] GetParameters();

        object Invoke(object fixture, params object[] args);

        IMethodInfo MakeGenericMethod(params System.Type[] typeArguments);
    }

    public interface IParameterDataProvider
    {
        System.Collections.IEnumerable GetDataFor(IParameterInfo parameter);

        bool HasDataFor(IParameterInfo parameter);
    }

    public interface IParameterDataSource
    {
        System.Collections.IEnumerable GetData(IParameterInfo parameter);
    }

    public interface IParameterInfo : IReflectionInfo
    {
        bool IsOptional { get; }

        IMethodInfo Method { get; }

        System.Reflection.ParameterInfo ParameterInfo { get; }

        System.Type ParameterType { get; }
    }

    public interface IPropertyBag : IXmlNodeBuilder
    {
        System.Collections.IList this[string key] { get; set; }

        System.Collections.Generic.ICollection<string> Keys { get; }

        void Add(string key, object value);

        bool ContainsKey(string key);

        object Get(string key);

        void Set(string key, object value);
    }

    public interface IReflectionInfo
    {
        T[] GetCustomAttributes<T>(bool inherit) where T : class;

        bool IsDefined<T>(bool inherit);
    }

    public interface ISimpleTestBuilder
    {
        NUnit.Framework.Internal.TestMethod BuildFrom(IMethodInfo method, NUnit.Framework.Internal.Test suite);
    }

    public interface ISuiteBuilder
    {
        NUnit.Framework.Internal.TestSuite BuildFrom(ITypeInfo typeInfo);

        bool CanBuildFrom(ITypeInfo typeInfo);
    }

    public interface ITest : IXmlNodeBuilder
    {
        object[] Arguments { get; }

        string ClassName { get; }

        object Fixture { get; }

        string FullName { get; }

        bool HasChildren { get; }

        string Id { get; }

        bool IsSuite { get; }

        IMethodInfo Method { get; }

        string MethodName { get; }

        string Name { get; }

        ITest Parent { get; }

        IPropertyBag Properties { get; }

        RunState RunState { get; }

        int TestCaseCount { get; }

        System.Collections.Generic.IList<ITest> Tests { get; }

        string TestType { get; }

        ITypeInfo TypeInfo { get; }
    }

    public interface ITestBuilder
    {
        System.Collections.Generic.IEnumerable<NUnit.Framework.Internal.TestMethod> BuildFrom(IMethodInfo method, NUnit.Framework.Internal.Test suite);
    }

    public interface ITestCaseBuilder
    {
        NUnit.Framework.Internal.Test BuildFrom(IMethodInfo method, NUnit.Framework.Internal.Test suite);

        bool CanBuildFrom(IMethodInfo method, NUnit.Framework.Internal.Test suite);
    }

    public interface ITestCaseData : ITestData
    {
        object ExpectedResult { get; }

        bool HasExpectedResult { get; }
    }

    public interface ITestData
    {
        object[] Arguments { get; }

        IPropertyBag Properties { get; }

        RunState RunState { get; }

        string TestName { get; }
    }

    public interface ITestFilter : IXmlNodeBuilder
    {
        bool IsExplicitMatch(ITest test);

        bool Pass(ITest test);
    }

    public interface ITestFixtureData : ITestData
    {
        System.Type[] TypeArgs { get; }
    }

    public interface ITestListener
    {
        void TestFinished(ITestResult result);

        void TestOutput(TestOutput output);

        void TestStarted(ITest test);
    }

    public interface ITestResult : IXmlNodeBuilder
    {
        int AssertCount { get; }

        System.Collections.Generic.IList<AssertionResult> AssertionResults { get; }

        System.Collections.Generic.IEnumerable<ITestResult> Children { get; }

        double Duration { get; }

        System.DateTime EndTime { get; }

        int FailCount { get; }

        string FullName { get; }

        bool HasChildren { get; }

        int InconclusiveCount { get; }

        string Message { get; }

        string Name { get; }

        string Output { get; }

        int PassCount { get; }

        ResultState ResultState { get; }

        int SkipCount { get; }

        string StackTrace { get; }

        System.DateTime StartTime { get; }

        ITest Test { get; }

        System.Collections.Generic.ICollection<TestAttachment> TestAttachments { get; }

        int WarningCount { get; }
    }

    public interface ITypeInfo : IReflectionInfo
    {
        System.Reflection.Assembly Assembly { get; }

        ITypeInfo BaseType { get; }

        bool ContainsGenericParameters { get; }

        string FullName { get; }

        bool IsAbstract { get; }

        bool IsGenericType { get; }

        bool IsGenericTypeDefinition { get; }

        bool IsSealed { get; }

        bool IsStaticClass { get; }

        string Name { get; }

        string Namespace { get; }

        System.Type Type { get; }

        object Construct(object[] args);

        System.Reflection.ConstructorInfo GetConstructor(System.Type[] argTypes);

        string GetDisplayName();

        string GetDisplayName(object[] args);

        System.Type GetGenericTypeDefinition();

        IMethodInfo[] GetMethods(System.Reflection.BindingFlags flags);

        bool HasConstructor(System.Type[] argTypes);

        bool HasMethodWithAttribute(System.Type attrType);

        bool IsType(System.Type type);

        ITypeInfo MakeGenericType(System.Type[] typeArgs);
    }

    public interface IWrapSetUpTearDown : ICommandWrapper
    {
    }

    public interface IWrapTestMethod : ICommandWrapper
    {
    }

    public interface IXmlNodeBuilder
    {
        TNode AddToXml(TNode parentNode, bool recursive);

        TNode ToXml(bool recursive);
    }

    public class NodeList : System.Collections.Generic.List<TNode>
    {
        public NodeList();
    }

    public class ResultState
    {
        public static readonly ResultState Cancelled;

        public static readonly ResultState ChildFailure;

        public static readonly ResultState Error;

        public static readonly ResultState Explicit;

        public static readonly ResultState Failure;

        public static readonly ResultState Ignored;

        public static readonly ResultState Inconclusive;

        public static readonly ResultState NotRunnable;

        public static readonly ResultState SetUpError;

        public static readonly ResultState SetUpFailure;

        public static readonly ResultState Skipped;

        public static readonly ResultState Success;

        public static readonly ResultState TearDownError;

        public static readonly ResultState Warning;

        public string Label { get; }

        public FailureSite Site { get; }

        public TestStatus Status { get; }

        public ResultState(TestStatus status);

        public ResultState(TestStatus status, string label);

        public ResultState(TestStatus status, FailureSite site);

        public ResultState(TestStatus status, string label, FailureSite site);

        public override bool Equals(object obj);

        public override int GetHashCode();

        public bool Matches(ResultState other);

        public override string ToString();

        public ResultState WithSite(FailureSite site);

        public static bool operator ==(ResultState left, ResultState right);

        public static bool operator !=(ResultState left, ResultState right);
    }

    public enum RunState : int
    {
        NotRunnable = 0,
        Runnable = 1,
        Explicit = 2,
        Skipped = 3,
        Ignored = 4
    }

    public class TestAttachment
    {
        public string Description { get; }

        public string FilePath { get; }

        public TestAttachment(string filePath, string description);
    }

    public class TestOutput
    {
        public string Stream { get; }

        public string TestId { get; }

        public string TestName { get; }

        public string Text { get; }

        public TestOutput(string text, string stream, string testId, string testName);

        public override string ToString();

        public string ToXml();
    }

    public enum TestStatus : int
    {
        Inconclusive = 0,
        Skipped = 1,
        Passed = 2,
        Warning = 3,
        Failed = 4
    }

    public class TNode
    {
        public AttributeDictionary Attributes { get; }

        public NodeList ChildNodes { get; }

        public TNode FirstChild { get; }

        public string Name { get; }

        public string OuterXml { get; }

        public string Value { get; set; }

        public bool ValueIsCDATA { get; }

        public static TNode FromXml(string xmlText);

        public TNode(string name);

        public TNode(string name, string value);

        public TNode(string name, string value, bool valueIsCDATA);

        public void AddAttribute(string name, string value);

        public TNode AddElement(string name);

        public TNode AddElement(string name, string value);

        public TNode AddElementWithCDATA(string name, string value);

        public NodeList SelectNodes(string xpath);

        public TNode SelectSingleNode(string xpath);

        public void WriteTo(System.Xml.XmlWriter writer);
    }
}
namespace NUnit.Framework.Internal
{
    // Warning; type cannot be ignored because it is referenced by:
    //  - NUnit.Framework.TestContext.get_Random return type
    //  - NUnit.Framework.Internal.Randomizer.GetRandomizer return type
    //  - NUnit.Framework.Internal.Randomizer.GetRandomizer return type
    //  - NUnit.Framework.Internal.Randomizer.CreateRandomizer return type
    public class Randomizer : System.Random
    {
        public const string DefaultStringChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789_";

        public static int InitialSeed { get; set; }

        public static Randomizer CreateRandomizer();

        public static Randomizer GetRandomizer(System.Reflection.MemberInfo member);

        public static Randomizer GetRandomizer(System.Reflection.ParameterInfo parameter);

        public string GetString(int outputLength, string allowedChars);

        public string GetString(int outputLength);

        public string GetString();

        public bool NextBool();

        public bool NextBool(double probability);

        public byte NextByte();

        public byte NextByte(byte max);

        public byte NextByte(byte min, byte max);

        public decimal NextDecimal();

        public decimal NextDecimal(decimal max);

        public decimal NextDecimal(decimal min, decimal max);

        public double NextDouble(double max);

        public double NextDouble(double min, double max);

        public object NextEnum(System.Type type);

        public T NextEnum<T>();

        public float NextFloat();

        public float NextFloat(float max);

        public float NextFloat(float min, float max);

        public System.Guid NextGuid();

        public long NextLong();

        public long NextLong(long max);

        public long NextLong(long min, long max);

        [System.CLSCompliant(false)]
        public sbyte NextSByte();

        [System.CLSCompliant(false)]
        public sbyte NextSByte(sbyte max);

        [System.CLSCompliant(false)]
        public sbyte NextSByte(sbyte min, sbyte max);

        public short NextShort();

        public short NextShort(short max);

        public short NextShort(short min, short max);

        [System.CLSCompliant(false)]
        public uint NextUInt();

        [System.CLSCompliant(false)]
        public uint NextUInt(uint max);

        [System.CLSCompliant(false)]
        public uint NextUInt(uint min, uint max);

        [System.CLSCompliant(false)]
        public ulong NextULong();

        [System.CLSCompliant(false)]
        public ulong NextULong(ulong max);

        [System.CLSCompliant(false)]
        public ulong NextULong(ulong min, ulong max);

        [System.CLSCompliant(false)]
        public ushort NextUShort();

        [System.CLSCompliant(false)]
        public ushort NextUShort(ushort max);

        [System.CLSCompliant(false)]
        public ushort NextUShort(ushort min, ushort max);
    }

    // Warning; type cannot be ignored because it is referenced by:
    //  - NUnit.Framework.Internal.TestMethod base type
    //  - NUnit.Framework.Internal.Commands.TestCommand.get_Test return type
    //  - NUnit.Framework.Internal.TestSuite base type
    //  - NUnit.Framework.Interfaces.ITestCaseBuilder.BuildFrom return type
    public abstract class Test : NUnit.Framework.Interfaces.ITest, NUnit.Framework.Interfaces.IXmlNodeBuilder, System.IComparable
    {
        protected NUnit.Framework.Interfaces.ITypeInfo DeclaringTypeInfo;

        public static string IdPrefix { get; set; }

        public abstract object[] Arguments { get; }

        public string ClassName { get; }

        public virtual object Fixture { get; set; }

        public string FullName { get; set; }

        public abstract bool HasChildren { get; }

        public string Id { get; set; }

        public bool IsSuite { get; }

        public NUnit.Framework.Interfaces.IMethodInfo Method { get; set; }

        public virtual string MethodName { get; }

        public string Name { get; set; }

        public NUnit.Framework.Interfaces.ITest Parent { get; set; }

        public NUnit.Framework.Interfaces.IPropertyBag Properties { get; }

        public NUnit.Framework.Interfaces.RunState RunState { get; set; }

        public int Seed { get; set; }

        public System.Reflection.MethodInfo[] SetUpMethods { get; protected set; }

        public System.Reflection.MethodInfo[] TearDownMethods { get; protected set; }

        public virtual int TestCaseCount { get; }

        public abstract System.Collections.Generic.IList<NUnit.Framework.Interfaces.ITest> Tests { get; }

        public virtual string TestType { get; }

        public NUnit.Framework.Interfaces.ITypeInfo TypeInfo { get; }

        public abstract string XmlElementName { get; }

        public abstract NUnit.Framework.Interfaces.TNode AddToXml(NUnit.Framework.Interfaces.TNode parentNode, bool recursive);

        public void ApplyAttributesToTest(System.Reflection.ICustomAttributeProvider provider);

        public int CompareTo(object obj);

        public virtual TAttr[] GetCustomAttributes<TAttr>(bool inherit) where TAttr : class;

        public void MakeInvalid(string reason);

        public abstract TestResult MakeTestResult();

        protected void PopulateTestNode(NUnit.Framework.Interfaces.TNode thisNode, bool recursive);

        public NUnit.Framework.Interfaces.TNode ToXml(bool recursive);
    }

    // Warning; type cannot be ignored because it is referenced by:
    //  - NUnit.Framework.TestCaseData base type
    public class TestCaseParameters : TestParameters, NUnit.Framework.Interfaces.ITestCaseData, NUnit.Framework.Interfaces.ITestData, NUnit.Framework.Interfaces.IApplyToTest
    {
        public object ExpectedResult { get; set; }

        public bool HasExpectedResult { get; set; }
    }

    // Warning; type cannot be ignored because it is referenced by:
    //  - NUnit.Framework.TestFixtureData base type
    public class TestFixtureParameters : TestParameters, NUnit.Framework.Interfaces.ITestFixtureData, NUnit.Framework.Interfaces.ITestData
    {
        public System.Type[] TypeArgs { get; }
    }

    // Warning; type cannot be ignored because it is referenced by:
    //  - NUnit.Framework.CombiningStrategyAttribute.BuildFrom return type
    //  - NUnit.Framework.TestAttribute.BuildFrom return type
    //  - NUnit.Framework.TestCaseAttribute.BuildFrom return type
    //  - NUnit.Framework.TestCaseSourceAttribute.BuildFrom return type
    //  - NUnit.Framework.Interfaces.ISimpleTestBuilder.BuildFrom return type
    //  - NUnit.Framework.Interfaces.ITestBuilder.BuildFrom return type
    public class TestMethod : Test
    {
        public override object[] Arguments { get; }

        public override bool HasChildren { get; }

        public override string MethodName { get; }

        public override System.Collections.Generic.IList<NUnit.Framework.Interfaces.ITest> Tests { get; }

        public override string XmlElementName { get; }

        public override NUnit.Framework.Interfaces.TNode AddToXml(NUnit.Framework.Interfaces.TNode parentNode, bool recursive);

        public override TestResult MakeTestResult();
    }

    // Warning; type cannot be ignored because it is referenced by:
    //  - NUnit.Framework.Internal.TestCaseParameters base type
    //  - NUnit.Framework.Internal.TestFixtureParameters base type
    public abstract class TestParameters : NUnit.Framework.Interfaces.ITestData, NUnit.Framework.Interfaces.IApplyToTest
    {
        public object[] Arguments { get; }

        public object[] OriginalArguments { get; }

        public NUnit.Framework.Interfaces.IPropertyBag Properties { get; }

        public NUnit.Framework.Interfaces.RunState RunState { get; set; }

        public string TestName { get; set; }

        public void ApplyToTest(Test test);
    }

    // Warning; type cannot be ignored because it is referenced by:
    //  - NUnit.Framework.Internal.TestMethod.MakeTestResult return type
    //  - NUnit.Framework.Internal.Test.MakeTestResult return type
    //  - NUnit.Framework.Internal.Commands.TestCommand.Execute return type
    //  - NUnit.Framework.RepeatAttribute.RepeatedTestCommand.Execute return type
    //  - NUnit.Framework.RetryAttribute.RetryCommand.Execute return type
    //  - NUnit.Framework.Internal.TestSuite.MakeTestResult return type
    public abstract class TestResult : NUnit.Framework.Interfaces.ITestResult, NUnit.Framework.Interfaces.IXmlNodeBuilder
    {
        protected int InternalAssertCount;

        protected System.Threading.ReaderWriterLockSlim RwLock;

        public int AssertCount { get; }

        public System.Collections.Generic.IList<NUnit.Framework.Interfaces.AssertionResult> AssertionResults { get; }

        public abstract System.Collections.Generic.IEnumerable<NUnit.Framework.Interfaces.ITestResult> Children { get; }

        public double Duration { get; set; }

        public System.DateTime EndTime { get; set; }

        public abstract int FailCount { get; }

        public virtual string FullName { get; }

        public abstract bool HasChildren { get; }

        public abstract int InconclusiveCount { get; }

        public string Message { get; }

        public virtual string Name { get; }

        public string Output { get; }

        public System.IO.TextWriter OutWriter { get; }

        public abstract int PassCount { get; }

        public int PendingFailures { get; }

        public NUnit.Framework.Interfaces.ResultState ResultState { get; }

        public abstract int SkipCount { get; }

        public virtual string StackTrace { get; }

        public System.DateTime StartTime { get; set; }

        public NUnit.Framework.Interfaces.ITest Test { get; }

        public System.Collections.Generic.ICollection<NUnit.Framework.Interfaces.TestAttachment> TestAttachments { get; }

        public abstract int WarningCount { get; }

        public NUnit.Framework.Interfaces.AssertionStatus WorstAssertionStatus { get; }

        public virtual NUnit.Framework.Interfaces.TNode AddToXml(NUnit.Framework.Interfaces.TNode parentNode, bool recursive);

        public void RecordAssertion(NUnit.Framework.Interfaces.AssertionResult assertion);

        public void RecordAssertion(NUnit.Framework.Interfaces.AssertionStatus status, string message, string stackTrace);

        public void RecordAssertion(NUnit.Framework.Interfaces.AssertionStatus status, string message);

        public void RecordException(System.Exception ex);

        public void RecordException(System.Exception ex, NUnit.Framework.Interfaces.FailureSite site);

        public void RecordTearDownException(System.Exception ex);

        public void RecordTestCompletion();

        public void SetResult(NUnit.Framework.Interfaces.ResultState resultState);

        public void SetResult(NUnit.Framework.Interfaces.ResultState resultState, string message);

        public void SetResult(NUnit.Framework.Interfaces.ResultState resultState, string message, string stackTrace);

        public NUnit.Framework.Interfaces.TNode ToXml(bool recursive);
    }

    // Warning; type cannot be ignored because it is referenced by:
    //  - NUnit.Framework.SetUpFixtureAttribute.BuildFrom return type
    //  - NUnit.Framework.TestFixtureAttribute.BuildFrom return type
    //  - NUnit.Framework.TestFixtureSourceAttribute.BuildFrom return type
    //  - NUnit.Framework.Interfaces.IFixtureBuilder.BuildFrom return type
    //  - NUnit.Framework.Interfaces.ISuiteBuilder.BuildFrom return type
    public class TestSuite : Test
    {
        public override object[] Arguments { get; }

        public override bool HasChildren { get; }

        protected bool MaintainTestOrder { get; set; }

        public System.Reflection.MethodInfo[] OneTimeSetUpMethods { get; protected set; }

        public System.Reflection.MethodInfo[] OneTimeTearDownMethods { get; protected set; }

        public override int TestCaseCount { get; }

        public override System.Collections.Generic.IList<NUnit.Framework.Interfaces.ITest> Tests { get; }

        public override string XmlElementName { get; }

        public void Add(Test test);

        public override NUnit.Framework.Interfaces.TNode AddToXml(NUnit.Framework.Interfaces.TNode parentNode, bool recursive);

        protected void CheckSetUpTearDownMethods(System.Reflection.MethodInfo[] methods);

        public override TestResult MakeTestResult();

        public void Sort();
    }
}
namespace NUnit.Framework.Internal.Commands
{
    // Warning; type cannot be ignored because it is referenced by:
    //  - NUnit.Framework.RepeatAttribute.RepeatedTestCommand base type
    //  - NUnit.Framework.RetryAttribute.RetryCommand base type
    public abstract class DelegatingTestCommand : TestCommand
    {
        protected TestCommand innerCommand;
    }

    // Warning; type cannot be ignored because it is referenced by:
    //  - NUnit.Framework.RepeatAttribute.Wrap return type
    //  - NUnit.Framework.Internal.Commands.DelegatingTestCommand.innerCommand
    //  - NUnit.Framework.Internal.Commands.DelegatingTestCommand base type
    //  - NUnit.Framework.RetryAttribute.Wrap return type
    //  - NUnit.Framework.Interfaces.ICommandWrapper.Wrap return type
    public abstract class TestCommand
    {
        public NUnit.Framework.Internal.Test Test { get; }

        public abstract NUnit.Framework.Internal.TestResult Execute(NUnit.Framework.Internal.TestExecutionContext context);
    }
}
