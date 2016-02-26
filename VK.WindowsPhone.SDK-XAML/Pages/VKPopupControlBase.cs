using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace VK.WindowsPhone.SDK_XAML.Pages
{
    public class VKPopupControlBase  : UserControl
    {
        private Popup _parentPopup;

        private static readonly List<VKPopupControlBase> _currentlyShownInstances = new List<VKPopupControlBase>();

        public static List<VKPopupControlBase> CurrentlyShownInstances => _currentlyShownInstances;


        public bool IsShown
        {
            get { return _parentPopup.IsOpen; }
            set
            {
                _parentPopup.IsOpen = value;

                if (!value)
                {
                    OnClosing();
                    _currentlyShownInstances.Remove(this);
                }
            }
        }



        public void ShowInPopup(double? width = null, double? height = null)
        {
            var popup = new Popup();
            if (width.HasValue)
            {
                popup.Width = width.Value;
                Width = width.Value;
            }
            if (height.HasValue)
            {
                popup.Height = height.Value;
                Height = height.Value;
            }

            _parentPopup = popup;

            popup.Child = this;

            popup.IsOpen = true;

            _currentlyShownInstances.Add(this);

            PrepareForLoad();
          
        }

        protected virtual void OnClosing()
        {

        }

        protected virtual void PrepareForLoad()
        {
        }
    }
}
