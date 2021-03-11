using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace GaiaProject.Common.Reflection
{
	public static class ReflectionUtils
	{
		public static void PrintLoadedNonGacAssemblies()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Console.WriteLine("Currently loaded assemblies:");
			foreach (var assembly in assemblies.Where(o => !o.GlobalAssemblyCache))
			{
				var name = assembly.GetName();
				Console.WriteLine($"- {name.Name} - v{name.Version} - @{assembly.Location}");
			}
		}

		public static IEnumerable<TypeWithInterfaces> GetGenericInterfaceImplementersInAssemblyOf<T>(Type genericInterfaceType)
		{
			return GetGenericInterfaceImplementersInAssemblyOf(typeof(T), genericInterfaceType);
		}

		public static IEnumerable<TypeWithInterfaces> GetGenericInterfaceImplementersInAssemblyOf(Type typeToSearch, Type genericInterfaceType)
		{
			return GetGenericInterfaceImplementersInAssembly(typeToSearch.Assembly, genericInterfaceType);
		}

		public static IEnumerable<TypeWithInterfaces> GetGenericInterfaceImplementersInAssemblies(IEnumerable<Assembly> assemblies, Type genericInterfaceType)
		{
			return assemblies
				.SelectMany(a => GetGenericInterfaceImplementersInAssembly(a, genericInterfaceType))
				.Distinct();
		}

		public static IEnumerable<TypeWithInterfaces> GetGenericInterfaceImplementersInAssembly(Assembly assembly, Type genericInterfaceType)
		{
			if (assembly == null)
				throw new ArgumentException(nameof(assembly));
			if (genericInterfaceType == null)
				throw new ArgumentException(nameof(genericInterfaceType));

			return assembly
				.GetTypes()
				.Where(t => !t.IsInterface && !t.IsAbstract)
				.Select(t => new TypeWithInterfaces
				{
					Type = t,
					Interfaces = t.GetImplementedInterfacesOfGenericType(genericInterfaceType)
				})
				.Where(t => t.Interfaces.Any());
		}

		public static IEnumerable<Type> GetInterfaceImplementersInAssemblyOf<T, TInterface>()
		{
			return GetInterfaceImplementersInAssemblyOf<TInterface>(typeof(T));
		}

		public static IEnumerable<Type> GetInterfaceImplementersInAssemblyOf<TInterface>(Type typeToSearch)
		{
			return GetInterfaceImplementersInAssembly(typeToSearch.Assembly, typeof(TInterface));
		}

		public static IEnumerable<Type> GetInterfaceImplementersInAssemblies<TInterface>(IEnumerable<Assembly> assemblies)
		{
			return GetInterfaceImplementersInAssemblies(assemblies, typeof(TInterface));
		}

		public static IEnumerable<Type> GetInterfaceImplementersInAssemblies(IEnumerable<Assembly> assemblies, Type interfaceType)
		{
			return assemblies
				.SelectMany(a => GetInterfaceImplementersInAssembly(a, interfaceType))
				.Distinct();
		}

		public static IEnumerable<Type> GetInterfaceImplementersInAssembly<TInterface>(Assembly assembly)
		{
			return GetInterfaceImplementersInAssembly(assembly, typeof(TInterface));
		}

		public static IEnumerable<Type> GetInterfaceImplementersInAssembly(Assembly assembly, Type interfaceType, bool excludeAbstractTypes = true)
		{
			if (assembly == null)
				throw new ArgumentException(nameof(assembly));
			if (interfaceType == null)
				throw new ArgumentException(nameof(interfaceType));

			return assembly
				.GetTypes()
				.Where(type => interfaceType.IsAssignableFrom(type) && !type.IsInterface && (!excludeAbstractTypes || !type.IsAbstract));
		}

		/// <summary>
		/// Gets an attribute on an enum field value
		/// </summary>
		/// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
		/// <param name="enumVal">The enum value</param>
		/// <returns>The attribute of type T that exists on the enum value</returns>
		public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
		{
			var type = enumVal.GetType();
			var memInfo = type.GetMember(enumVal.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
			return attributes.Length > 0 ? (T)attributes[0] : null;
		}

		/// <summary>
		/// Gets an attribute on an enum field value
		/// </summary>
		/// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
		/// <param name="enumVal">The enum value</param>
		/// <returns>The attribute of type T that exists on the enum value</returns>
		public static bool HasAttributeOfType<T>(this Enum enumVal) where T : Attribute
		{
			return GetAttributeOfType<T>(enumVal) != null;
		}

		public static T Clone<T>(T obj) where T : class
		{
			U CloneSingleValue<U>(U o) where U : class
			{
				var objType = o.GetType();
				if (objType.IsValueType)
				{
					return o;
				}
				var cloneFn = objType.GetMethod(nameof(Clone));
				if (cloneFn != null)
				{
					return cloneFn.Invoke(o, new object[0]) as U;
				}
				return Clone(o);
			}

			if (obj == null)
			{
				return null;
			}
			var actualType = obj.GetType();
			var ret = Activator.CreateInstance(actualType);
			var properties = actualType.GetProperties(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
			).ToList();
			properties.ForEach(propInfo =>
			{
				Debug.WriteLine("Processing property {0}", propInfo.Name);
				if (propInfo.SetMethod == null)
				{
					Debug.WriteLine("No setter found, skipping.", propInfo.Name);
					return;
				}
				var thisVal = propInfo.GetValue(obj);
				if (thisVal == null)
				{
					Debug.WriteLine("Value to copy is null, skipping.", propInfo.Name);
					propInfo.SetValue(ret, null);
					return;
				}
				var propActualType = propInfo.PropertyType;
				object clone;
				if (IsArray(propActualType) || IsList(propActualType))
				{
					if (IsArray(propActualType))
					{
						var arr = thisVal as Array;
						var clonedArray = Activator.CreateInstance(propActualType, arr.Length) as Array;
						for (var index = 0; index < arr.Length; ++index)
						{
							var clonedVal = CloneSingleValue(arr.GetValue(index));
							clonedArray.SetValue(clonedVal, index);
						}
						clone = clonedArray;
					}
					else
					{
						var getEnumeratorMethod = propActualType.GetMethod("GetEnumerator");
						var enumerator = getEnumeratorMethod.Invoke(thisVal, new object[0]);
						var moveNextMethod = enumerator.GetType().GetMethod("MoveNext");
						var getCurrentProperty = enumerator.GetType().GetProperty("Current");
						clone = Activator.CreateInstance(propActualType);
						var addMethod = propActualType.GetMethod("Add");
						while ((bool)moveNextMethod.Invoke(enumerator, new object[0]))
						{
							var val = getCurrentProperty.GetMethod.Invoke(enumerator, new object[0]);
							var clonedVal = CloneSingleValue(val);
							addMethod?.Invoke(clone, new[] { clonedVal });
						}
					}
				}
				else
				{
					clone = CloneSingleValue(thisVal);
				}
				propInfo.SetValue(ret, clone);
			});
			return ret as T;
		}

		private static bool IsArray(Type type)
		{
			return type.IsGenericType && type.IsArray;
		}

		private static bool IsList(Type type)
		{
			if (!type.IsGenericType)
			{
				return false;
			}
			Type a = type.GetGenericTypeDefinition();
			Type b = (new List<object>()).GetType().GetGenericTypeDefinition();
			return a == b;
		}
	}
}
