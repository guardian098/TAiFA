using System;
using System.Collections.Generic;
using System.IO;

using VirtualMachine;

using Xunit;

namespace VirtualMachine.UnitTests;

public class VirtualMachineTest
{
    [Theory]
    [MemberData(nameof(GetBasicOperationsData))]
    public void CanExecuteBasicOperations(List<Instruction> program, string expectedOutput)
    {
        string actual = Execute(program);
        Assert.Equal(expectedOutput, actual);
    }

    [Theory]
    [MemberData(nameof(GetBuiltinFunctionsData))]
    public void CanExecuteBuiltinFunctions(List<Instruction> program, string expectedOutput)
    {
        string actual = Execute(program);
        Assert.Equal(expectedOutput, actual);
    }

    [Theory]
    [MemberData(nameof(GetCompoundExpressionsData))]
    public void CanExecuteCompoundExpressions(List<Instruction> program, string expectedOutput)
    {
        string actual = Execute(program);
        Assert.Equal(expectedOutput, actual);
    }

    [Theory]
    [MemberData(nameof(GetOverflowAndRuntimeErrorsData))]
    public void CanReportRuntimeErrors(List<Instruction> program)
    {
        Assert.Throws<VirtualMachineException>(() => Execute(program));
    }

    [Theory]
    [MemberData(nameof(GetStackUnderflowData))]
    public void CanReportStackUnderflow(List<Instruction> program)
    {
        Assert.Throws<InvalidOperationException>(() => Execute(program));
    }

    public static TheoryData<List<Instruction>, string> GetBasicOperationsData()
    {
        return new TheoryData<List<Instruction>, string>
        {
            {
                new List<Instruction>
                {
                },
                string.Empty
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 10),
                    new Instruction(OpCode.Print),
                },
                "10" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 10),
                    new Instruction(OpCode.Push, 10),
                    new Instruction(OpCode.Add),
                    new Instruction(OpCode.Print),
                },
                "20" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 10),
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Sub),
                    new Instruction(OpCode.Print),
                },
                "7" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 10),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Mul),
                    new Instruction(OpCode.Print),
                },
                "50" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 20),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Div),
                    new Instruction(OpCode.Print),
                },
                "4" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 17),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Mod),
                    new Instruction(OpCode.Print),
                },
                "2" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Neg),
                    new Instruction(OpCode.Print),
                },
                "-5" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 42),
                    new Instruction(OpCode.Pop),
                },
                string.Empty
            },
        };
    }

    public static TheoryData<List<Instruction>, string> GetBuiltinFunctionsData()
    {
        return new TheoryData<List<Instruction>, string>
        {
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, -5),
                    new Instruction(OpCode.Abs),
                    new Instruction(OpCode.Print),
                },
                "5" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 7),
                    new Instruction(OpCode.Abs),
                    new Instruction(OpCode.Print),
                },
                "7" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 16),
                    new Instruction(OpCode.Sqrt),
                    new Instruction(OpCode.Print),
                },
                "4" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Min),
                    new Instruction(OpCode.Print),
                },
                "3" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Max),
                    new Instruction(OpCode.Print),
                },
                "5" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Min),
                    new Instruction(OpCode.Print),
                },
                "3" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Max),
                    new Instruction(OpCode.Print),
                },
                "3" + Environment.NewLine
            },
        };
    }

    public static TheoryData<List<Instruction>, string> GetCompoundExpressionsData()
    {
        return new TheoryData<List<Instruction>, string>
        {
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 16),
                    new Instruction(OpCode.Sqrt),
                    new Instruction(OpCode.Push, -5),
                    new Instruction(OpCode.Abs),
                    new Instruction(OpCode.Min),
                    new Instruction(OpCode.Print),
                },
                "4" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 3),
                    new Instruction(OpCode.Push, 5),
                    new Instruction(OpCode.Min),
                    new Instruction(OpCode.Push, 4),
                    new Instruction(OpCode.Max),
                    new Instruction(OpCode.Print),
                },
                "4" + Environment.NewLine
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 7),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Div),
                    new Instruction(OpCode.Push, 7),
                    new Instruction(OpCode.Push, 2),
                    new Instruction(OpCode.Mod),
                    new Instruction(OpCode.Add),
                    new Instruction(OpCode.Print),
                },
                "4" + Environment.NewLine
            },
        };
    }

    public static TheoryData<List<Instruction>> GetOverflowAndRuntimeErrorsData()
    {
        return new TheoryData<List<Instruction>>
        {
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, 10),
                    new Instruction(OpCode.Push, 0),
                    new Instruction(OpCode.Div),
                }
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, int.MaxValue),
                    new Instruction(OpCode.Push, 1),
                    new Instruction(OpCode.Add),
                }
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, -100),
                    new Instruction(OpCode.Sqrt),
                }
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, int.MinValue),
                    new Instruction(OpCode.Neg),
                }
            },
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Push, int.MinValue),
                    new Instruction(OpCode.Abs),
                }
            },
        };
    }

    public static TheoryData<List<Instruction>> GetStackUnderflowData()
    {
        return new TheoryData<List<Instruction>>
        {
            {
                new List<Instruction>
                {
                    new Instruction(OpCode.Print),
                }
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