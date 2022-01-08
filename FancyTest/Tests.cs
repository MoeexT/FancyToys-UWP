using System;
using System.Collections.Generic;
using System.Text;

using FancyLibrary.Nursery;
using FancyLibrary.Setting;
using FancyLibrary.Utils;

using NUnit.Framework;


namespace FancyTest {

    [TestFixture]
    public class ConverterTest {
        [Test]
        public void TestConvertStruct() {
            SettingStruct ss = new() {
                Type = SettingType.LogLevel,
                LogLevel = 0b11111,
            };
            Console.WriteLine(ss);
            Console.WriteLine(BitConverter.ToString(Converter.GetBytes(ss, Converter.ConvertMethod.Marshal)));
        }

        [Test]
        public void TestConvertStructList() {
            var list = new List<InformationStruct>() {
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

            Converter.FromBytes(arr, out List<InformationStruct> newList);

            for (int i = 0; i < list.Count; i++) {
                
                Console.WriteLine(list[i]);
                Console.WriteLine(newList[i]);
                Assert.True(list[i].Equals(newList[i]));
            }
        }

        [Test]
        public void TestConvertBoolean() {
            byte[] bytes = Converter.GetBytes(true, Converter.ConvertMethod.Json);
            Console.WriteLine(bytes.Length);
            foreach (byte bt in bytes) {
                Console.WriteLine((char)bt);
            }
            Console.WriteLine(BitConverter.ToString(bytes));
        }
    }

}