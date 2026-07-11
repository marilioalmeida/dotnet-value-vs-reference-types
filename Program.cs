using System.Runtime.CompilerServices;
using ObjectLayoutInspector;

// Companion code for the post "Value vs reference types: it's not about the stack".
//   EN: https://www.marilioalmeida.com/blog/value-vs-reference-types/
//   PT: https://www.marilioalmeida.com/pt/blog/tipos-por-valor-vs-por-referencia/
//
// Run with: dotnet run

struct SomeStruct { public int Field; }
class SomeClass { public int Field; }

// Mixed field sizes - sequential layout keeps declaration order, alignment adds padding.
struct Sample { public byte A; public int B; public byte C; public short D; }

// Same fields, largest-first - packs with no padding.
struct SamplePacked { public int B; public short D; public byte A; public byte C; }

class Node { public int Value; }

// A class (reference type) as a field of a struct (value type).
class Customer { public string Name { get; set; } = ""; }
struct Order { public int Id; public Customer Customer; }

struct EmptyStruct { }
class EmptyClass { }

static class Program
{
    static SomeStruct M(SomeStruct x) { x.Field++; return x; }
    static SomeClass M(SomeClass x) { x.Field++; return x; }
    static void MByRef(ref SomeStruct x) => x.Field++;

    static void Main()
    {
        Section("1. Pass by value vs pass by reference");
        var s = new SomeStruct { Field = 44 };
        M(s);
        Console.WriteLine($"struct after M(s):        {s.Field}   // 44 - a copy was mutated");
        var c = new SomeClass { Field = 44 };
        M(c);
        Console.WriteLine($"class  after M(c):        {c.Field}   // 45 - the shared instance was mutated");

        Section("2. Managed pointer (ref) - share a value type");
        var r = new SomeStruct { Field = 44 };
        MByRef(ref r);
        Console.WriteLine($"struct after MByRef(ref): {r.Field}   // 45 - ref shares the original");

        Section("3. Identity: value = by content, reference = by location");
        var s1 = new SomeStruct { Field = 1 };
        var s2 = new SomeStruct { Field = 1 };
        Console.WriteLine($"s1.Equals(s2):            {s1.Equals(s2)}   // True  - same content");
        var c1 = new SomeClass { Field = 1 };
        var c2 = new SomeClass { Field = 1 };
        Console.WriteLine($"ReferenceEquals(c1, c2):  {ReferenceEquals(c1, c2)}   // False - different objects");
        Console.WriteLine($"ReferenceEquals(c1, c1):  {ReferenceEquals(c1, c1)}   // True  - same object");

        Section("4. A struct that holds a reference (shallow copy)");
        var original = new Order { Id = 1, Customer = new Customer { Name = "Carlos" } };
        var copy = original;            // copies the whole struct
        copy.Id = 99;                   // Id is independent
        copy.Customer.Name = "Ana";     // Customer is shared
        Console.WriteLine($"original.Id:              {original.Id}   // 1  - the int was copied");
        Console.WriteLine($"original.Customer.Name:   {original.Customer.Name} // Ana - the Customer is shared");

        Section("5. Boxing: a value type wrapped as a reference type");
        int i = 42;
        object boxed = i;               // allocates a heap object
        int unboxed = (int)boxed;
        Console.WriteLine($"boxed.GetType().Name:     {boxed.GetType().Name}");
        Console.WriteLine($"unboxed == i:             {unboxed == i}");

        Section("6. Memory layout");
        TypeLayout.PrintLayout<Sample>();
        Console.WriteLine();
        TypeLayout.PrintLayout<SamplePacked>();
        Console.WriteLine();
        TypeLayout.PrintLayout<Node>();
        Console.WriteLine();
        TypeLayout.PrintLayout<Order>();

        Section("7. Size floor: empty struct vs empty class");
        Console.WriteLine($"Unsafe.SizeOf<EmptyStruct>(): {Unsafe.SizeOf<EmptyStruct>()} byte");
        Console.WriteLine();
        TypeLayout.PrintLayout<EmptyClass>();
    }

    static void Section(string title) => Console.WriteLine($"\n=== {title} ===");
}
