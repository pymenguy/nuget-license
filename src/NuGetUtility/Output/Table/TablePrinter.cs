// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using System.Text;

namespace NuGetUtility.Output.Table
{
    /// <summary>
    ///     Credits: https://stackoverflow.com/a/54943087/1199089
    /// </summary>
    public class TablePrinter
    {
        protected int[] Lengths { get; }
        protected List<string[][]> Rows { get; } = new List<string[][]>();
        protected string[] Titles { get; }

        private readonly Stream _stream;


        public TablePrinter(Stream stream, IEnumerable<string> titles)
        {
            _stream = stream;
            Titles = titles.ToArray();
            Lengths = titles.Select(t => t.Length).ToArray();
        }

        public void AddRow(object?[] row)
        {
            if (row.Length != Titles.Length)
            {
                throw new Exception(
                    $"Added row length [{row.Length}] is not equal to title row length [{Titles.Length}]");
            }

            string[][] rowElements = row.Select(GetLines).ToArray();
            for (int i = 0; i < Titles.Length; i++)
            {
                int maxLineLength = rowElements[i].Any() ? rowElements[i].Max(line => line.Length) : 0;
                if (maxLineLength > Lengths[i])
                {
                    Lengths[i] = maxLineLength;
                }
            }
            Rows.Add(rowElements);
        }

        private string[] GetLines(object? lines)
        {
            if (lines is IEnumerable<object> enumerable)
            {
                return enumerable.Select(o => o.ToString() ?? string.Empty).ToArray();
            }
            return new[] { lines?.ToString() ?? string.Empty };
        }

        public async Task Print()
        {
            using var writer = new StreamWriter(_stream, encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true), bufferSize: 1024, leaveOpen: true);

            await WriteHeaders(writer);
            await WriteBody(writer);
            await WriteFooter(writer);
        }

        protected virtual async Task WriteHeaders(TextWriter writer)
        {
            await WriteSeparator(writer);
            await WriteRow(Titles, writer);
            await WriteSeparator(writer);
        }

        protected virtual async Task WriteBody(TextWriter writer)
        {
            foreach (string[][] row in Rows)
            {
                await WriteRow(row, writer);
            }
        }

        protected virtual async Task WriteFooter(TextWriter writer)
        {
            await WriteSeparator(writer);
        }

        protected virtual async Task WriteRow(string[][] values, TextWriter writer)
        {
            int maximumLines = values.Max(lines => lines.Length);
            for (int line = 0; line < maximumLines; line++)
            {
                await WriteRow(values.Select(v => v.Length > line ? v[line] : string.Empty).ToArray(), writer);
            }
        }

        protected virtual async Task WriteRow(string[] values, TextWriter writer)
        {
            for (int i = 0; i < values.Length; i++)
            {
                await writer.WriteAsync("| ");
                await writer.WriteAsync(values[i].PadRight(Lengths[i]));
                await writer.WriteAsync(' ');
            }
            await writer.WriteLineAsync("|");
        }

        protected virtual async Task WriteSeparator(TextWriter writer)
        {
            foreach (int l in Lengths)
            {
                await writer.WriteAsync("+-" + new string('-', l) + '-');
            }
            await writer.WriteLineAsync("+");
        }
    }
}
