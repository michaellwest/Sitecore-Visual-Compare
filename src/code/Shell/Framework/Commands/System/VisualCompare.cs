﻿using System;
using System.Collections.Specialized;
using Sitecore.Diagnostics;
using Sitecore.Publishing;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Sites;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Sharedsource.Shell.Framework.Commands.System
{
    [Serializable]
    public class VisualCompare : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            var parameters = new NameValueCollection();
            var noLayout = false;

            if (context.Items.Length == 1)
            {
                var item = context.Items[0];
                parameters.Add("sc_lang", item.Language.ToString());

                if (item.Visualization.Layout != null)
                {
                    parameters.Add("sc_itemid", item.ID.ToString());
                }
                else
                {
                    noLayout = true;
                }
            }

            var args = new ClientPipelineArgs(parameters);

            if (!noLayout)
            {
                args.Result = "yes";
                args.Parameters.Add("needconfirmation", "false");
            }

            Context.ClientPage.Start(this, "Run", args);
        }

        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            if (UIUtil.IsIE() && (UIUtil.GetBrowserMajorVersion() < 7))
            {
                return CommandState.Hidden;
            }

            return base.QueryState(context);
        }

        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (!SheerResponse.CheckModified())
            {
                return;
            }

            if ((args.Parameters["needconfirmation"] == "false") || args.IsPostBack)
            {
                if (args.Result == "no")
                {
                    return;
                }

                var webSiteUrl = SiteContext.GetWebSiteUrl();
                webSiteUrl.Add("sc_mode", "preview");
                webSiteUrl.Add("sc_compare", true.ToString());

                if (!string.IsNullOrEmpty(args.Parameters["sc_itemid"]))
                {
                    webSiteUrl.Add("sc_itemid", args.Parameters["sc_itemid"]);
                }

                if (!string.IsNullOrEmpty(args.Parameters["sc_lang"]))
                {
                    webSiteUrl.Add("sc_lang", args.Parameters["sc_lang"]);
                }

                PreviewManager.RestoreUser();
                Context.ClientPage.ClientResponse.Eval("window.open('" + webSiteUrl + "', '_blank')");
            }
            else
            {
                SheerResponse.Confirm(
                    "The current item does not have layout details; do you want to open the home item instead?");
                args.WaitForPostBack();
            }
        }
    }
}