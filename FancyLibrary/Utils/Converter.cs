﻿using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

using FancyLibrary.Logger;

using Newtonsoft.Json;


namespace FancyLibrary.Utils {

    public static class Converter {

        public static byte[] GetBytes<T>(T sct) {
            try {
                return getBytes(sct);
            } catch (Exception e) {
                LogClerk.Error(e.Message);
                return GlobalSettings.Encoding.GetBytes(JsonConvert.SerializeObject(sct));
            }
        }

        public static bool FromBytes<T>(byte[] bytes, out T sct) {
            try {
                sct = fromBytes<T>(bytes);
                return true;
            } catch (Exception e) {
                LogClerk.Error(e.Message);
                bool success = parseStruct(GlobalSettings.Encoding.GetString(bytes), out sct);
                return success;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="sdu"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static bool parseStruct<T>(string content, out T sdu) {
            try {
                sdu = JsonConvert.DeserializeObject<T>(content);
                return true;
            } catch (JsonException e) {
                LogClerk.Error($"Deserialize object failed: {e.Message}");
                sdu = default;
                return false;
            }
        }

        /// <summary>
        /// convert a structure to byte array
        /// </summary>
        /// <param name="sct">the struct wanna to convert</param>
        /// <typeparam name="T">type of this struct</typeparam>
        /// <exception cref="AccessViolationException">if threw this exception,
        /// try to set `StructureToPtr.fDeleteOld` to false or add try-catch expression</exception>
        /// <returns></returns>
        private static byte[] getBytes<T>(T sct) {
            int size = Marshal.SizeOf(sct);
            byte[] bytes = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(sct, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }

        /// <summary>
        /// convert a byte array to a structure
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <typeparam name="T">type of target structure</typeparam>
        /// <returns></returns>
        private static T fromBytes<T>(byte[] bytes) {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);
            T sct = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return sct;
        }
    }

}