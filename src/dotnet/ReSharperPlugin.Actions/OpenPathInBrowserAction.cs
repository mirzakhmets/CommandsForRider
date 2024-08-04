using System;
using System.Diagnostics;
using System.Resources;
using System.Text.RegularExpressions;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.ReSharper.Features.Internal.Resources;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.Util;

namespace ReSharperPlugin.Actions;

[Action(
    ResourceType: typeof(Resources),
    TextResourceName: nameof(Resources.OpenPathInBrowserActionText),
    DescriptionResourceName = nameof(Resources.OpenPathInBrowserActionText),
    Icon = typeof(FeaturesInternalThemedIcons.QuickStartToolWindow))]
public class OpenPathInBrowserAction : IExecutableAction
{
    private readonly ResourceManager _resourceManager;

    public OpenPathInBrowserAction()
    {
        _resourceManager = new ResourceManager(typeof(Resources));
    }

    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
        return true;
    }

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
        var solution = context.GetData(JetBrains.ProjectModel.DataContext.ProjectModelDataConstants.SOLUTION);
        if (solution == null)
            return;

        var textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL);
        if (textControl == null)
            return;

        var document = textControl.Document;
        var caretOffset = textControl.Caret.Offset();
        var lineNumber = document.GetCoordsByOffset(caretOffset).Line.Plus1();

        var sourceFile = document.GetPsiSourceFile(solution);
        if (sourceFile == null)
            return;

        var fullPath = $"{sourceFile.GetLocation().FullPath}?line={lineNumber}";
        var modifyUrl = ModifyUrl(fullPath);
        var file = solution.SolutionDirectory / "file1.txt";
        file.WriteAllText(modifyUrl);

        OpenUrlInBrowser(fullPath);
    }

    private void OpenUrlInBrowser(string path)
    {
        var modifiedUrl = ModifyUrl(path);

        try
        {
            Process.Start(modifiedUrl);
        }
        catch (Exception ex)
        {
            // Handle browser opening exception
            var errorMessage = string.Format(_resourceManager.GetString(nameof(Resources.OpenPathInBrowserActionText)) ?? "Failed to open URL in browser: {0}", ex.Message);
            Console.WriteLine(errorMessage);
        }
    }
    
    private string ModifyUrl(string path)
    {
        // Regular expression to match the drive letter followed by a colon and a backslash
        var regex = new Regex(@"^[A-Za-z]:\\");

        // Replace the drive letter and backslashes with the desired format
        var modifiedPath = regex.Replace(path, "www.examplesite.com/").Replace('\\', '/');
        return modifiedPath;
    }
}
