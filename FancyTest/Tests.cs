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
            byte[] bs = Converter.GetBytes(lss);
            byte[] bytes = new byte[bs.Length];
            Array.Copy(bs, 0, bytes, 0, bs.Length);
            SettingStruct ss = new() {
                Type = SettingType.Log,
                Content = bytes,
            };
            Console.WriteLine(ss);
            Console.WriteLine(BitConverter.ToString(Converter.GetBytes(ss)));
        }
    }

}