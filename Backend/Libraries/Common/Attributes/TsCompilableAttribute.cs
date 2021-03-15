using System;

namespace GaiaProject.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
	public class TsCompilableAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public class TsIgnoreAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class TsOptionalAttribute : Attribute
	{
	}
}
