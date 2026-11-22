# Python++

Цель проекта: интерпретатор языка Python++ со своей стековой виртуальной машиной (кодогенерация в байт-код VM выполняется прямо при разборе, без AST).

- Язык реализации: C#
- Среда выполнения: .NET 10
- Редактор кода: Visual Studio Code

Описание языка: [docs/specification.md](docs/specification.md).

## Структура

| Проект | Назначение |
| --- | --- |
| `src/Lexems` | лексический анализатор |
| `src/Parser` | рекурсивный парсер, генерирующий инструкции VM |
| `src/VirtualMachine` | стековая виртуальная машина |
| `src/Interpreter` | консольное приложение `interpreter` |
| `tests/*` | модульные и приёмочные тесты |

## Сборка и запуск

```bash
dotnet build
dotnet run --project src/Interpreter -- <файл>
```

Либо запуск собранного файла: `src/Interpreter/bin/Debug/net10.0/interpreter <файл>`.

Интерпретатор читает программу из файла, ввод — из stdin (функция `readInt()`), а результат пишет в stdout. При любой ошибке разбора или выполнения он печатает сообщение в stderr и завершается с кодом `1`; при успехе код равен `0`.

Пример с вводом (PowerShell):

```powershell
"2 3" | dotnet run --project src/Interpreter -- program.ppp
```

Пример с вводом (bash):

```bash
echo "2 3" | dotnet run --project src/Interpreter -- program.ppp
```

## Тесты

```bash
dotnet test
```

Приёмочные тесты запускают интерпретатор отдельным процессом, подают данные в stdin и проверяют stdout, stderr и код возврата.

Тесты с отчётом о покрытии: `scripts/run-tests-with-coverage` (Linux/macOS) или `scripts/run-tests-with-coverage.ps1` (Windows).