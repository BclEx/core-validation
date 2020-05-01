using System;
using Xunit;
using static CoreValidation.Formats.Number;
using static CoreValidation.Globals;

namespace CoreValidation.Tests
{
  public class FormatNumberTest
  {
    [Fact]
    public void Bool()
    {
      // should format
      {
        Assert2.Equal(BoolFormater(null), NulFormat);
        Assert2.Equal(BoolFormater(""), "No");
        Assert2.Equal(BoolFormater("12"), "Yes");
      }
      // should format: *
      {
        var param = new { format = "*" }.ToParam();
        Assert.Throws<Exception>(() => BoolFormater("12", param));
      }
      // should format: trueFalse
      {
        var param = new { format = "trueFalse" }.ToParam();
        Assert2.Equal(BoolFormater("12", param), "True");
        Assert2.Equal(BoolFormater("", param), "False");
      }
      // should format: yesNo
      {
        var param = new { format = "yesNo" }.ToParam();
        Assert2.Equal(BoolFormater("12", param), "Yes");
        Assert2.Equal(BoolFormater("", param), "No");
      }
      // should format: values no values
      {
        Assert.Throws<Exception>(() => BoolFormater("12", new { format = "values" }.ToParam()));
        Assert.Throws<Exception>(() => BoolFormater("", new { format = "values" }.ToParam()));
        Assert.Throws<Exception>(() => BoolFormater("12", new { format = "values", values = "" }.ToParam()));
        Assert.Throws<Exception>(() => BoolFormater("1", new { format = "values", values = (string)null }.ToParam()));
        Assert.Throws<Exception>(() => BoolFormater("s", new { format = "values", values = "1" }.ToParam()));
        Assert.Throws<Exception>(() => BoolFormater("af", new { format = "values", values = new string[0] }.ToParam()));
        Assert.Throws<Exception>(() => BoolFormater("af", new { format = "values", values = new[] { 1 } }.ToParam()));
        Assert.Throws<Exception>(() => BoolFormater("af", new { format = "values", values = (1, 2, 3) }.ToParam()));
        Assert2.Equal(BoolFormater("af", new { format = "values", values = new[] { 1, 0 } }.ToParam()), "1");
        Assert2.Equal(BoolFormater("", new { format = "values", values = new[] { 1, 0 } }.ToParam()), "0");
      }
      // should parse
      {
        Assert2.Equal(BoolParser(null), (null, true, null));
        Assert2.Equal(BoolParser(""), ("", true, null));
        Assert2.Equal(BoolParser("A"), ("A", false, null));
        Assert2.Equal(BoolParser("Yes"), (true, true, null));
        Assert2.Equal(BoolParser("On"), (true, true, null));
        Assert2.Equal(BoolParser("true"), (true, true, null));
        Assert2.Equal(BoolParser("y"), (true, true, null));
        Assert2.Equal(BoolParser("1"), (true, true, null));
        Assert2.Equal(BoolParser("No"), (false, true, null));
        Assert2.Equal(BoolParser("Off"), (false, true, null));
        Assert2.Equal(BoolParser("false"), (false, true, null));
        Assert2.Equal(BoolParser("n"), (false, true, null));
        Assert2.Equal(BoolParser("0"), (false, true, null));
      }
    }

    [Fact]
    public void Decimal()
    {
      // should format
      {
        Assert2.Equal(DecimalFormater(null), NulFormat);
        Assert2.Equal(DecimalFormater(""), NulFormat);
        Assert2.Equal(DecimalFormater("12"), "12.0000");
        Assert2.Equal(DecimalFormater("ABC"), "NaN");
      }
      // should format: *
      {
        Assert.Throws<Exception>(() => DecimalFormater("12", new { format = "*" }.ToParam()));
      }
      // should format: comma
      {
        Assert2.Equal(DecimalFormater(float.NaN, new { format = "comma" }.ToParam()), NulFormat);
        Assert2.Equal(DecimalFormater("1232323", new { format = "comma" }.ToParam()), "1,232,323");
        Assert2.Equal(DecimalFormater("1232323.234", new { format = "comma" }.ToParam()), "1,232,323.234");
        Assert2.Equal(DecimalFormater("12", new { format = "comma" }.ToParam()), "12");
        Assert2.Equal(DecimalFormater("12.234", new { format = "comma" }.ToParam()), "12.234");
      }
      // should format: nx
      {
        Assert2.Equal(DecimalFormater("12", new { format = "n2" }.ToParam()), "12.00");
        Assert2.Equal(DecimalFormater("12", new { format = "n3" }.ToParam()), "12.000");
      }
      // should format: pattern
      {
        Assert2.Equal(DecimalFormater("12", new { format = "pattern", pattern = "2" }.ToParam()), "12.00");
        Assert2.Equal(DecimalFormater("12", new { format = "pattern", pattern = "A" }.ToParam()), "12");
      }
      //  should parse
      {
        Assert2.Equal(DecimalParser(null), (null, true, null));
        Assert2.Equal(DecimalParser(""), ("", true, null));
        Assert2.Equal(DecimalParser("NaN"), ("NaN", false, null));
        //Assert2.Equal(DecimalParser(NaN), (NaN, true, null));
        Assert2.Equal(DecimalParser("12"), (12, true, null));
      }
      // should parse: minValue
      {
        Assert2.Equal(DecimalParser("13.0", new { minValue = 12 }.ToParam()), (13, true, null));
        Assert2.Equal(DecimalParser("12.0", new { minValue = 13 }.ToParam()), (12, false, null));
      }
      // should parse: maxValue
      {
        Assert2.Equal(DecimalParser("12.0", new { maxValue = 23 }.ToParam()), (12, true, null));
        Assert2.Equal(DecimalParser("22.0", new { maxValue = 13 }.ToParam()), (22, false, null));
      }
      // should parse: precision
      {
        Assert2.Equal(DecimalParser("22.123", new { precision = 2 }.ToParam()), (22.123, false, null));
        Assert2.Equal(DecimalParser("22.12", new { precision = 2 }.ToParam()), (22.12, true, null));
      }
      // should parse: round
      {
        Assert2.Equal(DecimalParser("22.123", new { round = 1 }.ToParam()), (22.1, true, null));
        Assert2.Equal(DecimalParser("22.123", new { round = 2 }.ToParam()), (22.12, true, null));
      }
    }

    [Fact]
    public void Integer()
    {
      // should format
      {
        Assert2.Equal(IntegerFormater(null), NulFormat);
        Assert2.Equal(IntegerFormater(""), NulFormat);
        Assert2.Equal(IntegerFormater("A"), "NaN");
        Assert2.Equal(IntegerFormater("12"), "12");
      }
      // should format: *
      {
        Assert.Throws<Exception>(() => IntegerFormater("12", new { format = "*" }.ToParam()));
      }
      // should format: comma
      {
        Assert2.Equal(IntegerFormater(float.NaN, new { format = "comma" }.ToParam()), NulFormat);
        Assert2.Equal(IntegerFormater("1232323", new { format = "comma" }.ToParam()), "1,232,323");
        Assert2.Equal(IntegerFormater("12", new { format = "comma" }.ToParam()), "12");
      }
      // should format: byte
      {
        var param = new { format = "byte" }.ToParam();
        Assert2.Equal(IntegerFormater("1232323", param), "1.18 MB");
        Assert2.Equal(IntegerFormater("2048", param), "2 KB");
        Assert2.Equal(IntegerFormater("2", param), "2 bytes");
        Assert2.Equal(IntegerFormater("1", param), "1 byte");
        Assert2.Equal(IntegerFormater("0", param), "0 bytes");
      }
      // should format: pattern
      {
        Assert2.Equal(IntegerFormater("1232323", new { format = "pattern", pattern = 10 }.ToParam()), "1232323");
      }
      // should parse
      {
        Assert2.Equal(IntegerParser(null), (null, true, null));
        Assert2.Equal(IntegerParser(""), ("", true, null));
        Assert2.Equal(IntegerParser("NaN"), ("NaN", false, null));
        //Assert2.Equal(IntegerParser(NaN), (NaN, true, null));
        Assert2.Equal(IntegerParser("12"), (12, true, null));
      }
      // should parse: minValue
      {
        Assert2.Equal(IntegerParser("13.0", new { minValue = 12 }.ToParam()), (13, true, null));
        Assert2.Equal(IntegerParser("12.0", new { minValue = 13 }.ToParam()), (12, false, null));
      }
      // should parse: maxValue
      {
        Assert2.Equal(IntegerParser("12.0", new { maxValue = 23 }.ToParam()), (12, true, null));
        Assert2.Equal(IntegerParser("22.0", new { maxValue = 13 }.ToParam()), (22, false, null));
      }
    }

    [Fact]
    public void Real()
    {
      // should format
      {
        Assert2.Equal(RealFormater(null), NulFormat);
        Assert2.Equal(RealFormater(""), NulFormat);
        Assert2.Equal(RealFormater("A"), "NaN");
        Assert2.Equal(RealFormater("12"), "12.0000");
      }
      // should format: *
      {
        Assert.Throws<Exception>(() => RealFormater("12", new { format = "*" }.ToParam()));
      }
      // should format: comma
      {
        Assert2.Equal(RealFormater(float.NaN, new { format = "comma" }.ToParam()), NulFormat);
        Assert2.Equal(RealFormater("1232323", new { format = "comma" }.ToParam()), "1,232,323");
        Assert2.Equal(RealFormater("1232323.234", new { format = "comma" }.ToParam()), "1,232,323.234");
        Assert2.Equal(RealFormater("12", new { format = "comma" }.ToParam()), "12");
        Assert2.Equal(RealFormater("12.234", new { format = "comma" }.ToParam()), "12.234");
      }
      // should format: nx
      {
        Assert2.Equal(RealFormater("12", new { format = "n2" }.ToParam()), "12.00");
        Assert2.Equal(RealFormater("12", new { format = "n3" }.ToParam()), "12.000");
      }
      // should format: pattern
      {
        Assert2.Equal(RealFormater("12", new { format = "pattern", pattern = "2" }.ToParam()), "12.00");
        Assert2.Equal(RealFormater("12", new { format = "pattern", pattern = "A" }.ToParam()), "12");
      }
      // should parse
      {
        Assert2.Equal(RealParser(null), (null, true, null));
        Assert2.Equal(RealParser(""), ("", true, null));
        Assert2.Equal(RealParser("NaN"), ("NaN", false, null));
        //Assert2.Equal(RealParser(NaN), (NaN, true, null));
        Assert2.Equal(RealParser("12"), (12, true, null));
      }
      // should parse: minValue
      {
        Assert2.Equal(RealParser("13.0", new { minValue = 12 }.ToParam()), (13, true, null));
        Assert2.Equal(RealParser("12.0", new { minValue = 13 }.ToParam()), (12, false, null));
      }
      // should parse: maxValue
      {
        Assert2.Equal(RealParser("12.0", new { maxValue = 23 }.ToParam()), (12, true, null));
        Assert2.Equal(RealParser("22.0", new { maxValue = 13 }.ToParam()), (22, false, null));
      }
      // should parse: precision
      {
        Assert2.Equal(RealParser("22.123", new { precision = 2 }.ToParam()), (22.123, false, null));
        Assert2.Equal(RealParser("22.12", new { precision = 2 }.ToParam()), (22.12, true, null));
      }
      // should parse: round
      {
        Assert2.Equal(RealParser("22.123", new { round = 1 }.ToParam()), (22.1, true, null));
        Assert2.Equal(RealParser("22.123", new { round = 2 }.ToParam()), (22.12, true, null));
      }
    }

    [Fact]
    public void Money()
    {
      // should format
      {
        Assert2.Equal(MoneyFormater(null), NulFormat);
        Assert2.Equal(MoneyFormater(""), NulFormat);
        Assert2.Equal(MoneyFormater("A"), "$0.00");
        Assert2.Equal(MoneyFormater("12"), "$12.00");
      }
      // should format: *
      {
        Assert.Throws<Exception>(() => MoneyFormater("12", new { format = "*" }.ToParam()));
      }
      // should format: nx
      {
        Assert2.Equal(MoneyFormater("12", new { format = "c2" }.ToParam()), "$12.00");
        Assert2.Equal(MoneyFormater("12", new { format = "c3" }.ToParam()), "$12.000");
      }
      // should format: pattern
      {
        Assert2.Equal(MoneyFormater("12", new { format = "pattern", pattern = "2" }.ToParam()), "$12.00");
        Assert2.Equal(MoneyFormater("12", new { format = "pattern", pattern = "A" }.ToParam()), "$12.00");
      }
      // should parse
      {
        Assert2.Equal(MoneyParser(null), (null, true, null));
        Assert2.Equal(MoneyParser(""), ("", true, null));
        Assert2.Equal(MoneyParser("NaN"), ("", false, null));
        //Assert2.Equal(MoneyParser(NaN), (NaN, true, null));
        Assert2.Equal(MoneyParser("12"), (12, true, null));
      }
      // should parse: minValue
      {
        Assert2.Equal(MoneyParser("13.0", new { minValue = 12 }.ToParam()), (13, true, null));
        Assert2.Equal(MoneyParser("12.0", new { minValue = 13 }.ToParam()), (12, false, null));
      }
      // should parse: maxValue
      {
        Assert2.Equal(MoneyParser("12.0", new { maxValue = 23 }.ToParam()), (12, true, null));
        Assert2.Equal(MoneyParser("22.0", new { maxValue = 13 }.ToParam()), (22, false, null));
      }
      // should parse: precision
      {
        Assert2.Equal(MoneyParser("22.123", new { precision = 2 }.ToParam()), (22.123, false, null));
        Assert2.Equal(MoneyParser("22.12", new { precision = 2 }.ToParam()), (22.12, true, null));
      }
      // should parse: round
      {
        Assert2.Equal(MoneyParser("22.123", new { round = 1 }.ToParam()), (22.1, true, null));
        Assert2.Equal(MoneyParser("22.123", new { round = 2 }.ToParam()), (22.12, true, null));
      }
    }

    [Fact]
    public void Percent()
    {
      // should format
      {
        Assert2.Equal(PercentFormater(null), NulFormat);
        Assert2.Equal(PercentFormater(""), NulFormat);
        Assert2.Equal(PercentFormater("A"), "NaN%");
        Assert2.Equal(PercentFormater(".12"), "12.00%");
      }
      // should format: *
      {
        Assert.Throws<Exception>(() => PercentFormater("12", new { format = "*" }.ToParam()));
      }
      //should format: nx
      {
        Assert2.Equal(PercentFormater(".12", new { format = "p2" }.ToParam()), "12.00%");
        Assert2.Equal(PercentFormater(".12", new { format = "p3" }.ToParam()), "12.000%");
        Assert2.Equal(PercentFormater(".12", new { format = "p4" }.ToParam()), "12.0000%");
      }
      // should format: pattern
      {
        Assert2.Equal(PercentFormater(".12", new { format = "pattern", pattern = "2" }.ToParam()), "12.00%");
        Assert2.Equal(PercentFormater(".12", new { format = "pattern", pattern = "A" }.ToParam()), "12%");
      }
      // should parse
      {
        Assert2.Equal(PercentParser(null), (null, true, null));
        Assert2.Equal(PercentParser(""), ("", true, null));
        Assert2.Equal(PercentParser("NaN"), ("NaN", false, null));
        //Assert2.Equal(PercentParser(NaN), (NaN, true, null));
        Assert2.Equal(PercentParser("12"), (.12, true, null));
        Assert2.Equal(PercentParser("12%"), (.12, true, null));
      }
    }
  }
}