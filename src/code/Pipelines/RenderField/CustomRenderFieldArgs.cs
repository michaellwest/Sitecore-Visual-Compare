using Sitecore.Pipelines.RenderField;

namespace Sitecore.Sharedsource.Pipelines.RenderField
{
    // to avoid infinite recursion, this class indicates when 
    // a renderField processor calls the renderField pipeline

    public class CustomRenderFieldArgs : RenderFieldArgs
    {
    }
}