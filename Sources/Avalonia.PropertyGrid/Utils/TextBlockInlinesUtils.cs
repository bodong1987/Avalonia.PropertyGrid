using System;
using System.Net.Mime;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.PropertyGrid.Services;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Utils;

/// <summary>
/// generate text block inlines
/// </summary>
public static class TextBlockInlinesUtils
{
    /// <summary>
    /// generate highlighted text block inlines
    /// </summary>
    /// <param name="text"></param>
    /// <param name="highLightedText"></param>
    /// <returns></returns>
    public static InlineCollection Build(string? text, string? highLightedText)
    {
        // Update the inline collection to highlight the matching text.
        InlineCollection inlineCollection = [];
        if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(highLightedText))
        {
            var startIndex = 0;
            while (startIndex < text.Length)
            {
                var index = text.IndexOf(highLightedText, startIndex, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                {
                    inlineCollection.Add(new Run(text[startIndex..]));
                    break;
                }

                if (index > startIndex)
                {
                    inlineCollection.Add(new Run(text[startIndex..index]));
                }

                var backgroundBrush = Application.Current?
                    .TryGetResource("SystemAccentColor", 
                        Application.Current.ActualThemeVariant, 
                        out var value) == true && value is Color color
                    ? new SolidColorBrush(color, 0.7) 
                    : new SolidColorBrush(Colors.Transparent);
                inlineCollection.Add(new Run(text.Substring(index, highLightedText.Length))
                {
                    Background = backgroundBrush,
                    Foreground = new SolidColorBrush(Colors.White)
                });

                startIndex = index + highLightedText.Length;
            }
        }
        
        // Add the text as inline if no matching text has been found.
        if (inlineCollection.Count == 0)
        {
            inlineCollection.Add(new Run { Text = text });
        }

        return inlineCollection;
    }

    /// <summary>
    /// help function used to set inlines for textblock
    /// </summary>
    /// <param name="textBlock"></param>
    /// <param name="text"></param>
    /// <param name="highLightedText"></param>
    /// <param name="attribute"></param>
    public static void SetInlinesBinding(this TextBlock textBlock, string text, string? highLightedText = null,
        UnitAttribute? attribute = null)
    {
        var source = new TextBlockInlinesBindingDataModel(text, highLightedText, attribute);
        var binding = new Binding
        {
            Source = source,
            Path = nameof(source.Inlines),
            Mode = BindingMode.Default
        };

        textBlock.Bind(TextBlock.InlinesProperty, binding);
        textBlock.DataContext = source;
    }

    /// <summary>
    /// extension method. force update highlighted text for common text block
    /// </summary>
    /// <param name="textBlock"></param>
    /// <param name="highLightedText"></param>
    public static void UpdateHighlightedText(this TextBlock textBlock, string? highLightedText)
    {
        if (textBlock.DataContext is TextBlockInlinesBindingDataModel dm)
        {
            dm.UpdateHighlightedText(highLightedText);
        }
    }
}

/// <summary>
/// bind for text block
/// </summary>
public class TextBlockInlinesBindingDataModel : ReactiveObject
{
    private readonly string? _text;
    private string? _highLightedText;
    private readonly UnitAttribute? _unit; 
        
    /// <summary>
    /// construct this data model
    /// </summary>
    /// <param name="text"></param>
    /// <param name="highLightedText"></param>
    /// <param name="attribute"></param>
    public TextBlockInlinesBindingDataModel(string? text,  string? highLightedText = null, UnitAttribute? attribute = null)
    {
        _text = text;
        _unit = attribute;
        _highLightedText = highLightedText;
        
        RebuildInlines();
        
        LocalizationService.Default.OnCultureChanged += OnCultureChanged;
    }

    /// <summary>
    /// update highlighted text
    /// </summary>
    /// <param name="highLightedText"></param>
    public void UpdateHighlightedText(string? highLightedText)
    {
        this._highLightedText = highLightedText;
        RebuildInlines();;
    }

    private void OnCultureChanged(object? sender, EventArgs e)
    {
        RebuildInlines();
    }

    private static readonly SolidColorBrush ForegroundBrush = Application.Current?
        .TryGetResource("SystemControlPageTextBaseMediumBrush",
            Application.Current.ActualThemeVariant,
            out var value) == true && value is Color color
        ? new SolidColorBrush(color, 0.7)
        : new SolidColorBrush(Colors.Gray);
    
    private void RebuildInlines()
    {
        var collections = TextBlockInlinesUtils.Build(LocalizationService.Default[_text??""], _highLightedText);

        if (_unit != null && _unit.Unit.IsNotNullOrEmpty())
        {
            collections.Add(new Run
            {
                Text = $" ({_unit.Unit})",
                Foreground = ForegroundBrush
            });
        }

        this.Inlines = collections;
        RaisePropertyChanged(nameof(Inlines));
    }

    /// <summary>
    /// Get inlines.
    /// </summary>
    public InlineCollection? Inlines { get; private set; }
}
