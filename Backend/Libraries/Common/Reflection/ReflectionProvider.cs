using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScoreSheets.Common.Exceptions;

namespace ScoreSheets.Common.Reflection
{
	public static class ReflectionProvider
	{
		#region Statics

		private static readonly Dictionary<string, Assembly> Assemblies = new Dictionary<string, Assembly>();

		#endregion

		#region Reflection methods

		public static object GetInstance(string assemblyPath, string className, params object[] constructorParams)
		{
			return GetInstance<object>(assemblyPath, className, constructorParams);
		}

		public static T GetInstance<T>(string assemblyPath, string className, params object[] constructorParams)
		{
			if (!Assemblies.ContainsKey(assemblyPath))
				Assemblies[assemblyPath] = AssemblyManager.LoadAssembly(assemblyPath);
			var assembly = Assemblies[assemblyPath];
			var type = assembly.GetType(className);
			if (type == null)
				throw new ReflectionException($"Cannot find type {className} from assembly {assembly.GetName().Name}@{assemblyPath}");
			var baseType = typeof(T);
			if (baseType != typeof(object) && !baseType.IsAssignableFrom(type))
				throw new ReflectionException(
					$"Type {className} from assembly {assembly.GetName().Name}@{assemblyPath} does not derive from {baseType.FullName}");
			var argumentsTypes = constructorParams.Select(o => o.GetType()).ToArray();
			var arguments = string.Join(", ", constructorParams.Select(o => o.GetType().FullName));
			var constructor = type.GetConstructor(argumentsTypes);
			if (constructor == null)
				throw new ReflectionException(
					$"Cannot find constructor({arguments}) for type {className} from assembly {assembly.GetName().Name}@{assemblyPath}");
			return (T)constructor.Invoke(constructorParams);
		}

		public static object InvokeMethod(object instance, string methodName, params object[] methodParams)
		{
			var type = instance.GetType();
			var argumentsTypes = methodParams.Select(o => o.GetType()).ToArray();
			var arguments = string.Join(", ", methodParams.Select(o => o.GetType().FullName));
			var method = type.GetMethod(methodName, argumentsTypes);
			if (method == null)
				throw new ReflectionException($"Cannot find method {methodName}({arguments}) from type {type.FullName}");
			var parameters = string.Join(", ", method.GetParameters().Select(o => $"{o.ParameterType.FullName} {o.Name}"));
			try
			{
				return method.Invoke(instance, methodParams);
			}
			catch (ArgumentException e)
			{
				throw new ReflectionException(
					$"Mismatching parameters for invoked method {type.FullName}.{method.Name}: expected ({parameters}), supplied ({arguments})", e);
			}
			catch (TargetParameterCountException e)
			{
				throw new ReflectionException(
					$"Mismatching parameters for invoked method {type.FullName}.{method.Name}: expected ({parameters}), supplied ({arguments})", e);
			}
		}

		#endregion
	}
}
