using System;
using System.Collections.Generic;
using System.IO;

using VirtualMachine;

using Xunit;

namespace VirtualMachine.UnitTests;

public class VirtualMachineInputTest
{
    [Theory]
    [MemberData(nameof(GetSingleIntegerData))]
    public void CanReadSingleInteger(string input, string expected)
    {
        List<Instruction> program = new List<Instruction>
        {
            new Instruction(OpCode.Read),
            new Instruction(OpCode.Print),
        };

        Assert.Equal(expected + Environment.NewLine, Execute(program, input));
    }

    [Fact]
    public void ReadsIntegersInOrderAcrossSeparators()
    {
        List<Instruction> program = new List<Instruction>
        {
            new Instruction(OpCode.Read),
            new Instruction(OpCode.Print),
            new Instruction(OpCode.Read),
            new Instruction(OpCode.Print),
            new Instruction(OpCode.Read),
            new Instruction(OpCode.Print),
            new Instruction(OpCode.Read),
            new Instruction(OpCode.Print),
        };

        string expected = string.Join(Environment.NewLine, "1", "2", "3", "4") + Environment.NewLine;

        Assert.Equal(expected, Execute(program, "1 2\n3\r\n  4"));
    }

    [Fact]
    public void ReadResultParticipatesInArithmetic()
    {
        List<Instruction> program = new List<Instruction>
        {
            new Instruction(OpCode.Read),
            new Instruction(OpCode.Read),
            new Instruction(OpCode.Sub),
            new Instruction(OpCode.Print),
        };

        Assert.Equal("7" + Environment.NewLine, Execute(program, "10 3"));
    }

    [Fact]
    public void ReadResultCanBeDiscarded()
    {
        List<Instruction> program = new List<Instruction>
        {
            new Instruction(OpCode.Read),
            new Instruction(OpCode.Pop),
        };

        Assert.Equal(string.Empty, Execute(program, "5"));
    }

    [Fact]
    public void ReadsOnlyRequestedNumber()
    {
        List<Instruction> program = new List<Instruction>
        {
            new Instruction(OpCode.Read),
            new Instruction(OpCode.Pop),
        };

        using StringReader input = new StringReader("1 2");
        new VirtualMachine(program, new StringWriter(), input).Run();

        Assert.Equal("2", input.ReadToEnd());
    }

    [Theory]
    [MemberData(nameof(GetEndOfInputData))]
    public void ReportsEndOfInput(string input)
    {
        List<Instruction> program = new List<Instruction>
        {
            new Instruction(OpCode.Read),
        };

        VirtualMachineException exception = Assert.Throws<VirtualMachineException>(() => Execute(program, input));
        Assert.Contains("end of input", exception.Message);
    }

    [Fact]
    public void ReportsEndOfInputWhenNumbersRunOut()
    {
        List<Instruction> program = new List<Instruction>
        {
            new Instruction(OpCode.Read),
            new Instruction(OpCode.Read),
        };

        VirtualMachineException exception = Assert.Throws<VirtualMachineException>(() => Execute(program, "1"));
        Assert.Contains("end of input", exception.Message);
    }

    [Theory]
    [MemberData(nameof(GetInvalidIntegerData))]
    public void ReportsInvalidInteger(string input)
    {
        List<Instruction> program = new List<Instruction>
        {
            new Instruction(OpCode.Read),
        };

        VirtualMachineException exception = Assert.Throws<VirtualMachineException>(() => Execute(program, input));
        Assert.Contains("Invalid integer", exception.Message);
    }

    [Fact]
    public void KeepsOutputPrintedBeforeInputError()
    {
        List<Instruction> program = new List<Instruction>
        {
            new Instruction(OpCode.Push, 1),
            new Instruction(OpCode.Print),
            new Instruction(OpCode.Read),
        };

        StringWriter output = new StringWriter();
        VirtualMachine virtualMachine = new VirtualMachine(program, output, new StringReader(string.Empty));

        Assert.Throws<VirtualMachineException>(() => virtualMachine.Run());
        Assert.Equal("1" + Environment.NewLine, output.ToString());
    }

    public static TheoryData<string, string> GetSingleIntegerData()
    {
        return new TheoryData<string, string>
        {
            { "5", "5" },
            { "  42  ", "42" },
            { "-7\n", "-7" },
            { "+7\r\n", "7" },
            { "\n\n  13", "13" },
            { "2147483647", "2147483647" },
            { "-2147483648", "-2147483648" },
        };
    }

    public static TheoryData<string> GetEndOfInputData()
    {
        return new TheoryData<string>
        {
            { "" },
            { "   \n\r\n" },
        };
    }

    public static TheoryData<string> GetInvalidIntegerData()
    {
        return new TheoryData<string>
        {
            { "abc" },
            { "12abc" },
            { "1.5" },
            { "--5" },
            { "2147483648" },
            { "-2147483649" },
        };
    }

    private static string Execute(List<Instruction> program, string input)
    {
        StringWriter output = new StringWriter();
        VirtualMachine virtualMachine = new VirtualMachine(program, output, new StringReader(input));
        virtualMachine.Run();
        return output.ToString();
    }
}