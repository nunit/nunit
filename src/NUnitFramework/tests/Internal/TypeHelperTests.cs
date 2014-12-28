namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class TypeHelperTests
    {
        public class A
        {
        }

        public class B : A
        {
        }

        public void BestCommonTypeTest()
        {
            Assert.That( new TestCaseData( TypeHelper.NonmatchingType, typeof( object ) ), Is.EqualTo( TypeHelper.NonmatchingType ) );
            Assert.That( new TestCaseData( typeof( object ), TypeHelper.NonmatchingType ), Is.EqualTo( TypeHelper.NonmatchingType ) );
            Assert.That( new TestCaseData( typeof( A ), typeof( B ) ), Is.EqualTo( typeof( A ) ) );
            Assert.That( new TestCaseData( typeof( B ), typeof( A ) ), Is.EqualTo( typeof( A ) ) );
            Assert.That( new TestCaseData( typeof( A ), typeof( string ) ), Is.EqualTo( TypeHelper.NonmatchingType ) );
        }
    }
}
