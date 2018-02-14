using UnityEngine;

public static class Utilities {

	public static int ParseFast( this string s ) {
		int r = 0;
		for (var i = 0; i < s.Length; i++)
			{
			char letter = s[i];
			r = 10 * r;
			r = r + (int)char.GetNumericValue (letter);
			}
		return r;
	}
	public static int ParseFast(char c)
     {
         int result = 0;
         result = 10 * result + (int)char.GetNumericValue (c);
         return result;
     }
}
