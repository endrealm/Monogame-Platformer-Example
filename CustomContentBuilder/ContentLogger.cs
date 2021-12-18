using Microsoft.Xna.Framework.Content.Pipeline;

namespace CustomContentBuilder
{
    public static class ContentLogger
    {
        public static ContentBuildLogger Logger { get; set; }

        public static void LogMessage(string message)
        {
            Logger?.LogMessage(message);
        }
    }
}