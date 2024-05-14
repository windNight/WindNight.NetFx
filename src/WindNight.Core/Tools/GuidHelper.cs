namespace System.Text.Extension
{
    /// <summary> </summary>
    public static class GuidHelper
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static string GetGuid()
        {
            var strRandomResult = RandomHelper.NextRandom(1000, 1).ToString("D3");
            return string.Concat(Guid.NewGuid().ToString("N"), strRandomResult);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static string GenerateOrderNumber()
        {
            var strDateTimeNumber = HardInfo.Now.ToString("yyyyMMddHHmmssms");
            var strRandomResult = RandomHelper.NextRandom(1000, 1).ToString("D3");
            return string.Concat(strDateTimeNumber, strRandomResult);
        }
    }
}