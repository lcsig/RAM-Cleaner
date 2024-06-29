Imports System.Security.Principal
Public Class aRoot

    Private Const TASKNAME = "RAMCleaner"
    Private Const CMD = "SCHTASKS"
    Private Const CMD_RUN = "/Run /TN ""{0}"""
    Private Const CMD_CHECK = "/Query /TN ""{0}"" /v /fo LIST"
    'Private Const CMD_CREATE = "/Create /TN ""{0}"" /TR '""{1}""' /SC ONCE /SD 01/01/1950 /ST 00:00 /RL HIGHEST /f"
    Private Const CMD_CREATE = "/Create /TN ""{0}"" /TR ""'{1}'"" /SC ONCE /SD 01/01/1987 /ST 00:00 /RL HIGHEST /f"

    '// Private Const CMD_STOP = "/End /TN ""{0}"""
    '//
    '// //////////////////////////////////////////////////////////////////////////////
    '// [TN] = Task Name
    '// [TR] = Task Run { Exe Path }
    '// [ST] = Start Time
    '// [SD] = Start Date
    '// [/F] = Suppresses warnings if the specified task already exists {Over Write}
    '// [SC] = Schedule
    '// [RL] = Rights {LIMITED (default), HIGHEST}
    '// ///////////////////////////////////////////////////////////////////////////////
    '//
    '// https://msdn.microsoft.com/en-us/library/windows/desktop/bb736357%28v=vs.85%29.aspx

    Private Shared Function isInAdminMode() As Boolean
        Try
            Dim x As New WindowsPrincipal(WindowsIdentity.GetCurrent)
            Return x.IsInRole(WindowsBuiltInRole.Administrator)
        Catch
            Return False
        End Try
    End Function

    Public Shared Function ImRoot() As Boolean
        Try
            Dim proc As New Process
            proc.StartInfo.CreateNoWindow = True
            proc.StartInfo.UseShellExecute = False
            proc.StartInfo.RedirectStandardOutput = True

            proc.StartInfo.FileName = CMD
            proc.StartInfo.Arguments = String.Format(CMD_CHECK, TASKNAME)
            proc.Start()

            Dim str As String = proc.StandardOutput.ReadToEnd
            proc.Dispose()
            If str.Contains(TASKNAME) AndAlso str.ToLower.Contains(Application.ExecutablePath.ToLower) Then Return True
            Return False
        Catch
            Return False
        End Try
    End Function

    Public Shared Function RunAsAdmin() As Boolean
        Try
            If isInAdminMode() = False Then
                Dim proc As New Process
                proc.StartInfo.CreateNoWindow = True
                proc.StartInfo.UseShellExecute = False
                proc.StartInfo.RedirectStandardOutput = True

                proc.StartInfo.FileName = CMD
                proc.StartInfo.Arguments = String.Format(CMD_RUN, TASKNAME)
                proc.Start()

                Dim x As String = proc.StandardOutput.ReadToEnd()
                proc.Dispose()
                If x.Contains("SUCCESS") Then Return True
                Return False
            End If
            Return False
        Catch
            Return False
        End Try
    End Function

    Public Shared Function MakeMeRoot() As Boolean
        Try
            If isInAdminMode() Then
                Dim proc As New Process
                proc.StartInfo.CreateNoWindow = True
                proc.StartInfo.UseShellExecute = False
                proc.StartInfo.RedirectStandardOutput = True

                proc.StartInfo.FileName = CMD
                proc.StartInfo.Arguments = String.Format(CMD_CREATE, TASKNAME, Application.ExecutablePath)
                proc.Start()

                Dim x As String = proc.StandardOutput.ReadToEnd()
                proc.Dispose()
                If x.Contains("SUCCESS") Then Return True
                Return False
            End If
            Return False
        Catch
            Return False
        End Try
    End Function

End Class