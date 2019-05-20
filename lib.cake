public static T WithRawArgument<T>(this T settings, string rawArgument) where T : Cake.Core.Tooling.ToolSettings
{
    if (settings == null) throw new ArgumentNullException(nameof(settings));

    if (!string.IsNullOrEmpty(rawArgument))
    {
        var previousCustomizer = settings.ArgumentCustomization;
        if (previousCustomizer != null)
            settings.ArgumentCustomization = builder => previousCustomizer.Invoke(builder).Append(rawArgument);
        else
            settings.ArgumentCustomization = builder => builder.Append(rawArgument);
    }

    return settings;
}

// Workaround for https://github.com/cake-build/cake/issues/2337
void TFBuildPublishTestResults(string filePattern, TFBuildPublishTestResultsData settings)
{
    var tfsLoggingCommand = new StringBuilder("##vso[results.publish ");

    if (settings.TestRunner.HasValue)
        tfsLoggingCommand.Append("type=").Append(settings.TestRunner.Value).Append(';');

    if (settings.MergeTestResults.HasValue)
        tfsLoggingCommand.Append("mergeResults=").Append(settings.MergeTestResults.Value ? "true;" : "false;");

    if (!string.IsNullOrWhiteSpace(settings.Platform))
        tfsLoggingCommand.Append("platform='").Append(settings.Platform).Append("';");

    if (!string.IsNullOrWhiteSpace(settings.Configuration))
        tfsLoggingCommand.Append("config='").Append(settings.Configuration).Append("';");

    if (!string.IsNullOrWhiteSpace(settings.TestRunTitle))
        tfsLoggingCommand.Append("runTitle='").Append(settings.TestRunTitle).Append("';");

    if (settings.PublishRunAttachments.HasValue)
        tfsLoggingCommand.Append("publishRunAttachments=").Append(settings.PublishRunAttachments.Value ? "true;" : "false;");

    var resultFiles = GetFiles(filePattern);
    if (resultFiles.Any())
    {
        tfsLoggingCommand.Append("resultFiles=");

        var isFirst = true;
        foreach (var resultFile in resultFiles)
        {
            if (isFirst) isFirst = false; else tfsLoggingCommand.Append(',');

            var absolutePath = MakeAbsolute(resultFile);

            var separator = '/'; // Starting with Cake 0.31, this should be `= absolutePath.Separator`.
            var nativePath = separator != System.IO.Path.DirectorySeparatorChar
                ? absolutePath.FullPath.Replace(separator, System.IO.Path.DirectorySeparatorChar)
                : absolutePath.FullPath;

            tfsLoggingCommand.Append(nativePath);
        }
    }

    Console.WriteLine(tfsLoggingCommand.Append(']'));
}
