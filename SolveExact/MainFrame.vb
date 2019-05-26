Imports System.IO

Public Class MainFrame
    Private Sub btnCalc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalc.Click
        doParse()
    End Sub
    Private Sub Append(ByVal _cad As String)
        txtOutput.Text = txtOutput.Text & "» " & _cad & vbCrLf
    End Sub

    Private Sub init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'txtOutput.Is
        SymbolTable.Init()
    End Sub


    Private Sub txtParse(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtInput.KeyPress
        If e.KeyChar = ChrW(Keys.Enter) Then
            doParse()
        End If
    End Sub
    Private Sub doParse()
        Dim strReader As TextReader
        strReader = New StringReader(txtInput.Text)
        Setup()
        Parse(strReader)
        Dim result = Command.Execute()
        ErrorManager.Print(txtOutput)
        If result IsNot Nothing Then
            Append(result.ToString)
        End If
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub NewProjectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewProjectToolStripMenuItem.Click
        txtInput.Text = ""
        txtOutput.Text = ""
        SymbolTable.Init()
    End Sub
End Class
