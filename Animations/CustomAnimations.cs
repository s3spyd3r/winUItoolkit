using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;

namespace winUItoolkit.Animations
{
    /*
     * await CustomAnimations.ShowUIElementAnimationAsync(MyTextBlock, 0.3);
     * await CustomAnimations.HideUIElementAnimationAsync(MyTextBlock, 0.4);
     * await CustomAnimations.OnUIElementLoadedAsync(MyImage, durationSeconds: 0.5);
     */

    public static class CustomAnimations
    {
        public static async Task ShowUIElementAnimationAsync(UIElement element, double durationSeconds)
        {
            if (element is null) return;

            try
            {
                element.Visibility = Visibility.Visible;

                var storyboard = new Storyboard();

                var fadeIn = new DoubleAnimation
                {
                    To = 1,
                    Duration = new Duration(TimeSpan.FromSeconds(durationSeconds)),
                    EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
                };

                Storyboard.SetTarget(fadeIn, element);
                Storyboard.SetTargetProperty(fadeIn, "Opacity");

                storyboard.Children.Add(fadeIn);

                await RunStoryboardAsync(storyboard);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CustomAnimations.ShowUIElementAnimationAsync] {ex}");
            }
        }

        public static async Task HideUIElementAnimationAsync(UIElement element, double durationSeconds)
        {
            if (element is null) return;

            try
            {
                var storyboard = new Storyboard();

                var fadeOut = new DoubleAnimation
                {
                    To = 0,
                    Duration = new Duration(TimeSpan.FromSeconds(durationSeconds)),
                    EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
                };

                Storyboard.SetTarget(fadeOut, element);
                Storyboard.SetTargetProperty(fadeOut, "Opacity");

                storyboard.Children.Add(fadeOut);

                await RunStoryboardAsync(storyboard);
                element.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CustomAnimations.HideUIElementAnimationAsync] {ex}");
            }
        }

        public static async Task SlideInFromBottomAsync(UIElement element, double durationSeconds = 0.4, double offset = 50)
        {
            if (element is null) return;
            try
            {
                element.RenderTransform = new TranslateTransform { Y = offset };
                element.Visibility = Visibility.Visible;

                var storyboard = new Storyboard();

                var fade = new DoubleAnimation { From = 0, To = 1, Duration = TimeSpan.FromSeconds(durationSeconds) };
                Storyboard.SetTarget(fade, element);
                Storyboard.SetTargetProperty(fade, "Opacity");

                var slide = new DoubleAnimation { From = offset, To = 0, Duration = TimeSpan.FromSeconds(durationSeconds), EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } };
                Storyboard.SetTarget(slide, element);
                Storyboard.SetTargetProperty(slide, "(UIElement.RenderTransform).(TranslateTransform.Y)");

                storyboard.Children.Add(fade);
                storyboard.Children.Add(slide);

                await RunStoryboardAsync(storyboard);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CustomAnimations.SlideInFromBottomAsync] {ex}");
            }
        }

        public static async Task ScaleInAsync(UIElement element, double durationSeconds = 0.3, double startScale = 0.8)
        {
            if (element is null) return;
            try
            {
                var scale = new ScaleTransform { ScaleX = startScale, ScaleY = startScale };
                element.RenderTransform = scale;
                element.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
                element.Visibility = Visibility.Visible;

                var storyboard = new Storyboard();

                var scaleX = new DoubleAnimation { To = 1, Duration = TimeSpan.FromSeconds(durationSeconds), EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut } };
                var scaleY = new DoubleAnimation { To = 1, Duration = TimeSpan.FromSeconds(durationSeconds), EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut } };
                var fade = new DoubleAnimation { From = 0, To = 1, Duration = TimeSpan.FromSeconds(durationSeconds) };

                Storyboard.SetTarget(scaleX, element);
                Storyboard.SetTarget(scaleY, element);
                Storyboard.SetTarget(fade, element);

                Storyboard.SetTargetProperty(scaleX, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
                Storyboard.SetTargetProperty(scaleY, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
                Storyboard.SetTargetProperty(fade, "Opacity");

                storyboard.Children.Add(scaleX);
                storyboard.Children.Add(scaleY);
                storyboard.Children.Add(fade);

                await RunStoryboardAsync(storyboard);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CustomAnimations.ScaleInAsync] {ex}");
            }
        }

        public static async Task ShakeAsync(UIElement element, double durationSeconds = 0.5, double amplitude = 10)
        {
            if (element is null) return;
            try
            {
                var transform = new TranslateTransform();
                element.RenderTransform = transform;

                var storyboard = new Storyboard();

                var shake = new DoubleAnimationUsingKeyFrames();
                var keyTimeStep = durationSeconds / 6;

                for (int i = 0; i < 6; i++)
                {
                    double value = (i % 2 == 0) ? amplitude : -amplitude;
                    shake.KeyFrames.Add(new EasingDoubleKeyFrame { Value = value, KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(i * keyTimeStep)) });
                }
                shake.KeyFrames.Add(new EasingDoubleKeyFrame { Value = 0, KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(durationSeconds)) });

                Storyboard.SetTarget(shake, element);
                Storyboard.SetTargetProperty(shake, "(UIElement.RenderTransform).(TranslateTransform.X)");

                storyboard.Children.Add(shake);

                await RunStoryboardAsync(storyboard);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CustomAnimations.ShakeAsync] {ex}");
            }
        }

        public static async Task FlipAsync(UIElement element, double durationSeconds = 0.5, bool horizontal = true)
        {
            if (element is null) return;
            try
            {
                var axisTransform = horizontal
                    ? new ScaleTransform { ScaleX = 1 }
                    : new ScaleTransform { ScaleY = 1 };

                element.RenderTransform = axisTransform;
                element.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);

                var storyboard = new Storyboard();

                var flip = new DoubleAnimation
                {
                    To = 0,
                    Duration = TimeSpan.FromSeconds(durationSeconds / 2),
                    AutoReverse = true,
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard.SetTarget(flip, element);
                Storyboard.SetTargetProperty(flip, horizontal
                    ? "(UIElement.RenderTransform).(ScaleTransform.ScaleX)"
                    : "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");

                storyboard.Children.Add(flip);
                await RunStoryboardAsync(storyboard);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CustomAnimations.FlipAsync] {ex}");
            }
        }

        public static async Task OnUIElementLoadedAsync(object sender, double durationSeconds = 0.5, bool reverse = false)
        {
            if (sender is not FrameworkElement element) return;

            try
            {
                double targetOpacity = reverse ? 0 : 1;
                var storyboard = new Storyboard();

                // Main element fade
                var fade = new DoubleAnimation
                {
                    To = targetOpacity,
                    Duration = new Duration(TimeSpan.FromSeconds(durationSeconds))
                };

                Storyboard.SetTarget(fade, element);
                Storyboard.SetTargetProperty(fade, "Opacity");
                storyboard.Children.Add(fade);

                // Handle linked element in Tag (optional)
                if (element.Tag is UIElement tagElement)
                {
                    var hide = new DoubleAnimation
                    {
                        From = 1,
                        To = 0,
                        Duration = new Duration(TimeSpan.FromSeconds(durationSeconds))
                    };
                    Storyboard.SetTarget(hide, tagElement);
                    Storyboard.SetTargetProperty(hide, "Opacity");
                    storyboard.Children.Add(hide);

                    var collapse = new ObjectAnimationUsingKeyFrames
                    {
                        Duration = new Duration(TimeSpan.FromSeconds(durationSeconds))
                    };

                    var discrete = new DiscreteObjectKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(durationSeconds)),
                        Value = Visibility.Collapsed
                    };

                    collapse.KeyFrames.Add(discrete);
                    Storyboard.SetTarget(collapse, tagElement);
                    Storyboard.SetTargetProperty(collapse, "Visibility");

                    storyboard.Children.Add(collapse);
                }

                await RunStoryboardAsync(storyboard);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CustomAnimations.OnUIElementLoadedAsync] {ex}");
            }
        }

        /// <summary>
        /// Helper to await storyboard completion.
        /// </summary>
        private static Task RunStoryboardAsync(Storyboard storyboard)
        {
            var tcs = new TaskCompletionSource<bool>();

            void OnCompleted(object s, object e)
            {
                storyboard.Completed -= OnCompleted;
                tcs.TrySetResult(true);
            }

            storyboard.Completed += OnCompleted;
            storyboard.Begin();

            return tcs.Task;
        }
    }
}