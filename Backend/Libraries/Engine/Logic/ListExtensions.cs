using System.Collections.Generic;

namespace GaiaProject.Engine.Logic
{
	public static class ListExtensions
	{
		public static IList<T> Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n-- > 1)
			{
				int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list;
		}
	}
}
