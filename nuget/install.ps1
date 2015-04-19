param($installPath, $toolsPath, $package, $project)
If ($project.Type -ne "VB.NET")
{
    $project.ProjectItems.Item("Program.vb").Delete()
}
If ($project.Type -ne "C#")
{
    $project.ProjectItems.Item("Program.cs").Delete()
}
