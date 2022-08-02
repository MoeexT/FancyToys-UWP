using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace FancyTest;

[TestClass]
public class TypeTest {
    [TestMethod]
    public void ShowTypes() {
        object o = new Dictionary<int, string>();
        Console.WriteLine(o.GetType());
        Console.WriteLine(1.GetType());
        Console.WriteLine("1".GetType());
        Console.WriteLine(false.GetType());
        Console.WriteLine(typeof(int));
        Console.WriteLine(typeof(Int32));
        Console.WriteLine(typeof(Int64));
        Console.WriteLine(typeof(string));
        Console.WriteLine(typeof(String));
        Console.WriteLine(typeof(Dictionary<int, string>));
    }

    [TestMethod]
    public void CallGenericMethod() {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        MethodInfo? method = GetType()
                             .GetMethod(nameof(showType), BindingFlags.NonPublic | BindingFlags.Instance)
                             ?.MakeGenericMethod(typeof(int));

        if (method == null) {
            return;
        }
        
        for (int i = 0; i < 100_0000; i++) {
            method.Invoke(this, new object[1]);
        }
        
        stopwatch.Stop();
        Console.WriteLine($"ticks: {stopwatch.ElapsedMilliseconds}");
        stopwatch.Restart();
        for (int i = 0; i < 1000_0000; i++) {
            showTypi(i);
        }
        
        stopwatch.Stop();
        Console.WriteLine($"ticks: {stopwatch.ElapsedMilliseconds}");
    }

    private void showType<T>(int i) {
        var x = typeof(T);
    }
    
    private void showTypi(int i) {
        var x = i * i;
    }
}
