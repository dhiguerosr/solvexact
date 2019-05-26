Public Class Command
    Shared stack As New Stack(Of Node)
    Public Shared Sub AddCommand(ByVal _node As Node)
        stack.Push(_node)
    End Sub
    Public Shared Function Execute() As Object
        If stack.Count > 0 AndAlso stack.Peek IsNot Nothing Then
            Return stack.Pop.Execute()
        End If
        Return Nothing
    End Function
End Class