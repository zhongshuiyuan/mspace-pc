using System;
using System.Windows.Input;

namespace Mmc.Wpf.Commands
{
	public abstract class SimpleCommand : ICommand
	{
		public virtual event EventHandler CanExecuteChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		public virtual bool CanExecute(object parameter)
		{
			return true;
		}

		public abstract void Execute(object parameter);
	}
}
