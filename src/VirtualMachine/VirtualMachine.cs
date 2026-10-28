namespace VirtualMachine;

public class VirtualMachine
{
    private readonly IReadOnlyList<Instruction> _instructions;
    private readonly Stack<int> _stack;
    private int _ip;
    public VirtualMachine(IReadOnlyList<Instruction> instructions)
    {
        _instructions = instructions;
        _stack = new Stack<int>();
        _ip = 0;
    }

    public void Run() { }
}
