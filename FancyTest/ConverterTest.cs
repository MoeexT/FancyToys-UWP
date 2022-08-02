using System;
using System.Collections.Generic;
using System.Diagnostics;

using FancyLibrary;
using FancyLibrary.Bridges;
using FancyLibrary.Nursery;
using FancyLibrary.Setting;
using FancyLibrary.Utils;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;


namespace FancyTest;

[TestClass]
public class ConverterTest {

    [TestMethod]
    public void TestConvertStruct() {
        DatagramStruct ds = new() {
            Method = RequestMethod.Notify,
            Seq = 0,
            // StructType = typeof(SettingStruct),
            Content = Converter.GetBytes(new SettingStruct() {
                Type = SettingType.LogLevel,
                LogLevel = 0b111111,
            }),
        };
        Console.WriteLine(BitConverter.ToString(ds.Content));
        byte[] bytes = Converter.GetBytes(ds);
        Console.WriteLine(Converter.FromBytes(bytes, out DatagramStruct dss));
        Console.WriteLine(Converter.FromBytes(dss.Content, out SettingStruct ss));
    }

    [TestMethod]
    public void TestConvertStructList() {
        var list = new List<NurseryInformationStruct>() {
            new() {
                Id = 1,
                ProcessName = "Process1",
                CPU = 25.6,
                Memory = 35,
            },
            new() {
                Id = 1,
                ProcessName = "Process1",
                CPU = 23.33,
                Memory = 666,
            }
        };

        byte[] arr = Converter.GetBytes(list);
        Console.WriteLine(BitConverter.ToString(arr));

        Converter.FromBytes(arr, out List<NurseryInformationStruct> newList);

        for (int i = 0; i < list.Count; i++) {

            Console.WriteLine(list[i]);
            Console.WriteLine(newList[i]);
            Assert.IsTrue(list[i].Equals(newList[i]));
        }
    }

    [TestMethod]
    public void TestConvertBoolean() {
        byte[] bytes = Converter.GetBytes(true);
        Console.WriteLine(bytes.Length);

        foreach (byte bt in bytes) {
            Console.WriteLine((char)bt);
        }
        Console.WriteLine(BitConverter.ToString(bytes));
    }

    [TestMethod]
    public void TestTypes() {
        object ds = new DatagramStruct();
        Console.WriteLine(ds.GetType());
        Console.WriteLine(ds is DatagramStruct);
    }

    [TestMethod]
    public void TestGenericTypeInitialization() {
        DatagramStruct ds = new() {
            Method = RequestMethod.Request,
            Seq = 0,
            Ack = 0,
            Content = new byte[1],
        };





        Console.WriteLine(System.Runtime.InteropServices.Marshal.SizeOf(typeof(DatagramStruct)));
        // Console.WriteLine(o.GetType());
        Console.WriteLine("===========================");
    }

    [TestMethod]
    public void TestNewton() {
        JsonConvert.DeserializeObject("ss", typeof(int));
        
    }
}
