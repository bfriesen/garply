namespace Garply
{
    internal partial struct Instruction
    {
        public static Instruction FromTuple(Tuple tuple) => new Instruction((Opcode)(ushort)tuple.Items[0].Raw, tuple.Items[1]);
        public static Instruction FromOpcodeAndOperand(Opcode opcode, Value operand) => new Instruction(opcode, operand);
        public static Instruction Nop() => new Instruction(Opcode.Nop);
        public static Instruction LoadInteger(long value) => LoadInteger(new Value(value));
        public static Instruction LoadInteger(Value intValue) => new Instruction(Opcode.LoadInteger, intValue);
        public static Instruction LoadFloat(double value) => LoadFloat(new Value(value));
        public static Instruction LoadFloat(Value floatValue) => new Instruction(Opcode.LoadFloat, floatValue);
        public static Instruction True() => LoadBoolean(true);
        public static Instruction False() => LoadBoolean(false);
        public static Instruction LoadBoolean(bool value) => LoadBoolean(new Value(value));
        public static Instruction LoadBoolean(Value boolValue) => new Instruction(Opcode.LoadBoolean, boolValue);
        public static Instruction LoadType(Types type) => LoadType(new Value(type));
        public static Instruction LoadType(Value typeValue) => new Instruction(Opcode.LoadType, typeValue);
        public static Instruction LoadOpcode(Opcode opcode) => LoadOpcode(new Value(opcode));
        public static Instruction LoadOpcode(Value opcodeValue) => new Instruction(Opcode.LoadOpcode, opcodeValue);
        public static Instruction LoadString(long id) => LoadString(new Value(Types.@string, id));
        public static Instruction LoadString(Value stringValue) => new Instruction(Opcode.LoadString, stringValue);
        public static new Instruction GetType() => new Instruction(Opcode.GetType);
        public static Instruction TypeIs() => new Instruction(Opcode.TypeIs);
        public static Instruction TypeEquals() => new Instruction(Opcode.TypeEquals);
        public static Instruction TupleArity() => new Instruction(Opcode.TupleArity);
        public static Instruction TupleItem(int index) => TupleItem(new Value(index));
        public static Instruction TupleItem(Value indexValue) => new Instruction(Opcode.TupleItem, indexValue);
        public static Instruction NewTuple(int arity) => NewTuple(new Value(arity));
        public static Instruction NewTuple(Value arityValue) => new Instruction(Opcode.NewTuple, arityValue);
        public static Instruction NewExpression(int instructionCount) => NewExpression(new Value(instructionCount));
        public static Instruction NewExpression(Value instructionCountValue) => new Instruction(Opcode.NewExpression, instructionCountValue);
        public static Instruction ListEmpty() => new Instruction(Opcode.ListEmpty);
        public static Instruction ListAdd() => new Instruction(Opcode.ListAdd);
        public static Instruction ListHead() => new Instruction(Opcode.ListHead);
        public static Instruction ListTail() => new Instruction(Opcode.ListTail);
        public static Instruction AssignVariable(int variableIndex, bool mutable) => AssignVariable(new Value(variableIndex), mutable);
        public static Instruction AssignVariable(Value variableIndexValue, bool mutable) => new Instruction(mutable ? Opcode.AssignMutableVariable : Opcode.AssignVariable, variableIndexValue);
        public static Instruction ReadVariable(int variableIndex) => ReadVariable(new Value(variableIndex));
        public static Instruction ReadVariable(Value variableIndexValue) => new Instruction(Opcode.ReadVariable, variableIndexValue);
        public static Instruction EvaluateExpression() => new Instruction(Opcode.EvaluateExpression);
    }
}
