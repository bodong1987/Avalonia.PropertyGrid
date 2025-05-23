using System.ComponentModel;
using System.Windows.Input;
using Avalonia.PropertyGrid.Controls;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public class CancelableObject : SimpleObject
    {
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

        public CancelableObject(string description) : base(description)
        {
            UndoCommand = ReactiveCommand.Create(() => _recorder.Undo());
            RedoCommand = ReactiveCommand.Create(() => _recorder.Redo());
            ClearCommand = ReactiveCommand.Create(_recorder.Clear);

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
    }
}
