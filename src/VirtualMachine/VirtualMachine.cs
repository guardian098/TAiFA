using System.Globalization;

namespace VirtualMachine;

public class VirtualMachine
{
    private readonly IReadOnlyList<Instruction> _instructions;
    private readonly Stack<int> _stack;
    private readonly TextWriter _output;
    private int _ip;

    public VirtualMachine(IReadOnlyList<Instruction> instructions, TextWriter? output = null)
    {
        if (instructions == null)
        {
            throw new ArgumentNullException(nameof(instructions));
        }

        _instructions = instructions;
        _stack = new Stack<int>();
        _output = output ?? Console.Out;
        _ip = 0;
    }

    public void Run()
    {
        while (_ip < _instructions.Count)
        {
            Instruction instruction = _instructions[_ip++];
            try
            {
                switch (instruction.Code)
                {
                    case OpCode.Push:
                        _stack.Push(instruction.Operand);
                        break;

                    case OpCode.Pop:
                        Pop();
                        break;

                    case OpCode.Add:
                        {
                            int right = Pop();
                            int left = Pop();
                            _stack.Push(checked(left + right));
                        }

                        break;

                    case OpCode.Sub:
                        {
                            int right = Pop();
                            int left = Pop();
                            _stack.Push(checked(left - right));
                        }

                        break;

                    case OpCode.Mul:
                        {
                            int right = Pop();
                            int left = Pop();
                            _stack.Push(checked(left * right));
                        }

                        break;

                    case OpCode.Div:
                        {
                            int right = Pop();
                            int left = Pop();
                            if (right == 0)
                            {
                                throw new VirtualMachineException("Division by zero.");
                            }

                            _stack.Push(checked(left / right));
                        }

                        break;

                    case OpCode.Mod:
                        {
                            int right = Pop();
                            int left = Pop();
                            if (right == 0)
                            {
                                throw new VirtualMachineException("Modulo by zero.");
                            }

                            if (left == int.MinValue && right == -1)
                            {
                                throw new OverflowException();
                            }

                            _stack.Push(left % right);
                        }

                        break;

                    case OpCode.Neg:
                        {
                            int operand = Pop();
                            _stack.Push(checked(-operand));
                        }

                        break;

                    case OpCode.Abs:
                        {
                            int operand = Pop();
                            if (operand == int.MinValue)
                            {
                                throw new OverflowException();
                            }

                            _stack.Push(Math.Abs(operand));
                        }

                        break;

                    case OpCode.Sqrt:
                        {
                            int operand = Pop();
                            if (operand < 0)
                            {
                                throw new VirtualMachineException("Square root of a negative number.");
                            }

                            _stack.Push((int)Math.Sqrt(operand));
                        }

                        break;

                    case OpCode.Min:
                        {
                            int right = Pop();
                            int left = Pop();
                            _stack.Push(Math.Min(left, right));
                        }

                        break;

                    case OpCode.Max:
                        {
                            int right = Pop();
                            int left = Pop();
                            _stack.Push(Math.Max(left, right));
                        }

                        break;

                    case OpCode.Print:
                        {
                            int value = Pop();
                            _output.WriteLine(value.ToString(CultureInfo.InvariantCulture));
                        }

                        break;

                    default:
                        throw new NotImplementedException($"Unsupported instruction code: {instruction.Code}");
                }
            }
            catch (OverflowException ex)
            {
                throw new VirtualMachineException("Integer overflow during execution.", ex);
            }
        }
    }

    private int Pop()
    {
        if (_stack.Count == 0)
        {
            throw new VirtualMachineException("Stack underflow.");
        }

        return _stack.Pop();
    }
}