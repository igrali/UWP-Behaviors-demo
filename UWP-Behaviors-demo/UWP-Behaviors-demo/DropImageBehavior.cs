using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Xaml.Interactivity;

namespace UWP_Behaviors_demo
{
    [ContentProperty(Name = "Actions")]
    public class DropImageBehavior : DependencyObject, IBehavior
    {
        private BitmapImage bitmapImage = null;

        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(nameof(Actions), typeof(ActionCollection), typeof(DropImageBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty TargetImageProperty = DependencyProperty.Register(nameof(TargetImage), typeof(Image), typeof(DropImageBehavior), new PropertyMetadata(null));

        public ActionCollection Actions
        {
            get
            {
                ActionCollection actionCollection = this.GetValue(ActionsProperty) as ActionCollection;

                if (actionCollection == null)
                {
                    actionCollection = new ActionCollection();

                    this.SetValue(DropImageBehavior.ActionsProperty, actionCollection);
                }

                return actionCollection;
            }
        }

        public DependencyObject AssociatedObject { get; private set; }

        public Image TargetImage
        {
            get { return GetValue(TargetImageProperty) as Image; }
            set { SetValue(TargetImageProperty, value); }
        }

        public void Attach(DependencyObject associatedObject)
        {
            if (this.AssociatedObject != associatedObject)
            {
                this.AssociatedObject = associatedObject;

                var frameworkElement = associatedObject as FrameworkElement;

                if (frameworkElement != null)
                {
                    frameworkElement.AllowDrop = true;
                    frameworkElement.DragOver += FrameworkElement_DragOver;
                    frameworkElement.Drop += FrameworkElement_Drop;
                }
            }
        }

        public void Detach()
        {
            var frameworkElement = this.AssociatedObject as FrameworkElement;

            if (frameworkElement != null)
            {
                frameworkElement.AllowDrop = false;
                frameworkElement.DragOver -= FrameworkElement_DragOver;
                frameworkElement.Drop -= FrameworkElement_Drop;
            }

            this.bitmapImage = null;
            this.TargetImage = null;
        }

        private void FrameworkElement_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async void FrameworkElement_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                var storageFile = items.FirstOrDefault() as StorageFile;

                if ((storageFile != null) && (storageFile.ContentType == "image/jpeg") && (this.TargetImage != null))
                {
                    this.bitmapImage = new BitmapImage();

                    using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read))
                    {
                        await bitmapImage.SetSourceAsync(stream);
                    }

                    this.TargetImage.Source = bitmapImage;
                    Interaction.ExecuteActions(this.AssociatedObject, this.Actions, null);
                }
            }
        }
    }
}
