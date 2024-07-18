using System.ComponentModel;
using System.Text.RegularExpressions;

namespace EsiCodeGen;

public static partial class ParseExtensions
{
	public static int ParseEsiHexCode(this string hexCode)
	{
		Regex hexPattern = EsiHexRegex();
		if (hexPattern.IsMatch(hexCode))
		{
			hexCode = hexCode[2..];
		}
		else
		{
			throw new Exception("Invalid product code");
		}
		return int.Parse(hexCode, System.Globalization.NumberStyles.HexNumber);
	}
	public static bool IsEsiCustomDataType(this string dataType)
	{
		Regex customTypePattern = EsiCustomDataTypeRegex();
		return customTypePattern.IsMatch(dataType);
	}
	public static string GetEnumDescription(this Enum enumValue)
	{
		var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

		var descriptionAttributes = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[] ?? [];

		return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
	}


	[GeneratedRegex("#x[0-9|a-f|A-F]{1,}")]
	private static partial Regex EsiHexRegex();

	[GeneratedRegex("DT[0-9|a-f|A-F]{1,}")]
	private static partial Regex EsiCustomDataTypeRegex();

}
