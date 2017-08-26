using System;
using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Pipelines.RenderField;
using Sitecore.Sharedsource.VisualCompare.Data.Fields;
using Sitecore.Web;

namespace Sitecore.Sharedsource.VisualCompare.Pipelines.RenderField
{
    public class VisualCompare
    {
        private List<string> _excludeFieldTypes;

        public List<string> ExcludeFieldTypes => _excludeFieldTypes ?? (_excludeFieldTypes = new List<string>());

        public string SourceDb { get; set; }

        public string TargetDb { get; set; }

        public bool RenderTextualDifferences { get; set; }

        public bool RenderBeforeComparison { get; set; }

        public bool IndicateUnchangedFields { get; set; }

        protected string RenderWithoutDifferences(Item item, RenderFieldArgs args)
        {
            var diffArgs = new CustomRenderFieldArgs
            {
                Item = item,
                FieldName = args.FieldName,
                Parameters = args.Parameters,
                RawParameters = args.RawParameters,
                RenderParameters = args.RenderParameters,
                DisableWebEdit = args.DisableWebEdit
            };

            CorePipeline.Run("renderField", diffArgs);
            return diffArgs.Result.ToString();
        }

        protected Item GetItem(RenderFieldArgs args)
        {
            if (args is CustomRenderFieldArgs
                || args.Item.Database.Name != SourceDb
                || ExcludeFieldTypes.Contains(args.FieldTypeKey)
                || !Context.PageMode.IsPreview
                || !MainUtil.GetBool(WebUtil.GetQueryString("sc_compare"), false))
            {
                return null;
            }

            var target = Factory.GetDatabase(TargetDb);
            Assert.IsNotNull(target, "target");
            var item = target.GetItem(args.Item.ID, args.Item.Language);
            return item == null || item.Statistics.Revision == args.Item.Statistics.Revision ? null : item;
        }

        public void Process(RenderFieldArgs args)
        {
            var item = GetItem(args);

            if (item == null)
            {
                return;
            }

            var oldText = item[args.FieldName];
            var updatedText = args.Item[args.FieldName];

            if (String.CompareOrdinal(updatedText, oldText) == 0)
            {
                if (!IndicateUnchangedFields)
                {
                    return;
                }
            }

            if (RenderBeforeComparison)
            {
                oldText = RenderWithoutDifferences(item, args);
                updatedText = RenderWithoutDifferences(args.Item, args);
            }

            if (RenderTextualDifferences && args.FieldTypeKey != "image")
            {
                var evaluator = new FieldDifferenceEvaluator();
                args.Result.FirstPart = evaluator.GetDifferences(oldText, updatedText);
            }
        }
    }
}