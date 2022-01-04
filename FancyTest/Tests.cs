using System;

using FancyLibrary.Logging;
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
    }

}