﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Samples.PainterDemo.Models;
using PropertyModels.ComponentModel;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.ViewModel;

public enum ToolMode
{
    Select,
    Brush,
    CreateRectangle,
    CreateEllipse,
    CreateLine,
    CreateStar,
    CreateArrow
}

public class PainterViewModel : ReactiveObject
{
    #region Shape Properties
    public ObservableCollection<ShapeBase> Shapes { get; } = [];
    
    private ShapeBase? _selectedShape;
    public ShapeBase? SelectedShape
    {
        get => _selectedShape;
        set => SetProperty(ref _selectedShape, value);
    }
    
    private ToolMode _currentToolMode = ToolMode.Select;
    public ToolMode CurrentToolMode
    {
        get => _currentToolMode;
        set => SetProperty(ref _currentToolMode, value);
    }
    #endregion

    #region Default Configs

    public Color DefaultFillColor { get; set; } = Colors.Gray;
    public Color DefaultBorderColor { get; set; } = Colors.DarkGray;
    private double _defaultStrokeThickness = 2.0;

    public double DefaultStrokeThickness
    {
        get => _defaultStrokeThickness;
        set => SetProperty(ref _defaultStrokeThickness, value);
    }

    #endregion

    #region Commands Support

    private readonly CancelableCommandRecorder _recorder = new();
    
    [Browsable(false)]
    public bool CanUndo => _recorder.CanUndo;

    [Browsable(false)]
    public bool CanRedo => _recorder.CanRedo;

    [Browsable(false)]
    public string UndoDescription => _recorder.UndoCommandDescription;

    [Browsable(false)]
    public string RedoDescription => _recorder.RedoCommandDescription;

    [Browsable(false)]
    public ICommand UndoCommand { get; set; }

    [Browsable(false)]
    public ICommand RedoCommand { get; set; }

    [Browsable(false)]
    public ICommand ClearCommand { get; set; }

    #endregion

    public PainterViewModel()
    {
        UndoCommand = ReactiveCommand.Create(() => _recorder.Undo());
        RedoCommand = ReactiveCommand.Create(() => _recorder.Redo());
        
        ClearCommand = ReactiveCommand.Create(() =>
        {
            _recorder.Clear();
            Shapes.Clear();
        });
        
        _recorder.OnNewCommandAdded += (s, e) => RefreshFlags();
        _recorder.OnCommandCanceled += (s, e) => RefreshFlags();
        _recorder.OnCommandCleared += (s, e) => RefreshFlags();
        _recorder.OnCommandRedo += (s, e) => RefreshFlags();
    }
    
    private void RefreshFlags()
    {
        RaisePropertyChanged(nameof(CanUndo));
        RaisePropertyChanged(nameof(CanRedo));
        RaisePropertyChanged(nameof(UndoDescription));
        RaisePropertyChanged(nameof(RedoDescription));
    }

    public void OnCommandExecuted(object? sender, RoutedCommandExecutedEventArgs e) => _recorder.PushCommand(e.Command);
    public void OnCommandExecuted(ICancelableCommand command) => _recorder.PushCommand(command);
    public void ExecuteCommand(ICancelableCommand command) => _recorder.ExecuteCommand(command);
}