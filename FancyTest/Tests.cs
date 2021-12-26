using System;

using FancyLibrary.Setting;
using FancyLibrary.Utils;

using NUnit.Framework;


namespace FancyTest {

    [TestFixture]
    public class ConverterTest {
        [Test]
        public void TestConvertStruct() {
            LogSettingStruct lss = new() {
                LogLevel = 3,
            };
            SettingStruct ss = new() {
                Type = SettingType.Log,
                Content = Converter.GetBytes(lss, Converter.ConvertMethod.Marshal),
            };
            Console.WriteLine(ss);
            Console.WriteLine(BitConverter.ToString(Converter.GetBytes(ss, Converter.ConvertMethod.Marshal)));
        }
    }

}