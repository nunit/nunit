// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using NUnitLite.Runner;

namespace NUnitLite.Tests
{
    class Program
    {
        // The main program executes the tests. Output may be routed to
        // various locations, depending on the sub class of TextUI
        // that is created and any arument passed.
        //
        // Examples:
        //
        // Sending output to the console works on desktop Windows and Windows CE.
        //
        //      new ConsoleUI().Execute( args );
        //
        // Debug output works on all platforms.
        //
        //      new DebugUI().Execute( args );
        //
        // Sending output to a file works on all platforms, but care must be taken
        // to write to an accessible location on some platforms.
        //
        //      string myDocs = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        //      string path = System.IO.Path.Combine( myDocs, "TestResult.txt" );
        //      System.IO.TextWriter writer = new System.IO.StreamWriter(output);
        //      new TextUI( writer ).Execute( args );
        //      writer.Close();
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Platform == PlatformID.WinCE)
            {
                string path = System.IO.Path.Combine(Env.DocumentFolder, "TestResult.txt");
                System.IO.TextWriter writer = new System.IO.StreamWriter(path);
                new TextUI(writer).Execute(args);
                writer.Close();
                //            new TcpUI("ppp_peer", 9000).Execute(args);
            }
            else
            {
                //            new TcpUI("ferrari", 9000).Execute(args);
                new ConsoleUI().Execute(args);
            }
        }
    }
}