using dnlib.DotNet;
using System;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2) throw new ArgumentException("Usage: LoaderAnalyzer <input> <output>");
        var mod = ModuleDefMD.Load(args[0]);
        using var sw = new StreamWriter(args[1], false);
        sw.WriteLine("MODULE=" + mod.FullName);
        foreach (var t in mod.GetTypes())
        foreach (var m in t.Methods)
        {
            if (!m.HasBody) continue;
            bool gc = m.ReturnType.FullName.Contains("GCHandle", StringComparison.OrdinalIgnoreCase);
            bool main = m.Name.String.Contains("Main", StringComparison.OrdinalIgnoreCase);
            if (!gc && !main) continue;
            sw.WriteLine("\n=== " + m.FullName + " ===");
            foreach (var i in m.Body.Instructions)
                sw.WriteLine(i.Offset.ToString("X8") + ": " + i);
            if (gc && m.Parameters.Count == 2)
                sw.WriteLine("CANDIDATE_DECRYPT=" + m.FullName);
        }
    }
}
