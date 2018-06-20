using System.Collections.Generic;
using System.Text;

namespace uap10_0
{
    internal struct CommandLineArgsReader
    {
        private readonly string commandLine;
        private int position;
        private bool pointingAtProcessName;

        public CommandLineArgsReader(string commandLine, bool containsOnlyArgs)
        {
            this.commandLine = commandLine;
            position = 0;
            pointingAtProcessName = !containsOnlyArgs;
        }

        private static bool IsWhitespace(char c)
        {
            switch (c)
            {
                case ' ':
                case '\t':
                    return true;
                default:
                    return false;
            }
        }

        private void SkipProcessFileName()
        {
            if (commandLine.Length == 0) return;

            if (commandLine[position] == '"')
            {
                do position++;
                while (position < commandLine.Length && commandLine[position] != '"');
                position++;
            }
            else
            {
                do position++;
                while (position < commandLine.Length && !IsWhitespace(commandLine[position]));
            }
        }

        private bool MoveNextArgStart()
        {
            if (pointingAtProcessName)
            {
                SkipProcessFileName();
                pointingAtProcessName = false;
            }

            while (position < commandLine.Length)
            {
                if (!IsWhitespace(commandLine[position])) return true;
                position++;
            }

            return false;
        }

        public string ReadNext()
        {
            if (!MoveNextArgStart()) return null;

            var startPosition = position;

            if (commandLine[position] != '"')
            {
                do position++;
                while (position < commandLine.Length && !IsWhitespace(commandLine[position]));

                return commandLine.Substring(startPosition, position - startPosition);
            }

            // Reference: https://msdn.microsoft.com/library/17w5ykft.aspx

            var builder = new StringBuilder();
            while (true)
            {
                position++;
                var c = commandLine[position];
                switch (c)
                {
                    case '\\':
                        var backslashStart = position;
                        do
                        {
                            position++;
                            c = commandLine[position];
                        }
                        while (c == '\\');

                        var backslashCount = position - backslashStart;

                        if (c == '"')
                        {
                            builder.Append(commandLine, backslashStart, backslashCount / 2);
                            if (backslashCount % 2 == 0)
                            {
                                position++;
                                return builder.ToString();
                            }
                            builder.Append('"');
                        }
                        else
                        {
                            builder.Append(commandLine, backslashStart, backslashCount).Append(c);
                        }
                        break;
                    case '"':
                        position++;
                        return builder.ToString();
                    default:
                        builder.Append(c);
                        break;
                }
            }
        }

        public string[] ReadToEnd()
        {
            var all = new List<string>();

            while (true)
            {
                var arg = ReadNext();
                if (arg == null) return all.ToArray();
                all.Add(arg);
            }
        }
    }
}
