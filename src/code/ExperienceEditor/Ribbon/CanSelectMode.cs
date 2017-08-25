using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.ExperienceEditor.Utils;
using Sitecore.SecurityModel;

namespace Sitecore.SharedSource.VisualCompare.ExperienceEditor.Ribbon
{
    public class CanSelectMode : Sitecore.ExperienceEditor.Speak.Ribbon.Requests.SelectMode.CanSelectMode
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            Assert.IsNotNullOrEmpty(this.RequestContext.Value, "Could not get string value for requestArgs:{0}", (object)this.Args.Data);
            string mode = string.Empty;
            using (new SecurityDisabler())
            {
                Item obj = Client.CoreDatabase.GetItem(new ID(HttpUtility.UrlDecode(this.RequestContext.Value)));
                if (obj != null)
                    mode = obj["Message"];
            }
            switch (mode)
            {
                case "sharedsource:system:compare":
                    return new PipelineProcessorResponseValue()
                    {
                        Value = WebEditUtility.CanPreview()
                    };
                default:
                    return base.ProcessRequest();
            }
        }
    }
}
