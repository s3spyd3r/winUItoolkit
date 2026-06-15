using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Animations;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class AnimationsPage : UserControl
    {
        public AnimationsPage()
        {
            InitializeComponent();
            TargetBox.Visibility = Visibility.Collapsed;
        }

        private async void Show_Click(object sender, RoutedEventArgs e)
            => await CustomAnimations.ShowUIElementAnimationAsync(TargetBox, 0.3);

        private async void Hide_Click(object sender, RoutedEventArgs e)
            => await CustomAnimations.HideUIElementAnimationAsync(TargetBox, 0.3);

        private async void SlideIn_Click(object sender, RoutedEventArgs e)
            => await CustomAnimations.SlideInFromBottomAsync(TargetBox);

        private async void ScaleIn_Click(object sender, RoutedEventArgs e)
            => await CustomAnimations.ScaleInAsync(TargetBox);

        private async void Shake_Click(object sender, RoutedEventArgs e)
            => await CustomAnimations.ShakeAsync(TargetBox);

        private async void Flip_Click(object sender, RoutedEventArgs e)
            => await CustomAnimations.FlipAsync(TargetBox);

        private async void Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (TargetBox.Visibility == Visibility.Visible)
                await CustomAnimations.HideUIElementAnimationAsync(TargetBox, 0.25);
            else
                await CustomAnimations.ShowUIElementAnimationAsync(TargetBox, 0.25);
        }
    }
}
