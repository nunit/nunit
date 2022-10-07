// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Operator that tests for the presence of a particular attribute
    /// on a type and optionally applies further tests to the attribute.
    /// </summary>
    public class AttributeOperator : SelfResolvingOperator
    {
        private readonly Type type;

        /// <summary>
        /// Construct an AttributeOperator for a particular Type
        /// </summary>
        /// <param name="type">The Type of attribute tested</param>
        public AttributeOperator(Type type)
        {
            this.type = type;

            // Attribute stacks on anything and allows only 
            // prefix operators to stack on it.
            this.left_precedence = this.right_precedence = 1;
        }

        /// <summary>
        /// Reduce produces a constraint from the operator and 
        /// any arguments. It takes the arguments from the constraint 
        /// stack and pushes the resulting constraint on it.
        /// </summary>
        public override void Reduce(ConstraintBuilder.ConstraintStack stack)
        {
            if (RightContext is null or BinaryOperator)
                stack.Push(new AttributeExistsConstraint(type));
            else
                stack.Push(new AttributeConstraint(type, stack.Pop()));
        }
    }
 }