using System;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.PropertyGrid.Services;
using Avalonia.Utilities;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;
using PropertyModels.Localization;

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
        if ( textBlock.Inlines == null )
            return;
        textBlock.Inlines.Clear ( );
        string displayText = LocalizationService.Default[text];
        string keyword = highLightedText?.Trim() ?? string.Empty;

        if ( !string.IsNullOrEmpty ( displayText ) && !string.IsNullOrEmpty ( keyword ) )
        {
            int start = 0;
            while ( start < displayText.Length )
            {
                int idx = displayText.IndexOf(keyword, start, StringComparison.OrdinalIgnoreCase);
                if ( idx == -1 )
                {
                    textBlock.Inlines.Add ( new Run ( displayText [ start.. ] ) );
                    break;
                }
                if ( idx > start )
                    textBlock.Inlines.Add ( new Run ( displayText [ start..idx ] ) );

                // 画笔与原生逻辑保持一致
                if ( Application.Current!.TryGetResource ( "SystemAccentColor", out var cObj ) && cObj is Color c )
                {
                    textBlock.Inlines.Add ( new Run ( displayText.Substring ( idx, keyword.Length ) )
                    {
                        Background = new SolidColorBrush ( c, 0.7 ),
                        Foreground = Brushes.White
                    } );
                }
                start = idx + keyword.Length;
            }
        }
        else
        {
            textBlock.Inlines.Add ( new Run ( displayText ) );
        }

        // 单位后缀
        if ( attribute != null && !string.IsNullOrWhiteSpace ( attribute.Unit ) )
        {
            textBlock.Inlines.Add ( new Run ( $" ({attribute.Unit})" )
            {
                Foreground = new SolidColorBrush ( Colors.Gray, 0.7 )
            } );
        }

        // 核心：不再赋值泄漏Model到DataContext
        textBlock.DataContext = null;
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

    // 保存事件句柄用于注销
    private readonly EventHandler<EventArgs> _cultureChangedHandler;

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

        // 绑定当前实例的方法到委托，满足 TSubscriber = this 约束
        _cultureChangedHandler = OnCultureChanged;

        // 弱订阅静态 LocalizationService，TTarget=LocalizationService, TEventArgs=EventArgs, TSubscriber=当前类
        WeakEventHandlerManager.Subscribe<ILocalizationService, EventArgs, TextBlockInlinesBindingDataModel> (
            target: LocalizationService.Default,
            eventName: nameof ( LocalizationService.Default.OnCultureChanged ),
            subscriber: _cultureChangedHandler
        );

        RebuildInlines ();
    }

    #region IDisposable 释放弱订阅
    protected override void Dispose( bool disposing )
    {
        if ( _disposed )
            return;

        if ( disposing )
        {
            // Unsubscribe 严格匹配你提供的2泛型签名：<TEventArgs, TSubscriber>
            WeakEventHandlerManager.Unsubscribe<EventArgs, TextBlockInlinesBindingDataModel> (
                target: LocalizationService.Default, // 传object，不用ILocalizationService泛型
                eventName: nameof ( LocalizationService.Default.OnCultureChanged ),
                subscriber: _cultureChangedHandler
            );

            Inlines?.Clear ( );
            Inlines = null;
        }

        base.Dispose ( disposing );
    }
    #endregion

    /// <summary>
    /// update highlighted text
    /// </summary>
    /// <param name="highLightedText"></param>
    public void UpdateHighlightedText(string? highLightedText)
    {
        if ( _disposed )
            return;
        _highLightedText = highLightedText;
        RebuildInlines();
    }

    private void OnCultureChanged(object? sender, EventArgs e)
    {
        if ( _disposed )
            return;
        RebuildInlines ();
    }

    private static readonly SolidColorBrush ForegroundBrush = Application.Current?
        .TryGetResource("SystemControlPageTextBaseMediumBrush",
            Application.Current.ActualThemeVariant,
            out var value) == true && value is Color color
        ? new SolidColorBrush(color, 0.7)
        : new SolidColorBrush(Colors.Gray);
    
    private void RebuildInlines()
    {
        if ( _disposed )
            return;
        var collections = TextBlockInlinesUtils.Build(LocalizationService.Default[_text??""], _highLightedText);

        if (_unit != null && _unit.Unit.IsNotNullOrEmpty())
        {
            collections.Add(new Run
            {
                Text = $" ({_unit.Unit})",
                Foreground = ForegroundBrush
            });
        }

        Inlines = collections;
        RaisePropertyChanged(nameof(Inlines));
    }

    /// <summary>
    /// Get inlines.
    /// </summary>
    public InlineCollection? Inlines { get; private set; }
}
