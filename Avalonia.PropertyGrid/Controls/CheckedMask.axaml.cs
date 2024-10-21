using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.PropertyGrid.Localization;
using Avalonia.Reactive;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Class CheckedMask.
    /// Implements the <see cref="UserControl" />
    /// </summary>
    /// <seealso cref="UserControl" />
    public partial class CheckedMask : UserControl
    {
        /// <summary>
        /// The model
        /// </summary>
        private CheckedMaskModel? _model;

        /// <summary>
        /// The model property
        /// </summary>
        public static readonly DirectProperty<CheckedMask, CheckedMaskModel> ModelProperty =
            AvaloniaProperty.RegisterDirect<CheckedMask, CheckedMaskModel>(
                nameof(Model),
                o => o._model!,
                // ReSharper disable once RedundantSuppressNullableWarningExpression
                (o, v) => o.SetAndRaise(ModelProperty!, ref o._model!, v)
                );

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public CheckedMaskModel Model
        {
            get 
            {
                Debug.Assert(_model != null); 
                return _model;
            }

            set => SetAndRaise(ModelProperty, ref _model!, value);
        }

        /// <summary>
        /// The button minimum width property
        /// </summary>
        public static readonly StyledProperty<int> ButtonMinWidthProperty =
            AvaloniaProperty.Register<CheckedMask, int>(nameof(ButtonMinWidth), 80);

        /// <summary>
        /// Gets or sets the minimum width of the button.
        /// </summary>
        /// <value>The minimum width of the button.</value>
        public int ButtonMinWidth
        {
            get => GetValue(ButtonMinWidthProperty);
            set => SetValue(ButtonMinWidthProperty, value);
        }


        /// <summary>
        /// Initializes static members of the <see cref="CheckedMask"/> class.
        /// </summary>
        static CheckedMask()
        {
            ModelProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<CheckedMaskModel>>(OnModelChanged));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckedMask"/> class.
        /// </summary>
        public CheckedMask()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when [model changed].
        /// </summary>
        /// <param name="e">The e.</param>
        private static void OnModelChanged(AvaloniaPropertyChangedEventArgs<CheckedMaskModel> e)
        {
            if (e.Sender is CheckedMask cm)
            {
                cm.OnModelChanged(e.NewValue.Value);
            }
        }

        /// <summary>
        /// Called when [model changed].
        /// </summary>
        /// <param name="value">The value.</param>
        private void OnModelChanged(CheckedMaskModel? value)
        {
            mainPanel.Children.Clear();

            if (value == null)
            {
                return;
            }

            var allButton = new ToggleButton
            {
                IsChecked = value.IsAllChecked,
                Margin = new Thickness(6),
                MinWidth = ButtonMinWidth,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            // allButton.Content = value.All;
            allButton.SetLocalizeBinding(ContentProperty, value.All);

            allButton.IsCheckedChanged += (s, e) =>
            {
                if ((bool)allButton.IsChecked)
                {
                    value.Check(value.All);

                    // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                    foreach (ToggleButton btn in mainPanel.Children)
                    {
                        if (btn != allButton)
                        {
                            btn.IsChecked = false;
                        }
                    }
                }
                else
                {
                    value.UnCheck(value.All);
                }
            };

            mainPanel.Children.Add(allButton);

            foreach (var mask in value.Masks)
            {
                var button = new ToggleButton
                {
                    IsChecked = value.IsChecked(mask) && !value.IsChecked(value.All),
                    Margin = new Thickness(6),
                    MinWidth = ButtonMinWidth,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };

                //button.Content = mask.ToString();
                button.SetLocalizeBinding(ContentProperty, mask);

                button.IsCheckedChanged += (s, e) =>
                {
                    if ((bool)button.IsChecked)
                    {
                        value.UnCheck(value.All);
                        value.Check(mask);

                        allButton.IsChecked = false;
                    }
                    else
                    {
                        value.UnCheck(mask);
                    }
                };

                mainPanel.Children.Add(button);
            }
        }
    }
}