using System;

namespace WatKhaoWong.Utils.Core
{
    public static class ArrayExtension
    {
        public static bool IsNullOrEmpty(this Array array) => array == null || array.Length == 0;
    }
}