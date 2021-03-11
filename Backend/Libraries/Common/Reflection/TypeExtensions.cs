using System;
using System.Collections.Generic;
using System.Linq;

namespace GaiaProject.Common.Reflection
{
	public static class TypeExtensions
	{
		public static IEnumerable<Type> GetImplementedInterfacesOfGenericType(this Type type, Type interfaceType)
		{
			return type
				.GetInterfaces()
				.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
		}
	}
}
