Imports ITCCORE
Imports security
Partial Public Class itcdiscount
    Inherits System.Web.UI.Page

    Public url As String
    Private accesstype As Integer

    Private PMSName As PMSNAMES
    Private PMSVersion As String

    Private planId As Long
    Private profile As GuestProfile
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



        Dim ObjLog As LoggerService

        Dim qs, NSEID As String
        Dim commonFun As PMSCommonFun
        Dim UI, UURL, MAC, SC, OS, RN As String
        Dim objSysConfig As New CSysConfig
        Dim encrypt As New Datasealing

        '========= Latest code snippet: June 16, 2012 =========================
        '========= By: Subhadeep Ray ==========================================

        '======================================================================

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
            If RN = "" Then
                accesstype = 0
            Else
                accesstype = 1
            End If
            'End Get the query string from URL
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
        Dim nomadixIP As String = objSysConfig.GetConfig("NomadixIP")
        If nomadixIP = "" Then
            Session("Message") = Messaging.NomadixNotRegisteredMessage
            Response.Redirect("~/UserError.aspx")
            Return
        End If
        'Check url authentication end ########################

        'ObjLog = LoggerService.gtInstance
        'Try
        ' ObjLog.write2LogFile("discount", "====inside discount Page===" & vbNewLine)
        If Not IsPostBack() Then

            btnLogin.Visible = False
            chk1.Visible = False
            Label1.Text = ""
            lblErrorMsg.Text = ""

            Try
                complogin()
            Catch ex As Exception

            End Try

        End If

        'If Not Session("us") Is Nothing Then
        '            ObjUserContext = Session("us")
        '            ObjLog.write2LogFile("ITCdiscount", ObjUserContext.item("logintype"))

        '            '========= Latest code snippet: June 12, 2012 =========================
        '            '========= By: Subhadeep Ray ==========================================
        '            '========= Reason: Incorporating discount on ITC FIAS =================
        '            'Profile = New GuestProfile(ObjUserContext.GuestID, ObjUserContext.AccessID)


        '            '======================================================================



        '            'Populate plans to display
        '            'PopulatePlans()

        '            'Populate Days to select
        '            'PopulateDays(ObjUserContext.GuestID)

        '            'DisplayPanel(DISPLAY.LOGINPANEL)

        '            Try
        '                'ObjLog.write2LogFile("Test" & ObjUserContext.GuestID, "GuestId" & ObjUserContext.GuestID & "Accid" & ObjUserContext.AccessID & "IsComplimentaryConsiderable" & profile.IsComplimentaryConsiderable & "discount" & profile.DiscountApplicable)
        '            Catch ex As Exception

        '            End Try
        '            '========= Latest code snippet: November 20, 2012 =========================
        '            '========= By: Subhadeep Ray ==========================================
        '            '========= Reason: (1)Direct connection for auto connect plan
        '            '                  (2)Direct connection for complimentary plan =================
        '            If Not profile.ExceptionalAutoConnect Is Nothing Then
        '                Dim ObjGuest As GuestService = GuestService.getInstance
        '                Dim NoOfStay As Integer = ObjGuest.GetGuestDaysStay(profile.GuestID)
        '                DoNewLogin(profile.ExceptionalAutoConnect.AutoConnectPlan, NoOfStay, 0)

        '            ElseIf profile.IsComplimentaryConsiderable = True Then

        '                Try
        '                    ObjLog.write2LogFile("Test" & ObjUserContext.GuestID, "GuestId" & ObjUserContext.GuestID & "Accid" & ObjUserContext.AccessID & "Complimentary login")
        '                Catch ex As Exception

        '                End Try

        '                Dim ObjGuest As GuestService = GuestService.getInstance
        '                Dim NoOfStay As Integer = ObjGuest.GetGuestDaysStay(profile.GuestID)

        '                Try
        '                    ObjLog.write2LogFile("Welcome", "==== Redirect to itcdiscout Page ==========" & vbNewLine)
        '                    Response.Redirect("itcdiscount.aspx?" & url)
        '                Catch ex As Exception

        '                End Try

        '                ' DoNewLogin(profile.ComplimentaryPlan, NoOfStay, 0)

        '            ElseIf profile.DiscountApplicable > 0 Then

        '                Try
        '                    ObjLog.write2LogFile("Test" & ObjUserContext.GuestID, "GuestId" & ObjUserContext.GuestID & "Accid" & ObjUserContext.AccessID & "Discount login")
        '                Catch ex As Exception

        '                End Try

        '                Try
        '                    ObjLog.write2LogFile("Welcome", "==== Redirect to itcdiscount Page ==========" & vbNewLine)
        '                    Response.Redirect("itcdiscount.aspx?" & url)
        '                Catch ex As Exception

        '                End Try

        '                ' DoNewLogin(profile.DefaultDiscountPlan, profile.DefaultAutoConnectDaysForDiscount, 0)
        '            End If
        '            '=============================================================================

        '        End If

        '    Else
        '        ObjLog.write2LogFile("Welcome", "no Session Value===" & vbNewLine)
        '    End If
        '    End If
        'Catch ex As Exception
        '    If Not ex.Message.ToLower().Contains("thread") Then
        '        ObjLog.writeExceptionLogFile("WelcomeEXP", ex)
        '    End If
        'End Try

    End Sub

    Protected Sub RadioButton2_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton2.CheckedChanged

        Try
            RadioButton1.Checked = False
            chk1.Visible = True
            btnLogin.Visible = True
        Catch ex As Exception

        End Try


    End Sub

    Protected Sub RadioButton2_CheckedChanged1(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton1.CheckedChanged
        Try
            RadioButton2.Checked = False
            chk1.Visible = True
            btnLogin.Visible = True
        Catch ex As Exception

        End Try
    End Sub


    Protected Sub complogin()
        Try

            lblErrorMsg.Text = ""

            Dim seamless As String = ""


            seamless = "1"


            Dim ObjUserContext As UserContext
            Dim ObjLog As LoggerService
            ObjLog = LoggerService.gtInstance
            Dim objSysConfig As New CSysConfig

            If Not Session("us") Is Nothing Then
                ObjUserContext = Session("us")
                profile = New GuestProfile(ObjUserContext.GuestID, ObjUserContext.AccessID)

                If profile.ComplimentaryPlan = 15 Or profile.ComplimentaryPlan = 16 Then

                    Try
                        'ObjLog.write2LogFile("Discount" & ObjUserContext.GuestID, "GuestId" & ObjUserContext.GuestID & "Accid" & ObjUserContext.AccessID & "Complimentary login")
                    Catch ex As Exception

                    End Try




                    Try
                        '  ObjLog.write2LogFile("discount", "==== Redirect to itcdiscout Page ==========" & vbNewLine)
                        DoNewLogin(profile.ComplimentaryPlan, "1", seamless)
                    Catch ex As Exception

                    End Try

                    ' 
                ElseIf profile.ComplimentaryPlan = 19 Then

                    Label1.Text = "299 per 24 hours. Taxes are extra as applicable."


                ElseIf profile.ComplimentaryPlan = 17 Then

                    ' Label1.Text = "299 per 24 hours. Taxes are extra as applicable."

                    Dim str As String = ""



                    Try
                        Dim kk As Integer = profile.DiscountApplicable
                        str = Math.Floor(CDbl(299) * ((100.0 - kk) / 100))
                    Catch ex As Exception

                    End Try

                    Label1.Text = str & " per 24 hours. Taxes are extra as applicable."


                    'Try

                    '    ObjLog.write2LogFile("discount" & ObjUserContext.GuestID, "GuestId" & ObjUserContext.GuestID & "Accid" & ObjUserContext.AccessID & "Discount login")
                    'Catch ex As Exception

                    'End Try

                    'DoNewLogin(profile.DefaultDiscountPlan, profile.DefaultAutoConnectDaysForDiscount, seamless)

                    ' 
                End If



            Else
                ' ObjLog.write2LogFile("discount", "no Session Value===" & vbNewLine)

                Dim qs As String = ""
                Dim commonFun As PMSCommonFun
                Dim MAC As String = ""
                Dim ObjAccessCode As AccessCode
                Dim ObjUserCred As UserCredential
                Dim encrypt As New Datasealing

                '========= Latest code snippet: June 16, 2012 =========================
                '========= By: Subhadeep Ray ==========================================

                '======================================================================

                Dim guestid As Long
                Dim accid As Long


                qs = Request.QueryString("encry")
                commonFun = PMSCommonFun.getInstance

                Try
                    'Start Get the query string from URL

                    MAC = commonFun.DecrptQueryString("MA", qs)

                    ObjAccessCode = New AccessCode(MAC, "")
                    ObjUserCred = ObjAccessCode.CollectAccDetails(ITCCORE.ACCESSTYPE.ONLYMAC)
                    '  ObjLog.write2LogFile("discount", "no Session Value===" & vbNewLine & "Mac=" & MAC)

                    If ObjUserCred.usrId <> "" Then

                        guestid = ObjUserCred.GuestID
                        accid = ObjUserCred.ACCID

                        Try
                            '  ObjLog.write2LogFile("discount", "no Session Value===" & vbNewLine & "guestid=" & guestid & "accid=" & accid)
                        Catch ex As Exception

                        End Try
                        profile = New GuestProfile(guestid, accid)

                        If profile.ComplimentaryPlan = 15 Or profile.ComplimentaryPlan = 16 Then

                            seamless = 1

                            Dim ObjGuest As GuestService = GuestService.getInstance


                            Try
                                'ObjLog.write2LogFile("discount", "==== Redirect to itcdiscout Page ==========" & vbNewLine)
                                DoNewLogin(profile.ComplimentaryPlan, "1", seamless)
                            Catch ex As Exception

                            End Try

                            ' 





                            '   DoNewLogin(profile.DefaultDiscountPlan, profile.DefaultAutoConnectDaysForDiscount, seamless)

                            ' 
                        End If



                    End If

                    'End Get the query string from URL
                Catch ex As Exception

                End Try





            End If



        Catch ex As Exception

        End Try












    End Sub



    Protected Sub HtmlBtnPurchase_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLogin.Click
        Try

            lblErrorMsg.Text = ""

            Dim seamless As String = "0"

            If RadioButton1.Checked = True Then
                seamless = "1"


            ElseIf RadioButton2.Checked = True Then
                seamless = "0"
            End If

            Dim ObjUserContext As UserContext
            Dim ObjLog As LoggerService
            ObjLog = LoggerService.gtInstance
            Dim objSysConfig As New CSysConfig

            If Not Session("us") Is Nothing Then
                ObjUserContext = Session("us")
                profile = New GuestProfile(ObjUserContext.GuestID, ObjUserContext.AccessID)

               If profile.ComplimentaryPlan = 15 Or profile.ComplimentaryPlan = 16 Then
                    Try
                        ' ObjLog.write2LogFile("Discount" & ObjUserContext.GuestID, "GuestId" & ObjUserContext.GuestID & "Accid" & ObjUserContext.AccessID & "Complimentary login")
                    Catch ex As Exception

                    End Try

                    seamless = 1


                    Try
                        ' ObjLog.write2LogFile("discount", "==== Redirect to itcdiscout Page ==========" & vbNewLine)
                        DoNewLogin(profile.ComplimentaryPlan, "1", seamless)
                    Catch ex As Exception

                    End Try

                    ' 

                Else

                    Try

                        '  ObjLog.write2LogFile("discount" & ObjUserContext.GuestID, "GuestId" & ObjUserContext.GuestID & "Accid" & ObjUserContext.AccessID & "Discount login")
                    Catch ex As Exception

                    End Try

                    DoNewLogin(profile.ComplimentaryPlan, "1", seamless)

                    ' 
                End If



            Else
                ' ObjLog.write2LogFile("discount", "no Session Value===" & vbNewLine)

                Dim qs As String = ""
                Dim commonFun As PMSCommonFun
                Dim MAC As String = ""
                Dim ObjAccessCode As AccessCode
                Dim ObjUserCred As UserCredential
                Dim encrypt As New Datasealing

                '========= Latest code snippet: June 16, 2012 =========================
                '========= By: Subhadeep Ray ==========================================

                '======================================================================

                Dim guestid As Long
                Dim accid As Long


                qs = Request.QueryString("encry")
                commonFun = PMSCommonFun.getInstance

                Try
                    'Start Get the query string from URL

                    MAC = commonFun.DecrptQueryString("MA", qs)

                    ObjAccessCode = New AccessCode(MAC, "")
                    ObjUserCred = ObjAccessCode.CollectAccDetails(ITCCORE.ACCESSTYPE.ONLYMAC)
                    ' ObjLog.write2LogFile("discount", "no Session Value===" & vbNewLine & "Mac=" & MAC)

                    If ObjUserCred.usrId <> "" Then

                        guestid = ObjUserCred.GuestID
                        accid = ObjUserCred.ACCID

                        Try
                            'ObjLog.write2LogFile("discount", "no Session Value===" & vbNewLine & "guestid=" & guestid & "accid=" & accid)
                        Catch ex As Exception

                        End Try
                        profile = New GuestProfile(guestid, accid)

                        If profile.ComplimentaryPlan = 15 Or profile.ComplimentaryPlan = 16 Then



                            Dim ObjGuest As GuestService = GuestService.getInstance
                            seamless = "1"

                            Try
                                'ObjLog.write2LogFile("discount", "==== Redirect to itcdiscout Page ==========" & vbNewLine)
                                DoNewLogin(profile.ComplimentaryPlan, "1", seamless)
                            Catch ex As Exception

                            End Try

                            ' 

                        Else



                            DoNewLogin(profile.ComplimentaryPlan, "1", seamless)

                            ' 
                        End If



                    End If

                    'End Get the query string from URL
                Catch ex As Exception

                End Try





            End If



        Catch ex As Exception

        End Try












    End Sub

    Private Sub DoNewLogin(ByVal planId As Long, ByVal noDays As String, ByVal seamless As String)

        Dim ObjUserContext As UserContext
        Dim LogRoom As String
        Dim ObjLog As LoggerService = LoggerService.gtInstance

        ' ObjLog.write2LogFile("DoNewLogin", "============== Inside Discount New Login ===========" & vbNewLine)

        If Not Session("us") Is Nothing Then
            ObjUserContext = Session("us")
            LogRoom = ObjUserContext.roomNo

            ObjUserContext.selectedPlanId = planId
            ObjUserContext.item("noofdays") = noDays

            ObjUserContext.item("seam") = seamless.ToString()


            Try
                Dim noofstay As Integer = 1
                Dim ObjGuest As GuestService
                ObjGuest = GuestService.getInstance

                Try
                    noofstay = ObjGuest.GetGuestDaysStay(ObjUserContext.GuestID)
                Catch ex As Exception

                End Try

                Try
                    Dim qs As String

                    Dim commonFun As PMSCommonFun

                    qs = Request.QueryString("encry")
                    commonFun = PMSCommonFun.getInstance

                    Dim sip As String
                    sip = commonFun.DecrptQueryString("SIP", qs)

                    ObjLog.write2LogFile("SIPa", sip)
                    ObjUserContext.item("sip") = sip
                Catch ex As Exception

                End Try


                ObjUserContext.item("Totalnoofdays") = noofstay.ToString()
            Catch ex As Exception

            End Try


            Try
                ObjLog.write2LogFile(LogRoom, " ====== Inside Plan Selection New Login ====" & _
                                vbCrLf & " PlanId:" & planId & _
                                 vbCrLf & " PlanId:" & planId & _
                                  vbCrLf & " Seamless Connection:" & ObjUserContext.item("seam") & _
                                vbCrLf & " TotalPlanDays:" & ObjUserContext.item("Totalnoofdays") & _
                                vbCrLf & " AccessType:" & ObjUserContext.item("accesstype") & _
                                vbCrLf & " LoginType:" & ObjUserContext.item("logintype") & _
                                vbCrLf & " MAC:" & ObjUserContext.machineId)
            Catch ex As Exception

            End Try


            Login(ObjUserContext)

        Else
            Dim commonFun As PMSCommonFun = PMSCommonFun.getInstance
            url = commonFun.BrowserQueryString(Request)
            Response.Redirect("IdentifyLogin.aspx?" & url)
        End If

        ObjLog.write2LogFile("DoNewLogin", "============== Inside Plan Selection New Login ===========" & vbNewLine)

    End Sub
    Private Sub Login(ByRef UserCred As UserContext)
        Dim AAA As AAAService
        Dim ObjLog As LoggerService
        Dim OutPut As String = ""
        Dim commonFun As PMSCommonFun
        Try
            AAA = AAAService.getInstance
            commonFun = PMSCommonFun.getInstance
            url = commonFun.BrowserQueryString(Request)
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
            ElseIf UCase(OutPut) = "ACCEXCEED" Then
                Session("Message") = Messaging.DeviceExceededMessage
                Response.Redirect("~/UserError.aspx")
            ElseIf UCase(OutPut) = "COUPONDEACTIVE" Then
                Session("Message") = Messaging.CouponDeactivatedMessage
                Response.Redirect("~/UserError.aspx")
            Else
                Session("Message") = Messaging.TechnicalErrorMessage
                Response.Redirect("~/UserError.aspx")
            End If
        Catch ex As Exception

            If Not ex.Message.ToLower().Contains("thread") Then
                ObjLog = LoggerService.gtInstance
                ObjLog.writeExceptionLogFile("AAAEXP", ex)
            End If

        End Try
    End Sub




End Class