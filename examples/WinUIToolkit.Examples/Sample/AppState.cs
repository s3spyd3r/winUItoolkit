using winUItoolkit.Helpers;

namespace WinUIToolkit.Examples.Sample
{
    /// <summary>
    /// Concrete singleton for the examples app. The class is thread-safe by virtue of
    /// <see cref="SingletonBase{T}"/>'s Lazy&lt;T&gt; initialization.
    /// </summary>
    public sealed class AppState : SingletonBase<AppState>
    {
        public int InteractionCount { get; private set; }
        public string LastMessage { get; private set; } = "(none)";

        public void Record(string message)
        {
            InteractionCount++;
            LastMessage = message;
        }
    }
}
