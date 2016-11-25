using System;
using System.Diagnostics;
using System.IO;

namespace Garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class TypeValue : IOperand
    {
        private TypeValue(Types type)
        {
            Type = type;
        }

        public static TypeValue Error { get; } = new TypeValue(Types.Error);
        public static TypeValue Value { get; } = new TypeValue(Types.Value);
        public static TypeValue Boolean { get; } = new TypeValue(Types.Boolean);
        public static TypeValue String { get; } = new TypeValue(Types.String);
        public static TypeValue Number { get; } = new TypeValue(Types.Number);
        public static TypeValue Integer { get; } = new TypeValue(Types.Integer);
        public static TypeValue Float { get; } = new TypeValue(Types.Float);
        public static TypeValue Tuple { get; } = new TypeValue(Types.Tuple);
        public static TypeValue List { get; } = new TypeValue(Types.List);

        public static TypeValue Get(Types type)
        {
            switch (type)
            {
                case Types.Error: return Error;
                case Types.Value: return Value;
                case Types.Boolean: return Boolean;
                case Types.String: return String;
                case Types.Number: return Number;
                case Types.Integer: return Integer;
                case Types.Float: return Float;
                case Types.Tuple: return Tuple;
                case Types.List: return List;
                default: throw new ArgumentOutOfRangeException("type");
            }
        }

        public Types Type { get; }

        public void Write(Opcode opcode, BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            Debug.Assert(Type != Types.Error);
            writer.Write((uint)Type);
        }

        internal string DebuggerDisplay => Type.ToString();
    }
}
