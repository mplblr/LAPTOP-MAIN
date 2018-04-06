Imports System.Data.Common
Imports ITCCORE.Microsense.CodeBase

Public Class CSysConfig

    Private SysConfig As New Hashtable

    Private Function GetNSEID() As String

        Dim request As HttpRequest = HttpContext.Current.Request
        Dim qs As String = request.QueryString("encry")
        Dim commonFun As PMSCommonFun = PMSCommonFun.getInstance
        Dim ObjLog As LoggerService = LoggerService.gtInstance

        Dim nseid As String = ""
        Try
            nseid = commonFun.DecrptQueryString("UI", qs)
        Catch ex As Exception
            nseid = ""
            ObjLog.writeExceptionLogFile("URLEXP", ex)
        End Try

        Return nseid

    End Function

    Sub New()

        Dim nseid As String = GetNSEID()
        If String.IsNullOrEmpty(nseid) Then
            nseid = ""
        End If

        LoadConfig(nseid)

    End Sub

    Sub New(ByVal nseid As String)
        If String.IsNullOrEmpty(nseid) Then
            nseid = ""
        End If
        LoadConfig(nseid)
    End Sub

    Public Sub LoadConfig(ByVal nseid As String)

        LoadGeneralConfig()
        LoadNASConfig(nseid)

    End Sub

    Public Sub LoadGeneralConfig()

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "Select Variable_Name, Variable_Value From Config Where Variable_Name Not In ('NomadixIP', 'NomadixPort', 'License_ID')"

            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then
                For count = 0 To result.Rows.Count - 1
                    SysConfig.Add(result.Rows(count).Item("Variable_Name"), result.Rows(count).Item("Variable_Value"))
                Next
            Else
            End If

        Catch ex As Exception
            objLog.writeExceptionLogFile("SYSCONFIGEXP", ex)
        End Try
    End Sub

    Private Sub LoadNASConfig(ByVal nseid As String)

        Dim encodedNSEID As String = Hashing.Hash(nseid)

        Dim objLog As LoggerService = LoggerService.gtInstance()

        If SysConfig.ContainsKey("NomadixIP") Then
            SysConfig("NomadixIP") = ""
        Else
            SysConfig.Add("NomadixIP", "")
        End If

        If SysConfig.ContainsKey("NomadixPort") Then
            SysConfig("NomadixPort") = "1111"
        Else
            SysConfig.Add("NomadixPort", "1111")
        End If

        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "Select Top 1 NomadixIP, NomadixPort, NSEHASH From NasConfig Where NSEHASH = @NSEHASH"

            DatabaseUtil.AddInputParameter(com, "@NSEHASH", DbType.String, encodedNSEID)

            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then

                Dim row As DataRow = result.Rows(0)
                Dim nomadixIP As String = row("NomadixIP").ToString().Trim()
                Dim nomadixPort As String = row("NomadixPort").ToString().Trim()
                SysConfig("NomadixIP") = nomadixIP
                SysConfig("NomadixPort") = nomadixPort
            Else
                conn = DatabaseUtil.GetConnection()
                com = DatabaseUtil.GetCommand(conn)
                com.CommandText = "Select Top 1 NomadixIP, NomadixPort, NSEHASH From NasConfig "

                DatabaseUtil.AddInputParameter(com, "@NSEHASH", DbType.String, encodedNSEID)

                result = DatabaseUtil.ExecuteSelect(com)



                Dim row As DataRow = result.Rows(0)
                Dim nomadixIP As String = row("NomadixIP").ToString().Trim()
                Dim nomadixPort As String = row("NomadixPort").ToString().Trim()
                SysConfig("NomadixIP") = nomadixIP
                SysConfig("NomadixPort") = nomadixPort
            End If
        Catch ex As Exception
            objLog.writeExceptionLogFile("SYSCONFIGEXP", ex)
        End Try
    End Sub

    Public Function GetConfig(ByVal SrvVar As String) As String
        If SysConfig.Count = 0 Then
            Dim nseid As String = GetNSEID()
            LoadConfig(nseid)
        End If
        Return SysConfig(SrvVar)
    End Function

    Public Shared Function GetMaxDeviceCountForBillSharing() As Integer
        Dim default_device_count As Integer = 3
        Dim device_count As Integer = default_device_count
        Dim device_count_str As String = ConfigurationManager.AppSettings("MaxDeviceCountForBillSharing")

        If String.IsNullOrEmpty(device_count_str) Then
            'The required configuration is missing, so return the default value
            Return device_count
        End If

        device_count_str = device_count_str.Trim()
        Integer.TryParse(device_count_str, device_count)

        If device_count <= 0 Then
            device_count = default_device_count
        End If

        Return device_count

    End Function

    Public Shared Function DeviceExceededMessage() As String
        Dim maxDeviceCountForBillSharing = CSysConfig.GetMaxDeviceCountForBillSharing()
        Dim message As String = ""
        If maxDeviceCountForBillSharing > 1 Then
            message = String.Format("You have already connected with {0} devices.", maxDeviceCountForBillSharing)
        Else
            message = String.Format("You have already connected with {0} device.", maxDeviceCountForBillSharing)
        End If
        Return message
    End Function

End Class