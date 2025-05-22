namespace Avalonia.PropertyGrid.Controls;

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.PropertyGrid.Utils;
using PropertyModels.ComponentModel;

/// <summary>
/// Custom Avalonia <see cref="TextBlock"/> which supports highlighting.
/// </summary>
public partial class HighlightedTextBlock : TextBlock
{
    // -----------------------------------------
    #region Properties
    // -----------------------------------------

    /// <summary>
    /// The highlighted text property.
    /// </summary>
    public static readonly StyledProperty<string?> HighlightedTextProperty =
        AvaloniaProperty.Register<HighlightedTextBlock, string?>(nameof(HighlightedText));

    /// <summary>
    /// Gets or sets the highlighted text value.
    /// </summary>
    public string? HighlightedText
    {
        get => this.GetValue(HighlightedTextProperty);
        set => this.SetValue(HighlightedTextProperty, value);
    }

    // -----------------------------------------
    #endregion
    // -----------------------------------------

    // -----------------------------------------
    #region Constructor(s)
    // -----------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightedTextBlock"/> class.
    /// </summary>
    public HighlightedTextBlock()
    {
        this.InitializeComponent();

        // Observe the styled properties.
        this.GetObservable(HighlightedTextProperty)
            .Subscribe(new PropertyObserver<string?>(this.HighlightText));
    }

    // -----------------------------------------
    #endregion
    // -----------------------------------------

    // -----------------------------------------
    #region Methods
    // -----------------------------------------

    /// <summary>
    /// Highlights the specified text.
    /// </summary>
    /// <param name="textToHighlight">The text to highlight.</param>
    private void HighlightText(string? textToHighlight)
    {
        // Update the inline collection to highlight the matching text.
        InlineCollection inlineCollection = [];
        if (!string.IsNullOrEmpty(this.Text) && !string.IsNullOrEmpty(textToHighlight))
        {
            int startIndex = 0;
            while (startIndex < this.Text.Length)
            {
                int index = this.Text.IndexOf(textToHighlight, startIndex, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                {
                    inlineCollection.Add(new Run(this.Text[startIndex..]));
                    break;
                }

                if (index > startIndex)
                {
                    inlineCollection.Add(new Run(this.Text[startIndex..index]));
                }

                SolidColorBrush backgroundBrush = Application.Current?
                    .TryGetResource("SystemAccentColor", 
                        Application.Current.ActualThemeVariant, 
                        out object? value) == true && value is Color color
                    ? new SolidColorBrush(color, 0.7) 
                    : new SolidColorBrush(Colors.Transparent);
                inlineCollection.Add(new Run(this.Text.Substring(index, textToHighlight.Length))
                {
                    Background = backgroundBrush,
                    Foreground = new SolidColorBrush(Colors.White)
                });

                startIndex = index + textToHighlight.Length;
            }
        }
        
        // Add the text as inline if no matching text has been found.
        if (inlineCollection.Count == 0)
        {
            inlineCollection.Add(new Run { Text = this.Text });
        }

        // Add the previous unit inline back to the inline collection.
        if (this.Inlines?.FirstOrDefault(item => item.DataContext is UnitAttribute) is Run unitInlineRun)
        {
            inlineCollection.Add(unitInlineRun);
        }

        this.Inlines = inlineCollection;
    }

    // -----------------------------------------
    #endregion
    // -----------------------------------------
}
