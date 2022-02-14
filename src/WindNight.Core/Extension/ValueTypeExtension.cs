using System;

namespace WindNight.Core.Extension
{
    public static partial class ValueTypeExtension
    {
        #region Decimal

        /// <summary>Returns the smallest integral value that is greater than or equal to the specified decimal number.</summary>
        /// <param name="data">A decimal number.</param>
        /// <returns>
        /// The smallest integral value that is greater than or equal to <paramref name="data" />. Note that this method returns a <see cref="T:System.Decimal" /> instead of an integral type.
        /// </returns>
        public static decimal Ceiling(this decimal data)
        {
            return Math.Ceiling(data);
        }

        /// <summary>
        /// 四舍六入五取整
        /// </summary>
        /// <param name="data">A decimal number to be rounded.</param>
        /// <param name="decimals">The number of decimal places in the return value.</param>
        /// <returns>The number nearest to <paramref name="data" /> that contains a number of fractional digits equal to <paramref name="decimals" />.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="decimals" /> is less than 0 or greater than 28.</exception>
        /// <returns></returns>
        public static decimal Round(this decimal data, int decimals)
        {
            return Math.Round(data, decimals);
        }

        /// <summary>Returns the largest integral value less than or equal to the specified decimal number.</summary>
        /// <param name="data">A decimal number.</param>
        /// <returns>The largest integral value less than or equal to <paramref name="data" />.  Note that the method returns an integral value of type <see cref="T:System.Decimal" />.</returns>

        public static decimal Floor(this decimal data)
        {
            return Math.Floor(data);
        }

        /// <summary>Calculates the integral part of a specified decimal number.</summary>
        /// <param name="data">A number to truncate.</param>
        /// <returns>The integral part of <paramref name="data" />; that is, the number that remains after any fractional digits have been discarded.</returns>
        public static decimal Truncate(this decimal data)
        {
            return Math.Truncate(data);
        }

        #endregion //end Decimal

        #region Double


        /// <summary>Returns the smallest integral value that is greater than or equal to the specified double number.</summary>
        /// <param name="data">A double number.</param>
        /// <returns>
        /// The smallest integral value that is greater than or equal to <paramref name="data" />. Note that this method returns a <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NegativeInfinity" />, or <see cref="F:System.Double.PositiveInfinity" />, that value is returned. Note that this method returns a <see cref="T:System.Double" /> instead of an integral type.
        /// </returns>
        public static double Ceiling(this double data)
        {
            return Math.Ceiling(data);
        }

        /// <summary>
        /// 四舍六入五取整
        /// Rounds a double-precision floating-point value to a specified number of fractional digits, and rounds midpoint values to the nearest even number.</summary>
        /// <param name="value">A double-precision floating-point number to be rounded.</param>
        /// <param name="data">The number of fractional digits in the return value.</param>
        /// <returns>The number nearest to <paramref name="data" /> that contains a number of fractional digits equal to <paramref name="digits" />.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="digits" /> is less than 0 or greater than 15.</exception>
        /// 
        public static double Round(this double data, int digits)
        {
            return Math.Round(data, digits);
        }

        /// <summary>Returns the largest integral value less than or equal to the specified double-precision floating-point number.</summary>
        /// <param name="data">A double-precision floating-point number.</param>
        /// <returns>The largest integral value less than or equal to <paramref name="data" />. If <paramref name="data" /> is equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NegativeInfinity" />, or <see cref="F:System.Double.PositiveInfinity" />, that value is returned.</returns>
        public static double Floor(this double data)
        {
            return Math.Floor(data);
        }

        /// <summary>Calculates the integral part of a specified double-precision floating-point number.</summary>
        /// <param name="data">A number to truncate.</param>
        /// <returns>The integral part of <paramref name="data" />; that is, the number that remains after any fractional digits have been discarded, or one of the values listed in the following table.
        /// <paramref name="data" /> Return value
        /// <see cref="F:System.Double.NaN" /><see cref="F:System.Double.NaN" /><see cref="F:System.Double.NegativeInfinity" /><see cref="F:System.Double.NegativeInfinity" /><see cref="F:System.Double.PositiveInfinity" /><see cref="F:System.Double.PositiveInfinity" /></returns>
        public static double Truncate(this double data)
        {
            return Math.Truncate(data);
        }

        #endregion //end Double







    }
}
