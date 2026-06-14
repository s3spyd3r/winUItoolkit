using System;

namespace winUItoolkit.Helpers
{
    public static class RuntimeHelper
    {
        /// <summary>
        /// Returns <c>true</c> when the app is running with MSIX package identity.
        /// </summary>
        /// <remarks>
        /// <see cref="Windows.ApplicationModel.Package.Current"/> throws
        /// <see cref="InvalidOperationException"/> in unpackaged scenarios; that exception is the
        /// signal we use to distinguish packaged from unpackaged and is not a real error here.
        /// </remarks>
        public static bool IsPackaged()
        {
            try
            {
                return Windows.ApplicationModel.Package.Current != null;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
