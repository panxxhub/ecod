namespace EsiCodeGen;
public class GeneratorConfig
{
	public string? EsiFile { get; set; }
	public int DeviceCode { get; set; }
	public int ProfileIndex { get; set; }
	public List<int> ConstIndexes { get; set; } = [];

}