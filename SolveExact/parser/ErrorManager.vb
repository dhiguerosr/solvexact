Public Class ErrorManager
    Shared errors As New Queue(Of String)
    Public Shared Sub Add(ByVal _error As String)
        errors.Enqueue(_error)
    End Sub
    Public Shared Sub Print(ByVal text As TextBox)
        Do While errors.Count > 0
            text.Text = text.Text & "» " & errors.Dequeue & vbCrLf
        Loop
    End Sub
End Class
