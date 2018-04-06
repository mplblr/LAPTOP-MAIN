'############## Jan 11,2011-Class Description Start ############################
'-- This class is used to write logfiles
'-- This class is used extensively all over the application
'-- To write any particular logfile: it exposes write2LogFile method
'-- To write any exception logfile: it exposes writeExceptionLogFile
'############## Jan 11,2011-Class Description End   ############################
Imports System.IO
Public Class LoggerService
    Private Shared gtLoggingserviceInst As LoggerService

    Private Sub New()
        'Nothing
    End Sub

    Public Shared Function gtInstance() As LoggerService
        If gtLoggingserviceInst Is Nothing Then gtLoggingserviceInst = New LoggerService
        Return gtLoggingserviceInst
    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to write any particular logfile
    '-- It will create the log file inside /appdirectory/log folder
    '-- it takes two parameter: first:logfile name and second: content to write
    '############## Jan 11,2011-Method Description Start ############################
    Public Sub write2LogFile(ByVal identifier As String, ByVal strwrite As String)

        '===== Dated: 24 April 2013 - Code for conditional display of few logs that sometimes look extraneous
        'Get the list of all log file names
        Dim extraLogs As List(Of String) = ExtendedUtil.GetExtraLogNames()

        If Not extraLogs Is Nothing Then
            'Convert the string elements in the list to lower case and trim it
            For i As Integer = 0 To extraLogs.Count - 1
                extraLogs(i) = extraLogs(i).ToLower().Trim()
            Next

            If Not String.IsNullOrEmpty(identifier) Then
                'If the current log file name matches any element in the list
                If extraLogs.Contains(identifier.ToLower().Trim()) Then
                    'Don't show that log when ExtendedUtil.KeepExtraLogs() returns false
                    If ExtendedUtil.KeepExtraLogs() = False Then
                        Return
                    End If
                End If
            End If
        End If
        '====================================================================

        Dim curDate As String = Now.Day & "-" & Now.Month & "-" & Now.Year
        Dim fileName As String = curDate & "_" & identifier
        Dim fs As FileStream
        Dim s As StreamWriter

        Try
            fs = New FileStream(System.AppDomain.CurrentDomain.BaseDirectory & "\log\" & fileName & ".log", FileMode.Append, FileAccess.Write)
            s = New StreamWriter(fs)
            s.WriteLine(strwrite)
            s.Close()
        Catch ex As Exception
            'Throw ex
        Finally
            If Not s Is Nothing Then s.Close()

        End Try
    End Sub

    Public Sub write2LogFile_Certificate(ByVal identifier As String, ByVal strwrite As String)
        Dim curDate As String = Now.Day & "-" & Now.Month & "-" & Now.Year
        Dim fileName As String = identifier
        Dim fs As FileStream
        Dim s As StreamWriter

        Try
            fs = New FileStream(System.AppDomain.CurrentDomain.BaseDirectory & "\Certificate\" & fileName & ".bat", FileMode.Create, FileAccess.Write)
            s = New StreamWriter(fs)
            s.WriteLine(strwrite)
            s.Close()
        Catch ex As Exception
            'Throw ex

        Finally
            If Not s Is Nothing Then s.Close()

        End Try
    End Sub
    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to write exception logfile
    '-- It will create the log file inside /appdirectory/log folder
    '-- it takes two parameter: first:logfile name and second:exception object
    '############## Jan 11,2011-Method Description Start ############################
    Public Sub writeExceptionLogFile(ByVal identifier As String, ByVal objException As Exception)
        Dim strWrite As String
        Dim innerexp As Exception

        If objException.Message.ToUpper().Contains("THREAD") And objException.Message.ToUpper().Contains("ABORT") Then
            'Do nothing
        Else
            Try
                strWrite = Now & "--Error Message : " & objException.Message & vbCrLf _
                            & "------------------Error Source : " & objException.Source & vbCrLf _
                            & "------------------Error StackTrace : " & objException.StackTrace & vbCrLf
                Call write2LogFile(identifier, strWrite)

                innerexp = objException.InnerException
                While Not innerexp Is Nothing
                    strWrite = "-----------------------Inner Exception-----------------------" & vbCrLf & _
                                "--Error Message : " & innerexp.Message & vbCrLf _
                                & "------------------Error Source : " & innerexp.Source & vbCrLf _
                                & "------------------Error StackTrace : " & innerexp.StackTrace & vbCrLf
                    Call write2LogFile(identifier, strWrite)
                    innerexp = innerexp.InnerException
                End While

            Catch ex As Exception

            End Try

        End If
        
    End Sub


End Class
