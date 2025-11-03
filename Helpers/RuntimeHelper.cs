using System;

namespace winUItoolkit.Helpers
{
    public static class RuntimeHelper
    {
        public static bool IsPackaged()
        {
            try
            {
                var pkg = Windows.ApplicationModel.Package.Current;
                return pkg != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
