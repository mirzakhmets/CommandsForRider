using System;
using System.IO;
using System.Resources;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.Components;
using JetBrains.ReSharper.Features.Internal.Resources;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.Util;

namespace ReSharperPlugin.Actions;

// Action ID = ClassName.TrimEnd("Action")

[Action(
    ResourceType: typeof(Resources),
    TextResourceName: nameof(Resources.CopyPathActionText),
    DescriptionResourceName = nameof(Resources.CopyPathActionText),
    Icon = typeof(FeaturesInternalThemedIcons.QuickStartToolWindow))]
public class CopyPathAction : IExecutableAction
{
    private readonly ResourceManager _resourceManager;

    public CopyPathAction()
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
        var file = solution.SolutionDirectory / "file.txt";

        CopyToClipboard(file, fullPath);
    }
    
    private void CopyToClipboard(VirtualFileSystemPath file, string text)
    {
        file.WriteAllText(text);
        
        try
        {
            var clipboard = Shell.Instance.GetComponent<Clipboard>();
            clipboard.SetText(text);
        }
        catch (Exception ex)
        {
            // Handle clipboard exception
            var errorMessage = string.Format(_resourceManager.GetString(nameof(Resources.CopyPathActionText)) ?? "Failed to copy to clipboard: {0}", ex.Message);
            
            using (var stream = file.OpenFileForAppend())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(errorMessage);
                }
            }
            // Console.WriteLine(errorMessage);
        }
    }
}