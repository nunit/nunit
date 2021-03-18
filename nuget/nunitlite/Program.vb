' Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

Imports NUnitLite

Public Module Program

    ''' <summary>
    ''' The main program executes the tests. Output may be routed to
    ''' various locations, depending on the arguments passed.
    ''' </summary>
    ''' <remarks>Run with --help for a full list of arguments supported</remarks>
    Public Function Main(ByVal args() As String) As Integer
        Return New AutoRun().Execute(args)
    End Function

End Module
