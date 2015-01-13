
namespace System
{
    /// <summary>
    /// This is a trivial implementation of a Tuple with
    /// two members, sufficient to build mono.addins.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Tuple<T1, T2>
    {
        /// <summary>
        /// Gets the first item of the tuple
        /// </summary>
        public T1 Item1 { get; private set; }

        /// <summary>
        /// Gets the second item of the tuple
        /// </summary>
        public T2 Item2 { get; private set; }

        /// <summary>
        /// Constructs a two-item Tuple
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
}
