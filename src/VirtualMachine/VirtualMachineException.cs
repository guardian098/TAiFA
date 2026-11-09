namespace VirtualMachine;

public sealed class VirtualMachineException : Exception
{
    public VirtualMachineException()
    {
    }

    public VirtualMachineException(string message)
        : base(message)
    {
    }

    public VirtualMachineException(string message, Exception inner)
        : base(message, inner)
    {
    }
}