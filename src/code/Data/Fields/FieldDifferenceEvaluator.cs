using System.Text;
using Sitecore.Text.Diff;

namespace Sitecore.Sharedsource.VisualCompare.Data.Fields
{
    public class FieldDifferenceEvaluator
    {
        protected void Append(StringBuilder builder, string value, int index, int length)
        {
            Append(builder, value, index, length, null);
        }

        protected void Append(StringBuilder builder, string value, int index, int length, string color)
        {
            if (!string.IsNullOrEmpty(color))
            {
                builder.Append($"<span style='color:{color}'>");
            }

            if (length > 0 && index >= 0)
            {
                builder.Append(StringUtil.Mid(value, index, length));
            }

            if (!string.IsNullOrEmpty(color))
            {
                builder.Append("</span>");
            }
        }

        public string GetDifferences(string first, string second)
        {
            var engine = new DiffEngine();
            var source = new DiffListHtml(first);
            var destination = new DiffListHtml(second);

            engine.ProcessDiff(source, destination, DiffEngineLevel.SlowPerfect);
            var builder = new StringBuilder();

            foreach (DiffResultSpan span in engine.DiffReport())
            {
                if (span == null) continue;
                switch (span.Status)
                {
                    case DiffResultSpanStatus.NoChange:
                        Append(builder, first, span.SourceIndex, span.Length);
                        break;
                    case DiffResultSpanStatus.Replace:
                        Append(builder, first, span.SourceIndex, span.Length, "green");
                        Append(builder, second, span.DestIndex, span.Length, "red;text-decoration:line-through;font-weight:bold");
                        break;
                    case DiffResultSpanStatus.DeleteSource:
                        Append(builder, first, span.SourceIndex, span.Length, "red;text-decoration:line-through;font-weight:bold");
                        break;
                    case DiffResultSpanStatus.AddDestination:
                        Append(builder, second, span.DestIndex, span.Length, "blue;font-weight:bold");
                        break;
                }
            }

            return builder.ToString();
        }
    }
}