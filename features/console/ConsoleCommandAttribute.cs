using System;

namespace Research.Console;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class ConsoleCommandAttribute : Attribute
{
    public string Name { get; }

    public ConsoleCommandAttribute(string name)
    {
        Name = name;
    }
}
