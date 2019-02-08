using System.Text;

namespace GTS.InspectorGeneration.Utilities
{
    public class Builder
    {
        public string Build(string[] parts)
        {
            StringBuilder builder = new StringBuilder();
            foreach(var part in parts)
            {
                builder.Append(part);
            }

            MessageLogger.SuccessBuildSucceded();

            return builder.ToString();
        }
    }
}