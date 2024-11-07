// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using System.Text;
using NuGetUtility.Output.Table;

namespace NuGetUtility.Output.Markdown
{
    public class MarkdownPrinter : TablePrinter
    {
        public MarkdownPrinter(Stream stream, IEnumerable<string> titles) : base(stream, titles)
        {
        }

        protected override async Task WriteHeaders(TextWriter writer)
        {
            await WriteRow(Titles, writer);
            await WriteSeparator(writer);
        }

        protected override async Task WriteBody(TextWriter writer)
        {
            foreach (string[][] row in Rows)
            {
                await WriteRow(row, writer);
            }
        }

        protected override async Task WriteFooter(TextWriter writer)
        {
            await writer.WriteLineAsync();
        }


        protected override async Task WriteRow(string[] values, TextWriter writer)
        {
            for (int i = 0; i < values.Length; i++)
            {
                await writer.WriteAsync("| ");
                await writer.WriteAsync(values[i].PadRight(Lengths[i]));
                await writer.WriteAsync(' ');
            }
            await writer.WriteLineAsync("|");
        }

        protected override async Task WriteSeparator(TextWriter writer)
        {
            foreach (int l in Lengths)
            {
                await writer.WriteAsync("|-" + new string('-', l) + '-');
            }

            await writer.WriteLineAsync("|");
        }
    }
}
