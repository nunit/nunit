using System;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// CommandDecoratorList maintains a list of ICommandDecorators
    /// and is able to sort them by level so that they are applied
    /// in the proper order.
    /// </summary>
    public class CommandDecoratorList : System.Collections.Generic.List<ICommandDecorator>
    {
        /// <summary>
        /// Order command decorators by the stage at which they apply.
        /// </summary>
        public void OrderByStage()
        {
            Sort(CommandDecoratorComparison);
        }

        private int CommandDecoratorComparison(ICommandDecorator x, ICommandDecorator y)
        {
            return x.Stage.CompareTo(y.Stage);
        }
    }
}
