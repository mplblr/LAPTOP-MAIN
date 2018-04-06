'############## Jan 11,2011-Page Description Start ##########################
'-- This page is used to collect login information(roomno,access code,last name)
'-- from the user and validate him based on the information he/she entered
'-- If has one button called submit
'-- In page load it will set the errormsg text nothing by default
'############## Jan 11,2011-Page Description End   ##########################
Imports ITCCORE
Imports System.Text
Imports System.Threading


Partial Public Class login
    Inherits System.Web.UI.Page
    Dim URL As String
    Dim RqPg As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lblErrorMsg.Text = ""
        If Not IsPostBack Then

            Button2.Visible = False
        End If

        Try
            Dim qs As String = Request.QueryString("encry")
            Dim mac As String = ""
            Dim commonFun As PMSCommonFun
            commonFun = PMSCommonFun.getInstance
            mac = commonFun.DecrptQueryString("MA", qs)


            If ExtendedUtil.iptv(mac) = 1 Then

                Button2.Visible = True

            End If


        Catch ex As Exception

        End Try





    End Sub
    '############## Jan 11,2011-Method Description Start ##########################
    '-- This method one replica of ITCWelcome.aspx page login method
    '############## Jan 11,2011-Method Description End   ##########################


    Private Sub Login(ByRef UserCred As UserContext)
        Dim AAA As AAAService
        '  Dim ObjLog As LoggerService
        Dim OutPut As String = ""
        Dim commonFun As PMSCommonFun
        ' ObjLog = LoggerService.gtInstance

        Try
            AAA = AAAService.getInstance
            commonFun = PMSCommonFun.getInstance
            URL = commonFun.BrowserQueryString(Request)
            UserCred.item("radiusloginid") = Application("radiusloginid")
            OutPut = AAA.AAA(UserCred)

            If UCase(OutPut) = "SUCCESS" Then

                If Not Session("us") Is Nothing Then
                    Session.Remove("us")
                End If
                Try
                    Dim ObjMacInfo As MACINFO
                    Dim tot As Integer = 4

                    Try
                        ObjMacInfo = MACINFO.getInstance
                        tot = ObjMacInfo.Devcount(UserCred.AccessID, UserCred.machineId, UserCred.item("accesstype"))

                    Catch ex As Exception

                    End Try

                    Try
                        Dim objmail As Mail
                        objmail = Mail.getInstance
                        If tot = 3 Then
                            objmail.SendAdminMail(UserCred.roomNo, UserCred.guestName, "", "", UserCred.machineId, objmail.ErrTypes.ndxRLBPandANS)
                        Else
                            objmail.SendAdminMail(UserCred.roomNo, UserCred.guestName, "", "", UserCred.machineId, objmail.ErrTypes.ndxRReLANS)
                        End If


                    Catch exy As Exception

                    End Try

                    lblErrorMsg.Text = "YES"

                    ' Response.Redirect("msg_info1.aspx?" & URL & "&total=" & tot)

                Catch ex As Exception

                End Try

            ElseIf UCase(OutPut) = "COUPONDEACTIVE" Then
                Session("Message") = Messaging.CouponDeactivatedMessage
                Response.Redirect("~/UserError.aspx")

            ElseIf UCase(OutPut) = "ACCEXCEED" Then
                Session("Message") = Messaging.DeviceExceededMessage
                Response.Redirect("~/UserError.aspx")
            Else
                If UCase(OutPut) = "NOPLAN" Then
                    Session.Add("us", UserCred)
                    'ObjLog.write2LogFile("Welcome", "==== Redirect to Welcome Page ==========" & vbNewLine)
                    ' Response.Redirect("ITCWelcome.aspx?" & URL)


                    DoReLogin()
                Else
                    Session("Message") = Messaging.TechnicalErrorMessage
                    Response.Redirect("~/UserError.aspx")
                End If

            End If
        Catch ex As Exception

            If Not ex.Message.ToLower().Contains("thread") Then
                ' ObjLog = LoggerService.gtInstance
                ' ObjLog.writeExceptionLogFile("AAAEXP", ex)
            End If

        End Try
    End Sub

    Private Sub DoReLogin()

        Dim ObjUserContext As UserContext
        Dim LogRoom As String
        Dim ObjLog As LoggerService = LoggerService.gtInstance

        ObjLog.write2LogFile("DoReLogin", "============== Inside Plan Selection ReLogin ===========" & vbNewLine)

        If Not Session("us") Is Nothing Then
            ObjUserContext = Session("us")
            LogRoom = ObjUserContext.roomNo
            ObjLog.write2LogFile(LogRoom, " ====== Inside DoReLogin ====" & _
                                 vbCrLf & " PlanId:" & ObjUserContext.item("planid") & _
                                 vbCrLf & " Remaining days:" & ObjUserContext.item("remainingdays") & _
                                 vbCrLf & " AccessType:" & ObjUserContext.item("accesstype") & _
                                 vbCrLf & " LoginType:" & ObjUserContext.item("logintype") & _
                                 vbCrLf & " MAC:" & ObjUserContext.machineId)

            Try
                Dim qs As String

                Dim commonFun As PMSCommonFun

                qs = Request.QueryString("encry")
                commonFun = PMSCommonFun.getInstance

                Dim sip As String
                sip = commonFun.DecrptQueryString("SIP", qs)

                ObjLog.write2LogFile("SIPb", sip)
                ObjUserContext.item("sip") = sip
            Catch ex As Exception

            End Try




            Login3(ObjUserContext)
        Else
            Dim commonFun As PMSCommonFun = PMSCommonFun.getInstance
            URL = commonFun.BrowserQueryString(Request)
            Response.Redirect("IdentifyLogin.aspx?" & URL)
        End If

        ObjLog.write2LogFile("DoReLogin", "============== Inside Plan Selection ReLogin ===========" & vbNewLine)

    End Sub

    Private Sub Login3(ByRef UserCred As UserContext)
        Dim AAA As AAAService
        Dim ObjLog As LoggerService
        Dim OutPut As String = ""
        Dim commonFun As PMSCommonFun
        Try
            AAA = AAAService.getInstance
            commonFun = PMSCommonFun.getInstance
            URL = commonFun.BrowserQueryString(Request)
            UserCred.item("radiusloginid") = Application("radiusloginid")
            OutPut = AAA.AAA(UserCred)
            If UCase(OutPut) = "SUCCESS" Then
                If Not Session("us") Is Nothing Then
                    Session.Remove("us")
                End If
                Try
                    Dim ObjMacInfo As MACINFO
                    Dim tot As Integer = 4

                    Try
                        ObjMacInfo = MACINFO.getInstance
                        tot = ObjMacInfo.Devcount(UserCred.AccessID, UserCred.machineId, UserCred.item("accesstype"))

                    Catch ex As Exception

                    End Try
                    Try
                        Dim objmail As Mail
                        objmail = Mail.getInstance
                        If tot = 3 Then
                            objmail.SendAdminMail(UserCred.roomNo, UserCred.guestName, "", "", UserCred.machineId, objmail.ErrTypes.ndxRLBPandANS)
                        Else
                            objmail.SendAdminMail(UserCred.roomNo, UserCred.guestName, "", "", UserCred.machineId, objmail.ErrTypes.ndxRReLANS)
                        End If


                    Catch exy As Exception

                    End Try



                    lblErrorMsg.Text = "YES"

                Catch ex As Exception

                End Try
            ElseIf UCase(OutPut) = "ACCEXCEED" Then
                lblErrorMsg.Text = "NO"
                Session("Message") = Messaging.DeviceExceededMessage
                ' Response.Redirect("~/UserError.aspx")
            ElseIf UCase(OutPut) = "COUPONDEACTIVE" Then
                Session("Message") = Messaging.CouponDeactivatedMessage
                ' Response.Redirect("~/UserError.aspx")
                lblErrorMsg.Text = "NO"
            Else
                Session("Message") = Messaging.TechnicalErrorMessage
                lblErrorMsg.Text = "NO"
                ' Response.Redirect("~/UserError.aspx")
            End If
        Catch ex As Exception

            If Not ex.Message.ToLower().Contains("thread") Then
                lblErrorMsg.Text = "NO"
                ObjLog = LoggerService.gtInstance
                ObjLog.writeExceptionLogFile("AAAEXP", ex)
            End If

        End Try
    End Sub

    Private Sub Login2(ByRef UserCred As UserContext)
        Dim AAA As AAAService
        '  Dim ObjLog As LoggerService
        Dim OutPut As String = ""
        Dim commonFun As PMSCommonFun
        ' ObjLog = LoggerService.gtInstance

        Try
            AAA = AAAService.getInstance
            commonFun = PMSCommonFun.getInstance
            URL = commonFun.BrowserQueryString(Request)
            UserCred.item("radiusloginid") = Application("radiusloginid")
            OutPut = AAA.AAA(UserCred)

            If UCase(OutPut) = "SUCCESS" Then

                If Not Session("us") Is Nothing Then
                    Session.Remove("us")
                End If
                Try
                    Dim ObjMacInfo As MACINFO
                    Dim tot As Integer = 4

                    Try
                        ObjMacInfo = MACINFO.getInstance
                        tot = ObjMacInfo.Devcount(UserCred.AccessID, UserCred.machineId, UserCred.item("accesstype"))

                    Catch ex As Exception

                    End Try

                    Try
                        Dim objmail As Mail
                        objmail = Mail.getInstance
                        If tot = 3 Then
                            objmail.SendAdminMail(UserCred.roomNo, UserCred.guestName, "", "", UserCred.machineId, objmail.ErrTypes.ndxRLBPandANS)
                        Else
                            objmail.SendAdminMail(UserCred.roomNo, UserCred.guestName, "", "", UserCred.machineId, objmail.ErrTypes.ndxRReLANS)
                        End If


                    Catch exy As Exception

                    End Try
                    lblErrorMsg.Text = "YES"

                    ' Response.Redirect("msg_info1.aspx?" & URL & "&total=" & tot)

                Catch ex As Exception

                End Try
            ElseIf UCase(OutPut) = "COUPONDEACTIVE" Then
                Session("Message") = Messaging.CouponDeactivatedMessage
                Response.Redirect("~/UserError.aspx")

            ElseIf UCase(OutPut) = "ACCEXCEED" Then
                Session("Message") = Messaging.DeviceExceededMessage
                Response.Redirect("~/UserError.aspx")
            Else
                If UCase(OutPut) = "NOPLAN" Then
                    Session.Add("us", UserCred)
                    'ObjLog.write2LogFile("Welcome", "==== Redirect to Welcome Page ==========" & vbNewLine)
                    Response.Redirect("ITCWelcome.aspx?" & URL)
                Else
                    Session("Message") = Messaging.TechnicalErrorMessage
                    Response.Redirect("~/UserError.aspx")
                End If

            End If
        Catch ex As Exception

            If Not ex.Message.ToLower().Contains("thread") Then
                ' ObjLog = LoggerService.gtInstance
                ' ObjLog.writeExceptionLogFile("AAAEXP", ex)
            End If

        End Try
    End Sub

    '############## Jan 11,2011-Method Description Start ##########################
    '-- This method is used to validate the user as per details entered
    '-- It will first try to collect all details based on the access code entered by the user.
    '-- If no values able to get then show error msg invalid accesscode
    '-- then it will check the last name entered by the user and name coming from the collected details are matching
    '-- if not matching then insert one entry in GenericLoginFails and show error msg incorrect last name
    '-- If the Room No entered by the user and roomno from the earlier collected details are matching
    '-- If not matching then insert one entry into GenericLoginFails and show error msg invalid roomno
    '-- If all the above validation successfull then call the login process(login method)
    '############## Jan 11,2011-Method Description Start ##########################
    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLogin.Click
        Dim ObjAccCode As AccessCode
        Dim ACC_CODE As String
        Dim ObjUCred As UserCredential
        Dim ObjUserContext As UserContext
        Dim commonFun As PMSCommonFun
        Dim qs As String = Request.QueryString("encry")
        Dim RN, MAC As String
        Dim accesstype As Integer
        '  Dim ObjLog As LoggerService
        Dim GuestName As String
        Dim ObjLoginService As LoginService
        Dim ObjStrLoginFails As New strLoginFails
        Dim ObjGuest As GuestService

        commonFun = PMSCommonFun.getInstance
        RN = commonFun.DecrptQueryString("RN", qs)



        Try

        Catch ex As Exception

        End Try





        MAC = commonFun.DecrptQueryString("MA", qs)
        lblErrorMsg.Text = ""
        If RN = "" Then
            accesstype = 0
        Else
            accesstype = 1
        End If
        '  ObjLog = LoggerService.gtInstance
        'ObjLoginService = LoginService.getInstance
        Dim ob As LoggerService
        ob = LoggerService.gtInstance()
        Try


            Dim accessCode1 As String = ""
            Dim roomNo As String = txtRoomNo.Text.Trim()
            Dim gname As String = txtAccCode.Text.Trim()


            If ExtendedUtil.iptv3(roomNo) = "0" Then


                lblErrorMsg.Text = "Room Not checked in"

            Else
                Dim error1 As Integer = 0
                Dim regcode As String = ExtendedUtil.iptv5(roomNo)

                Dim coupon As String






                coupon = ExtendedUtil.getAcode(regcode)







                ObjAccCode = New AccessCode("", coupon)
                ObjUCred = ObjAccCode.CollectAccDetails(ITCCORE.ACCESSTYPE.ONLYCODE)

                ObjUserContext = New UserContext(ObjUCred, HttpContext.Current.Request)
                ObjUserContext.item("usertype") = EUSERTYPE.ROOM
                ObjUserContext.item("accesstype") = accesstype

                ObjUserContext.item("mac") = gname

                Login(ObjUserContext)




            End If

















            ' ob.write2LogFile("Password-" & roomNo, "Rn=" & roomNo & "Name=" & gname & "Res=" & res)












            '----------------- New Changes: 17 Jan 2013 ------------------------------------'
            'Dim accessCodeEntered = txtAccCode.Text.Trim().Replace("-", "")
            'ACC_CODE = accessCodeEntered
            'If accessCodeEntered.Length = 6 Then
            '    ACC_CODE = accessCodeEntered.Substring(0, 3) & "-" & accessCodeEntered.Substring(3, 3)
            'End If
            '----------------- New Changes: 17 Jan 2013 ------------------------------------'

            'ACC_CODE = CouponUtil.CreateCoupons(1, "ITC")




            'ObjAccCode = New AccessCode("", ACC_CODE)
            'ObjUCred = ObjAccCode.CollectAccDetails(ITCCORE.ACCESSTYPE.ONLYCODE)

            'If ObjUCred.usrId <> "" Then
            '    ObjGuest = GuestService.getInstance
            '    GuestName = ObjGuest.GetGuestName(ObjUCred.GuestID)

            '    If ObjUCred.usrId <> txtRoomNo.Text.Trim() Then

            '        ObjStrLoginFails.FailAccId = ObjUCred.ACCID
            '        ObjStrLoginFails.FailAccessCode = ACC_CODE
            '        ObjStrLoginFails.FailGuestName = ""
            '        ObjStrLoginFails.FailMac = MAC
            '        ObjStrLoginFails.FailMsg = "Incorrect Suite Number."
            '        ObjStrLoginFails.FailRoomNo = txtRoomNo.Text.Trim()
            '        ObjStrLoginFails.FailAccessType = accesstype
            '        ObjStrLoginFails.Remarks = "Suite:" & ObjUCred.usrId

            '        '------------------------------------------------------------------------------
            '        '---- For Room transfer, display message to a guest differently, in case
            '        '---- he/she entered the old room no
            '        If RoomTransfer.DisplayRoomTransferMessage(ObjUCred.ACCID, txtRoomNo.Text.Trim()) Then
            '            lblErrorMsg.Text = "Dear Guest,<br />Please re-enter your new room number to confirm room transfer.<br />The Access Code remains the same."
            '        Else
            '            ObjLoginService.GenericLoginFails(ObjStrLoginFails)
            '            lblErrorMsg.Text = "Incorrect Room Number."
            '        End If
            '        lblErrorMsg.Visible = True
            '        '------------------------------------------------------------------------------

            '        Return
            '    End If

            '    ObjUserContext = New UserContext(ObjUCred, HttpContext.Current.Request)
            '    ObjUserContext.item("usertype") = EUSERTYPE.ROOM
            '    ObjUserContext.item("accesstype") = accesstype
            '    Login(ObjUserContext)
            'Else

            '    Dim couponstatusid As Integer = ObjAccCode.GetStatusID(ACC_CODE)
            '    Dim statusMessage As String

            '    Select Case couponstatusid
            '        Case 0
            '            statusMessage = Messaging.IncorrectAccessCodeMessage.Replace(Messaging.LineSeperator, "<br />")
            '        Case -1
            '            statusMessage = Messaging.CouponNotActivatedMessage.Replace(Messaging.LineSeperator, "<br />")
            '        Case -2
            '            statusMessage = Messaging.CouponDeactivatedMessage.Replace(Messaging.LineSeperator, "<br />")
            '        Case Else
            '            statusMessage = Messaging.IncorrectAccessCodeMessage.Replace(Messaging.LineSeperator, "<br />")
            '    End Select

            '    ObjStrLoginFails.FailAccId = couponstatusid
            '    ObjStrLoginFails.FailAccessCode = ACC_CODE
            '    ObjStrLoginFails.FailGuestName = ""
            '    ObjStrLoginFails.FailMac = MAC
            '    ObjStrLoginFails.FailMsg = statusMessage

            '    If RN <> "" Then
            '        ObjStrLoginFails.FailRoomNo = RN
            '    Else
            '        ObjStrLoginFails.FailRoomNo = txtRoomNo.Text
            '    End If

            '    ObjStrLoginFails.FailAccessType = accesstype
            '    ObjStrLoginFails.Remarks = "Suite:NA"
            '    ObjLoginService.GenericLoginFails(ObjStrLoginFails)

            '    Select Case couponstatusid
            '        Case 0
            '            lblErrorMsg.Text = Messaging.IncorrectAccessCodeMessage.Replace(Messaging.LineSeperator, "<br />")
            '        Case -1
            '            lblErrorMsg.Text = Messaging.CouponNotActivatedMessage.Replace(Messaging.LineSeperator, "<br />")
            '        Case -2
            '            lblErrorMsg.Text = Messaging.CouponDeactivatedMessage.Replace(Messaging.LineSeperator, "<br />")
            '        Case Else
            '            lblErrorMsg.Text = Messaging.IncorrectAccessCodeMessage.Replace(Messaging.LineSeperator, "<br />")
            '    End Select
            '    lblErrorMsg.Visible = True
            '    Return
            'End If
        Catch ex As Exception

        End Try
    End Sub


    Public Function validateGuest(ByVal room_no As String, ByVal last_name As String) As Integer

        Dim pmsrcode As String = "-1"

        Try
            Dim intValid As Integer = 0
            Dim commonFun As PMSCommonFun = PMSCommonFun.getInstance

            Dim pmsguestname As String = ""
            Dim PMSrMsg As String = ""
            Dim errMsg As String = ""
            Dim code As String = ""

            Dim objguestnew As GuestService
            objguestnew = GuestService.getInstance
            intValid = ExtendedUtil.getGuestStatus(room_no, last_name)

            If intValid = 0 Then
                pmsrcode = "15"
                PMSrMsg = "Invalid Roomno / LastName."
            Else
                pmsrcode = "0"


            End If

            If pmsrcode <> "0" Then


                Dim msg As String = ""
                msg = errMsg

                Try
                    If code = "-1" Then
                        msg = "Technical Error"
                    End If
                Catch ex As Exception

                End Try



                If Not msg.Contains("GUEST NOT FOUND") Then
                    msg = "Invalid RoomNo/LastName."
                    code = 6
                Else
                    code = 16
                    msg = "Room Vacant."
                End If




                If ExtendedUtil.getGuestStatusLog(room_no, last_name) = 1 Then

                    '   Session("Message") = "Dear Guest , you have entered incorrect Suite number / Last Name /First Name"
                    ' Response.Redirect("~/UserError.aspx")
                    lblErrorMsg.Text = "Dear Guest, please enter your valid Room Number and </br>  First/Last Name"

                    Try
                        Dim qs As String = Request.QueryString("encry")
                        Dim mac As String = ""
                        mac = commonFun.DecrptQueryString("MA", qs)
                        Dim ObjLoginService As LoginService
                        Dim ObjStrLoginFails As New strLoginFails
                        ObjStrLoginFails.FailAccId = 0
                        ObjStrLoginFails.FailAccessCode = ""
                        ObjStrLoginFails.FailGuestName = last_name
                        ObjStrLoginFails.FailMac = mac
                        ObjStrLoginFails.FailMsg = "Guest checked out"
                        Dim rn As String = ""
                        rn = commonFun.DecrptQueryString("RN", qs)
                        Dim at As Integer = 0
                        If rn = "" Then
                            at = 0
                        Else
                            at = 1
                        End If

                        ObjStrLoginFails.FailRoomNo = txtRoomNo.Text

                        ObjStrLoginFails.FailAccessType = at
                        ObjStrLoginFails.Remarks = "Room:NA"

                        ObjLoginService = LoginService.getInstance()

                        ObjLoginService.GenericLoginFails(ObjStrLoginFails)


                    Catch ex As Exception
                        Dim objlog As LoggerService
                        objlog = LoggerService.gtInstance

                        objlog.writeExceptionLogFile("GenericLoginFailsExp", ex)
                    End Try




                ElseIf ExtendedUtil.getGuestStatusLog1(room_no, last_name) = 1 Then

                    ' Session("Message") = "Dear Guest , you have entered incorrect Suite number / Last Name /First Name"
                    'Response.Redirect("~/UserError.aspx")
                    lblErrorMsg.Text = "Dear Guest, please enter your valid Room Number and </br> First/Last Name"


                    Try
                        Dim qs As String = Request.QueryString("encry")
                        Dim mac As String = ""
                        mac = commonFun.DecrptQueryString("MA", qs)
                        Dim ObjLoginService As LoginService
                        Dim ObjStrLoginFails As New strLoginFails
                        ObjStrLoginFails.FailAccId = 0
                        ObjStrLoginFails.FailAccessCode = ""
                        ObjStrLoginFails.FailGuestName = last_name
                        ObjStrLoginFails.FailMac = mac
                        ObjStrLoginFails.FailMsg = "Room Vacant"
                        Dim rn As String = ""
                        rn = commonFun.DecrptQueryString("RN", qs)
                        Dim at As Integer = 0
                        If rn = "" Then
                            at = 0
                        Else
                            at = 1
                        End If

                        ObjStrLoginFails.FailRoomNo = txtRoomNo.Text

                        ObjStrLoginFails.FailAccessType = at
                        ObjStrLoginFails.Remarks = "Room:NA"

                        ObjLoginService = LoginService.getInstance()

                        ObjLoginService.GenericLoginFails(ObjStrLoginFails)


                    Catch ex As Exception
                        Dim objlog As LoggerService
                        objlog = LoggerService.gtInstance

                        objlog.writeExceptionLogFile("GenericLoginFailsExp", ex)
                    End Try




                Else

                    ' Session("Message") = "Dear Guest , you have entered incorrect Suite number / Last Name /First Name"
                    ' Response.Redirect("~/UserError.aspx")
                    lblErrorMsg.Text = "Dear Guest, please enter your valid Room Number and </br> First/Last Name"
                    Try
                        Dim qs As String = Request.QueryString("encry")
                        Dim mac As String = ""
                        mac = commonFun.DecrptQueryString("MA", qs)
                        Dim ObjLoginService As LoginService
                        Dim ObjStrLoginFails As New strLoginFails
                        ObjStrLoginFails.FailAccId = 0
                        ObjStrLoginFails.FailAccessCode = ""
                        ObjStrLoginFails.FailGuestName = last_name
                        ObjStrLoginFails.FailMac = mac
                        ObjStrLoginFails.FailMsg = "Incorrect Room Number or Lastname/FirstName"
                        Dim rn As String = ""
                        rn = commonFun.DecrptQueryString("RN", qs)
                        Dim at As Integer = 0
                        If rn = "" Then
                            at = 0
                        Else
                            at = 1
                        End If

                        ObjStrLoginFails.FailRoomNo = txtRoomNo.Text

                        ObjStrLoginFails.FailAccessType = at
                        ObjStrLoginFails.Remarks = "Room:NA"

                        ObjLoginService = LoginService.getInstance()

                        ObjLoginService.GenericLoginFails(ObjStrLoginFails)


                    Catch ex As Exception

                        Dim objlog As LoggerService
                        objlog = LoggerService.gtInstance

                        objlog.writeExceptionLogFile("GenericLoginFailsExp", ex)
                    End Try


                End If

            End If

        Catch ex As Exception

        End Try

        If pmsrcode = "0" Then

            Return 1

        Else

            Return 0

        End If




    End Function




    Private Function ValidateInput() As Boolean

        Dim roomNo As String = txtRoomNo.Text.Trim()
        Dim accesscode As String = txtAccCode.Text.Trim()

        Dim roomRegx As New Regex("^\d+$")
        Dim accessRegex As New Regex("^[a-zA-Z0-9]{3}\-{0,1}[a-zA-Z0-9]{3}$")

        If roomNo = "" Then
            lblErrorMsg.Text = "Dear Guest, please enter your valid Room Number"
            Return False
        End If

        If Not roomRegx.IsMatch(roomNo) Then
            lblErrorMsg.Text = "Room number should be numeric."
            Return False
        End If

        If accesscode = "" Then
            lblErrorMsg.Text = "Dear Guest, please enter your valid First/Last Name "
            Return False
        End If



        If chkIAgree.Checked = False Then
            lblErrorMsg.Text = "Please accept Terms and conditions"
            Return False
        End If

        Return True

    End Function


    Protected Sub btnEventLogin_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnEventLogin.Click

        Dim ObjSysConfig As New CSysConfig
        Dim EncryptedQS As String = Request.QueryString("encry")
        Dim eventLink As String = ObjSysConfig.GetConfig("Event_URL")

        If EncryptedQS <> "" Then
            eventLink = eventLink + "?encry=" + EncryptedQS
        End If

        Response.Redirect(eventLink, False)

    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click

        Try
            Dim commonFun As PMSCommonFun
            commonFun = PMSCommonFun.getInstance
            URL = commonFun.BrowserQueryString(Request)
            Response.Redirect("logged_guest1.aspx?" & URL)

        Catch ex As Exception

        End Try


    End Sub

    Protected Sub Button2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button2.Click
        Dim device_count_str As String = ConfigurationManager.AppSettings("IPTVURL")

        Response.Redirect(device_count_str)


    End Sub
End Class