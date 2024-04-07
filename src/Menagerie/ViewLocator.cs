using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Menagerie.ViewModels;

namespace Menagerie;

public class ViewLocator : IDataTemplate
{
    #region Public methods

    public Control? Build(object? data)
    {
        if (data is null)  return null;

        var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type == null) return new TextBlock { Text = "Not Found: " + name };

        var control = (Control)Activator.CreateInstance(type)!;
        control.DataContext = data;
        return control;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }

    #endregion
}