using System.Xml;

namespace Avalonia.PropertyGrid.Controls;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Media;
using Utils;

/// <summary>
/// Custom Avalonia <see cref="TextBlock"/> which supports highlighting.
/// </summary>
public partial class HighlightedTextBlock : TextBlock
{
    #region Properties
 
    /// <summary>
    /// The highlighted text property.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly StyledProperty<string?> HighlightedTextProperty =
        AvaloniaProperty.Register<HighlightedTextBlock, string?>(nameof(HighlightedText));

    /// <summary>
    /// Gets or sets the highlighted text value.
    /// </summary>
    public string? HighlightedText
    {
        get => GetValue(HighlightedTextProperty);
        set => SetValue(HighlightedTextProperty, value);
    }
    
    /// <summary>
    /// Extra inlines
    /// </summary>
    public static readonly DirectProperty<HighlightedTextBlock, InlineCollection?> ExtraInlinesProperty =
        AvaloniaProperty.RegisterDirect<HighlightedTextBlock, InlineCollection?>(
            nameof(ExtraInlines), t => t.ExtraInlines, (t, v) => t.ExtraInlines = v);

    /// <summary>
    /// Extra Inlines
    /// </summary>
    public InlineCollection? ExtraInlines
    {
        get;
        set => SetAndRaise(ExtraInlinesProperty, ref field, value);
    }

    #endregion
    
    #region Constructor(s)
    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightedTextBlock"/> class.
    /// </summary>
    public HighlightedTextBlock()
    {
        InitializeComponent();

        // Observe the styled properties.
        this.GetObservable(HighlightedTextProperty)
            .Subscribe(new PropertyObserver<string?>(OnHighLightTextChanged));

        this.GetObservable(ExtraInlinesProperty)
            .Subscribe(new PropertyObserver<InlineCollection?>(OnExtraInlinesChanged));

        this.GetObservable(InlinesProperty)
            .Subscribe(new PropertyObserver<InlineCollection?>(OnInternalInlinesChanged));
    }
    #endregion
    
    #region Methods

    private InlineCollection BuildFullyInlines()
    {
        // Update the inline collection to highlight the matching text.
        InlineCollection inlineCollection = [];
        if (!string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(HighlightedText))
        {
            var startIndex = 0;
            while (startIndex < Text.Length)
            {
                var index = Text.IndexOf(HighlightedText, startIndex, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                {
                    inlineCollection.Add(new Run(Text[startIndex..]));
                    break;
                }

                if (index > startIndex)
                {
                    inlineCollection.Add(new Run(Text[startIndex..index]));
                }

                var backgroundBrush = Application.Current?
                    .TryGetResource("SystemAccentColor", 
                        Application.Current.ActualThemeVariant, 
                        out var value) == true && value is Color color
                    ? new SolidColorBrush(color, 0.7) 
                    : new SolidColorBrush(Colors.Transparent);
                inlineCollection.Add(new Run(Text.Substring(index, HighlightedText.Length))
                {
                    Background = backgroundBrush,
                    Foreground = new SolidColorBrush(Colors.White)
                });

                startIndex = index + HighlightedText.Length;
            }
        }
        
        // Add the text as inline if no matching text has been found.
        if (inlineCollection.Count == 0)
        {
            inlineCollection.Add(new Run { Text = Text });
        }

        return inlineCollection;
    }
    
    /// <summary>
    /// Highlights the specified text.
    /// </summary>
    /// <param name="textToHighlight">The text to highlight.</param>
    private void OnHighLightTextChanged(string? textToHighlight)
    {
        if (_internalDecorateInlines)
        {
            return;
        }
        
        try
        {
            _internalDecorateInlines = true;
            
            var newInlines = BuildFullyInlines();
            if (this.ExtraInlines != null)
            {
                newInlines.AddRange(this.ExtraInlines);    
            }
            
            Inlines = newInlines;
        }
        finally
        {
            _internalDecorateInlines = false;
        }
    }

    /// <summary>
    /// merge extra lines
    /// </summary>
    private void OnExtraInlinesChanged(InlineCollection? collection)
    {
        OnHighLightTextChanged(this.HighlightedText);
    }

    private bool _internalDecorateInlines = false;
    private void OnInternalInlinesChanged(InlineCollection? collection)
    {
        if (_internalDecorateInlines)
        {
            return;
        }

        OnHighLightTextChanged(this.HighlightedText);
    }
    #endregion
}
