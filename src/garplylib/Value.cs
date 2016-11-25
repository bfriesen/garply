using System.Diagnostics;
using System.IO;

namespace Garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Value
    {
        public readonly Types Type;
        public readonly object Raw;

        public Boolean AsBoolean => (Boolean)Raw;
        public Float AsFloat => (Float)Raw;
        public Integer AsInteger => (Integer)Raw;
        public List AsList => (List)Raw;
        public String AsString => (String)Raw;
        public Tuple AsTuple => (Tuple)Raw;
        public TypeValue AsType => (TypeValue)Raw;

        public static Value Error { get; } = new Value(false);
        public static Value EmptyOperand { get; } = new Value(true);

        public void Write(Opcode opcode, BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            Debug.Assert((Type & Types.Operand) == Types.Operand);
            switch (Type & ~Types.Operand)
            {
                case Types.Error: break;
                case Types.Boolean:
                case Types.Float:
                case Types.Integer:
                case Types.List:
                case Types.String:
                case Types.Tuple:
                case Types.Type: ((IOperand)Raw).Write(opcode, writer, metadataDatabase); break;
                default: Debug.Fail($"Unknown/unwritable type: {Type}"); break;
            }
        }

        #region Numerous constructors

        private Value(bool isEmptyOperand)
        {
            Type = isEmptyOperand ? Types.Operand : default(Types);
            Raw = null;
        }

        public Value(Boolean boolean)
        {
            Debug.Assert(boolean != null);
            Type = Types.Boolean;
            Raw = boolean;
        }

        public Value(Boolean boolean, bool isOperand)
        {
            Debug.Assert(boolean != null);
            Type = Types.Boolean | (isOperand ? Types.Operand : 0);
            Raw = boolean;
        }

        public Value(Float @float)
        {
            Debug.Assert(@float != null);
            Type = Types.Float;
            Raw = @float;
        }

        public Value(Float @float, bool isOperand)
        {
            Debug.Assert(@float != null);
            Type = Types.Float | (isOperand ? Types.Operand : 0);
            Raw = @float;
        }

        public Value(Integer integer)
        {
            Debug.Assert(integer != null);
            Type = Types.Integer;
            Raw = integer;
        }

        public Value(Integer integer, bool isOperand)
        {
            Debug.Assert(integer != null);
            Type = Types.Integer | (isOperand ? Types.Operand : 0);
            Raw = integer;
        }

        public Value(List list)
        {
            Debug.Assert(list != null);
            Type = Types.List;
            Raw = list;
        }

        public Value(List list, bool isOperand)
        {
            Debug.Assert(list != null);
            Type = Types.List | (isOperand ? Types.Operand : 0);
            Raw = list;
        }

        public Value(String @string)
        {
            Debug.Assert(@string != null);
            Type = Types.String;
            Raw = @string;
        }

        public Value(String @string, bool isOperand)
        {
            Debug.Assert(@string != null);
            Type = Types.String | (isOperand ? Types.Operand : 0);
            Raw = @string;
        }

        public Value(Tuple tuple)
        {
            Debug.Assert(tuple != null);
            Type = Types.Tuple;
            Raw = tuple;
        }

        public Value(Tuple tuple, bool isOperand)
        {
            Debug.Assert(tuple != null);
            Type = Types.Tuple | (isOperand ? Types.Operand : 0);
            Raw = tuple;
        }

        public Value(TypeValue typeValue)
        {
            Debug.Assert(typeValue != null);
            Type = Types.Type;
            Raw = typeValue;
        }

        public Value(TypeValue typeValue, bool isOperand)
        {
            Debug.Assert(typeValue != null);
            Type = Types.Type | (isOperand ? Types.Operand : 0);
            Raw = typeValue;
        }

        #endregion

        internal string DebuggerDisplay
        {
            get
            {
                string valueString;
                switch (Type & ~Types.Operand)
                {
                    case Types.Boolean: valueString = AsBoolean.DebuggerDisplay; break;
                    case Types.Error: return "error";
                    case Types.Float: valueString = AsFloat.DebuggerDisplay; break;
                    case Types.Integer: valueString = AsInteger.DebuggerDisplay; break;
                    case Types.List: valueString = AsList.DebuggerDisplay; break;
                    case Types.String: valueString = AsString.DebuggerDisplay; break;
                    case Types.Tuple: valueString = AsTuple.DebuggerDisplay; break;
                    case Types.Type: valueString = AsType.DebuggerDisplay; break;
                    default: return $"Unknown Type: {Type}";
                }
                return $"{Type}:{valueString}";
            }
        }
    }
}
