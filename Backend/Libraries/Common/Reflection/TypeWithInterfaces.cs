﻿using System;
using System.Collections.Generic;

namespace ScoreSheets.Common.Reflection
{
	public class TypeWithInterfaces
	{
		public string Id => Type?.AssemblyQualifiedName;
		public Type Type { get; set; }
		public IEnumerable<Type> Interfaces { get; set; }
	}
}
