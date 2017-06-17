﻿using System;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Musicus.Helpers
{
    public class ListViewEdgeTappedEventArgs : EventArgs
    {
        private ListViewItem listViewItem;
        public ListViewItem ListViewItem { get { return listViewItem; } }
        public ListViewEdgeTappedEventArgs(ListViewItem listViewItem)
        {
            this.listViewItem = listViewItem;
        }
    }

    public class EdgeTappedListView : ListView
    {
        private const double HIT_TARGET = 32;

        private const double VISUAL_INDICATOR_WIDTH = 12;
        private const string VISUAL_INDICATOR_NAME = "VisualIndicator";
        private ListViewItem listViewItemHighlighted;
        private Rectangle visualIndicator;

        public delegate void ListViewEdgeTappedEventHandler(ListView sender, ListViewEdgeTappedEventArgs e);
        public event ListViewEdgeTappedEventHandler ItemLeftEdgeTapped;
        public Brush LeftEdgeBrush { get; set; }
        public bool IsItemLeftEdgeTapEnabled { get; set; }

        public EdgeTappedListView()
        {
            this.ContainerContentChanging += OnContainerContentChanging;
            listViewItemHighlighted = null;
            LeftEdgeBrush = Application.Current.Resources["SystemControlHighlightAltListAccentLowBrush"] as Brush;
        }


        //alternate



        public static readonly DependencyProperty OddRowBackgroundProperty =
        DependencyProperty.Register("OddRowBackground", typeof(Brush), typeof(AlternatingRowListView), null);


        public Brush OddRowBackground
        {
            get { return (Brush)GetValue(OddRowBackgroundProperty); }
            set { SetValue(OddRowBackgroundProperty, value); }
        }


        public static readonly DependencyProperty EvenRowBackgroundProperty =
        DependencyProperty.Register("EvenRowBackground", typeof(Brush), typeof(AlternatingRowListView), null);


        public Brush EvenRowBackground
        {
            get { return (Brush)GetValue(EvenRowBackgroundProperty); }
            set { SetValue(EvenRowBackgroundProperty, value); }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var listViewItem = element as ListViewItem;
            if (listViewItem != null)
            {
                var index = IndexFromContainer(element);
                if ((index + 1) % 2 == 1)
                {
                    listViewItem.Background = OddRowBackground;
                }
                else
                {
                    listViewItem.Background = EvenRowBackground;
                }

            }

        }

        private Brush GetColorFromHexString(string hexValue)
        {
            //var a = Convert.ToByte(hexValue.Substring(0, 2), 16);
            var r = Convert.ToByte(hexValue.Substring(0, 2), 16);
            var g = Convert.ToByte(hexValue.Substring(2, 2), 16);
            var b = Convert.ToByte(hexValue.Substring(4, 2), 16);
            return new SolidColorBrush(Color.FromArgb(0, r, g, b));
        }

        //alternate end


        private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.InRecycleQueue)
            {
                ListViewItem lvi = (args.ItemContainer as ListViewItem);
                if (lvi != null)
                {
                    UIElement element = VisualTreeHelper.GetChild(lvi, 0) as UIElement;
                    if (element != null)
                    {
                        element.PointerPressed -= OnPointerPressed;
                        element.PointerReleased -= OnPointerReleased;
                        element.PointerCaptureLost -= OnPointerCaptureLost;
                        element.PointerExited -= OnPointerExited;
                    }
                }
            }
            else if (args.Phase == 0)
            {
                ListViewItem lvi = (args.ItemContainer as ListViewItem);
                if (null != lvi)
                {
                    UIElement element = VisualTreeHelper.GetChild(lvi, 0) as UIElement;
                    if (null != element)
                    {
                        element.PointerPressed += OnPointerPressed;
                        element.PointerReleased += OnPointerReleased;
                        element.PointerCaptureLost += OnPointerCaptureLost;
                        element.PointerExited += OnPointerExited;
                    }
                }
            }
        }
        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (IsItemLeftEdgeTapEnabled)
            {
                // This conditional was commented to enable this on non-Mobile devices.
                // var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
                // if (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile")
                {
                    if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                    {
                        UIElement element = (sender as UIElement);
                        if (null != element)
                        {
                            PointerPoint pointerPoint = e.GetCurrentPoint(element);
                            if (pointerPoint.Position.X <= HIT_TARGET)
                            {
                                listViewItemHighlighted = VisualTreeHelper.GetParent(element) as ListViewItem;
                                ShowVisual();
                            }
                        }
                    }
                }
            }
        }
        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (listViewItemHighlighted != null)
            {
                ClearVisual();
            }
        }
        private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            ClearVisual();
        }
        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            ClearVisual();
        }
        private void ShowVisual()
        {
            if (listViewItemHighlighted != null)
            {
                visualIndicator = listViewItemHighlighted.FindName("VISUAL_INDICATOR_NAME") as Rectangle;
                if (visualIndicator == null)
                {
                    visualIndicator = new Rectangle();
                    visualIndicator.Name = VISUAL_INDICATOR_NAME;
                    visualIndicator.Height = listViewItemHighlighted.ActualHeight;
                    visualIndicator.HorizontalAlignment = HorizontalAlignment.Left;
                    visualIndicator.Width = VISUAL_INDICATOR_WIDTH;
                    visualIndicator.Fill = LeftEdgeBrush;
                    visualIndicator.Margin = new Thickness(-(listViewItemHighlighted.Padding.Left), 0, 0, 0);
                    Panel panel = listViewItemHighlighted.ContentTemplateRoot as Panel;
                    if (panel != null)
                    {
                        if (panel is Grid)
                        {
                            visualIndicator.SetValue(Grid.RowSpanProperty, (panel as Grid).RowDefinitions.Count);
                        }
                        panel.Children.Add(visualIndicator);
                    }
                }
                else
                {
                    visualIndicator.Opacity = 1;
                }
            }
        }
        private void ClearVisual()
        {
            if (listViewItemHighlighted != null)
            {
                if (visualIndicator != null)
                {
                    visualIndicator.Opacity = 0;
                    if (this.ItemLeftEdgeTapped != null)
                    {
                        ItemLeftEdgeTapped(this, new ListViewEdgeTappedEventArgs(listViewItemHighlighted));
                    }
                }
                listViewItemHighlighted = null;
            }
        }
    }
}