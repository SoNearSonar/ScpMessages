﻿using System;
using System.Text;

namespace ScpMessages
{
    public static class TokenReplacer
    {
        // Credit to RogerFK (https://github.com/RogerFK) for the code

        /*
			A great tool to replace strings based on an array of Tuples.
			Example usage:
				string myStr = "I'm a $job who likes $animals and likes $greatReplacementTools! Did I mention I like $animals?$";
				Console.WriteLine(myStr);
				string convertedStr = TokenReplace.ReplaceAfterToken(myStr, '$', new Tuple<string, object>[] {
					new Tuple<string, object>("job", "programmer"),
					new Tuple<string, object>("animal", "dog"),
					new Tuple<string, object>("music", "classical") // won't be used
					});
				Console.WriteLine(convertedStr);
			Output:
				I'm a $job who likes $animals and likes $greatReplacementTools! Did I mention I like $animals?$
				I'm a programmer who likes dogs and likes $greatReplacementTools! Did I mention I like dogs?$
		*/

        /// <summary>
        /// A method that replaces a <see cref="string"/> based on a <see cref="Tuple{T1, T2}"/>
        /// </summary>
        /// <param name="source">The string to use as source</param>
        /// <param name="token">The starting token</param>
        /// <param name="valuePairs">The value pairs (tuples) to use as "key -> value"</param>
        /// <returns>The string after replacement</returns>
        public static string ReplaceAfterToken(this string source, char token, Tuple<string, object>[] valuePairs)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            if (valuePairs == null)
            {
                throw new ArgumentNullException("valuePairs");
            }

            StringBuilder builder = new StringBuilder(Convert.ToInt32(Math.Ceiling(source.Length * 1.5f)));

            int i = 0;
            int sourceLength = source.Length;

            do
            {
                // Append characters until you find the token
                char auxChar = token == char.MaxValue ? (char)(char.MaxValue - 1) : char.MaxValue;
                for (; i < sourceLength && (auxChar = source[i]) != token; i++) builder.Append(auxChar);

                // Ensures no weird stuff regarding token being null
                if (auxChar == token)
                {
                    int movePos = 0;

                    // Try to find a tuple
                    foreach (Tuple<string, object> kvp in valuePairs)
                    {
                        if (kvp == null)
                            continue;

                        int j, k;
                        for (j = 0, k = i + 1; j < kvp.Item1.Length && k < source.Length && source[k] == kvp.Item1[j]; j++, k++) ;
                        // General condition for "key found"
                        if (j == kvp.Item1.Length)
                        {
                            movePos = j;
                            builder.Append(kvp.Item2); // append what we're replacing the key with
                            break;
                        }
                    }
                    // Don't skip the token if you didn't find the key, append it
                    if (movePos == 0) builder.Append(token);
                    else i += movePos;
                }
                i++;
            } while (i < sourceLength);

            return builder.ToString();
        }
    }
}