using System.Collections;
using System.Collections.Generic;

namespace Garply
{
    internal class ExpressionBuilder : IEnumerable
    {
        public Types Type { get; set; }
        public List<Instruction> Instructions { get; } = new List<Instruction>();

        public ExpressionBuilder SetType(Types type)
        {
            Type = type;
            return this;
        }

        public ExpressionBuilder Add(Instruction instruction)
        {
            Instructions.Add(instruction);
            return this;
        }

        public Expression Build()
        {
            return new Expression(Type, Instructions.ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Instructions).GetEnumerator();
    }
}
