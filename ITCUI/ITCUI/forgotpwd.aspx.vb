Imports ITCCORE
Imports security
Imports System.Data.SqlClient
Imports ITCCORE.Microsense.CodeBase
Imports System.Data.Common

Partial Public Class forgotpwd
    Inherits System.Web.UI.Page
    Public url As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ObjLog As LoggerService
        Dim qns As String = ""
        Dim qs, NSEID As String
        Dim commonFun As PMSCommonFun
        Dim UI, UURL, MAC, SC, OS, RN As String
        Dim objSysConfig As New CSysConfig
        Dim encrypt As New Datasealing

        qs = Request.QueryString("encry")
        commonFun = PMSCommonFun.getInstance
        ObjLog = LoggerService.gtInstance
        Try
            'Start Get the query string from URL
            UI = commonFun.DecrptQueryString("UI", qs)
            UURL = commonFun.DecrptQueryString("UURL", qs)
            MAC = commonFun.DecrptQueryString("MA", qs)
            SC = commonFun.DecrptQueryString("SC", qs)
            OS = commonFun.DecrptQueryString("OS", qs)
            RN = commonFun.DecrptQueryString("RN", qs)

            'End Get the query string from URL
        Catch ex As Exception
            ObjLog.writeExceptionLogFile("URLEXP", ex)
            Session("Message") = Messaging.AccessDenied
            Response.Redirect("~/UserError.aspx")
            'Redirect to error page ##############
        End Try
        url = commonFun.BrowserQueryString(Request)


        'Check url authentication ########################
        If MAC = "" Or UI = "" Or OS = "" Then
            Session("Message") = Messaging.AccessDenied
            Response.Redirect("~/UserError.aspx")
        End If

        'Check NSID is proper or not ------------------------------------------------
        Dim nomadixIP As String = objSysConfig.GetConfig("NomadixIP")
        If nomadixIP = "" Then
            Session("Message") = Messaging.NomadixNotRegisteredMessage
            Response.Redirect("~/UserError.aspx")
            Return
        End If

        'Check NSID is proper or not ------------------------------------------------
     

        If Not IsPostBack() Then
            Try
                kk1.Visible = False
                kk.Visible = False
                kk2.Visible = False
                kk3.Visible = False
                a1.Visible = False
                a2.Visible = False
                P1.Visible = False
                P3.Visible = False

                Button1.Visible = False
                Button2.Visible = False
                Button3.Visible = False
                P4.Visible = False

                Dim r1 As String = ""
                Dim r2 As String = ""

                Try
                    r1 = Request.QueryString("rm")
                    r2 = Request.QueryString("ln")
                    Try
                        Dim str1() = r1.Split(",")
                        Dim ind As Integer = 0
                        ind = str1.Length - 1

                        r1 = str1(ind)
                        ' objlog.write2LogFile("Mac_" & "Login1 roomno r1" & r1, r1)

                    Catch ex As Exception

                    End Try
                Catch ex As Exception

                End Try

                Try
                    Dim str2() = r2.Split(",")
                    Dim ind As Integer = 0
                    ind = str2.Length - 1

                    r2 = str2(ind)
                    ' ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI after split r2" & r2)

                Catch ex As Exception

                End Try



                Try


                    Try

                    Catch ex As Exception

                    End Try




                    Try
                        qns = GetQuestAnwerFromMac(MAC)
                        ' ObjLog.write2LogFile("Reset", "Getting question from mac" & qns)
                    Catch ex As Exception

                    End Try

                    If qns = "-1" Or qns = "" Then
                        qns = ""
                        If r1 <> "" And r2 <> "" Then
                            Try
                                Try
                                    qns = ExtendedUtil.getGuestStatus1(r1, r2)
                                Catch ex As Exception

                                End Try
                                ' ObjLog.write2LogFile("Reset", "Getting question from roomno name" & "room=" & r1 & "Name=" & r2 & "question" & qns)
                            Catch ex As Exception

                            End Try
                        End If
                    End If
                Catch ex As Exception

                End Try



                ' btnLogin.Visible = False
            Catch ex As Exception

            End Try

            If qns = "-1" Or qns = "" Then
                a1.Visible = True
                a2.Visible = True
                Button1.Visible = True


            Else
                P1.Visible = True
                p2.Text = qns
                Button2.Visible = True
                kk.Visible = True
                kk2.Visible = True

            End If




        End If


    End Sub

    Public Function GetQuestAnwerFromMac(ByVal mac As String) As String

        Dim dt As DataTable = New DataTable()


        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim query As SqlCommand = New SqlCommand("select qns from guest where GuestStatus='A' and guestid =(select guestid from macdetails where mac='" & mac & "')", conn)
            'dt = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(query)
            dt = DatabaseUtil.ExecuteSelect(query)
            If dt.Rows.Count > 0 Then

                Return dt.Rows(0)(0).ToString()
                ' ddlQuestion.Text = dt.Rows(0)("qns").ToString()

            Else
                Return "-1"
            End If

        Catch ex As Exception
            Dim elog As LoggerService = LoggerService.gtInstance
            elog.writeExceptionLogFile("Exc-GetQuestAnwerFromMac logged_guest1", ex)
        End Try

    End Function

    Public Function GetQuestAnwerFromMac1(ByVal mac As String) As String

        Dim dt As DataTable = New DataTable()


        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim query As SqlCommand = New SqlCommand("select ans from guest where GuestStatus='A' and guestid =(select guestid from macdetails where mac='" & mac & "')", conn)
            'dt = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(query)
            dt = DatabaseUtil.ExecuteSelect(query)
            If dt.Rows.Count > 0 Then

                Return dt.Rows(0)(0).ToString()
                ' ddlQuestion.Text = dt.Rows(0)("qns").ToString()

            Else
                Return "-1"
            End If

        Catch ex As Exception
            Dim elog As LoggerService = LoggerService.gtInstance
            elog.writeExceptionLogFile("Exc-GetQuestAnwerFromMac logged_guest1", ex)
        End Try

    End Function

    Public Function GetQuestname(ByVal mac As String) As String

        Dim dt As DataTable = New DataTable()


        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim query As SqlCommand = New SqlCommand("select Guestname from guest where GuestStatus='A' and guestid =(select guestid from macdetails where mac='" & mac & "')", conn)
            'dt = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(query)
            dt = DatabaseUtil.ExecuteSelect(query)
            If dt.Rows.Count > 0 Then

                Return dt.Rows(0)(0).ToString()
                ' ddlQuestion.Text = dt.Rows(0)("qns").ToString()

            Else
                Return "-1"
            End If

        Catch ex As Exception
            Dim elog As LoggerService = LoggerService.gtInstance
            elog.writeExceptionLogFile("Exc-GetQuestAnwerFromMac logged_guest1", ex)
        End Try

    End Function

    Public Function GetQuestroomno(ByVal mac As String) As String

        Dim dt As DataTable = New DataTable()


        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim query As SqlCommand = New SqlCommand("select Guestroomno from guest where GuestStatus='A' and guestid =(select guestid from macdetails where mac='" & mac & "')", conn)
            'dt = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(query)
            dt = DatabaseUtil.ExecuteSelect(query)
            If dt.Rows.Count > 0 Then

                Return dt.Rows(0)(0).ToString()
                ' ddlQuestion.Text = dt.Rows(0)("qns").ToString()

            Else
                Return "-1"
            End If

        Catch ex As Exception
            Dim elog As LoggerService = LoggerService.gtInstance
            elog.writeExceptionLogFile("Exc-GetQuestAnwerFromMac logged_guest1", ex)
        End Try

    End Function
  

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
                    lblErrorMsg.Text = "Dear Guest, please enter your valid Room Number and First/Last Name"

                ElseIf ExtendedUtil.getGuestStatusLog1(room_no, last_name) = 1 Then

                    ' Session("Message") = "Dear Guest , you have entered incorrect Room number / Last Name /First Name"
                    'Response.Redirect("~/UserError.aspx")
                    lblErrorMsg.Text = "Dear Guest, please enter your valid Room Number and First/Last Name"

                Else

                    ' Session("Message") = "Dear Guest , you have entered incorrect Room number / Last Name /First Name"
                    ' Response.Redirect("~/UserError.aspx")
                    lblErrorMsg.Text = "Dear Guest, please enter your valid Room Number and First/Last Name"



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


    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        lblErrorMsg.Text = ""

        Dim qns As String = ""
        Try
            Dim roomNo As String = txtRoomNo.Text.Trim()
            Dim gname As String = txtAccessCode.Text.Trim()


            Try
                gname = gname.Replace("/", "")
                'ob.write2LogFile("roomtest", gname)

                gname = gname.Replace("  ", " ")
                'ob.write2LogFile("roomtest", gname)



            Catch ex As Exception

            End Try


            Dim error1 As Integer = 0
            Try
                error1 = validateGuest(roomNo, gname)
            Catch ex As Exception

            End Try


            If error1 = 0 Then

                Return
                Exit Sub

            Else

                qns = ExtendedUtil.getGuestStatus1(roomNo, gname)
                If qns = "-1" Or qns = "" Then

                    lblErrorMsg.Text = "Dear Guest, security question not found"

                Else
                    P1.Visible = True
                    p2.Text = qns
                    Button2.Visible = True
                    kk.Visible = True

                    a1.Visible = False
                    a2.Visible = False
                    Button1.Visible = False

                    kk2.Visible = True
                End If



            End If

        Catch ex As Exception

        End Try


    End Sub

    Protected Sub Button2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button2.Click


        Dim t1 As String = ""
        Dim t2 As String = ""



        lblErrorMsg.Text = ""

        Try
            Dim qs As String = ""
            Dim mac As String = ""
            Dim ans As String = ""
            Dim ObjLog As LoggerService
            ObjLog = LoggerService.gtInstance

            Dim commonFun As PMSCommonFun
            qs = Request.QueryString("encry")
            commonFun = PMSCommonFun.getInstance
            mac = commonFun.DecrptQueryString("MA", qs)


            Try
                ans = GetQuestAnwerFromMac1(mac)

                ' ObjLog.write2LogFile("Reset", "Getting answer from mac" & ans)
            Catch ex As Exception

            End Try

            If ans = "-1" Or ans = "" Then

                Dim r1 As String = ""
                Dim r2 As String = ""

                Try
                    r1 = Request.QueryString("rm")
                    r2 = Request.QueryString("ln")
                    Try
                        Dim str1() = r1.Split(",")
                        Dim ind As Integer = 0
                        ind = str1.Length - 1

                        r1 = str1(ind)
                        ' objlog.write2LogFile("Mac_" & "Login1 roomno r1" & r1, r1)

                    Catch ex As Exception

                    End Try
                Catch ex As Exception

                End Try

                Try
                    Dim str2() = r2.Split(",")
                    Dim ind As Integer = 0
                    ind = str2.Length - 1

                    r2 = str2(ind)
                    ' ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI after split r2" & r2)

                Catch ex As Exception

                End Try


                Try
                    ans = ExtendedUtil.getGuestStatus2(r1, r2)
                Catch ex As Exception

                End Try

                Try
                    t1 = r1
                    t2 = r2
                Catch ex As Exception

                End Try

              



                Try
                    'ObjLog.write2LogFile("Reset", "Getting answer from roomno name" & "room=" & r1 & "Name=" & r2 & "answer" & ans)

                Catch ex As Exception

                End Try

            End If


            If ans = "-1" Or ans = "" Then

                Dim roomNo As String = txtRoomNo.Text.Trim()
                Dim gname As String = txtAccessCode.Text.Trim()


                Try
                    t1 = roomNo
                    t2 = gname
                Catch ex As Exception

                End Try



                ans = ExtendedUtil.getGuestStatus2(roomNo, gname)


                Try
                    ' ObjLog.write2LogFile("Reset", "Getting answer from textbox roomno name" & "room=" & roomNo & "Name=" & gname & "answer" & ans)

                Catch ex As Exception

                End Try

            End If


            Try
                If UCase(ans) = UCase(TextBox1.Text.Trim) Then
                    P1.Visible = False
                    p2.Visible = False
                    Button2.Visible = False
                    kk.Visible = False

                    kk2.Visible = False

                    kk3.Visible = True
                    kk1.Visible = True
                    P3.Visible = True
                    P4.Visible = True
                    Button3.Visible = True

                Else

                    lblErrorMsg.Text = "Dear Guest, please enter your valid security answer"


                    Try

                        commonFun = PMSCommonFun.getInstance

                        mac = commonFun.DecrptQueryString("MA", qs)
                        Dim ObjLoginService As LoginService
                        Dim ObjStrLoginFails As New strLoginFails
                        ObjStrLoginFails.FailAccId = 0
                        ObjStrLoginFails.FailAccessCode = ""
                        ObjStrLoginFails.FailGuestName = t2
                        ObjStrLoginFails.FailMac = mac
                        ObjStrLoginFails.FailMsg = "Incorrect security answer"
                        Dim rn As String = ""
                        rn = commonFun.DecrptQueryString("RN", qs)
                        Dim at As Integer = 0
                        If rn = "" Then
                            at = 0
                        Else
                            at = 1
                        End If

                        ObjStrLoginFails.FailRoomNo = t1

                        ObjStrLoginFails.FailAccessType = at
                        ObjStrLoginFails.Remarks = "Incorrect security answer"

                        ObjLoginService = LoginService.getInstance()

                        ObjLoginService.GenericLoginFails(ObjStrLoginFails)


                    Catch ex As Exception

                    End Try





                    hdAccept.Value = 0
                    Return

                End If

            Catch ex As Exception

            End Try














        Catch ex As Exception

        End Try
    End Sub

    Protected Sub Button3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button3.Click
        Dim ObjAccCode As AccessCode

        Dim ObjUCred As UserCredential
        Dim ObjUserContext As UserContext

        lblErrorMsg.Text = ""



        Dim r1 As String = ""
        Dim r2 As String = ""
        Dim acccode As String = ""


        Dim room_No As String = ""
        Dim last_Name As String = ""

        Dim acc As String = ""


        lblErrorMsg.Text = ""

        Dim qs As String = ""
        Dim mac As String = ""
        Dim ans As String = ""
        Dim ObjLog As LoggerService
        ObjLog = LoggerService.gtInstance

        Dim commonFun As PMSCommonFun
        qs = Request.QueryString("encry")
        commonFun = PMSCommonFun.getInstance
        mac = commonFun.DecrptQueryString("MA", qs)

        Try
            room_No = GetQuestroomno(mac)
        Catch ex As Exception

        End Try
        Try
            last_Name = GetQuestname(mac)
        Catch ex As Exception

        End Try


        Try
            acccode = Request.QueryString("acc")
            Dim str3() = acccode.Split(",")
            Dim ind As Integer = 0
            ind = str3.Length - 1

            acccode = str3(ind)
            ' ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI after split r2" & r2)

        Catch ex As Exception

        End Try




        Try
            ' ObjLog.write2LogFile("Reset", "Getting mac roomno name" & "room=" & room_No & "Name=" & last_Name & "answer" & ans)

        Catch ex As Exception

        End Try

        If room_No = "" Or last_Name = "" Or room_No = "-1" Or last_Name = "-1" Then
            Try
                r1 = Request.QueryString("rm")
                r2 = Request.QueryString("ln")


                Try
                    acccode = Request.QueryString("acc")

                Catch ex As Exception

                End Try



                ' ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI before split r1" & r1)

                Try
                    Dim str1() = r1.Split(",")
                    Dim ind As Integer = 0
                    ind = str1.Length - 1

                    r1 = str1(ind)
                    '    ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI after split r1" & r1)

                Catch ex As Exception

                End Try
                '  ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI before split r2" & r2)

                Try
                    Dim str2() = r2.Split(",")
                    Dim ind As Integer = 0
                    ind = str2.Length - 1

                    r2 = str2(ind)
                    ' ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI after split r2" & r2)

                Catch ex As Exception

                End Try

                Try
                    Dim str3() = acccode.Split(",")
                    Dim ind As Integer = 0
                    ind = str3.Length - 1

                    acccode = str3(ind)
                    ' ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI after split r2" & r2)

                Catch ex As Exception

                End Try

              
            Catch ex As Exception

            End Try

            Try
                If r1 <> "" And r2 <> "" Then
                    If room_No = "" Then
                        room_No = r1
                    End If
                    If last_Name = "" Then
                        last_Name = r2
                    End If
                End If

                room_No = r1
                last_Name = r2



            Catch ex As Exception

            End Try

            Try
                'ObjLog.write2LogFile("Reset", "Getting query string roomno name" & "room=" & room_No & "Name=" & last_Name & "access" & acccode)

            Catch ex As Exception

            End Try


        End If


        If room_No = "" Or last_Name = "" Or room_No = "-1" Or last_Name = "-1" Then

            room_No = txtRoomNo.Text.Trim()
            last_Name = txtAccessCode.Text.Trim()
            Try
                'ObjLog.write2LogFile("Reset", "Getting textbox roomno name" & "room=" & room_No & "Name=" & last_Name & "access" & acccode)

            Catch ex As Exception

            End Try

        End If

        






        Dim p1 As String = TextBox2.Text.Trim()
        Dim p2 As String = TextBox3.Text.Trim()


      

       






        If UCase(p1) <> UCase(p2) Then

            lblErrorMsg.Text = "Dear Guest, the password and the confirmed password do not match"
            hdAccept.Value = 0
            Return

        ElseIf p1.Length < 6 Then
            lblErrorMsg.Text = "Dear Guest, the password must have minimum 6 characters "
            hdAccept.Value = 0
            Return


        ElseIf UCase(p1) = UCase(last_Name) Then
            lblErrorMsg.Text = "Dear Guest, Login Password and Last Name should not be identical"
            hdAccept.Value = 0
            Return




        Else


            Try
                Dim GT As DataTable
                GT = ExtendedUtil.getGuestDetails(room_No, last_Name)
                Dim regcode As String = ""
                regcode = GT.Rows(0)(3)
                ExtendedUtil.PasswordHistory(room_No, last_Name, regcode, p1)
            Catch ex As Exception

            End Try




            Try
                ExtendedUtil.setpwd2(room_No, last_Name, p1)
            Catch ex As Exception

            End Try

            Try
                Dim objelog As LoggerService
                objelog = LoggerService.gtInstance
                ' objelog.write2LogFile("Accesscode_" & room_No, "Access code from query string" & acccode)
                Dim GT As DataTable
                GT = ExtendedUtil.getGuestDetails(room_No, last_Name)
                Dim regcode As String = ""
                regcode = GT.Rows(0)(3)



                Try

                Catch ex As Exception

                End Try






                acccode = ExtendedUtil.getAcode(regcode)
                ' objelog.write2LogFile("Accesscode_" & room_No, "Access code from data base" & acccode)







            Catch ex As Exception

            End Try


            Try
                If acccode = "" Or acccode = "-1" Then

                    Dim regcode As String = ExtendedUtil.getname2(room_No, last_Name)

                    Dim guestname1 As String
                    acccode = CouponUtil.CreateCoupons(1, "ITC")

                    Try
                        guestname1 = ExtendedUtil.getname1(room_No, last_Name)
                    Catch ex As Exception

                    End Try

                    ExtendedUtil.IssueAccessCodeToGuest("MSPL", "", acccode, room_No, last_Name, regcode)
                End If
            Catch ex As Exception

            End Try

            Try
                Dim objelog As LoggerService
                objelog = LoggerService.gtInstance
                ' objelog.write2LogFile("Accesscode_" & room_No, "Access code from final" & acccode)


            Catch ex As Exception

            End Try




            Dim url As String = ""
            Try
                commonFun = PMSCommonFun.getInstance
                url = commonFun.BrowserQueryString(Request)


            Catch ex As Exception

            End Try


            Try
                Dim ACCESSTYPE As Integer = 0
                Try
                    Dim rn As String = ""



                    commonFun = PMSCommonFun.getInstance
                    qs = Request.QueryString("encry")

                    rn = commonFun.DecrptQueryString("RN", qs)


                    If rn = "" Then
                        ACCESSTYPE = 0
                    Else
                        ACCESSTYPE = 1
                    End If
                Catch ex As Exception

                End Try




                ObjAccCode = New AccessCode("", acccode)
                ObjUCred = ObjAccCode.CollectAccDetails(ITCCORE.ACCESSTYPE.ONLYCODE)

                Try
                    Dim objelog As LoggerService
                    objelog = LoggerService.gtInstance
                    ' objelog.write2LogFile("test", "accid_" & ObjUCred.ACCID & "Gid " & ObjUCred.GuestID & "pwd" & ObjUCred.passwd & "uid" & ObjUCred.usrId)
                Catch ex As Exception

                End Try


                ObjUserContext = New UserContext(ObjUCred, HttpContext.Current.Request)
                ObjUserContext.item("usertype") = EUSERTYPE.ROOM
                ObjUserContext.item("accesstype") = ACCESSTYPE
                Login(ObjUserContext)
            Catch ex As Exception

            End Try


        End If



        'Try

        '    Response.Redirect("mifilogin.aspx?" & url & "&plan=" & 0 & "&ln=" & last_Name & "&rm=" & room_No)
        'Catch ex As Exception

        'End Try




    End Sub

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
                   
                    Response.Redirect("msg_info1.aspx?" & url & "&total=" & tot)

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

    Protected Sub Button4_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button4.Click
        Try

            Dim r1 As String = ""
            r1 = Request.QueryString("tp")


            Dim url As String = ""
            Dim commonfun As PMSCommonFun

            commonfun = PMSCommonFun.getInstance
            url = commonfun.BrowserQueryString(Request)



            ' ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI before split r1" & r1)

            Try
                Dim str1() = r1.Split(",")
                Dim ind As Integer = 0
                ind = str1.Length - 1

                r1 = str1(ind)
                '    ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI after split r1" & r1)
                If r1 = "1" Then
                    Response.Redirect("logged_guest1.aspx?" & url)

                Else
                    Response.Redirect("password.aspx?" & url)
                End If


            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try


    End Sub
End Class