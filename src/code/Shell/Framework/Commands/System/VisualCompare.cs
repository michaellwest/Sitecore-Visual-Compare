using System;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Pipelines.HasPresentation;
using Sitecore.Publishing;
using Sitecore.Shell.DeviceSimulation;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Sites;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Sharedsource.VisualCompare.Shell.Framework.Commands.System
{
    [Serializable]
    public class VisualCompare : PreviewItem
    {
        protected new void Run(ClientPipelineArgs args)
        {
            Item obj1 = Database.GetItem(ItemUri.Parse(args.Parameters["uri"]));
            if (obj1 == null)
            {
                SheerResponse.Alert("Item not found.");
            }
            else
            {
                string str = obj1.ID.ToString();
                if (args.IsPostBack)
                {
                    if (args.Result != "yes")
                        return;
                    Item obj2 = Context.ContentDatabase.GetItem(LinkManager.GetPreviewSiteContext(obj1).StartPath);
                    if (obj2 == null)
                    {
                        SheerResponse.Alert("Start item not found.");
                        return;
                    }
                    str = obj2.ID.ToString();
                }
                else if (!HasPresentationPipeline.Run(obj1))
                {
                    SheerResponse.Confirm("The current item cannot be previewed because it has no layout for the current device.\n\nDo you want to preview the start Web page instead?");
                    args.WaitForPostBack();
                    return;
                }
                SheerResponse.CheckModified(false);
                SiteContext previewSiteContext = LinkManager.GetPreviewSiteContext(obj1);
                if (previewSiteContext == null)
                {
                    SheerResponse.Alert(Translate.Text("Site \"{0}\" not found", Settings.Preview.DefaultSite));
                }
                else
                {
                    WebUtil.SetCookieValue(previewSiteContext.GetCookieKey("sc_date"), string.Empty);
                    PreviewManager.StoreShellUser(Settings.Preview.AsAnonymous);
                    UrlString urlString = new UrlString("/");
                    urlString["sc_itemid"] = str;
                    urlString["sc_mode"] = "preview";
                    urlString["sc_compare"] = "true";
                    urlString["sc_lang"] = obj1.Language.ToString();
                    urlString["sc_site"] = previewSiteContext.Name;
                    DeviceSimulationUtil.DeactivateSimulators();
                    if (UIUtil.IsChrome())
                        SheerResponse.Eval("setTimeout(function () { window.open('" + urlString + "', '_blank');}, 0);");
                    else
                        SheerResponse.Eval("window.open('" + urlString + "', '_blank');");
                }
            }
        }
    }
}