using System.Collections;
using System.ComponentModel;
using EsiBase;
using EsiCodeGen;
using EsiInfo;

namespace ObjListSourceGenerator;

public class EsiRecords(string recordName, EsiDataType dataType)
{
	public string RecordName { get; } = recordName;
	public EsiDataType DataType { get; } = dataType;
	public IEnumerator SumItemDataNameIter()
	{
		foreach (var subItem in DataType.SubItems)
		{
			// convert the name to snake case(trim the space and replace space with underscore, convert to lower case)
			var raw = subItem.Name;
			var name = raw.Replace(" ", "_").ToLower();
			yield return name;
		}
	}

}
public record EsiPredefinedRecord(string RecordTypeName, string Inst);

public class EsiParseContext
{
	public EsiParseContext(DeviceTypeProfile profile)
	{
		_profile = profile;

		foreach (var dataType in profile.Dictionary.DataTypes)
		{
			DataTypes.Add(dataType.Name, new EsiDataType(dataType));
		}

		foreach (var obj in profile.Dictionary.Objects)
		{
			Objs.Add(new EsiObj(obj, this));
		}

		foreach (var obj in Objs)
		{
			var hasSubEntries = obj.SubEntries.Count != 0;
			if (!hasSubEntries)
			{
				_flattenedEntries.Add(new EsiFlattenedEntry(obj));
			}
			else
			{
				foreach (var subEntry in obj.SubEntries)
				{
					_flattenedEntries.Add(new EsiFlattenedEntry(obj, subEntry));
				}
			}
		}
	}

	public static readonly Dictionary<int, EsiPredefinedRecord> RecordNames = new()
	{
		{0x1600, new EsiPredefinedRecord("pdo_mapping_t","rxpdo_mapping_0")},
		{0x1601, new EsiPredefinedRecord("pdo_mapping_t","rxpdo_mapping_1")},
		{0x1602, new EsiPredefinedRecord("pdo_mapping_t","rxpdo_mapping_2")},
		{0x1603, new EsiPredefinedRecord("pdo_mapping_t","rxpdo_mapping_3")},

		{0x1a00, new EsiPredefinedRecord("pdo_mapping_t","txpdo_mapping_0")},
		{0x1a01, new EsiPredefinedRecord("pdo_mapping_t","txpdo_mapping_1")},
		{0x1a02, new EsiPredefinedRecord("pdo_mapping_t","txpdo_mapping_2")},
		{0x1a03, new EsiPredefinedRecord("pdo_mapping_t","txpdo_mapping_3")},

		{0x1c32, new EsiPredefinedRecord("sm_sync_t","sm2_sync")},
		{0x1c33, new EsiPredefinedRecord("sm_sync_t","sm3_sync")},
	};
	public Dictionary<int, EsiPredefinedRecord> EnabledRecordNames => RecordNames.Where(x => Objs.Any(y => y.Index == x.Key)).ToDictionary(x => x.Key, x => x.Value);
	private readonly DeviceTypeProfile _profile;
	public DeviceTypeProfile Raw => _profile;
	public Dictionary<string, EsiDataType> DataTypes { get; } = [];
	public List<EsiObj> Objs { get; } = [];
	private readonly List<EsiFlattenedEntry> _flattenedEntries = [];
	public ILookup<int, EsiFlattenedEntry> FlattenedEntriesLookup => _flattenedEntries.ToLookup(x => x.Index);
	public List<EsiFlattenedEntry> FlattenedEntries => _flattenedEntries;


}
public enum CoeObjectCode
{
	[Description("OTYPE_VAR")]
	Var = 7,
	[Description("OTYPE_ARRAY")]
	Array = 8,

	[Description("OTYPE_RECORD")]
	Record = 9,

}
public class EsiFlattenedEntry
{
	public EsiFlattenedEntry(EsiObj obj)
	{
		_obj = obj;
		SubIndex = 0;
		Name = obj.Name;
		BitSize = obj.BitSize;
		AccessType = obj.AccessType;
		Value = obj.Value;
		IsParent = true;
	}
	public EsiFlattenedEntry(EsiObj obj, EsiObjSubEntry subEntry)
	{
		_obj = obj;
		SubIndex = subEntry.SubIndex;
		Name = subEntry.Name;
		BitSize = subEntry.BitSize;
		AccessType = subEntry.AccessType;
		Value = subEntry.Value;
		IsParent = false;
	}
	private readonly EsiObj _obj;
	public bool IsParent { get; }
	public int Index => _obj.Index;
	public int SubIndex { get; }
	public string Name { get; }
	public string NameIdentifier => IsParent ? $"obj_{Index:x4}_name" : $"obj_{Index:x4}_{SubIndex}_name";
	public int BitSize { get; }
	public string AccessType { get; }
	public int Value { get; }
}
public class EsiDataType(DataTypeType dataType)
{
	private readonly DataTypeType _dataType = dataType;
	public DataTypeType Raw => _dataType;
	public string Name { get; } = dataType.Name;
	public int BitSize { get; } = dataType.BitSize;
	public List<EsiDataTypeSubItem> SubItems { get; } = dataType.SubItem.Select(x => new EsiDataTypeSubItem(x)).ToList();
}
public class EsiDataTypeSubItem(SubItemType subItemType)
{
	private readonly SubItemType _subItemType = subItemType;
	public SubItemType Raw => _subItemType;
	public string Name { get; } = subItemType.Name;
	public int BitSize { get; } = subItemType.BitSize;
	public string DataType { get; } = subItemType.Type; // we shall use this key to find the data type in parse context
}

public class EsiObj
{
	public EsiObj(ObjectType obj, EsiParseContext context)
	{
		_obj = obj;

		var dataType = obj.Type;
		var esiDataType = context.DataTypes[dataType];

		for (int i = 0; i < obj.Info.SubItem.Count; i++)
		{
			var item = obj.Info.SubItem[i];
			var esiDataTypeSub = esiDataType.SubItems[i];
			SubEntries.Add(new EsiObjSubEntry(item, i, esiDataTypeSub));
		}

		var idx = ParseExtensions.ParseEsiHexCode(_obj.Index.Value);
		Index = idx;
		Name = obj.Name[0]?.Value ?? string.Empty;
		AccessType = (SubEntries.Count == 0) ? (obj.Flags.Access.Value == "ro" ? "ATYPE_RO" : "ATYPE_RW") : string.Empty;
		Value = obj.Info.DefaultValue == null ? 0 : ParseExtensions.ParseEsiHexCode(obj.Info.DefaultValue);
	}
	public static readonly int[] RecordIndexes = [0x1018, 0x1600, 0x1601, 0x1602, 0x1603, 0x1A00, 0x1A01, 0x1A02, 0x1A03, 0x1c32, 0x1c33, 0x6048, 0x6049, 0x604A, 0x60C1, 0x60c2, 0x60c4];
	public static CoeObjectCode ResolveObjectCode(EsiObj myObj)
	{

		if (myObj.SubEntries.Count == 0)
		{
			return CoeObjectCode.Var;
		}
		else if (RecordIndexes.Contains(myObj.Index))
		{
			return CoeObjectCode.Record;
		}
		else
		{
			if (myObj.SubEntries.Select(x => x.BitSize).Distinct().Count() > 2)
			{
				return CoeObjectCode.Record;
			}
			return CoeObjectCode.Array;
		}
	}
	public int MaxSub => SubEntries.Count;
	public string NameId => $"obj_{Index:x4}_name";
	public string StructId => $"sdo_{Index:x4}";
	public CoeObjectCode ObjectCode => ResolveObjectCode(this);
	public string ObjectCodeDescription => ParseExtensions.GetEnumDescription(ObjectCode);
	private readonly ObjectType _obj;
	public ObjectType Raw => _obj;
	public int Value { get; }
	public int Index { get; }
	public int BitSize => _obj.BitSize;
	public string Name { get; }
	public List<EsiObjSubEntry> SubEntries { get; } = [];
	public string AccessType { get; }
}
public class EsiObjSubEntry(ObjectInfoTypeSubItem subItem, int idx, EsiDataTypeSubItem dataType)
{
	private readonly ObjectInfoTypeSubItem _subItem = subItem;
	public ObjectInfoTypeSubItem Raw => _subItem;
	public int SubIndex { get; } = idx;
	public string Name { get; } = subItem.Name;
	public EsiDataTypeSubItem DataTypeSubItem { get; } = dataType;
	public int BitSize => DataTypeSubItem.BitSize;
	public string AccessType => DataTypeSubItem.Raw.Flags.Access.Value == "ro" ? "ATYPE_RO" : "ATYPE_RW";
	public int Value { get; } = subItem.Info.DefaultValue == null ? 0 : ParseExtensions.ParseEsiHexCode(subItem.Info.DefaultValue);

}
public partial class SdoObjects(DeviceTypeProfile profile)
{
	private readonly EsiParseContext _context = new(profile);
	public ILookup<int, EsiFlattenedEntry> FlattenedEntriesLookup => _context.FlattenedEntriesLookup;
	public List<EsiFlattenedEntry> FlattenedEntries => _context.FlattenedEntries;
	public List<EsiObj> Objs => _context.Objs;
}

public partial class SdoObjectsHeader(DeviceTypeProfile profile)
{
	private readonly EsiParseContext _context = new(profile);
	public ILookup<int, EsiFlattenedEntry> FlattenedEntriesLookup => _context.FlattenedEntriesLookup;
	public List<EsiFlattenedEntry> FlattenedEntries => _context.FlattenedEntries;
	public List<EsiObj> Objs => _context.Objs;
	public Dictionary<int, EsiPredefinedRecord> EnabledRecordNames => _context.EnabledRecordNames;
}