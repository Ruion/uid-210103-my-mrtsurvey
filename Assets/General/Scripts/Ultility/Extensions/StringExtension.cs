using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StringExtensions
{
    public static string FirstCharToUpper(this string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentException("There is no first letter");

        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }

    public static string LowerCaseAllUppercaseFirst(this string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        s = s.ToLower();
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }

    public static string FirstCharToLower(this string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentException("There is no first letter");

        char[] a = s.ToCharArray();
        a[0] = char.ToLower(a[0]);
        return new string(a);
    }

    public static bool ContainsAny(this string input, IEnumerable<string> containsKeywords, StringComparison comparisonType)
    {
        return containsKeywords.Any(keyword => input.IndexOf(keyword, comparisonType) >= 0);
    }

    public static string GetNumbers(string input)
    {
        return new string(input.Where(c => char.IsDigit(c)).ToArray());
    }

    public static string RemoveCharacters(string value, string[] charsToRemove)
    {
        string newValue = value;

        foreach (var c in charsToRemove)
        {
            newValue = newValue.Replace(c, string.Empty);
        }

        return newValue;
    }

    public static string RemoveAlphabets(string oldString)
    {
        var newValue = Regex.Replace(oldString, @"[A-Za-z]+", "");

        return newValue;
    }

    public static string FromBase64String(this string base64String)
    {
        byte[] data = System.Convert.FromBase64String(base64String);

        return System.Text.ASCIIEncoding.ASCII.GetString(data);
    }

    public static string DecodeBase64(this string base64String)
    {
        base64String = base64String.Trim();

        if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
           || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n") || !base64String.EndsWith("=="))
            return "false";

        return FromBase64String(base64String);
    }
}