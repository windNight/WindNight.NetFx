using System.Security.Cryptography;

namespace System
{
    /// <summary> </summary>
    public static class RandomHelper
    {
        //随机数对象
        private static readonly Random _random = new Random();

        #region 生成一个0.0到1.0的随机小数

        public static double GetRandomDouble()
        {
            return _random.NextDouble();
        }

        #endregion

        #region ====对一个数组进行随机排序===

        public static void GetRandomArray<T>(T[] arr)
        {
            var count = arr.Length;

            for (var i = 0; i < count; i++)
            {
                var randomNum1 = GetRandomInt(0, arr.Length);
                var randomNum2 = GetRandomInt(0, arr.Length);

                T temp;

                temp = arr[randomNum1];
                arr[randomNum1] = arr[randomNum2];
                arr[randomNum2] = temp;
            }
        }

        #endregion

        #region ===从字符串里随机得到规定个数的字符串====

        private static string GetRandomCode(string allChar, int CodeCount)
        {
            var allCharArray = allChar.Split(',');
            var RandomCode = "";
            var temp = -1;
            var rand = new Random();
            for (var i = 0; i < CodeCount; i++)
            {
                if (temp != -1) rand = new Random(temp * i * (int) DateTime.Now.Ticks);

                var t = rand.Next(allCharArray.Length - 1);

                while (temp == t) t = rand.Next(allCharArray.Length - 1);

                temp = t;
                RandomCode += allCharArray[t];
            }

            return RandomCode;
        }

        #endregion

        /// <summary>
        ///     Returns a random long from min (inclusive) to max (exclusive)
        /// </summary>
        /// <param name="random">The given random instance</param>
        /// <param name="min">The inclusive minimum bound</param>
        /// <param name="max">The exclusive maximum bound.  Must be greater than min</param>
        public static long NextLong(this Random random, long min, long max)
        {
            if (max <= min)
                throw new ArgumentOutOfRangeException("max", "max must be > min!");

            //Working with ulong so that modulo works correctly with values > long.MaxValue
            var uRange = (ulong) (max - min);

            //Prevent a modulo bias; see https://stackoverflow.com/a/10984975/238419
            //for more information.
            //In the worst case, the expected number of calls is 2 (though usually it's
            //much closer to 1) so this loop doesn't really hurt performance at all.
            ulong ulongRand;
            do
            {
                var buf = new byte[8];
                random.NextBytes(buf);
                ulongRand = (ulong) BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - (ulong.MaxValue % uRange + 1) % uRange);

            return (long) (ulongRand % uRange) + min;
        }

        #region 生成一个指定范围的随机整数

        public static int GetRandomInt(int minNum = 0, int maxNum = int.MinValue)
        {
            return _random.Next(minNum, maxNum);
        }

        /// <summary>
        /// </summary>
        /// <param name="numSeeds"></param>
        /// <param name="length"></param>
        /// <example>
        ///     Simple example to use NextRandom :
        ///     <code lang="c#">
        /// <![CDATA[   
        ///  var strRandomResult = RandomHelper.NextRandom(1000, 1).ToString("D3");
        /// ]]>
        /// </code>
        /// </example>
        /// <returns></returns>
        public static int NextRandom(int numSeeds, int length)
        {
            var randomNumber = new byte[length];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomNumber);
            uint randomResult = 0x0;
            for (var i = 0; i < length; i++) randomResult |= (uint) randomNumber[i] << ((length - 1 - i) * 8);
            return (int) (randomResult % numSeeds) + 1;
        }

        #endregion

        #region ===生成随机字符串===

        private static int rep;

        public static string GenerateCheckCodeNum(int codeCount)
        {
            var str = string.Empty;
            var num2 = DateTime.Now.Ticks + rep;
            rep++;
            var random = new Random((int) ((ulong) num2 & 0xffffffffL) | (int) (num2 >> rep));
            for (var i = 0; i < codeCount; i++)
            {
                var num = random.Next();
                str = str + (char) (0x30 + (ushort) (num % 10));
            }

            return str;
        }


        public static string GenerateCheckCode(int codeCount)
        {
            var str = string.Empty;
            var num2 = DateTime.Now.Ticks + rep;
            rep++;
            var random = new Random((int) ((ulong) num2 & 0xffffffffL) | (int) (num2 >> rep));
            for (var i = 0; i < codeCount; i++)
            {
                char ch;
                var num = random.Next();
                if (num % 2 == 0)
                    ch = (char) (0x30 + (ushort) (num % 10));
                else
                    ch = (char) (0x41 + (ushort) (num % 0x1a));
                str = str + ch;
            }

            return str;
        }

        #endregion
    }
}