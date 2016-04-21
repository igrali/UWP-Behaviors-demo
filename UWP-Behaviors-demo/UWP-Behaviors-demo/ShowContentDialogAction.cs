using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace UWP_Behaviors_demo
{
    public class ShowContentDialogAction : DependencyObject, IAction
    {
        public static readonly DependencyProperty ContentDialogProperty = DependencyProperty.Register(nameof(ContentDialog), typeof(ContentDialog), typeof(ShowContentDialogAction), new PropertyMetadata(null));

        public ContentDialog ContentDialog
        {
            get { return GetValue(ContentDialogProperty) as ContentDialog; }
            set { SetValue(ContentDialogProperty, value); }
        }

        public object Execute(object sender, object parameter)
        {
            ShowContentDialogAsync();
            return null;
        }

        private async Task ShowContentDialogAsync()
        {
            if (this.ContentDialog != null)
            {
                await this.ContentDialog.ShowAsync();
            }
        }
    }
}
