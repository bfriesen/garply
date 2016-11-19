<Query Kind="Program" />

void Main()
{
    var runtime = new Runtime();

    IModule fooModule = new Module("Foo", runtime);
    IModule barModule = new Module("Bar", runtime);

    fooModule.AddDependency(barModule);

    var bazBody = new Body
    {
        new Instruction(Operation.Push, -1000),
        new Instruction(Operation.PushArg),
        new Instruction(Operation.ArgItem, 0),
        new Instruction(Operation.Add),
        new Instruction(Operation.Return)
    };
    /*
    
    match arg
    (x number, y list) ->
    (x, _) ->
    () ->
    ([H | T]) ->
    _ ->
    
    */
    
    var bazCondition = new Body
    {
        new Instruction(Operation.PushArg),
        new Instruction(Operation.ArgArity),
        new Instruction(Operation.Push, 1),
        new Instruction(Operation.Equals),
        new Instruction(Operation.SkipIfFalse, 4),
        new Instruction(Operation.PushArg),
        new Instruction(Operation.ArgItem, 0),
//        new Instruction(Operation.IsNumber),
        new Instruction(Operation.IsList),
        new Instruction(Operation.Skip, 1),
        new Instruction(Operation.Push, false),
        new Instruction(Operation.Return)
    }; // func Baz (x Number) -> TODO: Body
    var bazFunction = new Function(barModule, "Baz", 1, bazBody, bazCondition);

    var mainBody = new Body
    {
        new Instruction(Operation.Push, 123),
        new Instruction(Operation.Push, 234),
        new Instruction(Operation.Add),
        new Instruction(Operation.Push, 345),
        new Instruction(Operation.Add),
        new Instruction(Operation.NewTuple, 1),
        new Instruction(Operation.Call, new CallInfo(barModule, "Baz", 1)),
        new Instruction(Operation.Return),
    };
    var mainFunction = Function.Main(fooModule, mainBody);

    fooModule.AddFunction(mainFunction).DumpTrace();
    barModule.AddFunction(bazFunction);

    bazBody = new Body
    {
        new Instruction(Operation.Push, 222222),
        new Instruction(Operation.PushArg),
        new Instruction(Operation.ArgItem, 0),
        new Instruction(Operation.Add),
        new Instruction(Operation.Return)
    };
    bazCondition = new Body
    {
        new Instruction(Operation.PushArg),
        new Instruction(Operation.ArgArity),
        new Instruction(Operation.Push, 1),
        new Instruction(Operation.Equals),
        new Instruction(Operation.SkipIfFalse, 4),
        new Instruction(Operation.PushArg),
        new Instruction(Operation.ArgItem, 0),
        new Instruction(Operation.IsNumber),
//        new Instruction(Operation.IsList),
        new Instruction(Operation.Skip, 1),
        new Instruction(Operation.Push, false),
        new Instruction(Operation.Return)
    }; // func Baz (x Number) -> TODO: Body
    bazFunction = new Function(barModule, "Baz", 1, bazBody, bazCondition);
    barModule.AddFunction(bazFunction);

    var interpreter = new Interpreter();

    var stopwatch = Stopwatch.StartNew();
    
    interpreter.Run(fooModule).Dump();
    
    stopwatch.Stop();
    stopwatch.Elapsed.Dump();

    fooModule.DumpTrace();
}

public class CallInfo
{
    public CallInfo(IModule module, string name, int arity)
    {
        Module = module;
        Name = name;
        Arity = arity;
    }

    public IModule Module { get; }
    public string Name { get; }
    public int Arity { get; }
}

public class Runtime
{
    public List<IModule> Modules { get; } = new List<IModule>();
}

public class XList
{
    public static readonly XList Empty = new XList(null, null, true);

    public XList(object head)
        : this(head, Empty, false)
    {
    }

    public XList(object head, XList tail)
        : this(head, tail, false)
    {
        Head = head;
        Tail = tail;
    }

    private XList(object head, XList tail, bool isEmpty)
    {
        Head = head;
        Tail = tail;
        IsEmpty = isEmpty;
    }

    public dynamic Head { get; }
    public XList Tail { get; }
    public bool IsEmpty { get; }

    public XList Push(dynamic value)
    {
        return new XList(value, this);
    }
}

internal static class ListExtensions
{
    public static XList ToList(this IEnumerable collection)
    {
        var list = XList.Empty;

        foreach (var item in collection)
        {
            list = list.Push(item);
        }

        return list;
    }
}

public class Interpreter
{
    public object Run(IModule module, params string[] args)
    {
        args = args ?? new string[0];

        var callStack = new Stack<Frame>();

        MatchedFunction main;
        if (module.TryMatchFunction("__main__", new XTuple(new dynamic[] { args.ToList() }), out main, callStack, -1))
        {
            main.DumpTrace();
            return main.Execute();
        }

        return new XTuple(new dynamic[] { "Error", "No main function found", callStack, -1 });
    }
}

public interface IModule
{
    string Name { get; }
    Runtime Runtime { get; }

    bool AddDependency(IModule module);
    bool AddFunction(Function function);

//    Result TryMatchFunction(string name, int arity, Stack evaluationStack, out MatchedFunction function);
//    Result TryMatchFunction(string name, XTuple arg, out MatchedFunction function);
    bool TryMatchFunction(string name, XTuple arg, out MatchedFunction function, Stack<Frame> callStack, int instructionIndex);
}

public class Module : IModule
{
    private Dictionary<Tuple<string, int>, List<Function>> _functionMap = new Dictionary<Tuple<string, int>, List<Function>>();

    public Module(string name, Runtime runtime)
    {
        Name = name;
        Runtime = runtime;
        runtime.Modules.Add(this);
    }

    public string Name { get; }
    public Runtime Runtime { get; }
    public IReadOnlyDictionary<Tuple<string, int>, List<Function>> FunctionMap { get { return _functionMap; } }

    public bool AddDependency(IModule module)
    {
        return true;
    }

    public bool AddFunction(Function function)
    {
        var key = Tuple.Create(function.Name, function.Arity);

        List<Function> functions;
        if (!_functionMap.TryGetValue(key, out functions))
        {
            functions = new List<Function>();
            _functionMap[key] = functions;
        }

        functions.Add(function);
        return true;
    }

    public bool TryMatchFunction(string name, XTuple arg, out MatchedFunction function, Stack<Frame> callStack, int instructionIndex)
    {
        List<Function> functions;
        if (_functionMap.TryGetValue(Tuple.Create(name, arg.Arity), out functions))
        {
            function = functions.Select(f => f.Matches(arg, callStack, instructionIndex)).FirstOrDefault(m => m.IsMatch);

            if (function == null)
            {
                return false;
            }

            return true;
        }

        function = null;
        return false;
    }
}

//public class Result
//{
//    private Result()
//    {
//    }
//
//    private Result(string errorMessage)
//    {
//        ErrorMessage = errorMessage;
//    }
//
//    public bool Success => ErrorMessage == null;
//    public string ErrorMessage { get; }
//
//    public static readonly Result Successful = new Result();
//
//    public static Result DuplicateFunction(string moduleName, string functionName, int arity)
//    {
//        return new Result($"Duplicate function: `{moduleName}.{functionName}/{arity}`");
//    }
//
//    public static Result MissingFunction(string moduleName, string functionName, int arity)
//    {
//        return new Result($"Missing function: `{moduleName}.{functionName}/{arity}`");
//    }
//
//    public static Result BadMatch(string moduleName, string functionName, object[] args)
//    {
//        return new Result($"Bad match: `{moduleName}.{functionName}/{args.Length}`");
//    }
//
//    public static Result BadMatch(string moduleName, string functionName, XTuple arg)
//    {
//        return new Result($"Bad match: `{moduleName}.{functionName}/{arg.Arity}`");
//    }
//}

public interface ICaller
{
    IModule Module { get; }
    string Name { get; }
    int Arity { get; }
}

public class Function
{
    private readonly Body Condition;

    public Function(IModule module, string name, int arity, Body body, Body condition)
    {
        Module = module;
        Name = name;
        Arity = arity;
        Body = body;
        Condition = condition;
    }

    public static Function Main(IModule module, Body body)
    {
        return new Function(module, "__main__", 1, body, MatchCondition.Main);
    }

    public IModule Module { get; }
    public string Name { get; }
    public int Arity { get; }
    public Body Body { get; }

    public MatchedFunction Matches(XTuple arg, Stack<Frame> callStack, int instructionIndex)
    {
        return Condition.Evaluate(arg, callStack)[0]
            ? new MatchedFunction(this, arg, callStack, instructionIndex)
            : MatchedFunction.NoMatch(callStack, instructionIndex);
    }
}

public static class MatchCondition
{
    public static readonly Body Main = GetMainMatchCondition();

    private static Body GetMainMatchCondition()
    {
        return new Body
        {
            new Instruction(Operation.PushArg),
            new Instruction(Operation.ArgArity),
            new Instruction(Operation.Push, 1),
            new Instruction(Operation.Equals),
            new Instruction(Operation.SkipIfFalse, 4),
            new Instruction(Operation.PushArg),
            new Instruction(Operation.ArgItem, 0),
            new Instruction(Operation.IsList),
            new Instruction(Operation.Skip, 1),
            new Instruction(Operation.Push, false),
            new Instruction(Operation.Return)
        };
    }
}

public class Variable
{
    public Variable(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public dynamic Value { get; set; }
}

public class XTuple
{
    private readonly IReadOnlyList<dynamic> _items;
    
    public XTuple(Stack stack, int arity)
        : this(GetItems(stack, arity))
    {
    }
    
    public static XTuple FromStack(Stack stack, int arity)
    {
        if (arity == 1 && stack.Count == 1 && stack.Peek() is XTuple)
        {
            return (XTuple)stack.Pop();
        }
        
        return new XTuple(stack, arity);
    }
    
    private static IReadOnlyList<dynamic> GetItems(Stack stack, int arity)
    {
        var items = new dynamic[arity];
        for (int i = arity - 1; i >= 0; i--)
        {
            items[i] = stack.Pop();
        }
        return items;
    }

    public XTuple(IReadOnlyList<dynamic> items)
    {
        _items = items;
    }

    public dynamic this[int index] => _items[index];
    public IReadOnlyList<dynamic> Items => _items;
    public int Arity => _items.Count;
}

public class MatchedFunction
{
    public MatchedFunction(Function function, XTuple arg, Stack<Frame> callStack, int callerInstructionIndex)
    {
        Function = function;
        Arg = arg;
        CallStack = callStack;
        CallerInstructionIndex = callerInstructionIndex;
    }

    public static MatchedFunction NoMatch(Stack<Frame> callStack, int callerInstructionIndex)
    {
        return new MatchedFunction(null, null, callStack, callerInstructionIndex);
    }

    public Function Function { get; }
    public XTuple Arg { get; }
    public Stack<Frame> CallStack { get; }
    public int CallerInstructionIndex { get; }
    public bool IsMatch => Function != null;

    public object Execute()
    {
        try
        {
            CallStack.Push(new Frame(Function, CallerInstructionIndex));
            return Function.Body.Evaluate(Arg, CallStack);
        }
        finally
        {
            CallStack.Pop();
        }
    }
}

public class Frame
{
    public Frame(Function function, int callerInstructionIndex)
    {
        Function = function;
        CallerInstructionIndex = callerInstructionIndex;
    }

    public Function Function { get; }
    public int CallerInstructionIndex { get; }
}

public class Body : IEnumerable
{
    private readonly List<Instruction> _instructions = new List<Instruction>();
    public IReadOnlyList<Instruction> Instructions { get { return _instructions; } }

    public void Add(Instruction operation)
    {
        _instructions.Add(operation);
    }

    public dynamic Evaluate(XTuple arg, Stack<Frame> callStack)
    {
        var stack = new Stack();

        for (int i = 0; i < _instructions.Count; i++)
        {
            var instruction = _instructions[i];

            switch (instruction.Operation)
            {
                case Operation.IsNumber:
                    {
                        var instance = stack.Pop();
                        stack.Push(instance is int);
                        break;
                    }
                case Operation.IsList:
                    {
                        var instance = stack.Pop();
                        stack.Push(instance is XList);
                        break;
                    }
                case Operation.IsTuple:
                    {
                        var instance = stack.Pop();
                        stack.Push(instance is XTuple);
                        break;
                    }
                case Operation.Skip:
                    {
                        i += instruction.Operand;
                        break;
                    }
                case Operation.SkipIfFalse:
                    {
                        if (!(bool)stack.Pop())
                        {
                            i += instruction.Operand;
                        }

                        break;
                    }
                case Operation.Equals:
                    {
                        var rhs = stack.Pop();
                        var lhs = stack.Pop();
                        stack.Push(object.Equals(lhs, rhs));
                        break;
                    }
                case Operation.ArgArity:
                    {
                        XTuple tuple = (XTuple)stack.Pop();
                        stack.Push(tuple.Arity);
                        break;
                    }
                case Operation.ArgItem:
                    {
                        XTuple tuple = (XTuple)stack.Pop();
                        stack.Push(tuple[instruction.Operand]);
                        break;
                    }
                case Operation.NewTuple:
                    {
                        stack.Push(new XTuple(stack, instruction.Operand));
                        break;
                    }
                case Operation.Return:
                    {
                        return XTuple.FromStack(stack, stack.Count);
                    }
                case Operation.PushArg:
                    {
                        stack.Push(arg);
                        break;
                    }
                case Operation.Push:
                    {
                        stack.Push(instruction.Operand);
                        break;
                    }
                case Operation.Add:
                    {
                        var rhs = stack.Pop();
                        var lhs = stack.Pop();
                        stack.Push((int)lhs + (int)rhs);
                        break;
                    }
                case Operation.Call:
                    {
                        var callInfo = (CallInfo)instruction.Operand;

                        MatchedFunction matchedFunction;

                        if (callInfo.Module.TryMatchFunction(callInfo.Name, (XTuple)stack.Pop(), out matchedFunction, callStack, i))
                        {
                            stack.Push(matchedFunction.Execute());
                        }
                        else
                        {
                            stack.Clear();
                            return new XTuple(new dynamic[] { "Error", "No match", new List<Frame>(callStack), i });
                        }

                        break;
                    }
            }
        }

        return 0;
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_instructions).GetEnumerator();
}

public class Instruction
{
    public Instruction(Operation operation, dynamic operand = null)
    {
        Operation = operation;
        Operand = operand;
    }

    public Operation Operation { get; }
    public dynamic Operand { get; }
}

public enum Operation : byte
{
    Return,
    Push,
    PushArg,
    Add,
    Call,
    ArgArity,
    ArgItem,
    NewTuple,
    Equals,
    IsList,
    IsNumber,
    IsBoolean,
    IsTuple,
    And,
    Skip,
    SkipIfFalse,
}