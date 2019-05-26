Imports System.IO

Public Class Symbol
    Property ID As String
    Property Value As Object
    Property IsFunction As Boolean
    Public Sub New(ByVal _id As String)
        ID = _id
        IsFunction = False
        Value = 0
    End Sub
    Public Overridable Function SetValue(ByVal _value As Double)
        Value = _value
        Return Value
    End Function
    Public Overridable Function SetValue(ByVal _Value As Node)
        Dim result = _Value.Execute
        Value = result
        Return ID & " = " & result.ToString
    End Function

    Public Overridable Function GetValue() As Object
        If TypeOf Value Is AlgValue Then
            Dim strReader As TextReader
            strReader = New StringReader(CType(Value, AlgValue).Value)
            Parse(strReader)
            Return Command.Execute
        Else
            Return Value
        End If
    End Function
End Class

Public Class Func
    Inherits Symbol
    Dim parameters As New List(Of String)
    Dim expression As Node
    Dim symbols As New Dictionary(Of String, Symbol)
    Public Sub New(ByVal _id As String)
        MyBase.New(_id)
        IsFunction = True
    End Sub
    Public Function ValidateP(ByVal args As Stack(Of String)) As String
        Do While args.Count > 0
            Dim s_id = args.Pop
            If SymbolTable.SimbolExist(s_id) Or symbols.ContainsKey(s_id) Then
                Return "El nombre de variable <" & s_id & "> ya ha sido declarado"
            Else
                parameters.Add(s_id)
                symbols.Add(s_id, New Symbol(s_id))
            End If
        Loop
        Return Nothing
    End Function
    Public Sub SetExpression(ByVal _value As Node)
        expression = _value
    End Sub
    Public Function Test() As Object
        SymbolTable.SetFunctionTable(symbols)
        Dim result = expression.Execute
        SymbolTable.SetGlobalTable()
        If TypeOf result Is AlgValue Then
            Return "La expresión < " & result.ToString & "> no está en términos de los parámetros."
        End If
        Return Nothing
    End Function
    Public Function SetArgs(ByVal args As Stack(Of Node)) As Object
        If parameters.Count = args.Count Then
            For Each p As String In parameters
                Dim sym As Symbol = Nothing
                symbols.TryGetValue(p, sym)
                If sym Is Nothing Then
                    Return "Ha ocurrido un error al asignar el valor al parámetro < " & p & ">."
                End If
                sym.SetValue(args.Pop.Execute)
            Next
        Else
            Return "Los argumentos de < " & ID & " > difieren en número."
        End If
        Return Nothing
    End Function
    Public Function GetEvaluation() As Object
        SymbolTable.SetFunctionTable(symbols)
        Dim result = expression.Execute
        SymbolTable.SetGlobalTable()
        Return result
    End Function
    Public Overrides Function ToString() As String
        'Return MyBase.ToString()
        Dim string_value As String
        string_value = Me.ID & "("
        For Each p As String In parameters
            string_value += p & ","
        Next
        string_value = string_value.Substring(0, string_value.Length - 1)
        string_value += ") = "
        string_value += CType(expression.Execute, AlgValue).ToString
        Return string_value
    End Function

End Class
