using EsiBase;
using EsiCodeGen;
using EsiInfo;

namespace ObjListSourceGenerator;

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
	}

	private readonly DeviceTypeProfile _profile;
	public DeviceTypeProfile Raw => _profile;
	public Dictionary<string, EsiDataType> DataTypes { get; } = [];
	public List<EsiObj> Objs { get; } = [];


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
		AccessType = obj.Flags.Access.Value == "ro" ? "ATYPE_RO" : "ATYPE_RW";
	}
	private readonly ObjectType _obj;
	public ObjectType Raw => _obj;
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
}
public partial class ObjsTemplate(DeviceTypeProfile profile)
{
	private readonly DeviceTypeProfile _profile = profile;
	private readonly EsiParseContext _context = new(profile);
	public List<EsiObj> Objs => _context.Objs;
}
