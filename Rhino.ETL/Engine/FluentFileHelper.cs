using System;
using System.Collections;
using System.IO;
using FileHelpers;

namespace Rhino.ETL
{
	public class FluentFileHelper
	{
		private readonly FileHelperAsyncEngine engine;

		public FluentFileHelper(Type type)
		{
			engine = new FileHelperAsyncEngine(type);
		}

		public NicerSyntaxAdapter From(string filename)
		{
			filename = NormalizeFilename(filename);
			engine.BeginReadFile(filename);
			return new NicerSyntaxAdapter(engine);
		}

		public NicerSyntaxAdapter To(string filename)
		{
			filename = NormalizeFilename(filename);
			engine.BeginWriteFile(filename);
			return new NicerSyntaxAdapter(engine);
		}
		
		public NicerSyntaxAdapter AppendTo(string filename)
		{
			   engine.BeginAppendToFile(filename);
			   return new NicerSyntaxAdapter(engine);
		}

		private static string NormalizeFilename(string filename)
		{
			//note that this ignores rooted paths
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
			                    filename);
		}

		public class NicerSyntaxAdapter : IDisposable, IEnumerable
		{
			private readonly FileHelperAsyncEngine engine;

			public NicerSyntaxAdapter(FileHelperAsyncEngine engine)
			{
				this.engine = engine;
			}

			public void Write(object t)
			{
				engine.WriteNext(t);
			}

			public NicerSyntaxAdapter OnError(ErrorMode errorMode)
			{
				engine.ErrorMode = errorMode;
				return this;
			}

			public bool HasErrors
			{
				get { return engine.ErrorManager.HasErrors;  }
			}

			public void OutputErrors(string file)
			{
				engine.ErrorManager.SaveErrors(file);
			}

			public void Dispose()
			{
				IDisposable d = engine;
				d.Dispose();
			}

			public IEnumerator GetEnumerator()
			{
				IEnumerable e = engine;
				return e.GetEnumerator();
			}
		}
	}
}