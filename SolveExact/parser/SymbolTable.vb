Public Class SymbolTable
    Shared table As New Dictionary(Of String, Symbol)
    Shared tables As Stack(Of Dictionary(Of String, Symbol))
    Public Shared Sub Init()
        tables = New Stack(Of Dictionary(Of String, Symbol))
        tables.Push(table)
    End Sub
    Public Shared Function SimbolExist(ByVal id As String) As Boolean
        Return tables.Peek().ContainsKey(id)
    End Function
    Public Shared Function AddSymbol(ByVal sym As Symbol) As Boolean
        Dim Added = Not SimbolExist(sym.ID)
        If (Added) Then
            tables.Peek().Add(sym.ID, sym)
        End If
        Return Added
    End Function
    Public Shared Sub RemoveSymbol(ByVal id As String)
        tables.Peek().Remove(id)
    End Sub
    Public Shared Function GetSymbol(ByVal id As String) As Symbol
        Dim sym As Symbol = Nothing
        tables.Peek.TryGetValue(id, sym)
        Return(sym)
    End Function
    Public Shared Sub SetFunctionTable(ByVal table As Dictionary(Of String, Symbol))
        tables.Push(table)
    End Sub
    Public Shared Sub SetGlobalTable()
        tables.Pop()
    End Sub
    
End Class
