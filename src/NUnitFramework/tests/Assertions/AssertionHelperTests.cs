using System;
using System.IO;
using NUnit.Framework.Constraints;
using NUnit.TestUtilities.Comparers;

namespace NUnit.Framework.Syntax
{
    [Obsolete("Test of Obsolete AssertionHelper class")]
    class AssertionHelperTests : AssertionHelper
    {
        private static readonly string DEFAULT_PATH_CASE = Path.DirectorySeparatorChar == '\\' ? "ignorecase" : "respectcase";

        #region Not

        [Test]
        public void NotNull()
        {
            var expression = Not.Null;
            Expect(expression, TypeOf<NullConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<NotConstraint>());
            Expect(constraint.ToString(), EqualTo("<not <null>>"));
        }

        [Test]
        public void NotNotNotNull()
        {
            var expression = Not.Not.Not.Null;
            Expect(expression, TypeOf<NullConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<NotConstraint>());
            Expect(constraint.ToString(), EqualTo("<not <not <not <null>>>>"));
        }

        #endregion

        #region All

        [Test]
        public void AllItems()
        {
            var expression = All.GreaterThan(0);
            Expect(expression, TypeOf<GreaterThanConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<AllItemsConstraint>());
            Expect(constraint.ToString(), EqualTo("<all <greaterthan 0>>"));
        }

        #endregion

        #region Some

        [Test]
        public void Some_EqualTo()
        {
            var expression = Some.EqualTo(3);
            Expect(expression, TypeOf<EqualConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<SomeItemsConstraint>());
            Expect(constraint.ToString(), EqualTo("<some <equal 3>>"));
        }

        [Test]
        public void Some_BeforeBinaryOperators()
        {
            var expression = Some.GreaterThan(0).And.LessThan(100).Or.EqualTo(999);
            Expect(expression, TypeOf<EqualConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<SomeItemsConstraint>());
            Expect(constraint.ToString(), EqualTo("<some <or <and <greaterthan 0> <lessthan 100>> <equal 999>>>"));
        }

        [Test]
        public void Some_Nested()
        {
            var expression = Some.With.Some.LessThan(100);
            Expect(expression, TypeOf<LessThanConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<SomeItemsConstraint>());
            Expect(constraint.ToString(), EqualTo("<some <some <lessthan 100>>>"));
        }

        [Test]
        public void Some_AndSome()
        {
            var expression = Some.GreaterThan(0).And.Some.LessThan(100);
            Expect(expression, TypeOf<LessThanConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<AndConstraint>());
            Expect(constraint.ToString(), EqualTo("<and <some <greaterthan 0>> <some <lessthan 100>>>"));
        }

        #endregion

        #region None

        [Test]
        public void NoItems()
        {
            var expression = None.LessThan(0);
            Expect(expression, TypeOf<LessThanConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<NoItemConstraint>());
            Expect(constraint.ToString(), EqualTo("<none <lessthan 0>>"));
        }

        #endregion

        #region Property

        [Test]
        public void Property()
        {
            var expression = Property("X");
            Expect(expression, TypeOf<ResolvableConstraintExpression>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<PropertyExistsConstraint>());
            Expect(constraint.ToString(), EqualTo("<propertyexists X>"));
        }

        [Test]
        public void Property_FollowedByAnd()
        {
            var expression = Property("X").And.EqualTo(7);
            Expect(expression, TypeOf<EqualConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<AndConstraint>());
            Expect(constraint.ToString(), EqualTo("<and <propertyexists X> <equal 7>>"));
        }

        [Test]
        public void Property_FollowedByConstraint()
        {
            var expression = Property("X").GreaterThan(5);
            Expect(expression, TypeOf<GreaterThanConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<PropertyConstraint>());
            Expect(constraint.ToString(), EqualTo("<property X <greaterthan 5>>"));
        }

        [Test]
        public void Property_FollowedByNot()
        {
            var expression = Property("X").Not.GreaterThan(5);
            Expect(expression, TypeOf<GreaterThanConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<PropertyConstraint>());
            Expect(constraint.ToString(), EqualTo("<property X <not <greaterthan 5>>>"));
        }

        [Test]
        public void LengthProperty()
        {
            var expression = Length.GreaterThan(5);
            Expect(expression, TypeOf<GreaterThanConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<PropertyConstraint>());
            Expect(constraint.ToString(), EqualTo("<property Length <greaterthan 5>>"));
        }

        [Test]
        public void CountProperty()
        {
            var expression = Count.EqualTo(5);
            Expect(expression, TypeOf<EqualConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<PropertyConstraint>());
            Expect(constraint.ToString(), EqualTo("<property Count <equal 5>>"));
        }

        [Test]
        public void MessageProperty()
        {
            var expression = Message.StartsWith("Expected");
            Expect(expression, TypeOf<StartsWithConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<PropertyConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<property Message <startswith ""Expected"">>"));
        }

        #endregion

        #region Attribute

        [Test]
        public void AttributeExistsTest()
        {
            var expression = Attribute(typeof(TestFixtureAttribute));
            Expect(expression, TypeOf<ResolvableConstraintExpression>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<AttributeExistsConstraint>());
            Expect(constraint.ToString(), EqualTo("<attributeexists NUnit.Framework.TestFixtureAttribute>"));
        }

        [Test]
        public void AttributeTest_FollowedByConstraint()
        {
            var expression = Attribute(typeof(TestFixtureAttribute)).Property("Description").Not.Null;
            Expect(expression, TypeOf<NullConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<AttributeConstraint>());
            Expect(constraint.ToString(), EqualTo("<attribute NUnit.Framework.TestFixtureAttribute <property Description <not <null>>>>"));
        }

        [Test]
        public void AttributeExistsTest_Generic()
        {
            var expression = Attribute<TestFixtureAttribute>();
            Expect(expression, TypeOf<ResolvableConstraintExpression>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<AttributeExistsConstraint>());
            Expect(constraint.ToString(), EqualTo("<attributeexists NUnit.Framework.TestFixtureAttribute>"));
        }

        [Test]
        public void AttributeTest_FollowedByConstraint_Generic()
        {
            var expression = Attribute<TestFixtureAttribute>().Property("Description").Not.Null;
            Expect(expression, TypeOf<NullConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<AttributeConstraint>());
            Expect(constraint.ToString(), EqualTo("<attribute NUnit.Framework.TestFixtureAttribute <property Description <not <null>>>>"));
        }

        #endregion

        #region Null

        [Test]
        public void NullTest()
        {
            var constraint = Null;
            Expect(constraint, TypeOf<NullConstraint>());
            Expect(constraint.ToString(), EqualTo("<null>"));
        }

        #endregion

        #region True

        [Test]
        public void TrueTest()
        {
            var constraint = True;
            Expect(constraint, TypeOf<TrueConstraint>());
            Expect(constraint.ToString(), EqualTo("<true>"));
        }

        #endregion

        #region False

        [Test]
        public void FalseTest()
        {
            var constraint = False;
            Expect(constraint, TypeOf<FalseConstraint>());
            Expect(constraint.ToString(), EqualTo("<false>"));
        }

        #endregion

        #region Positive

        [Test]
        public void PositiveTest()
        {
            var constraint = Positive;
            Expect(constraint, TypeOf<GreaterThanConstraint>());
            Expect(constraint.ToString(), EqualTo("<greaterthan 0>"));
        }

        #endregion

        #region Negative

        [Test]
        public void NegativeTest()
        {
            var constraint = Negative;
            Expect(constraint, TypeOf<LessThanConstraint>());
            Expect(constraint.ToString(), EqualTo("<lessthan 0>"));
        }

        #endregion

        #region Zero

        [Test]
        public void ZeroTest()
        {
            var constraint = Zero;
            Expect(constraint, TypeOf<EqualConstraint>());
            Expect(constraint.ToString(), EqualTo("<equal 0>"));
        }

        #endregion

        #region NaN

        [Test]
        public void NaNTest()
        {
            var constraint = NaN;
            Expect(constraint, TypeOf<NaNConstraint>());
            Expect(constraint.ToString(), EqualTo("<nan>"));
        }

        #endregion

        #region After
        
        [Test]
        public void After()
        {
            var constraint = EqualTo(10).After(1000);
            Expect(constraint, TypeOf<DelayedConstraint.WithRawDelayInterval>());
            Expect(constraint.ToString(), EqualTo("<after 1000 <equal 10>>"));
        }

        [Test]
        public void After_Property()
        {
            var constraint = Property("X").EqualTo(10).After(1000);
            Expect(constraint, TypeOf<DelayedConstraint.WithRawDelayInterval>());
            Expect(constraint.ToString(), EqualTo("<after 1000 <property X <equal 10>>>"));
        }

        [Test]
        public void After_And()
        {
            var constraint = GreaterThan(0).And.LessThan(10).After(1000);
            Expect(constraint, TypeOf<DelayedConstraint.WithRawDelayInterval>());
            Expect(constraint.ToString(), EqualTo("<after 1000 <and <greaterthan 0> <lessthan 10>>>"));
        }

        #endregion

        #region Unique

        [Test]
        public void UniqueItems()
        {
            var constraint = Unique;
            Expect(constraint, TypeOf<UniqueItemsConstraint>());
            Expect(constraint.ToString(), EqualTo("<uniqueitems>"));
        }

        #endregion

        #region Ordered

        [Test]
        public void CollectionOrdered()
        {
            var constraint = Ordered;
            Expect(constraint, TypeOf<CollectionOrderedConstraint>());
            Expect(constraint.ToString(), EqualTo("<ordered>"));
        }

        [Test]
        public void CollectionOrdered_Descending()
        {
            var constraint = Ordered.Descending;
            Expect(constraint, TypeOf<CollectionOrderedConstraint>());
            Expect(constraint.ToString(), EqualTo("<ordered descending>"));
        }

        [Test]
        public void CollectionOrdered_Comparer()
        {
            var constraint = Ordered.Using(ObjectComparer.Default);
            Expect(constraint, TypeOf<CollectionOrderedConstraint>());
            Expect(constraint.ToString(), EqualTo("<ordered NUnit.TestUtilities.Comparers.ObjectComparer>"));
        }

        [Test]
        public void CollectionOrdered_Comparer_Descending()
        {
            var constraint = Ordered.Using(ObjectComparer.Default).Descending;
            Expect(constraint, TypeOf<CollectionOrderedConstraint>());
            Expect(constraint.ToString(), EqualTo("<ordered descending NUnit.TestUtilities.Comparers.ObjectComparer>"));
        }

        #endregion

        #region OrderedBy

        [Test]
        public void CollectionOrderedBy()
        {
            var constraint = Ordered.By("SomePropertyName");
            Expect(constraint, TypeOf<CollectionOrderedConstraint>());
            Expect(constraint.ToString(), EqualTo("<orderedby SomePropertyName>"));
        }

        [Test]
        public void CollectionOrderedBy_Descending()
        {
            var constraint = Ordered.By("SomePropertyName").Descending;
            Expect(constraint, TypeOf<CollectionOrderedConstraint>());
            Expect(constraint.ToString(), EqualTo("<orderedby SomePropertyName descending>"));
        }

        [Test]
        public void CollectionOrderedBy_Comparer()
        {
            var constraint = Ordered.By("SomePropertyName").Using(ObjectComparer.Default);
            Expect(constraint, TypeOf<CollectionOrderedConstraint>());
            Expect(constraint.ToString(), EqualTo("<orderedby SomePropertyName NUnit.TestUtilities.Comparers.ObjectComparer>"));
        }

        [Test]
        public void CollectionOrderedBy_Comparer_Descending()
        {
            var constraint = Ordered.By("SomePropertyName").Using(ObjectComparer.Default).Descending;
            Expect(constraint, TypeOf<CollectionOrderedConstraint>());
            Expect(constraint.ToString(), EqualTo("<orderedby SomePropertyName descending NUnit.TestUtilities.Comparers.ObjectComparer>"));
        }

        #endregion

        #region Contains

        [Test]
        public void ContainsConstraint()
        {
            var constraint = Contains(42);
            Expect(constraint, TypeOf<SomeItemsConstraint>());
            Expect(constraint.ToString(), EqualTo("<some <equal 42>>"));
        }

        [Test]
        public void ContainsConstraintWithOr()
        {
            var constraint = Contains(5).Or.Contains(7);
            var collection1 = new[] { 3, 5, 1, 8 };
            var collection2 = new[] { 2, 7, 9, 2 };
            Assert.That(collection1, constraint);
            Assert.That(collection2, constraint);
        }

        [Test]
        public void ContainsConstraintWithUsing()
        {
            Func<int, int, bool> myIntComparer = (x, y) => x == y;
            var constraint = Contains(5).Using(myIntComparer);
            var collection = new[] { 3, 5, 1, 8 };
            Assert.That(collection, constraint);
        }

        #endregion

        #region Member

        [Test]
        public void MemberConstraintWithAnd()
        {
            var constraint = Member(1).And.Member(7);
            var collection = new[] { 2, 1, 7, 4 };
            Assert.That(collection, constraint);
        }

        [Test]
        public void MemberConstraintWithUsing()
        {
            Func<int, int, bool> myIntComparer = (x, y) => x == y;
            var constraint = Member(1).Using(myIntComparer);
            var collection = new[] { 2, 1, 7, 4 };
            Assert.That(collection, constraint);
        }

        #endregion

        #region SubsetOf

        [Test]
        public void SubsetOfConstraint()
        {
            var constraint = SubsetOf(new int[] { 1, 2, 3 });
            Expect(constraint, TypeOf<CollectionSubsetConstraint>());
            Expect(constraint.ToString(), EqualTo("<subsetof System.Int32[]>"));
        }

        #endregion

        #region EquivalentTo

        [Test]
        public void EquivalentConstraint()
        {
            var constraint = EquivalentTo(new int[] { 1, 2, 3 });
            Expect(constraint, TypeOf<CollectionEquivalentConstraint>());
            Expect(constraint.ToString(), EqualTo("<equivalent System.Int32[]>"));
        }

        #endregion

        #region ComparisonConstraints

        [Test]
        public void GreaterThan()
        {
            var constraint = GreaterThan(7);
            Expect(constraint, TypeOf<GreaterThanConstraint>());
            Expect(constraint.ToString(), EqualTo("<greaterthan 7>"));
        }

        [Test]
        public void GreaterThanOrEqual()
        {
            var constraint = GreaterThanOrEqualTo(7);
            Expect(constraint, TypeOf<GreaterThanOrEqualConstraint>());
            Expect(constraint.ToString(), EqualTo("<greaterthanorequal 7>"));
        }

        [Test]
        public void AtLeast()
        {
            var constraint = AtLeast(7);
            Expect(constraint, TypeOf<GreaterThanOrEqualConstraint>());
            Expect(constraint.ToString(), EqualTo("<greaterthanorequal 7>"));
        }

        [Test]
        public void LessThan()
        {
            var constraint = LessThan(7);
            Expect(constraint, TypeOf<LessThanConstraint>());
            Expect(constraint.ToString(), EqualTo("<lessthan 7>"));
        }

        [Test]
        public void LessThanOrEqual()
        {
            var constraint = LessThanOrEqualTo(7);
            Expect(constraint, TypeOf<LessThanOrEqualConstraint>());
            Expect(constraint.ToString(), EqualTo("<lessthanorequal 7>"));
        }

        [Test]
        public void AtMost()
        {
            var constraint = AtMost(7);
            Expect(constraint, TypeOf<LessThanOrEqualConstraint>());
            Expect(constraint.ToString(), EqualTo("<lessthanorequal 7>"));
        }

        #endregion

        #region EqualTo

        [Test]
        public void EqualConstraint()
        {
            var constraint = EqualTo(999);
            Expect(constraint, TypeOf<EqualConstraint>());
            Expect(constraint.ToString(), EqualTo("<equal 999>"));
        }

        [Test]
        public void Equal_IgnoreCase()
        {
            var constraint = EqualTo("X").IgnoreCase;
            Expect(constraint, TypeOf<EqualConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<equal ""X"">"));
        }

        [Test]
        public void Equal_WithinTolerance()
        {
            var constraint = EqualTo(0.7).Within(.005);
            Expect(constraint, TypeOf<EqualConstraint>());
            Expect(constraint.ToString(), EqualTo("<equal 0.7>"));
        }

        #endregion

        #region And

        [Test]
        public void AndConstraint()
        {
            var expression = GreaterThan(5).And.LessThan(10);
            Expect(expression, TypeOf<LessThanConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<AndConstraint>());
            Expect(constraint.ToString(), EqualTo("<and <greaterthan 5> <lessthan 10>>"));
        }

        [Test]
        public void AndConstraint_ThreeAndsWithNot()
        {
            var expression = Not.Null.And.Not.LessThan(5).And.Not.GreaterThan(10);
            Expect(expression, TypeOf<GreaterThanConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<AndConstraint>());
            Expect(constraint.ToString(), EqualTo("<and <not <null>> <and <not <lessthan 5>> <not <greaterthan 10>>>>"));
        }

        #endregion

        #region Or

        [Test]
        public void OrConstraint()
        {
            var expression = LessThan(5).Or.GreaterThan(10);
            Expect(expression, TypeOf<GreaterThanConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<OrConstraint>());
            Expect(constraint.ToString(), EqualTo("<or <lessthan 5> <greaterthan 10>>"));
        }

        [Test]
        public void OrConstraint_ThreeOrs()
        {
            var expression = LessThan(5).Or.GreaterThan(10).Or.EqualTo(7);
            Expect(expression, TypeOf<EqualConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<OrConstraint>());
            Expect(constraint.ToString(), EqualTo("<or <lessthan 5> <or <greaterthan 10> <equal 7>>>"));
        }

        [Test]
        public void OrConstraint_PrecededByAnd()
        {
            var expression = LessThan(100).And.GreaterThan(0).Or.EqualTo(999);
            Expect(expression, TypeOf<EqualConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<OrConstraint>());
            Expect(constraint.ToString(), EqualTo("<or <and <lessthan 100> <greaterthan 0>> <equal 999>>"));
        }

        [Test]
        public void OrConstraint_FollowedByAnd()
        {
            var expression = EqualTo(999).Or.GreaterThan(0).And.LessThan(100);
            Expect(expression, TypeOf<LessThanConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<OrConstraint>());
            Expect(constraint.ToString(), Is.EqualTo("<or <equal 999> <and <greaterthan 0> <lessthan 100>>>"));
        }

        #endregion

        #region SamePath

        [Test]
        public void SamePath()
        {
            var constraint = SamePath("/path/to/match");
            Expect(constraint, TypeOf<SamePathConstraint>());
            Expect(constraint.ToString(), EqualTo(
                string.Format(@"<samepath ""/path/to/match"" {0}>", DEFAULT_PATH_CASE)));
        }

        [Test]
        public void SamePath_IgnoreCase()
        {
            var constraint = SamePath("/path/to/match").IgnoreCase;
            Expect(constraint, TypeOf<SamePathConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<samepath ""/path/to/match"" ignorecase>"));
        }

        [Test]
        public void NotSamePath_IgnoreCase()
        {
            var expression = Not.SamePath("/path/to/match").IgnoreCase;
            Expect(expression, TypeOf<SamePathConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<NotConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<not <samepath ""/path/to/match"" ignorecase>>"));
        }

        [Test]
        public void SamePath_RespectCase()
        {
            var constraint = SamePath("/path/to/match").RespectCase;
            Expect(constraint, TypeOf<SamePathConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<samepath ""/path/to/match"" respectcase>"));
        }

        [Test]
        public void NotSamePath_RespectCase()
        {
            var expression = Not.SamePath("/path/to/match").RespectCase;
            Expect(expression, TypeOf<SamePathConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<NotConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<not <samepath ""/path/to/match"" respectcase>>"));
        }

        #endregion

        #region SamePathOrUnder

        [Test]
        public void SamePathOrUnder()
        {
            var constraint = SamePathOrUnder("/path/to/match");
            Expect(constraint, TypeOf<SamePathOrUnderConstraint>());
            Expect(constraint.ToString(), EqualTo(
                string.Format(@"<samepathorunder ""/path/to/match"" {0}>", DEFAULT_PATH_CASE)));
        }

        [Test]
        public void SamePathOrUnder_IgnoreCase()
        {
            var constraint = SamePathOrUnder("/path/to/match").IgnoreCase;
            Expect(constraint, TypeOf<SamePathOrUnderConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<samepathorunder ""/path/to/match"" ignorecase>"));
        }

        [Test]
        public void NotSamePathOrUnder_IgnoreCase()
        {
            var expression = Not.SamePathOrUnder("/path/to/match").IgnoreCase;
            Expect(expression, TypeOf<SamePathOrUnderConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<NotConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<not <samepathorunder ""/path/to/match"" ignorecase>>"));
        }

        [Test]
        public void SamePathOrUnder_RespectCase()
        {
            var constraint = SamePathOrUnder("/path/to/match").RespectCase;
            Expect(constraint, TypeOf<SamePathOrUnderConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<samepathorunder ""/path/to/match"" respectcase>"));
        }

        [Test]
        public void NotSamePathOrUnder_RespectCase()
        {
            var expression = Not.SamePathOrUnder("/path/to/match").RespectCase;
            Expect(expression, TypeOf<SamePathOrUnderConstraint>());
            var constraint = Resolve(expression);
            Expect(constraint, TypeOf<NotConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<not <samepathorunder ""/path/to/match"" respectcase>>"));
        }

        #endregion

        #region BinarySerializable

#if !NETCOREAPP1_1
        [Test]
        public void BinarySerializableConstraint()
        {
            var constraint = BinarySerializable;
            Expect(constraint, TypeOf<BinarySerializableConstraint>());
            Expect(constraint.ToString(), EqualTo("<binaryserializable>"));
        }
#endif

        #endregion

        #region XmlSerializable

#if !NETCOREAPP1_1
        [Test]
        public void XmlSerializableConstraint()
        {
            var constraint = XmlSerializable;
            Expect(constraint, TypeOf<XmlSerializableConstraint>());
            Expect(constraint.ToString(), EqualTo("<xmlserializable>"));
        }
#endif

        #endregion

        #region Contains

        [Test]
        public void ContainsTest()
        {
            var constraint = Contains("X");
            Expect(constraint, TypeOf<ContainsConstraint>());
            Expect(constraint.ToString(), EqualTo("<contains>"));
        }

        [Test]
        public void ContainsTest_IgnoreCase()
        {
            var constraint = Contains("X").IgnoreCase;
            Expect(constraint, TypeOf<ContainsConstraint>());
            Expect(constraint.ToString(), EqualTo("<contains>"));
        }

#endregion

#region StartsWith

        [Test]
        public void StartsWithTest()
        {
            var constraint = StartsWith("X");
            Expect(constraint, TypeOf<StartsWithConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<startswith ""X"">"));
        }

        [Test]
        public void StartsWithTest_IgnoreCase()
        {
            var constraint = StartsWith("X").IgnoreCase;
            Expect(constraint, TypeOf<StartsWithConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<startswith ""X"">"));
        }

#endregion

#region EndsWith

        [Test]
        public void EndsWithTest()
        {
            var constraint = EndsWith("X");
            Expect(constraint, TypeOf<EndsWithConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<endswith ""X"">"));
        }

        [Test]
        public void EndsWithTest_IgnoreCase()
        {
            var constraint = EndsWith("X").IgnoreCase;
            Expect(constraint, TypeOf<EndsWithConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<endswith ""X"">"));
        }

#endregion

#region Matches

        [Test]
        public void MatchesTest()
        {
            var constraint = Matches("X");
            Expect(constraint, TypeOf<RegexConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<regex ""X"">"));
        }

        [Test]
        public void MatchesTest_IgnoreCase()
        {
            var constraint = Matches("X").IgnoreCase;
            Expect(constraint, TypeOf<RegexConstraint>());
            Expect(constraint.ToString(), EqualTo(@"<regex ""X"">"));
        }

#endregion

#region TypeOf

        [Test]
        public void TypeOfTest()
        {
            var constraint = TypeOf(typeof(string));
            Expect(constraint, TypeOf<ExactTypeConstraint>());
            Expect(constraint.ToString(), EqualTo("<typeof System.String>"));
        }

        [Test]
        public void TypeOfTest_Generic()
        {
            var constraint = TypeOf<string>();
            Expect(constraint, TypeOf<ExactTypeConstraint>());
            Expect(constraint.ToString(), EqualTo("<typeof System.String>"));
        }

#endregion

#region InstanceOf

        [Test]
        public void InstanceOfTest()
        {
            var constraint = InstanceOf(typeof(string));
            Expect(constraint, TypeOf<InstanceOfTypeConstraint>());
            Expect(constraint.ToString(), EqualTo("<instanceof System.String>"));
        }

        [Test]
        public void InstanceOfTest_Generic()
        {
            var constraint = InstanceOf<string>();
            Expect(constraint, TypeOf<InstanceOfTypeConstraint>());
            Expect(constraint.ToString(), EqualTo("<instanceof System.String>"));
        }

#endregion

#region AssignableFrom

        [Test]
        public void AssignableFromTest()
        {
            var constraint = AssignableFrom(typeof(string));
            Expect(constraint, TypeOf<AssignableFromConstraint>());
            Expect(constraint.ToString(), EqualTo("<assignablefrom System.String>"));
        }

        [Test]
        public void AssignableFromTest_Generic()
        {
            var constraint = AssignableFrom<string>();
            Expect(constraint, TypeOf<AssignableFromConstraint>());
            Expect(constraint.ToString(), EqualTo("<assignablefrom System.String>"));
        }

#endregion

#region AssignableTo

        [Test]
        public void AssignableToTest()
        {
            var constraint = AssignableTo(typeof(string));
            Expect(constraint, TypeOf<AssignableToConstraint>());
            Expect(constraint.ToString(), EqualTo("<assignableto System.String>"));
        }

        [Test]
        public void AssignableToTest_Generic()
        {
            var constraint = AssignableTo<string>();
            Expect(constraint, TypeOf<AssignableToConstraint>());
            Expect(constraint.ToString(), EqualTo("<assignableto System.String>"));
        }

#endregion

#region Operator Overrides

        [Test]
        public void NotOperator()
        {
            var constraint = !Null;
            Expect(constraint, TypeOf<NotConstraint>());
            Expect(constraint.ToString(), EqualTo("<not <null>>"));
        }

        [Test]
        public void AndOperator()
        {
            var constraint = GreaterThan(5) & LessThan(10);
            Expect(constraint, TypeOf<AndConstraint>());
            Expect(constraint.ToString(), EqualTo("<and <greaterthan 5> <lessthan 10>>"));
        }

        [Test]
        public void OrOperator()
        {
            var constraint = LessThan(5) | GreaterThan(10);
            Expect(constraint, TypeOf<OrConstraint>());
            Expect(constraint.ToString(), EqualTo("<or <lessthan 5> <greaterthan 10>>"));
        }

#endregion

#region Helper Methods

        private static IConstraint Resolve(IResolveConstraint expression)
        {
            return ((IResolveConstraint)expression).Resolve();
        }

#endregion
    }
}
