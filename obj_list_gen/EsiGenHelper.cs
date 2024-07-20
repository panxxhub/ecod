using System.ComponentModel;
using EsiBase;
using EsiCodeGen;
using EsiInfo;

namespace ObjListSourceGenerator;

public class EsiRecords(string recordName, EsiDataType dataType)
{
	public string RecordName { get; } = recordName.Replace(" ", "_").ToLower();
	public EsiDataType DataType { get; } = dataType;
	public List<EsiDataTypeSubItem> SubItems => DataType.SubItems[1..];
}
public record EsiPredefinedRecord(string RecordTypeName, string Inst);

public class EsiParseContext
{
	public List<int> ConstIndexes { get; }
	public EsiParseContext(DeviceTypeProfile profile, List<int> constIndexes)
	{
		_profile = profile;
		ConstIndexes = constIndexes;

		foreach (var dataType in profile.Dictionary.DataTypes)
		{
			DataTypes.Add(dataType.Name, new EsiDataType(dataType));
		}

		foreach (var obj in profile.Dictionary.Objects)
		{
			Objs.Add(new EsiObj(obj, this));
		}

		var idx = 0;
		foreach (var obj in Objs)
		{
			var inst_name = $"str_{idx:x4}";
			var obj_str_content = obj.Name;
			EsiStringContentToInstName.TryAdd(obj_str_content, inst_name);
			idx++;
			foreach (var sub in obj.SubEntries)
			{
				var sub_inst_name = $"str_{idx:x4}";
				var sub_str_content = sub.Name;
				EsiStringContentToInstName.TryAdd(sub_str_content, sub_inst_name);
				idx++;
			}
		}
	}

	public static readonly Dictionary<int, EsiPredefinedRecord> RecordNames = new() {
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

		{0x1018, new EsiPredefinedRecord("identity_object_t","identity")},
		{0x10f3, new EsiPredefinedRecord("diagnosis_history_t","diagnosis_history")},
		{0x4c00, new EsiPredefinedRecord("analog_servo_parameters_t","analog_servo_parameters")},
		{0x60c2, new EsiPredefinedRecord("interpolation_time_period_t", "interpolation_time_period")}
	};
	public Dictionary<int, EsiPredefinedRecord> EnabledRecordNames => RecordNames.Where(x => Objs.Any(y => y.Index == x.Key)).ToDictionary(x => x.Key, x => x.Value);
	private readonly DeviceTypeProfile _profile;
	public DeviceTypeProfile Raw => _profile;
	public Dictionary<string, EsiDataType> DataTypes { get; } = [];
	public List<EsiObj> Objs { get; } = [];
	public List<EsiRecords> NPredefinedEsiRecords =>
		Objs.Where(x => (x.ObjectCode == CoeObjectCode.Record) & (!RecordNames.ContainsKey(x.Index)))
			.Select(x => new EsiRecords(x.Name
			, DataTypes[x.DataTypeName])).ToList();

	public Dictionary<string, string> EsiStringContentToInstName { get; } = [];
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
	public string CType => ParseExtensions.CoeTypeToCType(Raw.Type);
	public string SnakeCaseName => Name.Replace(" ", "_").Replace("-", "_").ToLower();
}

public class EsiObj
{
	public EsiObj(ObjectType obj, EsiParseContext context)
	{
		_obj = obj;

		var dataType = obj.Type;
		var esiDataType = context.DataTypes[dataType];
		var idx = ParseExtensions.ParseEsiHexCode(_obj.Index.Value);
		for (int sub_idx = 0; sub_idx < obj.Info.SubItem.Count; sub_idx++)
		{
			var item = obj.Info.SubItem[sub_idx];
			var esiDataTypeSub = esiDataType.SubItems[sub_idx];
			SubEntries.Add(new EsiObjSubEntry(item, sub_idx, esiDataTypeSub));
		}

		Index = idx;
		Name = obj.Name[0]?.Value ?? string.Empty;
		AccessType = (SubEntries.Count == 0) ? (obj.Flags.Access.Value == "ro" ? "ATYPE_RO" : "ATYPE_RW") : string.Empty;
		Value = obj.Info.DefaultValue == null ? 0 : ParseExtensions.ParseEsiHexCode(obj.Info.DefaultValue);
		DataTypeName = dataType;
	}
	public static readonly int[] RecordIndexes = [0x1018, 0x1600, 0x1601, 0x1602, 0x1603, 0x1A00, 0x1A01, 0x1A02, 0x1A03, 0x1c32, 0x1c33, 0x6048, 0x6049, 0x604A, 0x60C1, 0x60c2, 0x60c4];
	public string DataTypeName { get; }
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
public class EsiObjSubEntry(ObjectInfoTypeSubItem subItem, int subIdx, EsiDataTypeSubItem dataType)
{
	private readonly ObjectInfoTypeSubItem _subItem = subItem;
	public ObjectInfoTypeSubItem Raw => _subItem;
	public int SubIndex { get; } = subIdx;
	public string Name { get; } = subItem.Name;
	public string NameRef => TestAndGetDiagnosticMessage($"{Name.Replace(" ", "_").Replace("-", "_").ToLower()}");
	private static string TestAndGetDiagnosticMessage(string v)
	{
		if (v.Contains("diagnosis_message_"))
		{
			var idx = v.Replace("diagnosis_message_", "");
			var idx_val = int.Parse(idx);
			return $"msg_group[{idx_val - 1}]";
		}
		return v;
	}
	public EsiDataTypeSubItem DataTypeSubItem { get; } = dataType;
	public int BitSize => DataTypeSubItem.BitSize;
	public string AccessType => DataTypeSubItem.Raw.Flags.Access.Value == "ro" ? "ATYPE_RO" : "ATYPE_RW";
	public int Value { get; } = subItem.Info.DefaultValue == null ? 0 : ParseExtensions.ParseEsiHexCode(subItem.Info.DefaultValue);

}
public partial class SdoObjects(DeviceTypeProfile profile, List<int> constIndexes)
{
	private readonly EsiParseContext _context = new(profile, constIndexes);
	public List<EsiObj> Objs => _context.Objs;
	public Dictionary<int, EsiPredefinedRecord> EnabledRecordNames => _context.EnabledRecordNames;
	public Dictionary<string, string> EsiStringContentToInstName => _context.EsiStringContentToInstName;
	public string NameToInst(string name) => _context.EsiStringContentToInstName[name];
}

public partial class SdoObjectsHeader(DeviceTypeProfile profile, List<int> constIndexes)
{
	private readonly EsiParseContext _context = new(profile, constIndexes);
	public List<EsiObj> Objs => _context.Objs;
	public Dictionary<int, EsiPredefinedRecord> EnabledRecordNames => _context.EnabledRecordNames;
	public List<EsiRecords> NPredefinedEsiRecords => _context.NPredefinedEsiRecords;
	public Dictionary<string, string> EsiStringContentToInstName => _context.EsiStringContentToInstName;

}