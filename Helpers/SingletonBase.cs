using System;

namespace winUItoolkit.Helpers
{
    /// <summary>
    /// Generic thread-safe Singleton base class for WinUI 3 and .NET 6+.
    /// Example:
    /// public class NoticiasProvider : SingletonBase<NoticiasProvider>
    /// {
    ///     public async Task LoadAsync() { ... }
    /// }
    /// 
    /// Usage:
    /// await NoticiasProvider.Instance.LoadAsync();
    /// </summary>
    /// <typeparam name="T">The class that inherits from this base class.</typeparam>
    public abstract class SingletonBase<T> where T : class, new()
    {
        private static readonly Lazy<T> _instance = new(() => new T(), isThreadSafe: true);

        /// <summary>
        /// Gets the singleton instance of the derived class.
        /// </summary>
        public static T Instance => _instance.Value;

        /// <summary>
        /// Protected constructor to prevent external instantiation.
        /// </summary>
        protected SingletonBase()
        {
            if (_instance.IsValueCreated)
                throw new InvalidOperationException($"An instance of {typeof(T).Name} already exists. Use {nameof(Instance)} instead.");
        }
    }
}
