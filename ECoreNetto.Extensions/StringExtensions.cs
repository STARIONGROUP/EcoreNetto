// ------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="string"/> 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// splits a string into multiple lines
        /// </summary>
        /// <param name="input">
        /// the subject input string
        /// </param>
        /// <param name="maximumLineLength">
        /// the maximum length of a line
        /// </param>
        /// <returns>
        /// an <see cref="IEnumerable{String}"/>
        /// </returns>
        public static IEnumerable<string> SplitToLines(this string input, int maximumLineLength)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("The input string may not be null or empty", nameof(input));
            }

            return SplitToLinesIterator(input, maximumLineLength);
        }

        /// <summary>
        /// splits a string into multiple lines
        /// </summary>
        /// <param name="input">
        /// the subject input string
        /// </param>
        /// <param name="maximumLineLength">
        /// the maximum length of a line
        /// </param>
        /// <returns>
        /// an <see cref="IEnumerable{String}"/>
        /// </returns>
        private static IEnumerable<string> SplitToLinesIterator(string input, int maximumLineLength)
        {
            input = input.Replace("\r\n", " ").Trim();

            var words = input.Split(' ');
            var line = words[0];

            foreach (var word in words.Skip(1))
            {
                var test = $"{line} {word}";
                if (test.Length > maximumLineLength)
                {
                    yield return line;
                    line = word;
                }
                else
                {
                    line = test;
                }
            }

            yield return line.Trim();
        }

        /// <summary>
        /// Capitalize the first letter of a string
        /// </summary>
        /// <param name="input">
        /// The subject input string
        /// </param>
        /// <returns>
        /// Returns a string where the first character is converted to uppercase
        /// </returns>
        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("string can't be empty!");
            }

            return string.Concat(input[0].ToString(CultureInfo.InvariantCulture).ToUpper(CultureInfo.InvariantCulture), input.Substring(1));
        }

        /// <summary>
        /// Lower case the first letter of a string
        /// </summary>
        /// <param name="input">
        /// The subject input string
        /// </param>
        /// <returns>
        /// Returns a string where the first character is converted to lowercase
        /// </returns>
        public static string LowerCaseFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("string can't be empty!");
            }

            return string.Concat(input[0].ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture), input.Substring(1));
        }

        /// <summary>
        /// Prefixes the input string with another
        /// </summary>
        /// <param name="input">
        /// the string that is to be prefixed
        /// </param>
        /// <param name="prefix">
        /// the subject prefix
        /// </param>
        /// <returns>
        /// the inputs string prefixed with the provided prefix
        /// </returns>
        public static string Prefix(this string input, string prefix)
        {
            return $"{prefix}{input}";
        }
    }
}
