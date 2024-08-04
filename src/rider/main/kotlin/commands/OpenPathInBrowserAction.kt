package commands

import com.jetbrains.rider.actions.base.RiderAnAction
import icons.ReSharperIcons

import com.intellij.openapi.actionSystem.*
import com.intellij.openapi.project.Project
import com.intellij.openapi.project.PossiblyDumbAware
import com.intellij.openapi.editor.Caret
import com.intellij.openapi.editor.Editor
import com.intellij.openapi.editor.Document
import com.intellij.openapi.fileEditor.*

import java.awt.datatransfer.StringSelection
import java.awt.Toolkit
import java.awt.datatransfer.Clipboard
import java.awt.Desktop
import java.net.URI
import java.io.*

class OpenPathInBrowserAction : AnAction(), PossiblyDumbAware
{
    companion object {
        private val PROJECT_KEY = com.intellij.openapi.util.Key.create<Project>("OpenPathInBrowserActionProjectKey")
    }

    override fun actionPerformed(p0: AnActionEvent) {
        var editor = p0.getDataContext().getData(CommonDataKeys.EDITOR)
        var caret = p0.getDataContext().getData(CommonDataKeys.CARET)
        var project = p0.getDataContext().getData(CommonDataKeys.PROJECT)
        
        if (caret != null && editor != null) {
            var document = editor.getDocument()
            var file = FileDocumentManager.getInstance().getFile(editor.getDocument())            
            var position = editor.visualToLogicalPosition(caret.getVisualPosition())

            if (position != null && file != null) {
                var line = position.line
                var path = file.getPath()
                
                if (line != null && path != null) {
                    Desktop.getDesktop().browse(
                        URI("https://www.examplesite.com/" + path.replace("^[A-Za-z]:(\\\\|/)".toRegex(), "").replace('\\', '/') + "?line=" + (line + 1)))
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
