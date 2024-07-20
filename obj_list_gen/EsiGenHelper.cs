using System.ComponentModel;

namespace ObjListSourceGenerator;

public enum CoeObjectCode
{
	[Description("OTYPE_VAR")]
	Var = 7,
	[Description("OTYPE_ARRAY")]
	Array = 8,

	[Description("OTYPE_RECORD")]
	Record = 9,

}
