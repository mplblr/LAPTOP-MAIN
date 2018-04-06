'############## Jan 11,2011-Page Description Start ##########################
'-- This page is the entry point for laptop browser
'-- This page only contain PageLoad event
'-- It will get the mac,RN,OS from encrypted query String
'-- If mac/RN/OS does not contain value it will redirect to UserError page stating accessdenied
'-- If nseid received from the query string does not match with Config table License_ID value,
'-- it will redirect to UserError page stating accessdenied
'-- If the requested mac is bc machine then redirect to bc machine url
'-- First collectDetails based on the MAC
'-- If details not present redirect to Instruction Page asking the guest to enter login information
'-- If details present then call the Login method

'############## Jan 11,2011-Class Description End ############################
Imports ITCCORE
Imports security
Partial Public Class IdentifyLogin
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Variable Declaration Start ###################
        Dim commonFun As PMSCommonFun
        Dim objSysConfig As New CSysConfig
        Dim ObjLog As LoggerService
        Dim qs, NSEID As String

        Dim UI, UURL, MAC, SC, OS, RN As String
        Dim encrypt As New Datasealing
        Dim objLogInOut As LoginService
        Dim accesstype As Integer
        Dim url As String
        Dim roomno, altpasswd As String
        Dim GuestId As Long
        Dim ObjUserContext As UserContext
        Dim PMSName As PMSNAMES
        Dim PMSVersion As String
        Dim ObjAccCode As AccessCode
        'In Use #########
        Dim ObjUserCred As UserCredential

        'Variable Declaration Start ###################
        qs = Request.QueryString("encry")
        commonFun = PMSCommonFun.getInstance
        ObjLog = LoggerService.gtInstance

        Try

            '------ Start: Get the query string from URL -------
            UI = commonFun.DecrptQueryString("UI", qs)
            UURL = commonFun.DecrptQueryString("UURL", qs)
            MAC = commonFun.DecrptQueryString("MA", qs)
            SC = commonFun.DecrptQueryString("SC", qs)
            OS = commonFun.DecrptQueryString("OS", qs)
            RN = commonFun.DecrptQueryString("RN", qs)
            '----- End: Get the query string from URL -----

            'If RN value is present, the Laptop is connected through wired, otherwise wirlesss
            If RN = "" Then
                accesstype = 0
            Else
                accesstype = 1
            End If

        Catch ex As Exception
            ObjLog.writeExceptionLogFile("URLEXP", ex)
            Session("Message") = Messaging.AccessDenied
            Response.Redirect("~/UserError.aspx")
            'Redirect to error page ##############
        End Try

        url = commonFun.BrowserQueryString(Request)
        PMSName = commonFun.GetPMSType(Trim(objSysConfig.GetConfig("PMSName")))
        PMSVersion = objSysConfig.GetConfig("PMSVersion")

        'Check url authentication ########################
        If MAC = "" Or UI = "" Or OS = "" Then
            Session("Message") = Messaging.AccessDenied
            Response.Redirect("~/UserError.aspx")
        End If

        'Check NSID is proper or not ------------------------------------------------

        'NSEID = commonFun.DecrptQueryString("UI", qs)
        'Dim chknseid As String = objSysConfig.GetConfig("License_ID")
        'NSEID = encrypt.GetEncryptedData(NSEID)
        'If chknseid <> NSEID Then
        '    Session("Message") = Messaging.NomadixNotRegisteredMessage
        '    Response.Redirect("~/UserError.aspx")
        'End If


        Dim nomadixIP As String = objSysConfig.GetConfig("NomadixIP")
        If nomadixIP = "" Then
            Session("Message") = Messaging.NomadixNotRegisteredMessage
            Response.Redirect("~/UserError.aspx")
            Return
        End If

        '-------------------------------------------------------------------------

        '###################### Start new Process for Only AccessCode Validation ########################
        Try
            If Not IsPostBack() Then
                '$$$$$$$$$$$$$ ONLY By ACCESSCODE Starting 25 OCT, 10 $$$$$$$$$$$$$$$$$$
                objLogInOut = LoginService.getInstance
                ObjLog = LoggerService.gtInstance
                ObjAccCode = New AccessCode(MAC, "")

                ObjUserCred = ObjAccCode.CollectAccDetails(ITCCORE.ACCESSTYPE.ONLYMAC)

                If ObjUserCred.usrId <> "" Then

                    roomno = ObjUserCred.usrId
                    altpasswd = ObjUserCred.passwd
                    GuestId = ObjUserCred.GuestID

                    'Normal Guest Login Start $$$$$$$$$$$$$$$$$
                    ObjUserContext = New UserContext(ObjUserCred, HttpContext.Current.Request)
                    ObjUserContext.item("usertype") = EUSERTYPE.ROOM
                    ObjUserContext.item("accesstype") = accesstype
                    Login(ObjUserContext)
                    'Normal Guest Login End $$$$$$$$$$$$$$$$$$$
                Else

                    Response.Redirect("Instruction.aspx?" & url)

                End If

            End If
        Catch ex As Exception

        End Try
        '###################### End new Process for Only AccessCode Validation ########################
    End Sub

    '############## Jan 11,2011-Method Description Start ##########################
    '-- This method is used to get remaining time by calling AAA.GetOnlyRemainTime method
    '-- This method need to be called by passing a object of UserContext
    '-- If AAA.AAA returns APPERROR that implies the particular guestdetail is trying to use more than 
    '-- 3 devices (configured by MaxDeviceCountForBillSharing in Web.config)
    '-- put the usercontext in session variable and then
    '-- it will redirect to ITCWelcome.aspx with encrypted url
    '############## Jan 11,2011-Method Description End ############################

    Private Sub Login(ByVal userContext As UserContext)
        Dim AAA As AAAService
        Dim RemainTime As Long
        Dim commonFun As PMSCommonFun
        Dim qs As String
        Dim url As String
        Dim ObjLog As LoggerService
        ObjLog = LoggerService.gtInstance
        qs = Request.QueryString("encry")
        commonFun = PMSCommonFun.getInstance
        AAA = AAAService.getInstance
        RemainTime = AAA.GetOnlyRemainTime(userContext)

        If userContext.item("logintype") = LOGINTYPE.APPEROR Then
            'Go Error Page ########################
            Session("Message") = Messaging.DeviceExceededMessage
            Response.Redirect("~/UserError.aspx")
        ElseIf userContext.item("logintype") <> LOGINTYPE.FREEPLAN Then
            If RemainTime > 0 Then
                userContext.item("logintype") = LOGINTYPE.RELOGIN
            Else
                userContext.item("logintype") = LOGINTYPE.NEWLOGIN
            End If
        End If

        url = commonFun.BrowserQueryString(Request)

        If userContext.item("logintype") = LOGINTYPE.NEWLOGIN Then




            Try
                Session.Add("us", userContext)
                Response.Redirect("itc_alert.aspx?" & url)
            Catch ex As Exception

            End Try



            'New login scenario
            'Dim profile As GuestProfile = New GuestProfile(userContext.GuestID, userContext.AccessID)
            'Dim lastBillDiscounted As Boolean = ExtendedUtil.DidLastBillGetDiscount(userContext.GuestID, userContext.AccessID)

            'Dim redirectToLoginPage As Boolean = False

            'If lastBillDiscounted Then
            '    redirectToLoginPage = True
            'ElseIf profile.DiscountApplicable > 0 And profile.ComplimentaryPlan <= 0 Then
            '    redirectToLoginPage = True
            'End If


            'If redirectToLoginPage Then
            '    Response.Redirect("Instruction.aspx?" & url)
            'Else
            '    Session.Add("us", userContext)
            '    ObjLog.write2LogFile("Welcome", "==== Redirect to Welcome Page ==========" & vbNewLine)
            '    Response.Redirect("ITCWelcome.aspx?" & url, False)
            'End If
        Else
            'Relogin scenario
            Session.Add("us", userContext)
            ObjLog.write2LogFile("Welcome", "==== Redirect to Welcome Page ==========" & vbNewLine)
            Response.Redirect("ITCWelcome.aspx?" & url, False)
        End If

    End Sub
End Class