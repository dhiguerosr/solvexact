Imports System.IO
Imports System.Text

Public Class Graphviz
    Const dot_exe = "C:\\Program Files (x86)\\Graphviz2.36\\bin\\dot.exe"
    Const fileInputPath = "dot.txt"
    Const fileOutputPath = "dot.jpg"
    'Const DimtParam = "-Tjpg"
    'Const OParam = "-o"
    Dim Path = Directory.GetCurrentDirectory
    Dim text As String

    Public Sub New(ByVal _text As String)
        text = GetGraphText(_text)
    End Sub

    Public Sub Graph()
        ToFile()
        Dim command = String.Format("dot -Tjpg {2}{0}{2} -o {2}{1}{2}", Path & "\" & fileInputPath, Path & "\" & fileOutputPath, Chr(34))
        Console.Out.WriteLine(command)
        Dim procStartInfo = New System.Diagnostics.ProcessStartInfo("cmd", "/C " + command)
        Dim proc = New System.Diagnostics.Process()
        proc.StartInfo = procStartInfo
        proc.Start()
        proc.WaitForExit()
        System.Diagnostics.Process.Start("dot.jpg")
    End Sub
    Public Sub ToFile()
        Dim sw As New StreamWriter(fileInputPath)
        sw.WriteLine(text)
        sw.Close()
    End Sub
    Function GetGraphText(ByVal _text As String) As String
        Dim b = New StringBuilder()
        b.Append("digraph G {" + Environment.NewLine)
        b.Append(_text)
        b.Append("}")
        Return b.ToString()
    End Function
End Class


