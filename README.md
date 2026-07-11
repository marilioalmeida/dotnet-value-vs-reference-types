# Value vs reference types — .NET sample

Runnable companion code for the post **"Value vs reference types: it's not about the stack"**.

- 🇬🇧 English — https://www.marilioalmeida.com/blog/value-vs-reference-types/
- 🇧🇷 Português — https://www.marilioalmeida.com/pt/blog/tipos-por-valor-vs-por-referencia/

## What it shows

1. **Pass by value vs pass by reference** — the same method increments a field of a
   `struct` (the original stays `44`, a copy was mutated) and of a `class` (the shared
   instance becomes `45`).
2. **Memory layout** — [ObjectLayoutInspector](https://github.com/SergeyTeplyakov/ObjectLayoutInspector)
   prints the real in-memory layout of:
   - a struct with mixed fields (alignment padding, 12 bytes),
   - the same struct with fields reordered largest-first (packed, 8 bytes),
   - a class (object header + method table pointer + the 24-byte minimum object).

## Run

```bash
dotnet run
```

Requires the [.NET SDK](https://dotnet.microsoft.com/download) (9.0 or later).
