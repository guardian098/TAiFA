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


# Знаковая арифметика


- [] `Push -7`, `Push 2`, `Div`, `Print` - output: `-3\n` (деление с округлением к нулю)
- [] `Push 7`, `Push -2`, `Div`, `Print` - output: `-3\n`
- [] `Push -7`, `Push 2`, `Mod`, `Print` - output: `-1\n` (знак остатка совпадает со знаком делимого)
- [] `Push 7`, `Push -2`, `Mod`, `Print` - output: `1\n`


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


# Ввод (Read)


- [] Ввод `7`: `Read`, `Print` - output: `7\n`
- [] Ввод `2 3`: `Read`, `Read`, `Add`, `Print` - output: `5\n`
- [] Ввод с разделителями из пробелов, табуляций и переводов строк
- [] Ввод отрицательного числа: `-4`
- [] Ввод со знаком `+`: `+4`
- [] Пустой ввод (ошибка): `Read` - `VirtualMachineException`, `Unexpected end of input.`
- [] Ввод закончился раньше, чем выполнены все `Read` (ошибка)
- [] Нечисловой ввод (ошибка): `x` - `Invalid integer input 'x'.`
- [] Число вне диапазона `int` (ошибка): `2147483648`


# Ошибки: переполнения, stack underflow, деление на ноль


- [] `Push 10`, `Push 0`, `Div` - ошибка `VirtualMachineException`
- [] `Push 10`, `Push 0`, `Mod` - ошибка `VirtualMachineException`
- [] `Push int.MaxValue`, `Push 1`, `Add` - ошибка `VirtualMachineException`
- [] `Push int.MinValue`, `Push 1`, `Sub` - ошибка `VirtualMachineException`
- [] `Push int.MaxValue`, `Push 2`, `Mul` - ошибка `VirtualMachineException`
- [] `Push int.MinValue`, `Push -1`, `Div` - ошибка `VirtualMachineException`
- [] `Push int.MinValue`, `Push -1`, `Mod` - ошибка `VirtualMachineException`
- [] `Push -100`, `Sqrt` - ошибка `VirtualMachineException`
- [] `Print` (пустой стек) - ошибка `VirtualMachineException`
- [] `Add` с одним значением в стеке - ошибка `VirtualMachineException`, `Stack underflow`
- [] `Push int.MinValue`, `Neg` - ошибка `VirtualMachineException`
- [] `Push int.MinValue`, `Abs` - ошибка `VirtualMachineException`
- [] Неизвестный код операции - ошибка `VirtualMachineException`
- [] Вывод, выполненный до ошибки, сохраняется: `Push 1`, `Print`, `Push 1`, `Push 0`, `Div` - output: `1\n`