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
			return int.Parse(hexCode, System.Globalization.NumberStyles.HexNumber);
		}
		throw new Exception("Invalid product code");
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
	public static string CoeTypeToCType(this string coeType)
	{
		return coeType switch
		{
			"BOOL" => "bool",
			"USINT" => "uint8_t",
			"SINT" => "int8_t",
			"UINT" => "uint16_t",
			"INT" => "int16_t",
			"UDINT" => "uint32_t",
			"DINT" => "int32_t",
			"ARRAY [0..15] OF BYTE" => "uint8_t[16]",
			"STRING(2)" => "char[2]",
			_ => throw new Exception("Invalid COE type")
		};
	}


	[GeneratedRegex("#x[0-9|a-f|A-F]{1,}")]
	private static partial Regex EsiHexRegex();

	[GeneratedRegex("DT[0-9|a-f|A-F]{1,}")]
	private static partial Regex EsiCustomDataTypeRegex();

}
