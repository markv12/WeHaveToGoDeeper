public static class StringExcensions {
    public static string Capitalized(this string theString) {
        if (theString.Length == 0)
            return theString;
        else if (theString.Length == 1)
            return char.ToUpper(theString[0]).ToString();
        else
            return char.ToUpper(theString[0]) + theString.Substring(1);
    }
}
