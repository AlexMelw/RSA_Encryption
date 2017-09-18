namespace RSAEncDecLib.AlgorithmHelpers
{
    public static class IntegerHelper
    {
        /// <summary>
        ///     <paramref name="value" /> >= <paramref name="lhs" /> &amp;&amp; <paramref name="value" /> &lt;=
        ///     <paramref name="rhs" />
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="value" /> >= <paramref name="lhs" /> &amp;&amp;
        ///     <paramref name="value" /> &lt;= <paramref name="rhs" />, otherwise <see langword="false" />
        /// </returns>
        public static bool InRange(this int value, int lhs, int rhs) => value >= lhs && value <= rhs;


        /// <summary>
        ///     <paramref name="value" /> is not in range [<paramref name="lhs" />..<paramref name="rhs" />]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns>
        ///     <see langword="true" /> if is not in range [<paramref name="lhs" />..<paramref name="rhs" />], otherwise
        ///     <see langword="false" />
        /// </returns>
        public static bool NotInRange(this int value, int lhs, int rhs) => !InRange(value, lhs, rhs);
    }
}