using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using GaiaProject.Common.Exceptions;

namespace GaiaProject.Common.Reflection
{
	public class AssemblyManager
	{
		#region Settings

		private static readonly object Lock = new object();
		private static string _assembliesPath;
		public static string AssembliesPath
		{
			get
			{
				if (_assembliesPath == null)
					throw new ApplicationException("AssemblyManager.AssembliesPath was not initialized");
				return _assembliesPath;
			}
			set
			{
				if (_assembliesPath != null)
					return;

				lock (Lock)
				{
					if (_assembliesPath == null)
						_assembliesPath = value;
				}
			}
		}

		#endregion

		#region Assembly loading

		internal static Assembly LoadAssembly(string assemblyPath)
		{
			var assemblyFile = new FileInfo(assemblyPath);
			if (!assemblyFile.Exists)
			{
				// Fallback on local path
				assemblyFile = new FileInfo(assemblyFile.Name);
				if (!assemblyFile.Exists)
					throw new ReflectionException($"Requested assembly does not exist: {assemblyPath}");
			}

			// Get assembly metadata
			var assemblyMetadata = Assembly.ReflectionOnlyLoadFrom(assemblyFile.FullName);
			var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			var alreadyLoadedAssembly = appDomainAssemblies.FirstOrDefault(o => o.GetName().FullName == assemblyMetadata.GetName().FullName);
			if (alreadyLoadedAssembly != null)
				return alreadyLoadedAssembly;
			// If no similar assembly is already loaded, load the required one after copying the files
			if (string.IsNullOrEmpty(AssembliesPath))
				return Assembly.LoadFrom(assemblyFile.FullName);
			CopyAssemblies(assemblyFile.Directory, AssembliesPath);
			var reflectionAssemblyPath = Path.Combine(AssembliesPath, assemblyFile.Name);
			return Assembly.LoadFrom(reflectionAssemblyPath);
		}

		internal static void CopyAssemblies(DirectoryInfo directory, string assembliesPath)
		{
			Directory.CreateDirectory(assembliesPath);
			var files = directory.GetFiles("*.dll");
			foreach (var file in files)
			{
				var reflectionAssemblyPath = Path.Combine(assembliesPath, file.Name);
				if (!AreAssembliesEqual(file.FullName, reflectionAssemblyPath))
					file.CopyTo(reflectionAssemblyPath, true);
			}
		}

		#endregion

		#region Hash comparison

		private static bool AreAssembliesEqual(string srcPath, string dstPath)
		{
			if (!File.Exists(dstPath))
				return false;
			var srcHash = GetFileHash(srcPath);
			var dstHash = GetFileHash(dstPath);
			return srcHash.SequenceEqual(dstHash);
		}

		private static byte[] GetFileHash(string filePath)
		{
			using (var md5 = MD5.Create())
			{
				using (var stream = File.OpenRead(filePath))
				{
					return md5.ComputeHash(stream);
				}
			}
		}

		#endregion
	}
}
