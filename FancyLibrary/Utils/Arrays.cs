using System;


namespace FancyLibrary.Utils {

    public static class Arrays {
        public static byte[] FullCopy(byte[] src) {
            byte[] bytes = new byte[src.Length];
            Array.Copy(src, 0, bytes, 0, src.Length);
            return bytes;
        }
    }

}