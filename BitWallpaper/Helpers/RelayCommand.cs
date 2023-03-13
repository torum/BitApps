using System.Windows.Input;

namespace BitWallpaper.Helpers;

public class GenericRelayCommand<T> : ICommand
{
    private readonly Action<T> execute;

    public GenericRelayCommand(Action<T> execute) : this(execute, p => true)
    {
    }

    public GenericRelayCommand(Action<T> execute, Predicate<T> canExecuteFunc)
    {
        this.execute = execute;
        this.CanExecuteFunc = canExecuteFunc;
    }

    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }


    public Predicate<T> CanExecuteFunc
    {
        get; private set;
    }

    public bool CanExecute(object? parameter)
    {
        if (parameter != null)
        {
            var canExecute = this.CanExecuteFunc((T)parameter);
            return canExecute;
        }
        else
        {
            return false;
        }
    }

    public void Execute(object? parameter)
    {
        if (parameter is not null)
        {
            this.execute((T)parameter);
        }
    }
}

public class RelayCommand : ICommand
{
    private readonly Action methodToExecute;
    private readonly Func<bool>? canExecuteEvaluator;

    public RelayCommand(Action methodToExecute, Func<bool>? canExecuteEvaluator)
    {
        this.methodToExecute = methodToExecute;
        this.canExecuteEvaluator = canExecuteEvaluator;
    }

    public RelayCommand(Action methodToExecute)
        : this(methodToExecute, null)
    {
    }

    /*
    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
    */
    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool CanExecute(object? parameter)
    {
        if (canExecuteEvaluator == null)
        {
            return true;
        }
        else
        {
            var result = canExecuteEvaluator.Invoke();
            return result;
        }
    }

    public void Execute(object? parameter)
    {
        methodToExecute.Invoke();
    }
}
