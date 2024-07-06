using System;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Interface ICommand
    /// </summary>
    public interface IBaseCommand
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Determines whether this instance can execute.
        /// </summary>
        /// <returns><c>true</c> if this instance can execute; otherwise, <c>false</c>.</returns>
        bool CanExecute();
        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        bool Execute();
    }

    /// <summary>
    /// Interface ICancelableCommand
    /// Implements the <see cref="PropertyModels.ComponentModel.IBaseCommand" />
    /// </summary>
    /// <seealso cref="PropertyModels.ComponentModel.IBaseCommand" />
    public interface ICancelableCommand : IBaseCommand
    {
        /// <summary>
        /// Determines whether this instance can cancel.
        /// </summary>
        /// <returns><c>true</c> if this instance can cancel; otherwise, <c>false</c>.</returns>
        bool CanCancel();

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        bool Cancel();
    }

    /// <summary>
    /// Class AbstractCommand.
    /// Implements the <see cref="PropertyModels.ComponentModel.IBaseCommand" />
    /// </summary>
    /// <seealso cref="PropertyModels.ComponentModel.IBaseCommand" />
    public abstract class AbstractBaseCommand : IBaseCommand
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBaseCommand"/> class.
        /// </summary>
        public AbstractBaseCommand()
        {
            Name = GetType().Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBaseCommand"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public AbstractBaseCommand(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Determines whether this instance can execute.
        /// </summary>
        /// <returns><c>true</c> if this instance can execute; otherwise, <c>false</c>.</returns>
        public virtual bool CanExecute()
        {
            return true;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public abstract bool Execute();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Class GenericCommand.
    /// Implements the <see cref="PropertyModels.ComponentModel.AbstractBaseCommand" />
    /// </summary>
    /// <seealso cref="PropertyModels.ComponentModel.AbstractBaseCommand" />
    public class GenericCommand : AbstractBaseCommand
    {
        private Func<bool> _CanExecuteFunc;
        private Func<bool> _ExecuteFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCommand" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="executeFunc">The execute function.</param>
        /// <param name="canExecuteFunc">The can execute function.</param>
        public GenericCommand(string name, Func<bool> executeFunc, Func<bool> canExecuteFunc = null) :
            base(name)
        {            
            _ExecuteFunc = executeFunc;
            _CanExecuteFunc = canExecuteFunc;
        }

        /// <summary>
        /// Determines whether this instance can execute.
        /// </summary>
        /// <returns><c>true</c> if this instance can execute; otherwise, <c>false</c>.</returns>
        public override bool CanExecute()
        {
            return _CanExecuteFunc != null ? _CanExecuteFunc() : base.CanExecute();
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public override bool Execute()
        {
            return _ExecuteFunc != null && _ExecuteFunc();
        }
    }

    /// <summary>
    /// Class AbstractCancelableCommand.
    /// Implements the <see cref="PropertyModels.ComponentModel.AbstractBaseCommand" />
    /// Implements the <see cref="PropertyModels.ComponentModel.ICancelableCommand" />
    /// </summary>
    /// <seealso cref="PropertyModels.ComponentModel.AbstractBaseCommand" />
    /// <seealso cref="PropertyModels.ComponentModel.ICancelableCommand" />
    public abstract class AbstractCancelableCommand : AbstractBaseCommand, ICancelableCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractCancelableCommand"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public AbstractCancelableCommand(string name) :
            base(name)
        {
        }

        /// <summary>
        /// Determines whether this instance can cancel.
        /// </summary>
        /// <returns><c>true</c> if this instance can cancel; otherwise, <c>false</c>.</returns>
        public virtual bool CanCancel()
        {
            return true;
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public abstract bool Cancel();
    }

    /// <summary>
    /// Class GenericCancelableCommand.
    /// Implements the <see cref="PropertyModels.ComponentModel.AbstractCancelableCommand" />
    /// </summary>
    /// <seealso cref="PropertyModels.ComponentModel.AbstractCancelableCommand" />
    public class GenericCancelableCommand : AbstractCancelableCommand
    {
        private Func<bool> _CanCancelFunc, _CanExecuteFunc, _CancelFunc, _ExecuteFunc;

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCancelableCommand"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="executeFunc">The execute function.</param>
        /// <param name="cancelFunc">The cancel function.</param>
        /// <param name="canExecuteFunc">The can execute function.</param>
        /// <param name="canCancelFunc">The can cancel function.</param>
        public GenericCancelableCommand(
            string name, 
            Func<bool> executeFunc, 
            Func<bool> cancelFunc,
            Func<bool> canExecuteFunc = null,
            Func<bool> canCancelFunc = null
            ) :
            base(name)
        {
            _ExecuteFunc = executeFunc;
            _CancelFunc = cancelFunc;
            _CanExecuteFunc = canExecuteFunc;
            _CanCancelFunc = canCancelFunc;
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public override bool Cancel()
        {
            return _CancelFunc != null && _CancelFunc();
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public override bool Execute()
        {
            return _ExecuteFunc != null && _ExecuteFunc();
        }

        /// <summary>
        /// Determines whether this instance can cancel.
        /// </summary>
        /// <returns><c>true</c> if this instance can cancel; otherwise, <c>false</c>.</returns>
        public override bool CanCancel()
        {
            return _CanCancelFunc != null ? _CanCancelFunc() : base.CanCancel();
        }

        /// <summary>
        /// Determines whether this instance can execute.
        /// </summary>
        /// <returns><c>true</c> if this instance can execute; otherwise, <c>false</c>.</returns>
        public override bool CanExecute()
        {
            return _CanExecuteFunc != null ? _CanExecuteFunc() : base.CanExecute();
        }
    }
}
