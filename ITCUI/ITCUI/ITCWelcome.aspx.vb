'############## Jan 11,2011-Page Description Start ##########################
'-- This page is called by identifylogin
'-- This page contains 2 buttons(continue button, confirm Purchase) and corresponding event
'-- In pageload event it will check the seesion value to check relogin/freeplan/newlogin
'-- If Session says its relogin/freeplan then it will show the continue panel
'-- Continue Panel indicates that guest has valid plan(plan still not expired) or he entitled for free plan
'-- If session says its NEWLogin then purchase panel will appear.
'-- Puchase Panel indicates guest coming newly/first time and he does not have valid plan.
'-- In that case he needs to select a particular plan, check terms & condition and click confirm purchase button.
'############## Jan 11,2011-Page Description End   ##########################
Imports ITCCORE
Imports security
Partial Public Class ITCWelcome
    Inherits System.Web.UI.Page

    Public url As String
    Private accesstype As Integer

    Private PMSName As PMSNAMES
    Private PMSVersion As String

    Private planId As Long
    Private profile As GuestProfile

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim ObjUserContext As UserContext
        Dim ObjLog As LoggerService

        Dim lguestid As Long = 0
        Dim laccid As Long = 0


        Dim qs, NSEID As String
        Dim commonFun As PMSCommonFun
        Dim UI, UURL, MAC, SC, OS, RN As String
        Dim objSysConfig As New CSysConfig
        Dim encrypt As New Datasealing

        '========= Latest code snippet: June 16, 2012 =========================
        '========= By: Subhadeep Ray ==========================================
        lblWelcomeAddress.Text = ""
        lblWelcomeGuestName.Text = ""
        lblFirstDiscountInfo.Text = ""
        lblSecondDiscountInfo.Text = ""
        welcomeNoteWrapper.Visible = False
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
        '------------------------------------------------------------------------

        ObjLog = LoggerService.gtInstance
        Try
            ObjLog.write2LogFile("Welcome", "====inside WelCome Page===" & vbNewLine)






            If Not IsPostBack() Then
                lblErrorMsg.Visible = False
                If Not Session("us") Is Nothing Then
                    ObjUserContext = Session("us")
                    ObjLog.write2LogFile("welcome", ObjUserContext.item("logintype"))

                    '========= Latest code snippet: June 12, 2012 =========================
                    '========= By: Subhadeep Ray ==========================================
                    '========= Reason: Incorporating discount on ITC FIAS =================
                    profile = New GuestProfile(ObjUserContext.GuestID, ObjUserContext.AccessID)

                    'Display Guest related information in the upper tab
                    DisplayGuestInformation()
                    '======================================================================

                    If ObjUserContext.item("logintype") = LOGINTYPE.FREEPLAN Then
                        DisplayPanel(DISPLAY.CONTINUEPANEL)

                        'Relogin is always automatic
                        DoReLogin()

                    ElseIf ObjUserContext.item("logintype") = LOGINTYPE.RELOGIN Then
                        DisplayPanel(DISPLAY.CONTINUEPANEL)

                        'Relogin is always automatic
                        DoReLogin()

                    Else

                        'Populate plans to display
                        PopulatePlans()

                        'Populate Days to select
                        PopulateDays(ObjUserContext.GuestID)

                        DisplayPanel(DISPLAY.LOGINPANEL)

                        Try
                            ObjLog.write2LogFile("Test" & ObjUserContext.GuestID, "GuestId" & ObjUserContext.GuestID & "Accid" & ObjUserContext.AccessID & "IsComplimentaryConsiderable" & profile.IsComplimentaryConsiderable & "discount" & profile.DiscountApplicable)
                        Catch ex As Exception

                        End Try
                        '========= Latest code snippet: November 20, 2012 =========================
                        '========= By: Subhadeep Ray ==========================================
                        '========= Reason: (1)Direct connection for auto connect plan
                        '                  (2)Direct connection for complimentary plan =================

                        Try
                            Response.Redirect("itcdiscount.aspx?" & url)
                        Catch ex As Exception

                        End Try





                        If Not profile.ExceptionalAutoConnect Is Nothing Then
                            Dim ObjGuest As GuestService = GuestService.getInstance
                            Dim NoOfStay As Integer = ObjGuest.GetGuestDaysStay(profile.GuestID)
                            DoNewLogin(profile.ExceptionalAutoConnect.AutoConnectPlan, NoOfStay, 0)

                        ElseIf profile.IsComplimentaryConsiderable = True Then

                            Try
                                ObjLog.write2LogFile("Test" & ObjUserContext.GuestID, "GuestId" & ObjUserContext.GuestID & "Accid" & ObjUserContext.AccessID & "Complimentary login")
                            Catch ex As Exception

                            End Try



                            Try
                                ObjLog.write2LogFile("Welcome", "==== Redirect to itcdiscout Page ==========" & vbNewLine)
                                Response.Redirect("itcdiscount.aspx?" & url)
                            Catch ex As Exception

                            End Try

                            ' DoNewLogin(profile.ComplimentaryPlan, NoOfStay, 0)

                        ElseIf profile.DiscountApplicable > 0 Then

                            Try
                                ObjLog.write2LogFile("Test" & ObjUserContext.GuestID, "GuestId" & ObjUserContext.GuestID & "Accid" & ObjUserContext.AccessID & "Discount login")
                            Catch ex As Exception

                            End Try

                            Try
                                ObjLog.write2LogFile("Welcome", "==== Redirect to itcdiscount Page ==========" & vbNewLine)
                                Response.Redirect("itcdiscount.aspx?" & url)
                            Catch ex As Exception

                            End Try

                            ' DoNewLogin(profile.DefaultDiscountPlan, profile.DefaultAutoConnectDaysForDiscount, 0)
                        End If
                        '=============================================================================

                    End If

                Else
                    ObjLog.write2LogFile("Welcome", "no Session Value===" & vbNewLine)
                End If
            End If
        Catch ex As Exception
            If Not ex.Message.ToLower().Contains("thread") Then
                ObjLog.writeExceptionLogFile("WelcomeEXP", ex)
            End If
        End Try

    End Sub

    'Display Guest related information in the upper tab
    Private Sub DisplayGuestInformation()

        welcomeNoteWrapper.Visible = True
        lblFirstDiscountInfo.Text = ""
        lblSecondDiscountInfo.Text = ""
        If Not profile Is Nothing Then

            If profile.DiscountApplicable > 0 Then
                'To calculate the Bill to be posted from the javascript
                hdDiscount.Value = profile.DiscountApplicable.ToString()

                Dim discountInfoList As List(Of String) = profile.DiscountInfoList
                Dim displayList As List(Of String) = profile.DisplayList

                Dim discountInfo As String = ""
                For i As Integer = 0 To discountInfoList.Count - 1
                    discountInfo = discountInfoList(i)
                    If discountInfo.ToLower().IndexOf("% discount") > 0 Then
                        Exit For
                    End If
                Next

                lblSecondDiscountInfo.Text = discountInfo

            End If

        End If

        Dim firstDiscountInfo As String = lblFirstDiscountInfo.Text.Trim()
        Dim secondDiscountInfo As String = lblSecondDiscountInfo.Text.Trim()

        If firstDiscountInfo = "" And secondDiscountInfo = "" Then
            welcomeNote.Visible = False
            welcomeNoteWrapper.Visible = False
        ElseIf firstDiscountInfo = "" Then
            lblFirstDiscountInfo.Visible = False
        ElseIf secondDiscountInfo = "" Then
            lblSecondDiscountInfo.Visible = False
        End If



        'If Not profile Is Nothing Then

        '    If profile.DiscountApplicable > 0 Then
        '        'To calculate the Bill to be posted from the javascript
        '        hdDiscount.Value = profile.DiscountApplicable.ToString()
        '    End If

        '    Dim discountInfoList As List(Of String) = profile.DiscountInfoList
        '    Dim displayList As List(Of String) = profile.DisplayList

        '    lblWelcomeAddress.Text = "Welcome !"
        '    lblWelcomeGuestName.Text = profile.GuestFullName

        '    If displayList.Count >= 2 Then
        '        lblFirstDiscountInfo.Text = displayList(0)
        '        lblSecondDiscountInfo.Text = displayList(1)
        '    ElseIf displayList.Count = 1 Then
        '        lblSecondDiscountInfo.Text = displayList(0)
        '    Else
        '        'Do nothing
        '    End If


        '    Dim firstDiscountInfo As String = lblFirstDiscountInfo.Text.Trim()
        '    Dim secondDiscountInfo As String = lblSecondDiscountInfo.Text.Trim()

        '    If firstDiscountInfo = "" And secondDiscountInfo = "" Then
        '        welcomeNote.Visible = False
        '    ElseIf firstDiscountInfo = "" Then
        '        lblFirstDiscountInfo.Visible = False
        '    ElseIf secondDiscountInfo = "" Then
        '        lblSecondDiscountInfo.Visible = False
        '    End If

        'End If

    End Sub

    'Populate the Plans to display
    Private Sub PopulatePlans()

        Dim planName, planamount As String
        Dim PlanValidity As Long

        Dim discountInfoList As List(Of String) = profile.DiscountInfoList

        Dim complimentaryPlan As Integer = profile.ComplimentaryPlan
        If complimentaryPlan > 0 And profile.IsComplimentaryConsiderable = False Then
            complimentaryPlan = 0
        End If

        Dim ObjPlan As New CPlan()
        Dim allPlans As DataTable = ObjPlan.getAllPlans(PLANTYPES.ROOM, PLANSTATUS.ACTIVEONLY).Tables(0)

        Dim dataRowRemoveList As New List(Of DataRow)()
        If profile.HidePlans = True Then

            For i = 0 To allPlans.Rows.Count - 1
                planId = allPlans.Rows(i).Item("PlanId")
                If planId <> complimentaryPlan Then
                    dataRowRemoveList.Add(allPlans.Rows(i))
                End If
            Next


            For Each row As DataRow In dataRowRemoveList
                allPlans.Rows.Remove(row)
            Next

        ElseIf complimentaryPlan > 0 Then

            Dim complimentaryAmount As Double = 0

            For i = 0 To allPlans.Rows.Count - 1
                planId = allPlans.Rows(i).Item("PlanId")
                planamount = allPlans.Rows(i).Item("PlanAmount")
                If planId = complimentaryPlan Then
                    complimentaryAmount = CDbl(planamount)
                    Exit For
                End If
            Next

            For i = 0 To allPlans.Rows.Count - 1
                planId = allPlans.Rows(i).Item("PlanId")
                planamount = allPlans.Rows(i).Item("PlanAmount")
                If CDbl(planamount) < complimentaryAmount Then
                    dataRowRemoveList.Add(allPlans.Rows(i))
                End If
            Next


            For Each row As DataRow In dataRowRemoveList
                allPlans.Rows.Remove(row)
            Next


            If allPlans.Rows.Count <= 1 Then
                If discountInfoList.Count >= 2 Then
                    lblSecondDiscountInfo.Text = discountInfoList(0)
                End If
            End If

        End If
        '======================================================================

        Dim planCount As Integer = allPlans.Rows.Count
        If planCount > 0 Then
            divstart.InnerHtml = "<table cellSpacing='0' cellPadding='0' width='100%' style='background-color:#F5F5F5;'><tr class='inner_table_bg' align='center'><td width='60%' valign='center' class='ver11_contents_bold1' align='left'><b>&nbsp;&nbsp;&nbsp;Plan Names</b></td><td valign='center' width='19%' class='ver11_contents_bold1' align='right'><b> Validity</b></td><td valign='center' width='21%' class='ver11_contents_bold1' align='right'><b> Rate (INR)</b></td></tr></table>"
        End If

        For i As Integer = 0 To planCount - 1
            planName = allPlans.Rows(i).Item("PlanName")
            planId = allPlans.Rows(i).Item("PlanId")
            planamount = allPlans.Rows(i).Item("PlanAmount")
            PlanValidity = allPlans.Rows(i).Item("PlanValidity")

            '========= Latest code snippet: June 12, 2012 =========================
            '========= By: Subhadeep Ray ==========================================
            '========= Reason: Incorporating discount on ITC FIAS =================
            Dim planAmountText As String = planamount
            If planId = complimentaryPlan Then
                planamount = 0
                planAmountText = "<span class='complimentary'>Complimentary</span>"
            End If
            '======================================================================

            Dim planSpan As String = String.Format("<span id='forPlan_{0}' accesskey='{1}' style='display:inline-block; width:1%'></span>", planId, planamount)
            Dim validityText As String = "24 Hrs"
            If PlanValidity = 7200 Then
                planSpan = String.Format("<span id='forPlan_{0}' accesskey='{1}' class='NoNight' style='display:inline-block; width:1%'></span>", planId, planamount)
                validityText = "2 Hrs"
            End If

            Dim listItemText As String = String.Format("{0}<span style='display:inline-block; height:22; width:57%; font-weight:bold; text-align:left;'>{1}</span><span style='display:inline-block; height:22; width:19%; font-weight:bold; text-align:right;'>{2}</span><span style='display:inline-block; height:22; width:20%; font-weight:bold; text-align:right;'>{3}</span>", planSpan, planName, validityText, planAmountText)

            rdoplan.Items.Add(New ListItem(listItemText, planId))
        Next

        '======================================================================

        '======================================================================

    End Sub

    '-- This method is used to get GuestNoOfDaysStay(for a particular guestid) from Guest table and
    '-- Populate the DrpDwnNoofDays combobox
    '-- If guestnoofdaystay more than 10 then only 10 values will be added in the combobox
    '-- first default value of the combobox is No Of Nights. It indicates 
    '-- guest has to select a particular value other that No Of Nights in order to
    '-- raise a plan
    Private Sub PopulateDays(ByVal GuestId As Long)
        'Dim ObjGuest As GuestService
        'Dim NoOfStay As Integer
        'Dim ObjList As ListItem
        'Dim ListName As String
        'Try
        '    ObjGuest = GuestService.getInstance
        '    NoOfStay = ObjGuest.GetGuestDaysStay(GuestId)
        '    DrpDwnNoofDays.Items.Clear()
        '    ObjList = New ListItem("No of nights", 0, True)
        '    DrpDwnNoofDays.Items.Add(ObjList)
        '    If NoOfStay > 0 Then
        '        If NoOfStay > 10 Then
        '            NoOfStay = 10
        '        End If

        '        For count = 1 To NoOfStay
        '            If count = 1 Then
        '                ListName = count & " Night"
        '            Else
        '                ListName = count & " Nights"
        '            End If

        '            ObjList = New ListItem(ListName, count)
        '            DrpDwnNoofDays.Items.Add(ObjList)
        '        Next
        '    End If
        'Catch ex As Exception

        'End Try
    End Sub

    '############## Jan 11,2011-Method Description Start ##########################
    '-- This method is used to diplay particular panel based on the enum value pased
    '-- If the enum value is LOGINPANEL, it will make planselection div visible true
    '-- and continue div visible fale
    '-- otherwise vise versa
    '############## Jan 11,2011-Method Description End ##########################
    Private Sub DisplayPanel(ByVal display As DISPLAY)
        If display = display.LOGINPANEL Then
            PlanSelectionDiv.Visible = True
            ContinueDiv.Visible = False
        Else
            ContinueDiv.Visible = True
            PlanSelectionDiv.Visible = False
        End If
    End Sub
    '############## Jan 11,2011-Method Description Start ##########################
    '-- This event will be called when user click Confirm Purchase button
    '-- This will validate user select a particular plan
    '-- Otherwise it will show error msg.
    '-- After the data validation it will build the usercontext from session value and pass it to Login Method
    '############## Jan 11,2011-Method Description End   ##########################
    Protected Sub HtmlBtnPurchase_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HtmlBtnPurchase.Click


        If RadioButton1.Checked = False And RadioButton2.Checked = False Then

            lblErrorMsg.Text = "Dear Guest, Please select internet renewal option."
            lblErrorMsg.Visible = True
            Return

        Else



            Dim seamless As String = "0"

            If rdoplan.SelectedValue = "" Then
                planId = -1
            Else
                planId = rdoplan.SelectedValue
            End If

            Dim noDays As String = "1"


            If RadioButton1.Checked = True Then
                seamless = "1"


            ElseIf RadioButton2.Checked = True Then
                seamless = "0"
            End If


            If planId <= 0 Then
                lblErrorMsg.Text = "You have not selected the Internet Rate Plan, Please select the Rate Plan."
                lblErrorMsg.Visible = True
                Return
            ElseIf noDays <= "0" Then
                lblErrorMsg.Text = "Please select the number of nights."
                lblErrorMsg.Visible = True
                Return
            Else
                DoNewLogin(planId, noDays, seamless)
            End If

        End If

    End Sub

    '############## Jan 11,2011-Method Description Start ##########################
    '-- This event will be called when user click Continue button
    '-- It will build the usercontext from session value and pass it to Login Method
    '############## Jan 11,2011-Method Description End   ##########################
    Protected Sub HtmlBtnContinue_Click(ByVal sender As Object, ByVal e As EventArgs) Handles HtmlBtnContinue.Click
        DoReLogin()
    End Sub

    Private Sub DoNewLogin(ByVal planId As Long, ByVal noDays As String, ByVal seamless As String)

        Dim ObjUserContext As UserContext
        Dim LogRoom As String
        Dim ObjLog As LoggerService = LoggerService.gtInstance

        ObjLog.write2LogFile("DoNewLogin", "============== Inside Plan Selection New Login ===========" & vbNewLine)

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




            Login(ObjUserContext)
        Else
            Dim commonFun As PMSCommonFun = PMSCommonFun.getInstance
            url = commonFun.BrowserQueryString(Request)
            Response.Redirect("IdentifyLogin.aspx?" & url)
        End If

        ObjLog.write2LogFile("DoReLogin", "============== Inside Plan Selection ReLogin ===========" & vbNewLine)

    End Sub


    '############## Jan 11,2011-Method Description Start ##########################
    '-- This method is used to start login process by calling AAA.AAA method
    '-- This method need to be called by passing a object of UserContext
    '-- If AAA.AAA returns ACCEXCEED that implies the particular guestdetail is trying to use more than 
    '-- 3 devices (configured by MaxDeviceCountForBillSharing in Web.config)
    '-- If it returns SUCCESS then it will redirect to user requested page that indicates login successfull and 
    '-- user can use internet
    '############## Jan 11,2011-Method Description End ############################
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

    Protected Sub RadioButton1_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton1.CheckedChanged
        Try
            pid.Visible = True
            RadioButton2.Checked = False
            lblErrorMsg.Text = ""
            PlanConfirm.Visible = True
        Catch ex As Exception

        End Try



    End Sub

    Protected Sub RadioButton2_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles RadioButton2.CheckedChanged
        Try
            pid.Visible = True
            lblErrorMsg.Text = ""
            RadioButton1.Checked = False
            PlanConfirm.Visible = True
        Catch ex As Exception

        End Try

    End Sub
End Class