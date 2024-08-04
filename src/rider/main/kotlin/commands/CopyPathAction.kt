package commands

import com.jetbrains.rd.ui.bedsl.dsl.description
import com.jetbrains.rider.actions.RiderActionsBundle
import com.jetbrains.rider.actions.base.RiderAnAction
import icons.ReSharperIcons

import com.intellij.openapi.actionSystem.*
import com.intellij.openapi.project.Project
import com.intellij.openapi.project.PossiblyDumbAware
import com.intellij.openapi.editor.Caret
import com.intellij.openapi.editor.Editor
import com.intellij.openapi.editor.Document
import com.intellij.openapi.editor.LogicalPosition
import com.intellij.openapi.fileEditor.*

import java.awt.datatransfer.StringSelection
import java.awt.Toolkit
import java.awt.datatransfer.Clipboard

import javax.swing.*;
import java.awt.*;
import java.io.*
import javax.imageio.*
import com.intellij.openapi.util.*

class CopyPathAction : AnAction(), PossiblyDumbAware
{
    companion object {
        private val PROJECT_KEY = com.intellij.openapi.util.Key.create<Project>("CopyPathActionProjectKey")
    }

    override fun actionPerformed(p0: AnActionEvent) {
        var editor = p0.getDataContext().getData(CommonDataKeys.EDITOR)
        var caret = p0.getDataContext().getData(CommonDataKeys.CARET)
        var project = p0.getDataContext().getData(CommonDataKeys.PROJECT)
        
        if (editor != null) {
            var document = editor.getDocument()
            var file = FileDocumentManager.getInstance().getFile(editor.getDocument())            
            
            if (caret != null) {
                var position = editor.visualToLogicalPosition(caret.getVisualPosition())
                
                if (position != null && file != null) {
                    var line = position.line
                    var myString = file.getPath() + "?line=" + (line + 1);
                    var stringSelection = StringSelection(myString);
                    var clipboard = Toolkit.getDefaultToolkit().getSystemClipboard();
                    clipboard.setContents(stringSelection, null);
                }
            }
        }
    }
    
    override fun getActionUpdateThread() = ActionUpdateThread.BGT
    
    override fun update(e: AnActionEvent) {
        e.presentation.setEnabled (true)
        e.project?.let { e.presentation.putClientProperty(PROJECT_KEY, it) }
    }
}
