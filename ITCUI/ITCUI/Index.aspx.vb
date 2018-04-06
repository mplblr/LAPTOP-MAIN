'############## Jan 10,2011-Page Description Start ############################
'----- Entry point for the application to start ------------------
'----- Encrypt the URL, sending by Nomadix -----------------
'----- Use Page Header (HTTP_USER_AGENT) Info to Identify Mobile or Laptop
'----- If Mobile then get the Mobile url from Config Table and redirect with encrypted query String
'----- If Laptop then redirect to IdentifyLogin.aspx with encrypted URL -------------
'############## Jan 10,2011-Page Description End ############################
Imports ITCCORE
Imports System
Imports Microsense.Mobile

Partial Public Class Index
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim url As String
        Dim commonFun As PMSCommonFun
        Dim objSysConfig As New CSysConfig
        Dim MobileUrl, MAC, UI As String
        Dim ObjLog As LoggerService
        Dim SQL_QUERY As String

        Dim ObjDb As DbaseServiceOLEDB

        Dim ObjMacInfo As MACINFO
        Dim CheckBCMachine As BCACCESS
        Dim BCMachineURL As String

        ObjMacInfo = MACINFO.getInstance
        ObjLog = LoggerService.gtInstance
        commonFun = PMSCommonFun.getInstance

        If Not IsPostBack Then

            MAC = Request.QueryString("MA")
UI = Request.QueryString("UI")
            objSysConfig = New CSysConfig(UI)
            url = "encry=" & commonFun.EncrptQueryString(Request)



            Try
                Try
                    ObjLog.write2LogFile("port", "port=" & Request.QueryString("PORT"))
                Catch ex As Exception

                End Try




                'If Request.QueryString("PORT") = 11 Then
                '    Response.Redirect("http://192.168.13.5/event/couponlogin.aspx?" & url)
                'End If
            Catch ex As Exception

            End Try





            '--------- Synchronize ACCESSUSAGE and MacDetails based on Mac -----'
            ' This functionality will consider the Room transfer and Guest name change schenario
            Try

                If MAC <> "" Then
                    ExtendedUtil.SynchronizeByMac(MAC)
                End If

            Catch ex As Exception

            End Try
            '---------------------------------------------------------------------------'

            '################ START: CHECK FOR BC MACHINE ACCESS ###############
            ' 1. Check whether the Mac is a BC Machine
            ' 2. If Yes, check whether this BC Machine was allocated for a particular Resident / Non-Resident guest
            ' 3. If Yes, Redirect to BC Machine application else redirect to the error page.
            CheckBCMachine = New BCACCESS(MAC)
            If CheckBCMachine.isBCMachine Then
                If CheckBCMachine.isActiveBCMachine Then
                    BCMachineURL = objSysConfig.GetConfig("BCMACHINEURL")
                    Response.Redirect(BCMachineURL & "?" & url)
                Else
                    Session("Message") = Messaging.BCInternetServiceNotActivatedMessage
                    Response.Redirect("~/UserError.aspx")
                End If
            End If
            '################ END: CHECK FOR BC MACHINE ACCESS ###############


          





            Try
                Dim browserprops As HttpBrowserCapabilities = Request.Browser
                Dim browsername As String = browserprops.Browser
                Dim version As String = browserprops.Version
                Dim platform As String = browserprops.Platform
                Dim cookiesupport As Boolean = browserprops.Cookies
                Dim userAgent As String = Request.ServerVariables("HTTP_USER_AGENT")

                If String.IsNullOrEmpty(userAgent) Then
                    userAgent = ""
                End If

                '--------------------------------------------------------------------------------------------------------

                ObjLog.write2LogFile("HTTP_USER_AGENT", userAgent)

                Dim mobile As Boolean = MobileManager.IsMobileDevice(userAgent)
                mobile = False
            
                If mobile = True Then
                    Dim brand As String = MobileManager.MobileDeviceType
                    SQL_QUERY = MobileManager.GetMobileSQL(MAC, browsername, version, platform, cookiesupport, brand)
                Else
                    SQL_QUERY = MobileManager.GetLaptopSQL(MAC, browsername, version, platform, cookiesupport)
                End If

                Try
                    Try

                        If MAC.Trim() <> "" Then
                            Try
                                ExtendedUtil.RemoveLaptopMobileMacSettings(MAC.Trim())
                            Catch ex As Exception

                            End Try

                            ObjDb = DbaseServiceOLEDB.getInstance
                            ObjDb.insertUpdateDelete(SQL_QUERY)
                        End If

                    Catch ex As Exception
                        ObjLog.writeExceptionLogFile("MAC_IDENTITY_EXP", ex)
                    End Try

                    mobile = False

                    If mobile = True Then
                        '  ObjLog.write2LogFile("MOBILE_APP", SQL_QUERY)
                        MobileUrl = objSysConfig.GetConfig("MobileURL1")
                        Response.Redirect(MobileUrl & "?" & url)
                    Else
                        'ObjLog.write2LogFile("LAPTOP_APP", SQL_QUERY)
                        Response.Redirect("IdentifyLogin.aspx?" & url)
                    End If

                Catch ex As Exception

                    If Not ex.Message.ToLower().Contains("thread") Then

                        ObjLog.write2LogFile("MOBILE_DETECTION_EXP", "Mac Address: " & MAC)
                        ObjLog.writeExceptionLogFile("MOBILE_DETECTION_EXP", ex)

                        MobileUrl = objSysConfig.GetConfig("MobileURL1")
                        Response.Redirect(MobileUrl & "?" & url)

                    End If

                End Try
                '--------------------------------------------------------------------------------------------------------

            Catch ex As Exception

                If Not ex.Message.ToLower().Contains("thread") Then
                    ObjLog.writeExceptionLogFile("INDEX_PAGE_EXCEPTION", ex)
                End If

            End Try

        End If

    End Sub

End Class