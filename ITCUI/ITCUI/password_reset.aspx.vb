Imports ITCCORE
Imports security

Partial Class password_reset
    Inherits System.Web.UI.Page
    Public url As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ObjLog As LoggerService

        Dim qs, NSEID As String
        Dim commonFun As PMSCommonFun
        Dim UI, UURL, MAC, SC, OS, RN As String
        Dim objSysConfig As New CSysConfig
        Dim encrypt As New Datasealing


        Try



            Dim p1 As String = txtAccCode.Text.Trim()
            Dim p2 As String = TextBox1.Text.Trim()

            txtAccCode.Attributes.Add("value", p1)
            TextBox1.Attributes.Add("value", p2)


        Catch ex As Exception

        End Try



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

        'Check url authentication end ########################


    End Sub

   



    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLogin.Click

        Dim ObjAccCode As AccessCode

        Dim ObjUCred As UserCredential
        Dim ObjUserContext As UserContext
       




        Dim r1 As String = ""
        Dim r2 As String = ""
        Dim acccode As String = ""


        Dim room_No As String = ""
        Dim last_Name As String = ""

        Dim acc As String = ""



        lblerr1.Text = ""
        Try
            r1 = Request.QueryString("rm")
            r2 = Request.QueryString("ln")

            acccode = Request.QueryString("acc")



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

        Catch ex As Exception

        End Try

        Dim qus As String = ""
        Dim ans As String = ""

        Try
            ans = ""
        Catch ex As Exception

        End Try

        Try

            Dim value As String

            value = 1

            If value = 1 Then
                qus = "Name of your first school?"
            ElseIf value = 2 Then
                qus = "Your favourite sport?"

            ElseIf value = 3 Then
                qus = "Your favourite city?"
            ElseIf value = 4 Then
                qus = "Your favorite author?"
            ElseIf value = 5 Then
                qus = "Your favorite colour?"

            End If
        Catch ex As Exception

        End Try




        Dim p1 As String = txtAccCode.Text.Trim()
        Dim p2 As String = TextBox1.Text.Trim()

        If UCase(p1) <> UCase(p2) Then

            lblerr1.Text = "Dear Guest, the password and the confirmed password do not match"
            hdAccept.Value = 0
            Return

        ElseIf p1.Length < 6 Then
            lblerr1.Text = "Dear Guest, the password must have minimum 6 characters "
            hdAccept.Value = 0
            Return


        ElseIf UCase(p1) = UCase(last_Name) Then
            lblerr1.Text = "Dear Guest, Login Password and Last Name should not be identical"
            hdAccept.Value = 0
            Return

     

        Else


            Try
                System.Threading.Thread.Sleep(1000)
            Catch ex As Exception

            End Try




            Try
                ExtendedUtil.setpwd(room_No, last_Name, p1, qus, ans)
            Catch ex As Exception

            End Try

            Try
                Dim objelog As LoggerService
                objelog = LoggerService.gtInstance
                '  objelog.write2LogFile("Accesscode_" & room_No, "Access code from query string" & acccode)


            Catch ex As Exception

            End Try


            Try

                Dim GT As DataTable
                GT = ExtendedUtil.getGuestDetails(room_No, last_Name)
                Dim regcode As String = ""
                regcode = GT.Rows(0)(3)

                acccode = ExtendedUtil.getAcode(regcode)

                Try
                    Dim objelog As LoggerService
                    objelog = LoggerService.gtInstance
                    'objelog.write2LogFile("Accesscode_" & room_No, "Access code from Database " & acccode)


                Catch ex As Exception

                End Try

                If acccode = "" Or acccode = "-1" Then

                    Dim regcode1 As String = ExtendedUtil.getname2(room_No, last_Name)

                    Dim guestname1 As String
                    acccode = CouponUtil.CreateCoupons(1, "ITC")

                    Try
                        guestname1 = ExtendedUtil.getname1(room_No, last_Name)
                    Catch ex As Exception

                    End Try

                    ExtendedUtil.IssueAccessCodeToGuest("MSPL", "", acccode, room_No, last_Name, regcode1)
                End If
            Catch ex As Exception

            End Try

            Try
                Dim objelog As LoggerService
                objelog = LoggerService.gtInstance
                'objelog.write2LogFile("Accesscode_" & room_No, "Access code from final" & acccode)


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




                ObjAccCode = New AccessCode("", acccode)
                ObjUCred = ObjAccCode.CollectAccDetails(ITCCORE.ACCESSTYPE.ONLYCODE)
                ObjUserContext = New UserContext(ObjUCred, HttpContext.Current.Request)
                ObjUserContext.item("usertype") = EUSERTYPE.ROOM
                ObjUserContext.item("accesstype") = ACCESSTYPE
                Login(ObjUserContext)
            Catch ex As Exception

            End Try





            'Try

            '    Response.Redirect("mifilogin.aspx?" & url & "&plan=" & 0 & "&ln=" & last_Name & "&rm=" & room_No)
            'Catch ex As Exception

            'End Try







        End If

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

End Class
