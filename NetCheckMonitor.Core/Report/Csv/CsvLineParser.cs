using System.Text;

namespace NetCheckMonitor.Core.Report.Csv;

public static class CsvLineParser
{
    public static IReadOnlyList<string> Parse(string line)
    {
        ArgumentNullException.ThrowIfNull(line);

        var fields = new List<string>();
        var field = new StringBuilder();
        bool quoted = false;

        for (int index = 0; index < line.Length; index++)
        {
            char character = line[index];

            if (character == '"')
            {
                if (quoted &&
                    index + 1 < line.Length &&
                    line[index + 1] == '"')
                {
                    field.Append('"');
                    index++;
                }
                else
                {
                    quoted = !quoted;
                }
            }
            else if (character == ',' && !quoted)
            {
                fields.Add(field.ToString());
                field.Clear();
            }
            else
            {
                field.Append(character);
            }
        }

        fields.Add(field.ToString());

        return fields;
    }
}