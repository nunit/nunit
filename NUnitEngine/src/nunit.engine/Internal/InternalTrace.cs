// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;

namespace NUnit.Engine.Internal
{
    /// <summary>
	/// Summary description for InternalTrace.
	/// </summary>
	public class InternalTrace
	{
        private readonly static string TIME_FMT = "HH:mm:ss.fff";

		private static bool initialized;

        private static InternalTraceWriter writer;
        public static InternalTraceWriter Writer
        {
            get { return writer; }
        }

		public static InternalTraceLevel Level;

        public static void Initialize(string logName)
        {
            int lev = (int) new System.Diagnostics.TraceSwitch("NTrace", "NUnit internal trace").Level;
            Initialize(logName, (InternalTraceLevel)lev);
        }

        public static void Initialize(string logName, InternalTraceLevel level)
        {
			if (!initialized)
			{
				Level = level;

				if (writer == null && Level > InternalTraceLevel.Off)
				{
					writer = new InternalTraceWriter(logName);
					writer.WriteLine("InternalTrace: Initializing at level " + Level.ToString());
				}

				initialized = true;
			}
        }

        public static void Flush()
        {
            if (writer != null)
                writer.Flush();
        }

        public static void Close()
        {
            if (writer != null)
                writer.Close();

            writer = null;
        }

        public static Logger GetLogger(string name)
		{
			return new Logger( name );
		}

		public static Logger GetLogger( Type type )
		{
			return new Logger( type.FullName );
		}

        public static void Log(InternalTraceLevel level, string message, string category)
        {
            Log(level, message, category, null);
        }

        public static void Log(InternalTraceLevel level, string message, string category, Exception ex)
        {
            if (Writer != null)
            {
                Writer.WriteLine("{0} {1,-5} [{2,2}] {3}: {4}",
                DateTime.Now.ToString(TIME_FMT),
                level == InternalTraceLevel.Verbose ? "Debug" : level.ToString(),
                System.Threading.Thread.CurrentThread.ManagedThreadId,
                category,
                message);

                if (ex != null)
                    Writer.WriteLine(ex.ToString());
            }
        }
    }
}
