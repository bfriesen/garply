using System.Collections;
using System.Collections.Generic;

namespace garply
{
    public class ExpressionBuilder : IEnumerable
    {
        public List<Instruction> Instructions { get; } = new List<Instruction>();

        public ExpressionBuilder Add(Instruction instruction)
        {
            Instructions.Add(instruction);
            return this;
        }

        public Expression Build()
        {
            return new Expression(Instructions.ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Instructions).GetEnumerator();
    }
}
