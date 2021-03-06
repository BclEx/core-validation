using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using static CoreValidation.Globals;

namespace CoreValidation.Formats
{
  public static class Number
  {
    static readonly Regex _digitPatern = new Regex(@"[^0-9\.]+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    static bool TryBool(object value, out bool result)
    {
      try { result = Convert.ToBoolean(value); return true; }
      catch (FormatException)
      {
        try { result = Convert.ToInt32(value) != 0; return true; }
        catch (FormatException) { result = default; return false; }
      }
    }

    static bool TryDecimal(object value, out decimal result)
    {
      if (value is float f && float.IsNaN(f)) { result = default; return false; }
      try { result = Convert.ToDecimal(value); return true; }
      catch (FormatException)
      {
        result = default; return false;
      }
    }

    static bool TryDouble(object value, out double result)
    {
      if (value is float f && float.IsNaN(f)) { result = default; return false; }
      if (value is double d && double.IsNaN(d)) { result = default; return false; }
      try { result = Convert.ToDouble(value); return true; }
      catch (FormatException)
      {
        result = default; return false;
      }
    }

    static bool TryInt32(object value, out int result)
    {
      if (value is float f && float.IsNaN(f)) { result = default; return false; }
      try { result = Convert.ToInt32(value); return true; }
      catch (FormatException)
      {
        result = default; return false;
      }
    }

    // bool
    public static string BoolFormatter(object value, IDictionary<string, object> param = null)
    {
      if (value == null || (value is string h && h.Length == 0) || !TryBool(value, out var value_)) return NulFormat;
      if (param != null && param.TryGetValue("format", out var z) && z is string format)
        switch (format)
        {
          case "trueFalse": return value_ ? "True" : "False";
          case "yesNo": return value_ ? "Yes" : "No";
          case "values":
            if (param.TryGetValue("values", out z) && z is string[] values && values != null)
            {
              if (values.Length != 2) throw new ArgumentOutOfRangeException(nameof(param), "param.values invalid");
              return value_ ? values[0] : values[1];
            }
            throw new ArgumentOutOfRangeException(nameof(param), "param.values undefined");
          default: throw new ArgumentOutOfRangeException(nameof(param), "param.format invalid");
        }
      return value_ ? "Yes" : "No";
    }
    public static (object, bool, Func<object>) BoolParser(string text, IDictionary<string, object> param = null, Func<object> error = null)
    {
      if (string.IsNullOrEmpty(text)) return (text, true, error);
      switch (text.ToLowerInvariant())
      {
        case "1": case "y": case "true": case "yes": case "on": return (true, true, error);
        case "0": case "n": case "false": case "no": case "off": return (false, true, error);
        default: return (text, false, error);
      }
    }

    // decimal
    public static string DecimalFormatter(object value, IDictionary<string, object> param = null)
    {
      if (value == null || (value is string h && h.Length == 0) || !TryDecimal(value, out var value_)) return NulFormat;
      if (param != null && param.TryGetValue("format", out var z) && z is string format)
        switch (format)
        {
          case "comma": return value_.ToString("#,#.####", CultureInfo.InvariantCulture);
          case "n2": return value_.ToString("n2", CultureInfo.InvariantCulture);
          case "n3": return value_.ToString("n3", CultureInfo.InvariantCulture);
          case "pattern": return param.TryGetValue("pattern", out z) && z is string pattern ? value_.ToString(pattern, CultureInfo.InvariantCulture) : string.Empty;
          default: throw new ArgumentOutOfRangeException(nameof(param), "param.format invalid");
        }
      return value_.ToString("n4", CultureInfo.InvariantCulture);
    }
    public static (object, bool, Func<object>) DecimalParser(string text, IDictionary<string, object> param = null, Func<object> error = null)
    {
      if (string.IsNullOrEmpty(text)) return (text, true, error);
      if (!decimal.TryParse(text, out var value)) return (text, false, error);
      if (param != null)
      { // check param
        if (param.TryGetValue("minValue", out var z) && z is decimal minValue) { if (minValue != 0 && value < minValue) return (text, false, error); }
        if (param.TryGetValue("maxValue", out z) && z is decimal maxValue) { if (maxValue != 0 && value > maxValue) return (text, false, error); }
        if (param.TryGetValue("precision", out z) && z is int precision) { if (precision != 0 && value != Math.Round(value, precision)) return (text, false, error); }
        if (param.TryGetValue("round", out z) && z is int round) { if (round != 0) value = Math.Round(value, round); }
      }
      return (value, true, error);
    }

    // integer
    public static string IntegerFormatter(object value, IDictionary<string, object> param = null)
    {
      if (value == null || (value is string h && h.Length == 0) || !TryInt32(value, out var value_)) return NulFormat;
      if (param != null && param.TryGetValue("format", out var z) && z is string format)
        switch (format)
        {
          case "comma": return value_.ToString("#,#.####", CultureInfo.InvariantCulture);
          case "byte":
            var radix = (int)Math.Floor((decimal)(value_.ToString().Length - 1) / 3);
            if (radix > 0) return $"{Math.Round(value_ / (decimal)(1 << 10 * radix), 2)} {"  KBMBGB".Substring(radix << 1, 2)}";
            if (value_ == 1) return "1 byte";
            return $"{value_} bytes";
          case "pattern": return param.TryGetValue("pattern", out z) && z is string pattern ? value_.ToString(pattern, CultureInfo.InvariantCulture) : string.Empty;
          default: throw new ArgumentOutOfRangeException(nameof(param), "param.format invalid");
        }
      return value_.ToString(CultureInfo.InvariantCulture);
    }
    public static (object, bool, Func<object>) IntegerParser(string text, IDictionary<string, object> param = null, Func<object> error = null)
    {
      if (string.IsNullOrEmpty(text)) return (text, true, error);
      if (!decimal.TryParse(text, out var v)) return (text, false, error);
      var value = (int)v;
      if (param != null)
      { // check param
        if (param.TryGetValue("minValue", out var z) && z is int minValue) { if (minValue != 0 && value < minValue) return (text, false, error); }
        if (param.TryGetValue("maxValue", out z) && z is int maxValue) { if (maxValue != 0 && value > maxValue) return (text, false, error); }
      }
      return (value, true, error);
    }

    // money
    public static string MoneyFormatter(object value, IDictionary<string, object> param = null)
    {
      if (value == null || (value is string h && h.Length == 0) || !TryDecimal(value, out var value_)) return NulFormat;
      if (param != null && param.TryGetValue("format", out var z) && z is string format)
        switch (format)
        {
          case "c2": return value_.ToString("c2");
          case "c3": return value_.ToString("c3");
          case "pattern": return param.TryGetValue("pattern", out z) && z is string pattern ? value_.ToString(pattern) : string.Empty;
          default: throw new ArgumentOutOfRangeException(nameof(param), "param.format invalid");
        }
      return value_.ToString("c2");
    }
    public static (object, bool, Func<object>) MoneyParser(string text, IDictionary<string, object> param = null, Func<object> error = null)
    {
      if (string.IsNullOrEmpty(text)) return (text, true, error);
      text = _digitPatern.Replace(text, string.Empty); if (string.IsNullOrEmpty(text)) return (text, false, error);
      if (!decimal.TryParse(text, out var value)) return (text, false, error);
      value = Math.Round(value, 4);
      if (param != null)
      { // check param
        if (param.TryGetValue("minValue", out var z) && z is decimal minValue) { if (minValue != 0 && value < minValue) return (text, false, error); }
        if (param.TryGetValue("maxValue", out z) && z is decimal maxValue) { if (maxValue != 0 && value > maxValue) return (text, false, error); }
        if (param.TryGetValue("precision", out z) && z is int precision) { if (precision != 0 && value != Math.Round(value, precision)) return (text, false, error); }
        if (param.TryGetValue("round", out z) && z is int round) { if (round != 0) value = Math.Round(value, round); }
      }
      return (value, true, error);
    }

    // percent
    public static string PercentFormatter(object value, IDictionary<string, object> param = null)
    {
      if (value == null || (value is string h && h.Length == 0) || !TryDouble(value, out var value_)) return NulFormat;
      if (param != null && param.TryGetValue("format", out var z) && z is string format)
        switch (format)
        {
          case "p2": return value_.ToString("p2");
          case "p3": return value_.ToString("p3");
          case "p4": return value_.ToString("p4");
          case "pattern": return param.TryGetValue("pattern", out z) && z is string pattern ? value_.ToString(pattern) : string.Empty;
          default: throw new ArgumentOutOfRangeException(nameof(param), "param.format invalid");
        }
      return value_.ToString("p2");
    }
    public static (object, bool, Func<object>) PercentParser(string text, IDictionary<string, object> param = null, Func<object> error = null)
    {
      if (string.IsNullOrEmpty(text)) return (text, true, error);
      if (text[text.Length - 1] == '%') text = text.Substring(0, text.Length - 1);
      if (!double.TryParse(text, out var value) || double.IsNaN(value)) return (text, false, error);
      return (value / 100, true, error);
    }

    // real
    public static string RealFormatter(object value, IDictionary<string, object> param = null)
    {
      if (value == null || (value is string h && h.Length == 0) || !TryDouble(value, out var value_)) return NulFormat;
      if (param != null && param.TryGetValue("format", out var z) && z is string format)
        switch (format)
        {
          case "comma": return value_.ToString("#,#.####", CultureInfo.InvariantCulture);
          case "n2": return value_.ToString("n2", CultureInfo.InvariantCulture);
          case "n3": return value_.ToString("n3", CultureInfo.InvariantCulture);
          case "pattern": return param.TryGetValue("pattern", out z) && z is string pattern ? value_.ToString(pattern, CultureInfo.InvariantCulture) : string.Empty;
          default: throw new ArgumentOutOfRangeException(nameof(param), "param.format invalid");
        }
      return value_.ToString("n4", CultureInfo.InvariantCulture);
    }
    public static (object, bool, Func<object>) RealParser(string text, IDictionary<string, object> param = null, Func<object> error = null)
    {
      if (string.IsNullOrEmpty(text)) return (text, true, error);
      if (!double.TryParse(text, out var value) || double.IsNaN(value)) return (text, false, error);
      if (param != null)
      { // check param
        if (param.TryGetValue("minValue", out var z) && z is double minValue) { if (minValue != 0 && value < minValue) return (text, false, error); }
        if (param.TryGetValue("maxValue", out z) && z is double maxValue) { if (maxValue != 0 && value > maxValue) return (text, false, error); }
        if (param.TryGetValue("precision", out z) && z is int precision) { if (precision != 0 && value != Math.Round(value, precision)) return (text, false, error); }
        if (param.TryGetValue("round", out z) && z is int round) { if (round != 0) value = Math.Round(value, round); }
      }
      return (value, true, error);
    }
  }
}
