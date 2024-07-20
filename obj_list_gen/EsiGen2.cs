using System.Security.Cryptography;
using System.Text.RegularExpressions;
using EsiBase;
using EsiInfo;
using ObjListSourceGenerator;

namespace EsiCodeGen;

public class EsiGen2
{

}
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
		return $"{name.Replace(" ", "_").Replace("-", "_").ToLower()}";
	}

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
public record SdoEntry(uint Id, uint Addr32, string CANOpenDataType, int BitSize, string AccessType, string StrInstName, int Value, string DataRef)
{
	public bool Exposure => Addr32 != 0;

}

public class SdoObjBase(int index, bool expose, bool non_volatile, string name)
{
	public int Index { get; } = index;
	public bool Expose { get; } = expose;
	public bool NonVolatile { get; } = non_volatile;
	public string Name { get; } = name;

}
public class SdoObjVar(int index, bool expose, bool non_volatile, string name, SdoEntry entry) : SdoObjBase(index, expose, non_volatile, name)
{
	public SdoEntry Entry { get; } = entry;

}

public class SdoObjArray(int index, bool expose, bool non_volatile, string name, List<SdoEntry> entries) : SdoObjBase(index, expose, non_volatile, name)
{
	public List<SdoEntry> Entries { get; } = entries;


}

public class SdoObjRecord(int index, bool expose, bool non_volatile, string name, List<SdoEntry> entries) : SdoObjBase(index, expose, non_volatile, name)
{
	public string InstName => Expose ? Name.ToInstName() : string.Empty;
	public List<SdoEntry> Entries { get; } = entries;
}

public class EsiGen2ContextFactory(DeviceTypeProfile Profile, List<int> RecordIndexesSpecified, List<int> ExposedObjectIndexes, List<int> NonVolatileStoredObjects, int AddrAlignment)
{
	public CoeObjectCode GetObjectCodeFromObjectType(ObjectType obj, List<DataTypeType> dataTypes)
	{
		var idx = ParseExtensions.ParseEsiHexCode(obj.Index.Value);
		if (obj.Info.SubItem.Count == 0)
		{
			return CoeObjectCode.Var;
		}

		if (RecordIndexesSpecified.Contains(idx))
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
		var idx = 0;
		if (dict.TryGetValue(name, out int value))
		{
			idx = value;
		}
		else
		{
			dict[name] = dict.Count;
		}
		return $"str_{idx:x4}";
	}
	public EsiGen2Context Create()
	{
		// assert AddrAlignment is power of 2
		if ((AddrAlignment & (AddrAlignment - 1)) != 0)
		{
			throw new Exception("AddrAlignment must be power of 2");
		}

		// dataType
		var dataTypes = Profile.Dictionary.DataTypes.ToList();

		int addr = 0;

		var indexedNames = new Dictionary<string, int>();
		var sdoObjects = new List<SdoObjBase>();

		foreach (var obj in Profile.Dictionary.Objects)
		{
			var sdoIdx = ParseExtensions.ParseEsiHexCode(obj.Index.Value);
			var objCode = GetObjectCodeFromObjectType(obj, dataTypes);
			var expose = ExposedObjectIndexes.Contains(sdoIdx);
			var non_volatile = NonVolatileStoredObjects.Contains(sdoIdx) | ExposedObjectIndexes.Contains(sdoIdx);
			switch (objCode)
			{
				case CoeObjectCode.Var:
					{
						var start_addr = addr & ~(AddrAlignment - 1);
						var dataSize = obj.BitSize;
						int offset = non_volatile ? ((dataSize + AddrAlignment - 1) & ~(AddrAlignment - 1)) : 0;
						uint idx = (uint)sdoIdx << 16;
						string dtype = obj.Type.ToCANOpenDType();
						string accessType = obj.Flags.Access.Value == "ro" ? "ATYPE_RO" : "ATYPE_RW";
						var name = obj.Name[0].Value;
						var name_str_inst_name = GetInstName(indexedNames, name);
						var data_ref = non_volatile ? $"&{name.ToInstName()}" : "NULL";

						var entry = new SdoEntry(
							Id: idx,
							Addr32: non_volatile ? (uint)start_addr : 0,
							CANOpenDataType: dtype,
							BitSize: obj.BitSize,
							AccessType: accessType,
							StrInstName: name_str_inst_name,
							Value: obj.Info.DefaultValue == null ? 0 : ParseExtensions.ParseEsiHexCode(obj.Info.DefaultValue),
							DataRef: data_ref);

						var sdoObj = new SdoObjVar(sdoIdx, expose, non_volatile, name, entry);
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
							var start_addr = addr & ~(AddrAlignment - 1);
							var subItemDataType = dataType.SubItem[subIdx];
							var dataSize = subItemDataType.BitSize;
							var offset = non_volatile ? ((dataSize + AddrAlignment - 1) & ~(AddrAlignment - 1)) : 0;
							var subItemName = subItem.Name.ToInstName();
							var subEntry = new SdoEntry(
								Id: (uint)sdoIdx << 16 | (uint)subIdx,
								Addr32: non_volatile ? (uint)start_addr : 0,
								CANOpenDataType: subItemDataType.Name.ToCANOpenDType(),
								BitSize: dataSize,
								AccessType: subItemDataType.Flags.Access.Value == "ro" ? "ATYPE_RO" : "ATYPE_RW",
								StrInstName: GetInstName(indexedNames, subItem.Name),
								Value: subItem.Info.DefaultValue == null ? 0 : ParseExtensions.ParseEsiHexCode(subItem.Info.DefaultValue),
								DataRef: non_volatile ? $"&{objInstName}.{subItemName}" : "NULL"
							);
							subEntries.Add(subEntry);

							addr += offset;
						}
						var sdoObj = new SdoObjRecord(sdoIdx, expose, non_volatile, obj.Name[0].Value, subEntries);
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
							var start_addr = addr & ~(AddrAlignment - 1);
							var offset = non_volatile ? ((dataType.DataTypeTypeBitSize() + AddrAlignment - 1) & ~(AddrAlignment - 1)) : 0;
							var sub_item_dtype = dataType.SubItem[subIdx].Name.ToCANOpenDType();
							var sub_item_data_size = dataType.SubItem[subIdx].BitSize;
							var subEntry = new SdoEntry(
								Id: (uint)sdoIdx << 16 | (uint)subIdx,
								Addr32: non_volatile ? (uint)start_addr : 0,
								CANOpenDataType: sub_item_dtype,
								BitSize: sub_item_data_size,
								AccessType: dataType.SubItem[subIdx].Flags.Access.Value == "ro" ? "ATYPE_RO" : "ATYPE_RW",
								StrInstName: GetInstName(indexedNames, dataType.SubItem[subIdx].Name),
								Value: dataType.SubItem[subIdx].DefaultValue == null ? 0 : ParseExtensions.ParseEsiHexCode(dataType.SubItem[subIdx].DefaultValue),
								DataRef: non_volatile ? $"&{obj.Name[0].Value.ToInstName()}.elements[{subIdx}]" : "NULL"
							);

							subEntries.Add(subEntry);
							addr += offset;
						}
						var sdoObj = new SdoObjArray(sdoIdx, expose, non_volatile, obj.Name[0].Value, subEntries);
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
	public Dictionary<string, int> IndexedNames { get; } = indexedNames;

	public int GetDataTypeBitSize(string dataTypeName)
	{
		var dataType = DataTypes.FirstOrDefault(x => x.Name == dataTypeName) ?? throw new Exception($"Failed to find data type {dataTypeName}");
		return dataType.DataTypeTypeBitSize();
	}

}