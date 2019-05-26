Public MustInherit Class Node
    Property Nodes As New List(Of Node)
    MustOverride Function Execute() As Object
    Public Function Graph(ByRef _nodes As List(Of String), ByVal _parent As String, ByVal _content As String) As String
        Dim this_node = "node" & _nodes.Count
        _nodes.Add(String.Format("node{0} [label={1}{2}{1}];" & vbNewLine, _nodes.Count, Chr(34), Me.Execute))
        _content += _parent & "->" & this_node & ";" & vbNewLine
        For Each N As Node In Nodes
            _content = N.Graph(_nodes, this_node, _content)
        Next
        Return _content
    End Function

End Class
Public Class AlgValue
    Property Value As String
    Property Precedence As Integer
    Public Sub New(ByVal _value As String)
        Value = _value
        Precedence = 6
    End Sub
    Public Sub New(ByVal _value As String, ByVal operand As String)
        Value = _value
        Precedence = AlgValue.GetPrecedente(operand)
    End Sub
    Public Shared Function GetPrecedente(ByVal operand As String) As Integer
        Select Case operand
            Case "+", "-"
                Return 2
            Case "*", "/"
                Return 3
            Case "^"
                Return 4
            Case Else
                Return 6
        End Select
    End Function
    Public Overrides Function ToString() As String
        Return Value
    End Function
End Class
Public Class AritOp
    Inherits Node
    Dim node1 As Node
    Dim node2 As Node
    Dim operand As String
    Dim unary As Boolean
    Dim post As Boolean

    Public Sub New(ByVal arg1 As Node, ByVal arg2 As Node, ByVal op As String)
        'MyBase.New(arg1, arg2)
        Nodes.Add(arg1)
        Nodes.Add(New StringValue(op))
        Nodes.Add(arg2)
        node1 = arg1
        node2 = arg2
        operand = op
    End Sub
    Public Sub New(ByVal arg As Node, ByVal op As String)
        Nodes.Add(New StringValue(op))
        Nodes.Add(arg)
        node1 = arg
        operand = op
        unary = True
        post = False
    End Sub
    Public Sub New(ByVal arg As Node, ByVal op As String, ByVal _post As Boolean)
        Nodes.Add(arg)
        Nodes.Add(New StringValue(op))
        node1 = arg
        operand = op
        unary = True
        post = _post
    End Sub

    Public Overrides Function Execute() As Object
        If Not unary Then
            Dim result1 As Object = node1.Execute
            Dim result2 As Object = node2.Execute
            If TypeOf result1 Is AlgValue OrElse TypeOf result2 Is AlgValue Then
                If operand = "^" AndAlso TypeOf result2 Is Double AndAlso result2 = 0.5 Then
                    Return New AlgValue("sqrt(" & result1.ToString & ")", "^")
                Else
                    If TypeOf result1 Is AlgValue AndAlso CType(result1, AlgValue).Precedence < AlgValue.GetPrecedente(operand) Then
                        Return New AlgValue("(" & result1.ToString & ")" & operand & result2.ToString, operand)
                    End If
                    Return New AlgValue(result1.ToString & operand & result2.ToString, operand)
                End If
            End If
            Select Case operand
                Case "+"
                    Return result1 + result2
                Case "-"
                    Return result1 - result2
                Case "*"
                    Return result1 * result2
                Case "/"
                    Return result1 / result2
                Case "^"
                    Return Math.Pow(result1, result2)
            End Select
        Else
            Dim result1 As Object = node1.Execute
            If TypeOf result1 Is AlgValue Then
                If post Then
                    Return New AlgValue(result1.ToString & operand)
                Else
                    Return New AlgValue(operand & result1.ToString)
                End If
            End If
            Select Case operand
                Case "-"
                    Return result1 * -1
                Case "++"
                    Dim new_value = result1 + 1
                    SymbolTable.GetSymbol(CType(node1, GetValue).ID).SetValue(new_value)
                    If post Then
                        Return result1
                    End If
                    Return new_value
                Case "--"
                    Dim new_value = result1 - 1
                    SymbolTable.GetSymbol(CType(node1, GetValue).ID).SetValue(new_value)
                    If post Then
                        Return result1
                    End If
                    Return new_value
                    Return Nothing
            End Select
        End If
        Return Nothing
    End Function
End Class
Public Class Num
    Inherits Node
    Dim node As Double
    Public Sub New(ByVal arg1 As Object)
        node = CDbl(arg1)
    End Sub
    Public Overrides Function Execute() As Object
        Return node
    End Function
End Class
Public Class StringValue
    Inherits Node
    Dim node As String
    Public Sub New(ByVal arg1 As String)
        node = arg1
    End Sub
    Public Overrides Function Execute() As Object
        Return node
    End Function
End Class
Public Class Log
    Inherits Node
    Dim node1 As Node
    Dim node2 As Node
    Public Sub New(ByVal arg1 As Node, ByVal arg2 As Node)
        'MyBase.New(arg1, arg2)
        node1 = arg1
        node2 = arg2
        Dim result2 As Object = node2.Execute
        Select Case result2
            Case Math.E
                Nodes.Add(New StringValue("ln"))
            Case 10
                Nodes.Add(New StringValue("log"))
            Case 2
                Nodes.Add(New StringValue("log2"))
            Case Else
                Nodes.Add(New StringValue("log" & result2))
        End Select
        Nodes.Add(arg1)

    End Sub
    Public Overrides Function Execute() As Object
        Dim result1 As Object = node1.Execute
        Dim result2 As Object = node2.Execute
        If TypeOf result1 Is AlgValue Then
            Select Case result2
                Case Math.E
                    Return New AlgValue("ln(" & result1.ToString & ")")
                Case 10
                    Return New AlgValue("log(" & result1.ToString & ")")
                Case 2
                    Return New AlgValue("log2(" & result1.ToString & ")")
                Case Else
                    Return New AlgValue("log " & result2 & "(" & result1.ToString & ")")
            End Select
        End If
        Return Math.Log(result1, result2)
    End Function
End Class
Public Class TrigFunc
    Inherits Node
    Dim node As Node
    Dim command As String
    Public Sub New(ByVal arg As Node, ByVal op As String)
        Nodes.Add(New StringValue(op))
        Nodes.Add(arg)
        node = arg
        command = op
    End Sub
    Public Overrides Function Execute() As Object
        Dim result As Object = node.Execute
        If TypeOf result Is AlgValue Then
            Return New AlgValue(command & "(" & result.ToString & ")")
        End If
        Select Case command
            Case "arctan"
                Return Math.Tanh(result)
            Case "arccos"
                Return Math.Cosh(result)
            Case "arcsin"
                Return Math.Sinh(result)
            Case "cot"
                Return 1 / Math.Tan(result)
            Case "sec"
                Return 1 / Math.Cos(result)
            Case "csc"
                Return 1 / Math.Sin(result)
            Case "tan"
                Return Math.Tan(result)
            Case "cos"
                Return Math.Cos(result)
            Case "sin"
                Return Math.Sin(result)
        End Select
        Return Nothing
    End Function
End Class
Public Class AssignVar
    Inherits Node
    Dim value As Node
    Property ID As String
    
    Public Sub New(ByVal _id As String, ByVal _value As Node)
        MyBase.New()
        ID = _id
        value = _value
    End Sub
    Public Overrides Function Execute() As Object
        Dim sym As Symbol = SymbolTable.GetSymbol(id)
        If sym IsNot Nothing AndAlso Not sym.IsFunction Then
            Return sym.SetValue(value)
        Else
            'El simbolo no ha sido declarado, no se puede asignar
            Return "< " & ID & " > no ha sido declarado. No se puede asignar."
        End If
    End Function
End Class
Public Class DecVar
    Inherits Node
    Property ID As String
    Dim Assign As AssignVar
    Public Sub New(ByVal _id As String)
        'MyBase.New()
        ID = _id
        Assign = Nothing
    End Sub
    Public Sub New(ByVal _assign As AssignVar)
        MyBase.New()
        Assign = _assign
        ID = Assign.ID
    End Sub
    Public Overrides Function Execute() As Object
        If SymbolTable.AddSymbol(New Symbol(ID)) Then
            If Assign IsNot Nothing Then
                Return Assign.Execute()
            Else
                Return ID & " = 0"
            End If
        Else
            'El simbolo ya ha sido declarado, no se puede asignar
            Return "< " & ID & " > ya ha sido declarado. No se puede asignar."
        End If
    End Function
End Class

Public Class DecFunc
    Inherits Node
    Property ID As String
    Property Parameters As Stack(Of String)
    Property Expression As Node
    Public Sub New(ByVal _id As String, ByVal _args As Stack(Of String))
        ID = _id
        Parameters = _args
    End Sub
    Public Sub New(ByVal _id As String, ByVal _args As Stack(Of String), ByVal _value As Node)
        ID = _id
        Parameters = _args
        Expression = _value
    End Sub
    Overrides Function Execute() As Object
        'If SymbolTable.SimbolExist(ID) Then
        'El simbolo ya ha sido declarado, no se puede asignar
        'Return "< " & ID & " > ya ha sido declarado. No se puede asignar."
        'Else
        Dim f = New Func(ID)
        Dim validate = f.ValidateP(Parameters)

        If validate Is Nothing Then
            If Expression IsNot Nothing Then
                f.SetExpression(Expression)
                Dim test = f.Test
                If test IsNot Nothing Then
                    Return test
                End If
            End If
            If SymbolTable.SimbolExist(f.ID) Then
                SymbolTable.RemoveSymbol(f.ID)
            End If
            If SymbolTable.AddSymbol(f) Then
                Return f.ToString
            Else
                Return "No se ha podido definir la función < " & ID & " >."
            End If
        Else
            Return validate
        End If
        'End If
    End Function
End Class
Public Class GetValue
    Inherits Node
    Property ID As String
    Public Sub New(ByVal _id As String)
        Nodes.Add(New StringValue(_id))
        ID = _id
    End Sub

    Public Overrides Function Execute() As Object
        If SymbolTable.SimbolExist(ID) Then
            Dim sym As Symbol = SymbolTable.GetSymbol(ID)
            Return sym.GetValue()
        Else
            Return New AlgValue(ID)
        End If
    End Function

End Class
Public Class EvaluateFunc
    Inherits Node
    Dim ID As String
    Dim expressions As Stack(Of Node)
    Public Sub New(ByVal _id As String, ByVal _nodes As Stack(Of Node))
        ID = _id
        expressions = _nodes
        Nodes.Add(New StringValue(_id & "("))
        For Each N As Node In _nodes
            Nodes.Add(N)
            Nodes.Add(New StringValue(","))
        Next
        Nodes.RemoveAt(Nodes.Count - 1)
        Nodes.Add(New StringValue(")"))

    End Sub
    Public Overrides Function Execute() As Object
        Dim f = SymbolTable.GetSymbol(ID)
        If f IsNot Nothing AndAlso f.IsFunction Then
            Dim set_p = CType(f, Func).SetArgs(expressions)
            If set_p Is Nothing Then
                Return CType(f, Func).GetEvaluation()
            Else
                ErrorManager.Add(set_p)
                Return New AlgValue(ID)
            End If
        Else
            ErrorManager.Add("La función < " & ID & "> no ha sido definida.")
            Return New AlgValue(ID)
        End If
    End Function
End Class
Public Class GraphExp
    : Inherits Node
    Dim exp As Node
    Public Sub New(ByVal _exp As Node)
        MyBase.New()
        exp = _exp
    End Sub
    Public Overrides Function Execute() As Object
        Dim nodes = New List(Of String)
        Dim this_node = "node0"
        nodes.Add(String.Format("node{0} [label={1}{2}{1}];" & vbNewLine, nodes.Count, Chr(34), exp.Execute))
        Dim text = exp.Graph(nodes, this_node, "")
        Dim nodes_s = ""
        For Each N As String In nodes
            nodes_s += N
        Next
        Dim graph = New Graphviz(nodes_s & text & "")
        graph.Graph()
        Return Nothing
    End Function
End Class