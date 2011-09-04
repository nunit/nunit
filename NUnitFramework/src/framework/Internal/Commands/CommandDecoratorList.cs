using System;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// CommandDecoratorList maintains a list of ICommandDecorators
    /// and is able to sort them by level so that they are applied
    /// in the proper order.
    /// </summary>
#if CLR_2_0 || CLR_4_0
    public class CommandDecoratorList : System.Collections.Generic.List<ICommandDecorator>
    {
        public void OrderByStage()
        {
            Sort((x, y) => x.Stage.CompareTo(y.Stage));
        }
    }
#else
    public class CommandDecoratorList : System.Collections.ArrayList
    {
        private static CommandDecoratorComparer levelComparer = new CommandDecoratorComparer();

        public void SortByStageAndPriority()
        {
            Sort(levelComparer);
        }

        private class CommandDecoratorComparer : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                ICommandDecorator d1 = x as ICommandDecorator;
                ICommandDecorator d2 = y as ICommandDecorator;

                if (d1 == null && d2 == null) return 0;
                if (d1 == null) return -1;
                if (d2 == null) return +1;

                return d1.Level.CompareTo(d2.Level);
            }
        }
    }
#endif
}
