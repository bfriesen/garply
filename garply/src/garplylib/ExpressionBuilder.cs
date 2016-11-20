using System.Collections;
using System.Collections.Generic;

namespace garply
{
    public class ExpressionBuilder : IEnumerable
    {
        private readonly List<Instruction> _instructions = new List<Instruction>();

        public ExpressionBuilder Add(Instruction instruction)
        {
            _instructions.Add(instruction);
            return this;
        }

        public Expression Build()
        {
            return new Expression(_instructions.ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_instructions).GetEnumerator();
    }
}
