using System.Collections;
using System.Collections.Generic;

namespace garply
{
    public class ExpressionBuilder : IEnumerable
    {
        public byte Arity { get; private set; }
        public byte Constants { get; private set; }
        public byte Variables { get; private set; }
        public List<Instruction> Instructions { get; } = new List<Instruction>();

        public ExpressionBuilder SetArity(byte arity)
        {
            Arity = arity;
            return this;
        }

        public ExpressionBuilder SetConstants(byte constants)
        {
            Constants = constants;
            return this;
        }

        public ExpressionBuilder SetVariables(byte variables)
        {
            Variables = variables;
            return this;
        }

        public ExpressionBuilder Add(Instruction instruction)
        {
            Instructions.Add(instruction);
            return this;
        }

        public Expression Build()
        {
            return new Expression(Instructions.ToArray(), Arity, Constants, Variables);
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Instructions).GetEnumerator();
    }
}
