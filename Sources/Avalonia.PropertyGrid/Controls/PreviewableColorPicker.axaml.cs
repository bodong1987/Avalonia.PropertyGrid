using System;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// A custom control that allows users to pick a color with a preview feature.
    /// </summary>
    public class PreviewableColorPicker : TemplatedControl
    {
        /// <summary>
        /// Defines the <see cref="Color"/> property.
        /// </summary>
        public static readonly StyledProperty<Color> ColorProperty =
            AvaloniaProperty.Register<PreviewableColorPicker, Color>(nameof(Color), Colors.Transparent);
        
        /// <summary>
        /// Gets or sets the selected color.
        /// </summary>
        public Color Color
        {
            get => GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        /// <summary>
        /// Defines the <see cref="PreviewColorChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> PreviewColorChangedEvent =
            RoutedEvent.Register<PreviewableColorPicker, RoutedEventArgs>(
                nameof(PreviewColorChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Defines the <see cref="ColorChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> ColorChangedEvent =
            RoutedEvent.Register<PreviewableColorPicker, RoutedEventArgs>(
                nameof(ColorChanged), RoutingStrategies.Bubble);
        
        /// <summary>
        /// Occurs when the color preview is changed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> PreviewColorChanged
        {
            add => AddHandler(PreviewColorChangedEvent, value);
            remove => RemoveHandler(PreviewColorChangedEvent, value);
        }

        /// <summary>
        /// Occurs when the color is changed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> ColorChanged
        {
            add => AddHandler(ColorChangedEvent, value);
            remove => RemoveHandler(ColorChangedEvent, value);
        }
        
        private ColorPreviewer? _colorPreviewer;
        private TextBlock? _colorText;
        private Flyout? _flyout;
        private ColorView? _colorView;

        /// <summary>
        /// old preview color
        /// </summary>
        [Browsable(false)]
        public Color StartPreviewColor { get; private set; }

        static PreviewableColorPicker()
        {
            ColorProperty.Changed.AddClassHandler<PreviewableColorPicker>((x, e) => x.UpdateColorDisplay());
        }
        
        /// <summary>
        /// Updates the color display in the UI.
        /// </summary>
        private void UpdateColorDisplay()
        {
            if (_colorPreviewer != null)
            {
                _colorPreviewer.HsvColor = Color.ToHsv();
            }

            if (_colorText != null)
            {
                _colorText.Text = $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}{Color.A:X2}";
            }
        }
        
        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
                
            _colorPreviewer = e.NameScope.Find<ColorPreviewer>("PART_ColorPreviewer");
            _colorText = e.NameScope.Find<TextBlock>("PART_ColorText");
            _colorView = new ColorView
            {
                Width = 400,
                Height = 400,
                Palette = new MaterialHalfColorPalette()
            };
            
            Debug.Assert(_colorPreviewer != null);
            Debug.Assert(_colorText != null);
            Debug.Assert(_colorView != null);

            UpdateColorDisplay();

            var brush = Application.Current?.FindResource("ColorControlDarkSelectorBrush") as ISolidColorBrush;
            
            // Create and configure the Flyout
            _flyout = new Flyout
            {
                Content = new Border
                {
                    Background = brush,
                    BorderThickness = new Thickness(1),
                    Child = _colorView
                }
            };

            if (_colorPreviewer != null)
            {
                _colorPreviewer.PointerPressed += OnColorPreviewPressed;
            }

            if (_flyout != null)
            {
                _flyout.Closed += OnFlyoutClosed;
            }

            if (_colorView != null)
            {
                _colorView.ColorChanged += OnColorViewChanged;
            }
            
            // Add event handler for the context menu item
            if (_colorText is { ContextMenu: { } contextMenu })
            {
                if (contextMenu.Items[0] is MenuItem copyMenuItem)
                {
                    copyMenuItem.Click += CopyColorTextToClipboard;
                }
            }
        }
        
        /// <summary>
        /// Handles the PointerPressed event on the color previewer.
        /// </summary>
        private void OnColorPreviewPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint((sender as Control)!).Properties.IsLeftButtonPressed)
            {
                return;
            }
            
            if (_flyout is { IsOpen: false })
            {
                if (_colorView != null)
                {
                    _colorView.Color = Color;
                }

                StartPreviewColor = Color;
                    
                _flyout.ShowAt(_colorPreviewer!);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the ColorChanged event from the ColorView.
        /// </summary>
        private void OnColorViewChanged(object? sender, Avalonia.Controls.ColorChangedEventArgs e)
        {
            Color = e.NewColor;
            UpdateColorDisplay();
                
            var previewArgs = new ColorChangedEventArgs
            {
                RoutedEvent = PreviewColorChangedEvent,
                OldColor = StartPreviewColor,
                NewColor = e.NewColor
            };
            RaiseEvent(previewArgs);
        }

        /// <summary>
        /// Handles the Closed event of the Flyout.
        /// </summary>
        private void OnFlyoutClosed(object? sender, EventArgs e)
        {
            var colorArgs = new ColorChangedEventArgs
            {
                RoutedEvent = ColorChangedEvent,
                OldColor = StartPreviewColor,
                NewColor = Color
            };
            RaiseEvent(colorArgs);
        }

        /// <inheritdoc/>
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            if (_colorPreviewer != null)
            {
                _colorPreviewer.PointerPressed -= OnColorPreviewPressed;
            }

            if (_flyout != null)
            {
                _flyout.Closed -= OnFlyoutClosed;
            }

            if (_colorView != null)
            {
                _colorView.ColorChanged -= OnColorViewChanged;
            }
                
            base.OnDetachedFromVisualTree(e);
        }
        
        // ReSharper disable once UnusedMember.Local
        private void CopyColorTextToClipboard(object? sender, RoutedEventArgs e)
        {
            if (_colorText != null)
            {
                var text = _colorText.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    if (TopLevel.GetTopLevel(this)?.Clipboard is { } clipboard)
                    {
                        clipboard.SetTextAsync(text);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Provides data for the ColorChanged event.
    /// </summary>
    public class ColorChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets or sets the old color.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Color OldColor { get; set; }

        /// <summary>
        /// Gets or sets the new color.
        /// </summary>
        public Color NewColor { get; set; }
    }
}