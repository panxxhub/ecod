using System.Text.RegularExpressions;
using EsiBase;
using EsiInfo;
using ObjListSourceGenerator;

namespace EsiCodeGen;


public static partial class EsiHelperExtensions
{
	public static int DataTypeTypeBitSize(this DataTypeType d)
	{
		return d.BitSize;
	}
	public static IEnumerable<SubItemType> GetSubItems(this DataTypeType d)
	{
		return d.SubItem;
	}
	public static string ToInstName(this string name)
	{
		var n = $"{name.Replace(" ", "_").Replace("-", "_").Replace("/", "_")
				.Replace("'", "_").Replace("(", "").Replace(")", "")
				.Replace(".", "").Replace(",", "").Replace("&", "")
				.Replace(":", "").Replace(";", "").Replace("!", "").Replace("?", "").Replace(">", "").Replace("<", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace("|", "").Replace("\\", "").Replace("\"", "").Replace("`", "").Replace("~", "").Replace("@", "").Replace("#", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("*", "").Replace("+", "")
			.ToLower()}";
		// test if n starts with digit
		if (char.IsDigit(n[0]))
		{
			n = $"x_{n}";
		}
		return n;
	}

	// public static string 

	// ARRAY [0..xx] OF BYTE
	[GeneratedRegex("ARRAY \\[0\\.\\.\\d+\\] OF BYTE")]
	private static partial Regex OctetStringRegex();
	// STRING(xx)
	[GeneratedRegex("STRING\\(\\d+\\)")]
	private static partial Regex VisibleStringRegex();

	public static string ToCANOpenDType(this string dataTypeName)
	{
		var typeName = dataTypeName;
		if (typeName == "BOOL")
		{
			return "DTYPE_BOOL";
		}
		if (typeName == "BYTE")
		{
			return "DTYPE_UNSIGNED8";
		}
		if (typeName == "USINT")
		{
			return "DTYPE_UNSIGNED8";
		}
		if (typeName == "SINT")
		{
			return "DTYPE_INTEGER8";
		}
		if (typeName == "UINT")
		{
			return "DTYPE_UNSIGNED16";
		}
		if (typeName == "INT")
		{
			return "DTYPE_INTEGER16";
		}
		if (typeName == "UDINT")
		{
			return "DTYPE_UNSIGNED32";
		}
		if (typeName == "DINT")
		{
			return "DTYPE_INTEGER32";
		}
		if (OctetStringRegex().IsMatch(typeName))
		{
			return "DTYPE_OCTET_STRING";
		}
		if (VisibleStringRegex().IsMatch(typeName))
		{
			return "DTYPE_VISIBLE_STRING";
		}
		throw new Exception($"Failed to find data type {typeName}");
	}

	public static string ToCTypeDef(string canOpenDataType, string typeName, int count)
	{
		if (canOpenDataType == "DTYPE_OCTET_STRING" | canOpenDataType == "DTYPE_VISIBLE_STRING")
		{
			return $"char {typeName}[{count}]";
		}
		else if (canOpenDataType == "DTYPE_BOOL")
		{
			return $"bool {typeName}";
		}
		else if (canOpenDataType == "DTYPE_UNSIGNED8")
		{
			return $"uint8_t {typeName}";
		}
		else if (canOpenDataType == "DTYPE_INTEGER8")
		{
			return $"int8_t {typeName}";
		}
		else if (canOpenDataType == "DTYPE_UNSIGNED16")
		{
			return $"uint16_t {typeName}";
		}
		else if (canOpenDataType == "DTYPE_INTEGER16")
		{
			return $"int16_t {typeName}";
		}
		else if (canOpenDataType == "DTYPE_UNSIGNED32")
		{
			return $"uint32_t {typeName}";
		}
		else if (canOpenDataType == "DTYPE_INTEGER32")
		{
			return $"int32_t {typeName}";
		}
		throw new Exception("Invalid CANOpenDataType");
	}
}


/// <summary>
/// the most important class in the EsiGen2 namespace 
/// </summary>
/// <param name="Id"> index<<16 | sub_index  </param>
/// <param name="Addr32"> if exposed, the data can be saved to eeprom, we assign an addr to this entry</param>
/// <param name="CANOpenDataType">DTYPE_UNSIGNED8/16/32/BOOL</param>
/// <param name="BitSize"></param>
/// <param name="AccessType">ATYPE_RO/RW</param>
/// <param name="StrInstName">static str inst name</param>
/// <param name="Value">if not exposed, this data would be statically assigned</param>
/// <param name="DataRef">if exposed, this would be the pointer to data</param>
public record SdoEntry(uint Id, uint Addr32, string CANOpenDataType, int BitSize, string AccessType, string StrInstName, int Value, string DataRef, string Name)
{
	public bool Exposure => Addr32 != 0;
	public int SubIdx => (int)(Id & 0xFFFF);
	public string RecordTypeEntry => EsiHelperExtensions.ToCTypeDef(CANOpenDataType, Name.ToInstName(), BitSize / 8);
	public string ArrayTypeEntry => EsiHelperExtensions.ToCTypeDef(CANOpenDataType, "element", BitSize / 8);
}

public abstract class SdoObjBase(int index, bool expose, bool non_volatile, string name, string str_inst_name)
{
	public int Index { get; } = index;
	public string SdoInstName => $"sdo_{Index:x4}";
	public bool Expose { get; } = expose;
	public bool NonVolatile { get; } = non_volatile;
	public string Name { get; } = name;
	public string StrInstName { get; } = str_inst_name;
	public string TypeName => $"{Name.ToInstName()}_t";
	public string StructName => $"{Name.ToInstName()}";
	public string InstId => $"{Name.ToInstName()}";
	public abstract List<SdoEntry> GetEntries();
	public abstract string ObjectType { get; }
	public abstract int MaxSubIndex { get; }

}
public class SdoObjVar(int index, bool expose, bool non_volatile, string name, string str_inst_name, SdoEntry entry) : SdoObjBase(index, expose, non_volatile, name, str_inst_name)
{
	public SdoEntry Entry { get; } = entry;

	public string TypeDef => EsiHelperExtensions.ToCTypeDef(Entry.CANOpenDataType, TypeName, Entry.BitSize / 8);

	public override List<SdoEntry> GetEntries()
	{
		return [Entry];
	}

	public override string ObjectType => "OTYPE_VAR";

	public override int MaxSubIndex => 0;
}

public class SdoObjArray(int index, bool expose, bool non_volatile, string name, string str_inst_name, List<SdoEntry> entries) : SdoObjBase(index, expose, non_volatile, name, str_inst_name)
{
	public List<SdoEntry> Entries { get; } = entries;

	public override List<SdoEntry> GetEntries()
	{
		return Entries;
	}



	public override string ObjectType => "OTYPE_ARRAY";

	public override int MaxSubIndex => Entries.Count - 1;
}

public class SdoObjRecord(int index, bool expose, bool non_volatile, string name, string str_inst_name, List<SdoEntry> entries) : SdoObjBase(index, expose, non_volatile, name, str_inst_name)
{
	public string InstName => Expose ? Name.ToInstName() : string.Empty;
	public List<SdoEntry> Entries { get; } = entries;

	public override List<SdoEntry> GetEntries()
	{
		return Entries;
	}


	public override string ObjectType => "OTYPE_RECORD";

	public override int MaxSubIndex => Entries.Count - 1;
}

public class EsiGen2ContextFactory(DeviceTypeProfile Profile, GeneratorConfig GenConfig)
{
	public CoeObjectCode GetObjectCodeFromObjectType(ObjectType obj, List<DataTypeType> dataTypes)
	{
		var idx = ParseExtensions.ParseEsiHexCode(obj.Index.Value);
		if (obj.Info.SubItem.Count == 0)
		{
			return CoeObjectCode.Var;
		}

		if (GenConfig.SpecifiedRecordIndexes.Contains(idx))
		{
			return CoeObjectCode.Record;
		}
		var obj_type = obj.Type;
		var objDataType = dataTypes.FirstOrDefault(x => x.Name == obj_type) ?? throw new Exception($"Failed to find data type {obj_type}");
		if (objDataType.SubItem.Select(x => x.BitSize).Distinct().Count() > 2)
		{
			return CoeObjectCode.Record;
		}
		return CoeObjectCode.Array;
	}

	private static string GetInstName(Dictionary<string, int> dict, in string name)
	{
		int idx;
		if (dict.TryGetValue(name, out int value))
		{
			idx = value;
		}
		else
		{
			idx = dict.Count;
			dict[name] = idx;
		}
		return $"str_inst_{idx:x4}";
	}
	public EsiGen2Context Create()
	{
		// assert AddrAlignment is power of 2
		if (!GenConfig.AddrAlignmentIsPowerOfTwo)
		{
			throw new Exception("AddrAlignment must be power of 2");
		}

		// dataType
		var dataTypes = Profile.Dictionary.DataTypes.ToList();

		int addr = 0;

		var indexedNames = new Dictionary<string, int>();
		var sdoObjects = new List<SdoObjBase>();
		var align = GenConfig.AddrAlignment;

		static string atype_flag(string access_flag)
		{
			return access_flag == "ro" ? "ATYPE_RO" : "ATYPE_RW";
		}
		static bool atype_is_ro(string access_flag)
		{
			return access_flag.ToLower().Contains("ro");
		}
		static int addr_offset(bool non_volatile, int dataSize, int align)
		{
			return non_volatile ? ((dataSize + align - 1) & ~(align - 1)) : 0;
		}
		static int GetDefaultValue(string defaultValue)
		{
			if (defaultValue == null)
			{
				return 0;
			}
			else if (string.IsNullOrWhiteSpace(defaultValue))
			{
				return 0;
			}
			return defaultValue.ParseEsiHexCode();
		}


		foreach (var obj in Profile.Dictionary.Objects)
		{
			var sdoIdx = ParseExtensions.ParseEsiHexCode(obj.Index.Value);
			var objCode = GetObjectCodeFromObjectType(obj, dataTypes);
			var expose = GenConfig.ExposedIndexes.Contains(sdoIdx);
			var non_volatile = GenConfig.NonVolatileIndexes.Contains(sdoIdx) | expose;
			switch (objCode)
			{
				case CoeObjectCode.Var:
					{
						var start_addr = addr & ~(align - 1);
						var dataSize = obj.BitSize;
						int offset = addr_offset(non_volatile, dataSize, align);
						uint idx = (uint)sdoIdx << 16;
						string dtype = obj.Type.ToCANOpenDType();
						string accessType = atype_flag(obj.Flags.Access.Value);
						var name = obj.Name[0].Value;
						var name_str_inst_name = GetInstName(indexedNames, name);
						var is_string = dtype == "DTYPE_OCTET_STRING" || dtype == "DTYPE_VISIBLE_STRING";
						var data_ref = non_volatile ? (is_string ? $"{name.ToInstName()}" : $"&{name.ToInstName()}") : "NULL";

						var entry = new SdoEntry(
							Id: idx,
							Addr32: non_volatile ? (uint)start_addr : 0,
							CANOpenDataType: dtype,
							BitSize: obj.BitSize,
							AccessType: accessType,
							StrInstName: name_str_inst_name,
							Value: obj.Info.DefaultValue == null ? 0 : ParseExtensions.ParseEsiHexCode(obj.Info.DefaultValue),
							DataRef: data_ref,
							Name: name);
						var sdo_obj_str_inst_name = GetInstName(indexedNames, name);
						var sdoObj = new SdoObjVar(sdoIdx, expose, non_volatile, name, sdo_obj_str_inst_name, entry);
						sdoObjects.Add(sdoObj);
						addr += offset;
						break;
					}
				case CoeObjectCode.Record:
					{
						var dataType = dataTypes.FirstOrDefault(x => x.Name == obj.Type) ?? throw new Exception($"Failed to find data type {obj.Type}");
						var entriesCount = obj.Info.SubItem.Count;
						var entriesCountMinusOne = entriesCount - 1;
						var objInstName = obj.Name[0].Value.ToInstName();

						var subEntries = new List<SdoEntry>();
						for (int subIdx = 0; subIdx < entriesCount; subIdx++)
						{
							var subItem = obj.Info.SubItem[subIdx];
							var start_addr = addr & ~(align - 1);
							var subItemDataType = dataType.SubItem[subIdx];
							var dataSize = subItemDataType.BitSize;
							var subItemName = subItem.Name.ToInstName();
							var a_flag = atype_flag(subItemDataType.Flags.Access.Value);
							var default_value = GetDefaultValue(subItem.Info.DefaultValue);
							var n_ro_or_default_zero = !atype_is_ro(dataType.SubItem[subIdx].Flags.Access.Value) | default_value == 0;
							var this_non_volatile = (subIdx == 0 & n_ro_or_default_zero & non_volatile) | (subIdx != 0 & non_volatile);



							var offset = addr_offset(this_non_volatile, dataSize, align);

							var subEntry = new SdoEntry(
								Id: (uint)sdoIdx << 16 | (uint)subIdx,
								Addr32: this_non_volatile ? (uint)start_addr : 0,
								CANOpenDataType: subItemDataType.Type.ToCANOpenDType(),
								BitSize: dataSize,
								AccessType: a_flag,
								StrInstName: GetInstName(indexedNames, subItem.Name),
								Value: default_value,
								DataRef: this_non_volatile ? $"&{objInstName}.{subItemName}" : "NULL",
								Name: subItem.Name
							);
							subEntries.Add(subEntry);

							addr += offset;
						}
						var sdo_obj_str_inst_name = GetInstName(indexedNames, obj.Name[0].Value);
						var sdoObj = new SdoObjRecord(sdoIdx, expose, non_volatile, obj.Name[0].Value, sdo_obj_str_inst_name, subEntries);

						sdoObjects.Add(sdoObj);

						break;
					}
				case CoeObjectCode.Array:
					{
						var dataType = dataTypes.FirstOrDefault(x => x.Name == obj.Type) ?? throw new Exception($"Failed to find data type {obj.Type}");
						var entriesCount = obj.Info.SubItem.Count;
						var objInstName = obj.Name[0].Value.ToInstName();

						var subEntries = new List<SdoEntry>();
						for (int subIdx = 0; subIdx < entriesCount; subIdx++)
						{
							var subItem = obj.Info.SubItem[subIdx];
							var start_addr = addr & ~(align - 1);
							var dataSize = dataType.DataTypeTypeBitSize();
							var sub_item_dtype = dataType.SubItem[subIdx].Type.ToCANOpenDType();
							var sub_item_data_size = dataType.SubItem[subIdx].BitSize;
							var default_value = GetDefaultValue(subItem.Info.DefaultValue);
							var n_ro_or_default_zero = !atype_is_ro(dataType.SubItem[subIdx].Flags.Access.Value) | default_value == 0;

							var this_non_volatile = (subIdx == 0 & n_ro_or_default_zero & non_volatile) | (subIdx != 0 & non_volatile);

							var subItemName = dataType.SubItem[subIdx].Name.ToInstName();
							var suffix = subIdx == 0 ? subItemName : $"elements[{subIdx - 1}]";

							var offset = addr_offset(this_non_volatile, dataSize, align);

							var subEntry = new SdoEntry(
								Id: (uint)sdoIdx << 16 | (uint)subIdx,
								Addr32: this_non_volatile ? (uint)start_addr : 0,
								CANOpenDataType: sub_item_dtype,
								BitSize: sub_item_data_size,
								AccessType: atype_flag(dataType.SubItem[subIdx].Flags.Access.Value),
								StrInstName: GetInstName(indexedNames, dataType.SubItem[subIdx].Name),
								Value: default_value,
								DataRef: this_non_volatile ? $"&{obj.Name[0].Value.ToInstName()}.{suffix}" : "NULL",
								Name: subItem.Name
							);

							subEntries.Add(subEntry);
							addr += offset;
						}
						var sdo_obj_str_inst_name = GetInstName(indexedNames, obj.Name[0].Value);
						var sdoObj = new SdoObjArray(sdoIdx, expose, non_volatile, obj.Name[0].Value, sdo_obj_str_inst_name, subEntries);

						sdoObjects.Add(sdoObj);

						break;
					}

			}
		}


		var ctx = new EsiGen2Context(dataTypes, sdoObjects, indexedNames);


		return ctx;
	}
}

public class EsiGen2Context(List<DataTypeType> dataTypes, List<SdoObjBase> sdoObjects, Dictionary<string, int> indexedNames)
{
	public List<DataTypeType> DataTypes { get; } = dataTypes;
	public List<SdoObjBase> SdoObjects { get; } = sdoObjects;
	public int SdoObjectsCount => SdoObjects.Count;
	public int SdoObjectsCountLog2Up => (int)Math.Ceiling(Math.Log2(SdoObjects.Count));
	public Dictionary<string, int> IndexedNames { get; } = indexedNames;
	public List<SdoObjBase> SdoObjectsToExpose => SdoObjects.Where(x => x.Expose).ToList();
	public List<SdoObjVar> SdoObjVarToExpose => SdoObjectsToExpose.OfType<SdoObjVar>().ToList();
	public List<SdoObjArray> SdoObjArrayToExpose => SdoObjectsToExpose.OfType<SdoObjArray>().ToList();
	public List<SdoObjRecord> SdoObjRecordToExpose => SdoObjectsToExpose.OfType<SdoObjRecord>().ToList();
	public int GetDataTypeBitSize(string dataTypeName)
	{
		var dataType = DataTypes.FirstOrDefault(x => x.Name == dataTypeName) ?? throw new Exception($"Failed to find data type {dataTypeName}");
		return dataType.DataTypeTypeBitSize();
	}

}

public partial class SdoObjects(EsiGen2Context ctx)
{
	public EsiGen2Context Ctx { get; } = ctx;

}

public partial class SdoObjectsHeader(EsiGen2Context ctx)
{
	public EsiGen2Context Ctx { get; } = ctx;

}