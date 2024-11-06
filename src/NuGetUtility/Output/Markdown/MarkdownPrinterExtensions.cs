// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

namespace NuGetUtility.Output.Markdown
{
    internal static class MarkdownPrinterExtensions
    {
        public static MarkdownPrinter Create(Stream stream, params string[] headings)
        {
            return new MarkdownPrinter(stream, headings);
        }
        public static MarkdownPrinter Create(Stream stream, IEnumerable<string> headings)
        {
            return new MarkdownPrinter(stream, headings);
        }

        public static MarkdownPrinter FromValues<T>(this MarkdownPrinter printer,
            IEnumerable<T> values,
            Func<T, IEnumerable<object?>> formatter)
        {
            foreach (T? value in values)
            {
                printer.AddRow(formatter(value).ToArray());
            }

            return printer;
        }
    }
}
