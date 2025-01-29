namespace WatKhaoWong.Utils.Core
{
    public static class NumberExtension
    {
        public static bool IsNegative(this int number)
        {
            return number < 0;
        }

        public static bool IsNegative(this decimal number)
        {
            return number < 0m;
        }
    }
}