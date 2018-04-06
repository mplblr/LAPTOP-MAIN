Imports ITCCORE
Imports security
Partial Class password
    Inherits System.Web.UI.Page
    Public url As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ObjLog As LoggerService

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

    End Sub

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLogin.Click
        Dim r1 As String = ""
        lblErr.Text = ""
        Try
            r1 = Request.QueryString("rm")
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




        Dim objlog As LoggerService
        objlog = LoggerService.gtInstance




        Try
            If txtAccessCode.Text = "" Then
                lblerr.Visible = True
                lblerr.Text = "Dear Guest, Please enter your Login Password. "

                Return
            End If


            Dim roomno As String = ""
            Dim name As String = ""
            Dim res As String = ""


            Try
                roomno = r1
                name = txtAccessCode.Text.Trim()




                res = ExtendedUtil.getname(roomno, name)


                Try
                    ' objlog.write2LogFile("LoginTest" & roomno, "Guest Name=" & res)
                Catch ex As Exception

                End Try


                If res = "-1" Or res = "" Then

                    lblErr.Text = "Dear Guest, please enter a valid password"

                    Try
                        Dim commonFun As PMSCommonFun
                        commonFun = PMSCommonFun.getInstance
                        Dim qs As String = Request.QueryString("encry")
                        Dim mac As String = ""
                        mac = commonFun.DecrptQueryString("MA", qs)
                        Dim ObjLoginService As LoginService
                        Dim ObjStrLoginFails As New strLoginFails
                        ObjStrLoginFails.FailAccId = 0
                        ObjStrLoginFails.FailAccessCode = ""
                        ObjStrLoginFails.FailGuestName = name
                        ObjStrLoginFails.FailMac = mac
                        ObjStrLoginFails.FailMsg = "Incorrect Room Number or Login Password"
                        Dim rn As String = ""
                        rn = commonFun.DecrptQueryString("RN", qs)
                        Dim at As Integer = 0
                        If rn = "" Then
                            at = 0
                        Else
                            at = 1
                        End If

                        ObjStrLoginFails.FailRoomNo = roomno

                        ObjStrLoginFails.FailAccessType = at
                        ObjStrLoginFails.Remarks = "Room:NA"

                        ObjLoginService = LoginService.getInstance()

                        ObjLoginService.GenericLoginFails(ObjStrLoginFails)


                    Catch ex As Exception

                    End Try



                    'hdAccept.Value = 0
                    Return

                Else
                    Dim regcode As String = ""
                    Dim accesscode As String = ""
                    Dim guestname1 As String = ""

                    regcode = res

                    Try
                        ' accesscode = ExtendedUtil.getAcode(regcode)

                        'Dim GT As DataTable
                        'GT = ExtendedUtil.getGuestDetails(roomno, res)

                        'regcode = GT.Rows(0)(3)

                        accesscode = ExtendedUtil.getAcode(regcode)

                        Try
                            Dim objelog As LoggerService
                            objelog = LoggerService.gtInstance
                            ' objelog.write2LogFile("Accesscode_" & roomno, "Access code from Database " & accesscode)
                            'objlog.write2LogFile("AlreadyPassword-" & roomno, "Regcode=" & regcode & "Name=" & guestname1 & "Res=" & res & "old coupon=" & accesscode)

                        Catch ex As Exception

                        End Try
                    Catch ex As Exception

                    End Try

                    Try
                        If accesscode = "" Or accesscode = "-1" Then
                            accesscode = CouponUtil.CreateCoupons(1, "ITC")

                            Try
                                guestname1 = ExtendedUtil.getname1(roomno, name)
                            Catch ex As Exception

                            End Try

                            ExtendedUtil.IssueAccessCodeToGuest("MSPL", "", accesscode, roomno, guestname1, regcode)

                            '  objlog.write2LogFile("pwd-" & roomno, "Regcode=" & regcode & "Name=" & guestname1 & "Res=" & res & "new coupon=" & accesscode)



                        End If
                    Catch ex As Exception

                    End Try
                    Dim commonFun As PMSCommonFun
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
                            Dim mac As String = ""
                            Dim qs As String = ""


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


                        Dim ObjAccCode As AccessCode

                        Dim ObjUCred As UserCredential
                        Dim ObjUserContext As UserContext



                        ObjAccCode = New AccessCode("", accesscode)
                        ObjUCred = ObjAccCode.CollectAccDetails(ITCCORE.ACCESSTYPE.ONLYCODE)
                        ObjUserContext = New UserContext(ObjUCred, HttpContext.Current.Request)
                        ObjUserContext.item("usertype") = EUSERTYPE.ROOM
                        ObjUserContext.item("accesstype") = ACCESSTYPE
                        Login(ObjUserContext)
                    Catch ex As Exception

                    End Try







                End If





            Catch ex As Exception

            End Try





        Catch ex As Exception

        End Try
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

   

   

    'Protected Sub btnLogin2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLogin2.Click

    '    Dim r1 As String = ""
    '    Dim r2 As String = ""
    '    Dim acccode As String = ""
    '    Dim ObjLog As LoggerService
    '    ObjLog = LoggerService.gtInstance
    '    Dim commonFun As PMSCommonFun
    '    commonFun = PMSCommonFun.getInstance
    '    url = commonFun.BrowserQueryString(Request)

    '    Dim room_No As String = ""
    '    Dim last_Name As String = ""

    '    Dim acc As String = ""


    '    Try
    '        r1 = Request.QueryString("rm")
    '        r2 = Request.QueryString("ln")


    '        Try
    '            acccode = Request.QueryString("acc")

    '        Catch ex As Exception

    '        End Try



    '        ' ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI before split r1" & r1)

    '        Try
    '            Dim str1() = r1.Split(",")
    '            Dim ind As Integer = 0
    '            ind = str1.Length - 1

    '            r1 = str1(ind)
    '            '    ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI after split r1" & r1)

    '        Catch ex As Exception

    '        End Try
    '        '  ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI before split r2" & r2)

    '        Try
    '            Dim str2() = r2.Split(",")
    '            Dim ind As Integer = 0
    '            ind = str2.Length - 1

    '            r2 = str2(ind)
    '            ' ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI after split r2" & r2)

    '        Catch ex As Exception

    '        End Try

    '        Try
    '            Dim str3() = acccode.Split(",")
    '            Dim ind As Integer = 0
    '            ind = str3.Length - 1

    '            acccode = str3(ind)
    '            ' ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI after split r2" & r2)

    '        Catch ex As Exception

    '        End Try

    '        Try
    '            Dim str3() = acccode.Split(",")
    '            Dim ind As Integer = 0
    '            ind = str3.Length - 1

    '            acccode = str3(ind)
    '            ' ObjElog.write2LogFile("Mac_" & commonFun.DecrptQueryString("MA", qs), "MIFI after split r2" & r2)

    '        Catch ex As Exception

    '        End Try

    '    Catch ex As Exception

    '    End Try

    '    Try
    '        If r1 <> "" And r2 <> "" Then
    '            If room_No = "" Then
    '                room_No = r1
    '            End If
    '            If last_Name = "" Then
    '                last_Name = r2
    '            End If
    '        End If

    '    Catch ex As Exception

    '    End Try

    '    Try
    '        ' ObjLog.write2LogFile("Reset", "redirecting forget  roomno name" & "room=" & room_No & "Name=" & last_Name & "access" & acccode)

    '    Catch ex As Exception

    '    End Try

    '    Try
    '        Response.Redirect("forgotpwd.aspx?" & url & "&ln=" & last_Name & "&rm=" & room_No & "&acc=" & acccode & "&tp=" & "2")
    '    Catch ex As Exception

    '    End Try



    'End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        Try
            Dim url As String = ""
            Dim commonfun As PMSCommonFun

            commonfun = PMSCommonFun.getInstance
            url = commonfun.BrowserQueryString(Request)
            Response.Redirect("Instruction.aspx?" & url)
        Catch ex As Exception

        End Try
    End Sub
End Class
