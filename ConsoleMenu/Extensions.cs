using System;
using System.Text;

namespace ConsoleMenu
{
    /// <summary>
    /// ConsoleMenu mxtension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns boolean depending on wheather or not the integer lies between the two arguments.
        /// </summary>
        /// <param name="thisInt">The integer to test.</param>
        /// <param name="bottom">Greater than this.</param>
        /// <param name="top">Less or equal than this.</param>
        /// <returns>True/False</returns>
        public static bool Between(this int thisInt, int bottom, int top)
        {
            return thisInt <= top && thisInt > bottom;
        }

        /// <summary>
        /// Reverse SubString(). Starts at the fisrt position and takes as many characters as set in the argument.
        /// </summary>
        /// <param name="thisString">Origin string</param>
        /// <param name="lenght">Number of characters to take.</param>
        /// <returns>Reverse substring.</returns>
        public static string TakeUntill(this string thisString, int lenght)
        {
            StringBuilder newString = new StringBuilder();

            for (int i = 0; i < lenght && i < thisString.Length; i++)
            {
                newString.Append(thisString[i]);
            }

            return newString.ToString();
        }
    }
}
