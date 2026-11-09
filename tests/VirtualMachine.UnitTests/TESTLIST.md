## Список тестов 

# Базовые операции

- [] Ноль операций - корректная работа 
- [] `Push 10`, `Print` - output: `10\n`
- [] `Push 10`, `Push 10`, `Add`, `Print` - output: `20\n` 
- [] `Push 10`, `Push 3`, `Sub`, `Print` - output: `7\n`
- [] `Push 10`, `Push 5`, `Mul`, `Print` - output: `50\n`
- [] `Push 20`, `Push 5`, `Div`, `Print` - output: `4\n`
- [] `Push 17`, `Push 5`, `Mod`, `Print` - output: `2\n`
- [] `Push 5`, `Neg`, `Print` - output: `-5\n`
- [] `Push 42`, `Pop` - корректная работа (output пустой)

# Встроенные функции

- [] `Push -5`, `Abs`, `Print` - output: `5\n`
- [] `Push 7`, `Abs`, `Print` - output: `7\n`
- [] `Push 16`, `Sqrt`, `Print` - output: `4\n`
- [] `Push 5`, `Push 3`, `Min`, `Print` - output: `3\n`
- [] `Push 3`, `Push 5`, `Max`, `Print` - output: `5\n`
- [] `Push 3`, `Push 3`, `Min`, `Print` - output: `3\n`
- [] `Push 3`, `Push 3`, `Max`, `Print` - output: `3\n`

# Составные выражения

- [] `Push 16`, `Sqrt`, `Push -5`, `Abs`, `Min`, `Print` - output: `4\n`
- [] `Push 3`, `Push 5`, `Min`, `Push 4`, `Max`, `Print` - output: `4\n`
- [] `Push 7`, `Push 2`, `Div`, `Push 7`, `Push 2`, `Mod`, `Add`, `Print` - output: `4\n`

# Ошибки: переполнения, stack underflow, деленеие на ноль 

- [] `Push 10`, `Push 0`, `Div` - ошибка `VirtualMachineException`
- [] `Push int.MaxValue`, `Push 1`, `Add` - ошибка `VirtualMachineException`
- [] `Push -100`, `Sqrt` - ошибка `VirtualMachineException`
- [] `Print` (пустой стек) - ошибка `InvalidOperationException`
- [] `Push int.MinValue`, `Neg` - ошибка `VirtualMachineException`
- [] `Push int.MinValue`, `Abs` - ошибка `VirtualMachineException`