using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.ExperienceEditor.Utils;
using Sitecore.SecurityModel;
using Sitecore.Text;
using Sitecore.Web;

namespace Sitecore.SharedSource.VisualCompare.ExperienceEditor.Ribbon
{
    public class SelectModeRequest : Sitecore.ExperienceEditor.Speak.Ribbon.Requests.SelectMode.SelectModeRequest
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            var response = base.ProcessRequest();

            if (response.AbortMessage != null)
                return response;

            string mode = string.Empty;
            UrlString url = new UrlString(response.Value.ToString());
            string[] strArray = this.RequestContext.Value.Split('|');

            using (new SecurityDisabler())
            {
                Item obj = Client.CoreDatabase.GetItem(new ID(HttpUtility.UrlDecode(strArray[0])));
                if (obj != null)
                    mode = obj["Message"];
            }

            if (mode == "webedit:preview")
            {
                url.Parameters["sc_compare"] = "false";
                response.Value = url.ToString();
            }

            if (mode == "sharedsource:system:compare")
            {
                WebUtil.SetCookieValue("sc_last_page_mode_command", mode);
                WebEditUtility.ActivatePreview(url);
                url.Parameters["sc_compare"] = "true";
                response.Value = url.ToString();
            }

            return response;
        }
    }
}
