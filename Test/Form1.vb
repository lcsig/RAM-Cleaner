Imports System.Runtime.InteropServices
Imports System.Security.Principal

Public Class Form1

#Region "Design And Settings ..."

    Function getTime() As Int32
        Return Val(GetSetting("RAMCleaner", "Startup", "Time", 10000))
    End Function

    Sub setTime(time As Int32)
        SaveSetting("RAMCleaner", "Startup", "Time", time.ToString)
    End Sub

    Dim first As Boolean = True
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs)
        Dim interval As Int32 = Val(TextBox1.Text)
        Label1.Text = "Time : " + Math.Round(interval / 1000, 1).ToString + "s"
        If interval < 1000 Then
            Label1.ForeColor = Color.Red
            Button1.PerformClick()
            If Timer1.Enabled = True Then
                Timer1.Stop()
                Button1.Text = "Start"
            End If
        Else
            Label1.ForeColor = Color.Lime
            Timer1.Interval = interval
            If Timer1.Enabled = False Then
                Timer1.Start()
                Button1.Text = "Stop"
            End If
            setTime(interval)
        End If
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.ShowInTaskbar = True
        Me.Visible = True
        Me.Show()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        NotifyIcon1.Dispose()
        End
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        If Button1.Text = "Start" Then
            If MessageBox.Show("It hasn't started yet ... ?", "Sure ?", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = Windows.Forms.DialogResult.Yes Then
                Me.Visible = False
                Me.ShowInTaskbar = False
                Me.Hide()
            End If
        Else
            Me.Visible = False
            Me.ShowInTaskbar = False
            Me.Hide()
        End If
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Button1.Text = "Start" And Label1.ForeColor <> Color.Red Then
            Timer1.Start()
            Button1.Text = "Stop"
        ElseIf Button1.Text = "Stop" Then
            Timer1.Stop()
            Button1.Text = "Start"
        End If
    End Sub
#End Region
   
#Region "Cleaning Code And API's"

    Const PROCESS_SET_QUOTA = &H100
    <DllImport("Kernel32.dll")> _
    Public Shared Function OpenProcess(dwDesiredAccess As Int32, bInheritHandle As Boolean, dwProcessId As Int32) As IntPtr
    End Function
    <DllImport("Kernel32.dll")> _
    Public Shared Function CloseHandle(hObject As IntPtr) As Boolean
    End Function
    <DllImport("Kernel32.dll")> _
    Public Shared Function SetProcessWorkingSetSize(hProcess As IntPtr, dwMinimumWorkingSetSize As Int32, dwMaximumWorkingSetSize As Int32) As Boolean
    End Function

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        For Each x As Process In Process.GetProcesses
            Application.DoEvents()
            Dim j As IntPtr = OpenProcess(PROCESS_SET_QUOTA, False, x.Id)
            If j <> 0 Then
                SetProcessWorkingSetSize(j, -1, -1)
                CloseHandle(j)
            End If
        Next
    End Sub

#End Region


    Public Function isInAdminMode() As Boolean
        Try
            Dim x As New WindowsPrincipal(WindowsIdentity.GetCurrent)
            Return x.IsInRole(WindowsBuiltInRole.Administrator)
        Catch
            Return False
        End Try
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Hide()
        Me.ShowInTaskbar = False
        Me.Visible = False

        If isInAdminMode() Then

            '//////////////////////////////////////
            '// 
            Me.MaximumSize = Me.Size
            Me.MinimumSize = Me.Size
            TextBox1.Text = getTime()
            AddHandler TextBox1.TextChanged, AddressOf TextBox1_TextChanged
            TextBox1_TextChanged(Nothing, Nothing)

            Dim Path As String
            Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\RAMCleaner\FirstStart.Txt"
            Dim FirstStart As Boolean
            FirstStart = Not My.Computer.FileSystem.FileExists(Path)
            If FirstStart Then
                If MessageBox.Show("Please ,,, Do you want the system to run this program after startup ?", "Startup Mode", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = Windows.Forms.DialogResult.Yes Then
                    My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", My.Application.Info.ProductName, """" + Application.ExecutablePath + """")
                End If
                My.Computer.FileSystem.CreateDirectory(Mid(Path, 1, Path.IndexOf("\FirstStart.Txt") + 1))
                My.Computer.FileSystem.WriteAllText(Path, "Nothing", False)
            End If
            '//
            '///////////////////////////////////////

            If aRoot.ImRoot = False Then
                If aRoot.MakeMeRoot() = False Then
                    MessageBox.Show("Error has occurred, Could not change sys settings to run the program with administrator permissions ...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If

        ElseIf aRoot.ImRoot Then
            aRoot.RunAsAdmin()
            ExitToolStripMenuItem_Click(Nothing, Nothing)
        Else
            If MessageBox.Show("Please, You have to run this application with Admin permissions ...", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = Windows.Forms.DialogResult.Yes Then
                Dim x As New ProcessStartInfo
                x.FileName = Application.ExecutablePath
                x.Verb = "runas"
                Process.Start(x)
            End If
            ExitToolStripMenuItem_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub StartupModeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartupModeToolStripMenuItem.Click
        Dim Exist As String
        Dim x As Microsoft.Win32.RegistryKey
        x = My.Computer.Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)

        If x.GetValue(My.Application.Info.ProductName, "NotExist") <> ("""" + Application.ExecutablePath + """") Then
            Exist = "Not in the auto-run case."
        Else
            Exist = "in the auto-run case."
        End If

        If MessageBox.Show("This Program Is " + Exist + vbCrLf + vbCrLf + "Do you want to change this case ?", "Startup", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = Windows.Forms.DialogResult.Yes Then
            If Exist = "Not in the auto-run case." Then
                My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", My.Application.Info.ProductName, """" + Application.ExecutablePath + """")
            Else
                x.DeleteValue(My.Application.Info.ProductName)
            End If
        End If
        x.Close()
    End Sub
End Class
