// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using NUnit.Engine.Services;

namespace NUnit.Engine.Internal
{
    /// <summary>
	/// Summary description for InternalTrace.
	/// </summary>
	public class InternalTrace
	{
        private static InternalTraceService service = new InternalTraceService();

		public static InternalTraceLevel Level
        {
            get { return service.Level; }
        }

        public static void Initialize(string logName, InternalTraceLevel level)
        {
            service.Initialize(logName, level);
        }

        public static Logger GetLogger(string name)
		{
			return service.GetLogger( name );
		}

		public static Logger GetLogger( Type type )
		{
			return service.GetLogger( type.FullName );
		}

        public static void Log(InternalTraceLevel level, string message, string category)
        {
            service.Log(level, message, category, null);
        }

        public static void Log(InternalTraceLevel level, string message, string category, Exception ex)
        {
            service.Log(level, message, category, ex);
        }
    }
}
