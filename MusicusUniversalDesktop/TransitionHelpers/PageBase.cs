﻿#region

using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


#endregion

namespace Musicus
{
    public abstract class PageBase : Page
    {
        public AppBar Bar
        {
            get;
            protected set;
        }

        public virtual void NavigatedFrom(NavigationMode mode)
        {
            _navigatedAway = mode == NavigationMode.Back;
        }

        public virtual void BeforeNavigateTo()
        {
            if (_navigatedAway && _size != Size.Empty)
            {
                Width = _size.Width;
                Height = _size.Height;
            }
            _navigatedAway = false;
        }


        public virtual void NavigatedTo(NavigationMode mode, Object parameter)
        {
            
            //var pageName = "HomePage";
            // if (App.Navigator.CurrentPage != null)
            // {
            //     pageName = App.Navigator.CurrentPage.ToString();
            //     pageName = pageName.Remove(0, pageName.LastIndexOf(".", StringComparison.Ordinal) + 1);
            // }
        }

        private bool _navigatedAway;
        private Size _size = Size.Empty;
        public void SetSize(Size size)
        {
            if (_navigatedAway)
            {
                _size = size;
                return;
            }

            Width = size.Width;
            Height = size.Height;
            Measure(size);
        }
    }
}