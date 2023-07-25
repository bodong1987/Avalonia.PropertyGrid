using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public class CancelableObject : SimpleObject, INotifyCommandExecuting
    {
        CancelableCommandRecorder _Recorder = new CancelableCommandRecorder();

        [Browsable(false)]
        public bool CanUndo => _Recorder.Undoable;

        [Browsable(false)]
        public bool CanRedo => _Recorder.Redoable;

        [Browsable(false)]
        public string UndoDescription => _Recorder.UndoCommandDescription;

        [Browsable(false)]
        public string RedoDescription => _Recorder.RedoCommandDescription;

        [Browsable(false)]
        public ICommand UndoCommand { get; set; }

        [Browsable(false)]
        public ICommand RedoCommand { get; set; }        

        public CancelableObject(string description) : base(description)
        {
            CommandExecuting += OnCommandExecuting;

            UndoCommand = ReactiveCommand.Create(() =>
            {
                _Recorder.Undo();
            });

            RedoCommand = ReactiveCommand.Create(() =>
            {
                _Recorder.Redo();
            });

            _Recorder.OnNewCommandAdded += (s, e) => RefreshFlags();
            _Recorder.OnCommandCanceled += (s, e) => RefreshFlags();
            _Recorder.OnCommandCleared += (s, e) => RefreshFlags();
            _Recorder.OnCommandRedo += (s, e) => RefreshFlags();
        }

        private void RefreshFlags()
        {
            RaisePropertyChanged(nameof(CanUndo));
            RaisePropertyChanged(nameof(CanRedo));
            RaisePropertyChanged(nameof(UndoDescription));
            RaisePropertyChanged(nameof(RedoDescription));
        }

        private void OnCommandExecuting(object sender, CommandExecutingEventArgs e)
        {
            _Recorder.PushCommand(e.Command);
        }

        public event EventHandler<CommandExecutingEventArgs> CommandExecuting;

        public void RaiseCommandExecuting(CommandExecutingEventArgs eventArgs)
        {
            CommandExecuting?.Invoke(this, eventArgs);
        }
    }
}
