'############## Jan 11,2011-Class Description Start ############################
'-- This class is used to encrypt and decrypt query string or any other 
'-- string values
'-- It has two methods to encrypt and decrypt query string:
'-- EncrptQueryString and DecrptQueryString
'############## Jan 11,2011-Class Description End   ############################
Imports security

Public Class PMSCommonFun
    Private Shared gtPMSCommonFunInst As PMSCommonFun

    Private Sub New()
        'nothing
    End Sub

    Public Shared Function getInstance() As PMSCommonFun
        If gtPMSCommonFunInst Is Nothing Then gtPMSCommonFunInst = New PMSCommonFun
        Return gtPMSCommonFunInst
    End Function

    Private Function CheckRequestedPage(ByVal requestedPage As String) As String
        Dim objLog As LoggerService = LoggerService.gtInstance
        requestedPage = requestedPage.Trim()

        Dim request As HttpRequest = HttpContext.Current.Request
        Dim appVirtualUrl As String = ""
        Try
            appVirtualUrl = request.Url.ToString().Trim().Replace(request.RawUrl.Trim(), "")
        Catch ex As Exception
            appVirtualUrl = ""
        End Try

        Dim defaultRequestPage As String = "http://www.google.co.in"

        Dim modifiedRequestedPage As String = defaultRequestPage

        If (requestedPage Is Nothing) Or (requestedPage = "") Then
            modifiedRequestedPage = defaultRequestPage
        ElseIf requestedPage.Contains(appVirtualUrl) Then
            modifiedRequestedPage = defaultRequestPage
        Else

            If Not requestedPage.Contains("://") Then
                modifiedRequestedPage = "http://" + requestedPage
            Else
                modifiedRequestedPage = requestedPage
            End If

        End If

        'Final check for SQL Truncation error
        If modifiedRequestedPage.Length > 200 Then
            modifiedRequestedPage = defaultRequestPage
        End If

        If modifiedRequestedPage <> requestedPage Then
            ' objLog.write2LogFile("USER_REQUESTED_PAGE", "Actual URL: " & requestedPage & ", Modified URL: " & modifiedRequestedPage)
        End If

        Return modifiedRequestedPage

    End Function

    Public Function BrowserQueryString(ByRef Req As HttpRequest, Optional ByVal Rcount As Integer = 0) As String
        Dim querykeys As String()
        Dim i As Integer
        Dim url As String = ""
        Dim EnCrpt As New Datasealing

        Try
            querykeys = Req.QueryString.AllKeys()

            Dim queryKey As String
            Dim queryKeyValue As String
            For i = 0 To querykeys.Length - (Rcount + 1)

                queryKey = querykeys(i)
                queryKeyValue = Req.QueryString(queryKey)

                If queryKey = "OS" Then
                    queryKeyValue = CheckRequestedPage(queryKeyValue)
                End If

                url = url & queryKey & "=" & queryKeyValue & "&"
                'url = url & querykeys(i) & "=" & Req.QueryString.Item(querykeys(i)) & "&"
            Next

            Return url.Substring(0, url.Length - 1)

        Catch ex As Exception
            'Throw ex
            Return ""
        End Try

    End Function

    Public Function EncrptQueryString(ByRef Req As HttpRequest) As String
        Dim querykeys As String()
        Dim i As Integer
        Dim url As String = ""
        Dim EnCrpt As New Datasealing

        Try
            querykeys = Req.QueryString.AllKeys()

            Dim queryKey As String
            Dim queryKeyValue As String
            For i = 0 To querykeys.Length - 1
                queryKey = querykeys(i)
                queryKeyValue = Req.QueryString(queryKey)

                If queryKey = "OS" Then
                    queryKeyValue = CheckRequestedPage(queryKeyValue)
                End If
                url = url & queryKey & "=" & queryKeyValue & "&"
            Next

            If Trim(url) = "" Then
                Return ""

            Else
                Return EnCrpt.GetEncryptedData(url.Substring(0, url.Length - 1))

            End If

        Catch ex As Exception
            'Throw ex
            Return ""
        End Try
    End Function

    Public Function GetPMSType(ByVal PMSName As String) As PMSNAMES
        Select Case UCase(PMSName)
            Case "CLS"
                Return PMSNAMES.CLS
            Case "IDS"
                Return PMSNAMES.IDS
            Case "OPERA"
                Return PMSNAMES.OPERA
            Case "FIDELIO"
                Return PMSNAMES.FIDELIO
            Case "BRILLIANT"
                Return PMSNAMES.BRILLIANT
            Case "WINHMS"
                Return PMSNAMES.WINHMS
            Case "AMEDEUS"
                Return PMSNAMES.AMEDEUS
            Case Else
                Return PMSNAMES.UNKNOWN
        End Select
    End Function

    Public Function DecrptQueryString(ByVal Key As String, ByVal EQrystr As String) As String
        Dim url As String
        Dim EnCrpt As New Datasealing
        Dim tmpKey As String
        Dim tmpKeyPos, tmpKeyValPos As Integer
        Try
            If Trim(EQrystr) = "" Then
                Return ""

            Else
                url = "&" & EnCrpt.GetDecryptedData(EQrystr)
                tmpKey = "&" & UCase(Key) & "="

                tmpKeyPos = InStr(UCase(url), tmpKey)

                If tmpKeyPos > 0 Then
                    tmpKeyValPos = InStr(Mid(url, tmpKeyPos + tmpKey.Length), "&")
                    If tmpKeyValPos > 0 Then
                        Return Mid(Mid(url, tmpKeyPos + tmpKey.Length), 1, tmpKeyValPos - 1)
                    Else
                        Return Mid(url, tmpKeyPos + tmpKey.Length)
                    End If

                Else
                    Return ""

                End If

            End If

        Catch ex As Exception
            'Throw ex
            Return ""

        End Try

    End Function

End Class
