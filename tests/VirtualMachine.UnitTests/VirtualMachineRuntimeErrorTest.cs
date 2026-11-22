using System;
using System.Collections.Generic;
using System.IO;

using VirtualMachine;

using Xunit;

namespace VirtualMachine.UnitTests;

public class VirtualMachineRuntimeErrorTest
{
    [Theory]
    [MemberData(nameof(GetRuntimeErrorData))]
    public void CanReportRuntimeErrorWithMessage(List<Instruction> program, string expectedMessage)
    {
        VirtualMachineException exception = Assert.Throws<VirtualMachineException>(() => Execute(program));
        Assert.Contains(expectedMessage, exception.Message);
    }

    [Theory]
    [MemberData(nameof(GetSignedDivisionData))]
    public void CanExecuteSignedDivisionAndModulo(List<Instruction> program, string expectedOutput)
    {
        Assert.Equal(expectedOutput, Execute(program));
    }

    [Fact]
    public void KeepsOutputPrintedBeforeError()
    {
        List<Instruction> program = new List<Instruction>
        {
            new Instruction(OpCode.Push, 1),
            new Instruction(OpCode.Print),
            new Instruction(OpCode.Push, 1),
            new Instruction(OpCode.Push, 0),
            new Instruction(OpCode.Div),
        };

        StringWriter output = new StringWriter();
        VirtualMachine virtualMachine = new VirtualMachine(program, output);

        Assert.Throws<VirtualMachineException>(() => virtualMachine.Run());
        Assert.Equal("1" + Environment.NewLine, output.ToString());
    }

    public static TheoryData<List<Instruction>, string> GetRuntimeErrorData()
    {
        return new TheoryData<List<Instruction>, string>
        {
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 10),
                    new Instruction(OpCode.Push, 0),
                    new Instruction(OpCode.Mod),
                },
                "Modulo by zero"
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, int.MinValue),
                    new Instruction(OpCode.Push, -1),
                    new Instruction(OpCode.Mod),
                },
                "overflow"
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, int.MinValue),
                    new Instruction(OpCode.Push, -1),
                    new Instruction(OpCode.Div),
                },
                "overflow"
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, int.MaxValue),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Mul),
                },
                "overflow"
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, int.MinValue),
                    new Instruction(OpCode.Push, 1),
                    new Instruction(OpCode.Sub),
                },
                "overflow"
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Pop),
                },
                "Stack underflow"
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 1),
                    new Instruction(OpCode.Add),
                },
                "Stack underflow"
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 1),
                    new Instruction(OpCode.Min),
                },
                "Stack underflow"
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Neg),
                },
                "Stack underflow"
            },
            {
                new List<Instruction>
                {
                    new Instruction((OpCode)999),
                },
                "Unsupported instruction"
            },
        };
    }

    public static TheoryData<List<Instruction>, string> GetSignedDivisionData()
    {
        return new TheoryData<List<Instruction>, string>
        {
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, -7),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Div),
                    new Instruction(OpCode.Print),
                },
                "-3" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, -7),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Mod),
                    new Instruction(OpCode.Print),
                },
                "-1" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 7),
                    new Instruction(OpCode.Push, -2),
                    new Instruction(OpCode.Mod),
                    new Instruction(OpCode.Print),
                },
                "1" + Environment.NewLine
            },
        };
    }

    private static string Execute(List<Instruction> program)
    {
        StringWriter output = new StringWriter();
        VirtualMachine virtualMachine = new VirtualMachine(program, output);
        virtualMachine.Run();
        return output.ToString();
    }
}