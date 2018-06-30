using Genersoft.Platform.AppFramework.Service;
using Genersoft.Platform.Core.DataAccess;

namespace ResourceCore
{
    internal class Utility
    {
        internal static IGSPDatabase CurDatabase
        {
            get { return GSPContext.Current.Database; }
        }
    }
}