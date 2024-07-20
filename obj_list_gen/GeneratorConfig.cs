namespace EsiCodeGen;
public class GeneratorConfig
{
	public string? EsiFile { get; set; }
	public int DeviceCode { get; set; }
	public int ProfileIndex { get; set; }
	public int AddrAlignment { get; set; }
	public bool AddrAlignmentIsPowerOfTwo => (AddrAlignment & (AddrAlignment - 1)) == 0;
	public List<int> SpecifiedRecordIndexes { get; set; } = [];
	public List<int> ExposedIndexes { get; set; } = [];
	public List<int> NonVolatileIndexes { get; set; } = [];

}