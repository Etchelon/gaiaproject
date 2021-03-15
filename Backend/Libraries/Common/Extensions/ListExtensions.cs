using System.Collections.Generic;
using GaiaProject.Common.Utils;

namespace GaiaProject.Common.Extensions
{
	public static class ListExtensions
	{
		public static IList<T> Shuffle<T>(this IList<T> list)
		{
			var n = list.Count;
			while (n-- > 1)
			{
				var k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list;
		}
	}
}
