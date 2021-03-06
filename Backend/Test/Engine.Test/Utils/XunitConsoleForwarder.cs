using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit.Abstractions;

namespace Engine.Test.Utils
{
	public class XunitConsoleForwarder : TextWriter
	{
		private readonly ITestOutputHelper output;

		private IList<char> line = new List<char>();

		public XunitConsoleForwarder(ITestOutputHelper output)
		{
			this.output = output;
		}

		public override Encoding Encoding => Console.Out.Encoding;

		public override void Write(char value)
		{
			if (value == '\n')
			{
				FlushLine();
				line = new List<char>();
				return;
			}

			line.Add(value);
		}

		protected override void Dispose(bool disposing)
		{
			if (line.Count > 0)
			{
				FlushLine();
			}

			base.Dispose(disposing);
		}

		private void FlushLine()
		{
			if (line.Count > 0 && line.Last() == '\r')
			{
				line.RemoveAt(line.Count - 1);
			}

			output.WriteLine(new string(line.ToArray()));
		}
	}
}
