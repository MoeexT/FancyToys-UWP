using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

using Newtonsoft.Json;


namespace FancyLibrary.Utils {

    public static class Converter {

        public enum ConvertMethod {
            Json = 1, // Newtonsoft.Json serialization
            Marshal = 2, // Marshal.StructureToPtr serialization
        }

        public static byte[] GetBytes<T>(T o, ConvertMethod method = 0) {
            switch (method) {
                case ConvertMethod.Json:
                    return Consts.Encoding.GetBytes(JsonConvert.SerializeObject(o));
                case ConvertMethod.Marshal:
                    return getBytes(o);
                default:
                    try { return getBytes(o); } catch (Exception e) {
                        return Consts.Encoding.GetBytes(JsonConvert.SerializeObject(o));
                    }
            }
        }

        public static byte[] GetBytes<T>(List<T> sa, ConvertMethod method = ConvertMethod.Json) {
            switch (method) {
                // TODO not test
                case ConvertMethod.Marshal:
                    try {
                        BinaryFormatter bf = new();
                        MemoryStream ms = new();
                        bf.Serialize(ms, sa);
                        return ms.ToArray();
                    } catch (Exception e) {
                        return null;
                    }
                default:
                    return Consts.Encoding.GetBytes(JsonConvert.SerializeObject(sa));
            }
        }

        public static bool FromBytes<T>(byte[] bytes, out T o, ConvertMethod method = 0) {
            bool success;

            switch (method) {
                case ConvertMethod.Json:
                    success = parseJson(Consts.Encoding.GetString(bytes), out o);
                    return success;
                case ConvertMethod.Marshal:
                    o = fromBytes<T>(bytes);
                    return true;
                default:
                    try {
                        o = fromBytes<T>(bytes);
                        return true;
                    } catch (Exception e) {
                        success = parseJson(Consts.Encoding.GetString(bytes), out o);
                        return success;
                    }
            }
        }

        public static bool FromBytes<T>(byte[] bytes, out List<T> sa, ConvertMethod method = ConvertMethod.Json) {
            switch (method) {
                case ConvertMethod.Marshal:
                    // TODO not test
                    try {
                        MemoryStream ms = new();
                        BinaryFormatter bf = new();
                        ms.Write(bytes, 0, bytes.Length);
                        ms.Position = 0;
                        sa = bf.Deserialize(ms) as List<T>;
                        return true;
                    } catch (Exception e) {
                        sa = null;
                        return false;
                    }
                default:
                    parseJson(Consts.Encoding.GetString(bytes), out sa);
                    return true;
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
            Marshal.StructureToPtr(sct, ptr, false);
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            Marshal.DestroyStructure<IntPtr>(ptr);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="sdu"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static bool parseJson<T>(string content, out T sdu) {
            try {
                sdu = JsonConvert.DeserializeObject<T>(content);
                return true;
            } catch (JsonException e) {
                sdu = default;
                return false;
            }
        }
    }

}