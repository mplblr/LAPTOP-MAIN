Imports ITCCORE
Imports security


Partial Public Class itc_alert
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
        'ObjLog.write2LogFile("ITC alert", "====inside itc alert Page===" & vbNewLine)
        If Not IsPostBack() Then

            Try
                msg.InnerHtml = " <span style= 'display:inline-block; margin-bottom:10px;'>Dear Guest, </span> <br /> Would you like to renew your Internet plan for next 24 hours? "
            Catch ex As Exception

            End Try


            Try
                chkoutstring()
            Catch ex As Exception

            End Try


        End If



    End Sub
    Public Sub chkoutstring()
        Try

            Dim seamless As String = "0"

            Dim gid As Long = 0
            Dim aid As Long = 0

            Dim qs As String = Request.QueryString("encry")
            Dim mac1 As String = ""
            Dim commonFun As PMSCommonFun
            commonFun = PMSCommonFun.getInstance
            Try
                mac1 = commonFun.DecrptQueryString("MA", qs)
            Catch ex As Exception

            End Try

            'Try
            '    msg.InnerHtml = " <span style= 'display:inline-block; margin-bottom:10px;'>Dear Guest, </span> <br />Would you like to renew your Internet plan for next 24 hours? "
            'Catch ex As Exception

            'End Try

            Try
                msg.InnerHtml = " <span style= 'display:inline-block; margin-bottom:10px;'>Dear Guest, </span> <br /> The validity of your Internet access has expired.<br> Please login to resume Internet access till 0000 hours at 50% concessional charge ."
            Catch ex As Exception

            End Try


            Dim ObjUserContext As UserContext
            Dim ObjLog As LoggerService
            ObjLog = LoggerService.gtInstance
            Dim objSysConfig As New CSysConfig

            If Not Session("us") Is Nothing Then
                ObjUserContext = Session("us")

                gid = ObjUserContext.GuestID
                aid = ObjUserContext.AccessID

                profile = New GuestProfile(gid, aid)

                If profile.ComplimentaryPlan = 16 Or profile.ComplimentaryPlan = 15 Then
                    Try
                        msg.InnerHtml = " <span style= 'display:inline-block; margin-bottom:10px;'>Dear Guest, </span> <br />Would you like to renew your Internet plan for next 24 hours? "
                    Catch ex As Exception

                    End Try

                    Return
                End If
            Else


                Try
                    ' ObjLog.write2LogFile("chk_" & mac1, "no Session Value===" & vbNewLine)
                Catch ex As Exception

                End Try


                'Dim qs As String = ""

                Dim MAC As String = ""
                Dim ObjAccessCode As AccessCode
                Dim ObjUserCred As UserCredential
                Dim encrypt As New Datasealing





                qs = Request.QueryString("encry")
                commonFun = PMSCommonFun.getInstance

                Try
                    'Start Get the query string from URL

                    MAC = commonFun.DecrptQueryString("MA", qs)

                    ObjAccessCode = New AccessCode(MAC, "")
                    ObjUserCred = ObjAccessCode.CollectAccDetails(ITCCORE.ACCESSTYPE.ONLYMAC)
                    ' ObjLog.write2LogFile("chk_" & mac1, "no Session Value===" & vbNewLine & "Mac=" & MAC)

                    If ObjUserCred.usrId <> "" Then

                        gid = ObjUserCred.GuestID
                        aid = ObjUserCred.ACCID


                        profile = New GuestProfile(gid, aid)

                        If profile.ComplimentaryPlan = 16 Or profile.ComplimentaryPlan = 15 Then
                            Try
                                msg.InnerHtml = " <span style= 'display:inline-block; margin-bottom:10px;'>Dear Guest, </span> <br />Would you like to renew your Internet plan for next 24 hours? "
                            Catch ex As Exception

                            End Try

                            Return
                        End If






                    End If

                    'End Get the query string from URL
                Catch ex As Exception

                End Try





            End If


            Try
                Try
                    'ObjLog.write2LogFile("chk_" & mac1, "Guestid" & gid & "accid=" & aid)
                Catch ex As Exception

                End Try


                If gid > 0 Then

                    Dim bt As BillService
                    bt = BillService.getInstance
                    Dim exptime As String = ""
                    Dim et As String = ""
                    Dim bid As Long = 0
                    Dim et1 As String = ""

                    Try
                        exptime = bt.getExpifo(gid)
                    Catch ex As Exception

                    End Try

                    Try
                        'ObjLog.write2LogFile("chk_" & mac1, "Guest expcheckout date=" & exptime)

                    Catch ex As Exception

                    End Try


                    If exptime <> "" Then

                        Try
                            bid = bt.getbid(aid)
                            '   ObjLog.write2LogFile("chk_" & mac1, "Guest billno=" & bid)
                        Catch ex As Exception

                        End Try

                        If bid > 0 Then

                            Try
                                et = bt.getExpifo1(bid)
                                ' ObjLog.write2LogFile("chk_" & mac1, "Guest Expired plan Date =" & et)

                                If et <> "" Then
                                    et1 = bt.getExpifo2(bid)
                                    ' ObjLog.write2LogFile("chk_" & mac1, "Guest Expired plan time=" & et1)

                                    If Now.Hour < 15 Then


                                        Try
                                            Try
                                                ' msg.InnerHtml = " <span style= 'display:inline-block; margin-bottom:10px;'>Dear Guest, </span> <br /> The validity of the 24 hours Internet plan hereby ends but Internet access is made available for your convenience till 1500 hours. "
                                            Catch ex As Exception

                                            End Try
                                        Catch ex As Exception

                                        End Try




                                        Try
                                            ' ObjLog.write2LogFile("chk_" & mac1, "Plan expired before 3 PM  and current time less than 3 pm ")

                                        Catch ex As Exception

                                        End Try

                                        Dim duration As String = ""
                                        Dim dt As DateTime

                                        Try
                                            dt = New DateTime(Now.Year, Now.Month, Now.Day, 15, 0, 0)

                                            dt = dt.AddMinutes(1)

                                        Catch ex As Exception

                                        End Try




                                        Try
                                            duration = DateDiff(DateInterval.Second, Now, dt).ToString()

                                            ' ObjLog.write2LogFile("chk_" & mac1, "Duration=" & duration)
                                        Catch ex As Exception

                                        End Try


                                        Try
                                            bt.renewal(aid, gid, et1, duration, et, bid)
                                        Catch ex As Exception

                                        End Try

                                        Dim planid As Long = 2

                                        Try
                                            Try
                                                '  done = 1
                                            Catch ex As Exception

                                            End Try
                                            planid = ExtendedUtil.GetLastPlanid(gid, aid)
                                            DoNewLoginextend(planid, "1", seamless, bid)
                                            Return
                                        Catch ex As Exception

                                        End Try





                                    End If




                                    Try
                                        Dim str() As String
                                        str = et1.Split(":")
                                        ' ObjLog.write2LogFile("chk_" & mac1, "Hour=" & str(0) & "minutes=" & str(1))

                                        If str(0) < 15 Then


                                            'ObjLog.write2LogFile("chk_" & mac1, "Plan expired before 3 PM" & "current time Hour" & Now.Hour & "minutes" & Now.Minute)


                                            Try
                                                If Now.Hour < 15 Then


                                                    Try
                                                        Try
                                                            ' msg.InnerHtml = " <span style= 'display:inline-block; margin-bottom:10px;'>Dear Guest, </span> <br /> The validity of the 24 hours Internet plan hereby ends but Internet access is made available for your convenience till 1500 hours. "
                                                        Catch ex As Exception

                                                        End Try
                                                    Catch ex As Exception

                                                    End Try




                                                    Try
                                                        'ObjLog.write2LogFile("chk_" & mac1, "Plan expired before 3 PM  and current time less than 3 pm ")

                                                    Catch ex As Exception

                                                    End Try

                                                    Dim duration As String = ""
                                                    Dim dt As DateTime

                                                    Try
                                                        dt = New DateTime(Now.Year, Now.Month, Now.Day, 15, 0, 0)

                                                        dt = dt.AddMinutes(1)

                                                    Catch ex As Exception

                                                    End Try




                                                    Try
                                                        duration = DateDiff(DateInterval.Second, Now, dt).ToString()

                                                        'ObjLog.write2LogFile("chk_" & mac1, "Duration=" & duration)
                                                    Catch ex As Exception

                                                    End Try


                                                    Try
                                                        bt.renewal(aid, gid, et1, duration, et, bid)
                                                    Catch ex As Exception

                                                    End Try

                                                    Dim planid As Long = 2

                                                    Try
                                                        Try
                                                            '  done = 1
                                                        Catch ex As Exception

                                                        End Try
                                                        planid = ExtendedUtil.GetLastPlanid(gid, aid)
                                                        DoNewLoginextend(planid, "1", seamless, bid)
                                                        '  Return done
                                                    Catch ex As Exception

                                                    End Try






                                                ElseIf Now.Hour >= 15 Then


                                                    Try
                                                        Try
                                                            msg.InnerHtml = " <span style= 'display:inline-block; margin-bottom:10px;'>Dear Guest, </span> <br /> The validity of your Internet access has expired.<br> Please login to resume Internet access till 0000 hours at 50% concessional charge ."
                                                        Catch ex As Exception

                                                        End Try
                                                    Catch ex As Exception

                                                    End Try

                                                    Try
                                                        'ObjLog.write2LogFile("chk_" & mac1, "Plan expired after 3 PM" & "guest logged after 3 pm")

                                                    Catch ex As Exception

                                                    End Try



                                                    'ObjLog.write2LogFile("chk_" & mac1, "Plan expired before 12 PM" & "guest logged before 12 pm")
                                                End If
                                            Catch ex As Exception

                                            End Try

                                        Else
                                            Try
                                                msg.InnerHtml = " <span style= 'display:inline-block; margin-bottom:10px;'>Dear Guest, </span> <br /> The validity of your Internet access has expired.<br> Please login to resume Internet access till 0000 hours at 50% concessional charge ."
                                            Catch ex As Exception

                                            End Try


                                        End If




                                    Catch ex As Exception

                                    End Try





                                End If



                            Catch ex As Exception

                            End Try

                        End If



                    End If




                End If



            Catch ex As Exception

            End Try


        Catch ex As Exception

        End Try




    End Sub




    Public Function extend() As Integer
        Dim done As Integer = 0


        Try

            Dim seamless As String = "0"

            Dim gid As Long = 0
            Dim aid As Long = 0

            Dim qs As String = Request.QueryString("encry")
            Dim mac1 As String = ""
            Dim commonFun As PMSCommonFun
            commonFun = PMSCommonFun.getInstance
            Try
                mac1 = commonFun.DecrptQueryString("MA", qs)
            Catch ex As Exception

            End Try




            Dim ObjUserContext As UserContext
            Dim ObjLog As LoggerService
            ObjLog = LoggerService.gtInstance
            Dim objSysConfig As New CSysConfig

            If Not Session("us") Is Nothing Then
                ObjUserContext = Session("us")

                gid = ObjUserContext.GuestID
                aid = ObjUserContext.AccessID


                Try
                    profile = New GuestProfile(gid, aid)

                    If profile.ComplimentaryPlan = 16 Or profile.ComplimentaryPlan = 15 Then
                        Return 0
                    End If
                Catch ex As Exception

                End Try


            Else


                Try
                    'ObjLog.write2LogFile("chk_" & mac1, "no Session Value===" & vbNewLine)
                Catch ex As Exception

                End Try


                'Dim qs As String = ""

                Dim MAC As String = ""
                Dim ObjAccessCode As AccessCode
                Dim ObjUserCred As UserCredential
                Dim encrypt As New Datasealing





                qs = Request.QueryString("encry")
                commonFun = PMSCommonFun.getInstance

                Try
                    'Start Get the query string from URL

                    MAC = commonFun.DecrptQueryString("MA", qs)

                    ObjAccessCode = New AccessCode(MAC, "")
                    ObjUserCred = ObjAccessCode.CollectAccDetails(ITCCORE.ACCESSTYPE.ONLYMAC)
                    ' ObjLog.write2LogFile("chk_" & mac1, "no Session Value===" & vbNewLine & "Mac=" & MAC)

                    If ObjUserCred.usrId <> "" Then

                        gid = ObjUserCred.GuestID
                        aid = ObjUserCred.ACCID


                        profile = New GuestProfile(gid, aid)

                        If profile.ComplimentaryPlan = 16 Or profile.ComplimentaryPlan = 15 Then
                            Return 0
                        End If






                    End If

                    'End Get the query string from URL
                Catch ex As Exception

                End Try





            End If


            Try
                Try
                    ' ObjLog.write2LogFile("chk_" & mac1, "Guestid" & gid & "accid=" & aid)
                Catch ex As Exception

                End Try


                If gid > 0 Then

                    Dim bt As BillService
                    bt = BillService.getInstance
                    Dim exptime As String = ""
                    Dim et As String = ""
                    Dim bid As Long = 0
                    Dim et1 As String = ""

                    Try
                        exptime = bt.getExpifo(gid)
                    Catch ex As Exception

                    End Try

                    Try
                        '  ObjLog.write2LogFile("chk_" & mac1, "Guest expcheckout date=" & exptime)

                    Catch ex As Exception

                    End Try


                    If exptime <> "" Then

                        Try
                            bid = bt.getbid(aid)
                            ' ObjLog.write2LogFile("chk_" & mac1, "Guest billno=" & bid)
                        Catch ex As Exception

                        End Try

                        If bid > 0 Then

                            Try
                                et = bt.getExpifo1(bid)
                                ' ObjLog.write2LogFile("chk_" & mac1, "Guest Expired plan Date =" & et)

                                If et <> "" Then
                                    et1 = bt.getExpifo2(bid)
                                    '  ObjLog.write2LogFile("chk_" & mac1, "Guest Expired plan time=" & et1)


                                    Try
                                        Dim str() As String
                                        str = et1.Split(":")
                                        '  ObjLog.write2LogFile("chk_" & mac1, "Hour=" & str(0) & "minutes=" & str(1))

                                        If str(0) < 15 Then

                                            '  ObjLog.write2LogFile("chk_" & mac1, "Plan expired before 3 PM" & "current time Hour" & Now.Hour & "minutes" & Now.Minute)


                                            Try
                                                If Now.Hour < 15 Then


                                                    Try



                                                        Dim duration As String = ""
                                                        Dim dt As DateTime

                                                        Try
                                                            dt = New DateTime(Now.Year, Now.Month, Now.Day, 15, 0, 0)
                                                            dt = dt.AddMinutes(1)
                                                        Catch ex As Exception

                                                        End Try




                                                        Try
                                                            duration = DateDiff(DateInterval.Second, Now, dt).ToString()

                                                            'ObjLog.write2LogFile("chk_" & mac1, "Duration=" & duration)
                                                        Catch ex As Exception

                                                        End Try


                                                        Try
                                                            bt.renewal(aid, gid, et1, duration, et, bid)
                                                        Catch ex As Exception

                                                        End Try

                                                        Dim planid As Long = 2

                                                        Try

                                                            Try
                                                                done = 1
                                                            Catch ex As Exception

                                                            End Try
                                                            planid = ExtendedUtil.GetLastPlanid(gid, aid)
                                                            DoNewLoginextend(planid, "1", seamless, bid)
                                                            Return done
                                                        Catch ex As Exception

                                                        End Try





                                                    Catch ex As Exception

                                                    End Try

                                                    Try
                                                        'ObjLog.write2LogFile("chk_" & mac1, "Plan expired before 3 PM  and current time less than 3 pm ")

                                                    Catch ex As Exception

                                                    End Try



                                                ElseIf Now.Hour >= 15 Then

                                                    Try



                                                        Dim duration As String = ""
                                                        Dim dt As DateTime

                                                        Try
                                                            dt = New DateTime(Now.Year, Now.Month, Now.Day, 23, 59, 59)

                                                            dt = dt.AddMinutes(1)
                                                        Catch ex As Exception

                                                        End Try




                                                        Try
                                                            duration = DateDiff(DateInterval.Second, Now, dt).ToString()

                                                            ' ObjLog.write2LogFile("chkA_" & mac1, "Duration=" & duration)
                                                        Catch ex As Exception

                                                        End Try


                                                        Try
                                                            bt.renewalpay(aid, gid, et1, duration, et, bid)
                                                        Catch ex As Exception

                                                        End Try

                                                        Dim planid As Long = 2

                                                        Try

                                                            Try
                                                                done = 1
                                                            Catch ex As Exception

                                                            End Try
                                                            planid = ExtendedUtil.GetLastPlanid(gid, aid)
                                                            DoNewLoginextend2(planid, "1", seamless, bid)
                                                            Return done
                                                        Catch ex As Exception

                                                        End Try





                                                    Catch ex As Exception

                                                    End Try

                                                    Try
                                                        ' ObjLog.write2LogFile("chk_" & mac1, "Plan expired before 3 PM  and current time less than 3 pm ")

                                                    Catch ex As Exception

                                                    End Try
                                                    'ObjLog.write2LogFile("chk_" & mac1, "Plan expired before 12 PM" & "guest logged before 12 pm")
                                                End If
                                            Catch ex As Exception

                                            End Try

                                        Else
                                            Try



                                                Dim duration As String = ""
                                                Dim dt As DateTime

                                                Try
                                                    dt = New DateTime(Now.Year, Now.Month, Now.Day, 23, 59, 59)

                                                    dt = dt.AddMinutes(1)
                                                Catch ex As Exception

                                                End Try




                                                Try
                                                    duration = DateDiff(DateInterval.Second, Now, dt).ToString()

                                                    ' ObjLog.write2LogFile("chkA_" & mac1, "Duration=" & duration)
                                                Catch ex As Exception

                                                End Try


                                                Try
                                                    bt.renewalpay(aid, gid, et1, duration, et, bid)
                                                Catch ex As Exception

                                                End Try

                                                Dim planid As Long = 2

                                                Try

                                                    Try
                                                        done = 1
                                                    Catch ex As Exception

                                                    End Try
                                                    planid = ExtendedUtil.GetLastPlanid(gid, aid)
                                                    DoNewLoginextend2(planid, "1", seamless, bid)
                                                    Return done
                                                Catch ex As Exception

                                                End Try





                                            Catch ex As Exception

                                            End Try

                                            Try
                                                'ObjLog.write2LogFile("chk_" & mac1, "Plan expired before 3 PM  and current time less than 3 pm ")

                                            Catch ex As Exception

                                            End Try




                                        End If




                                    Catch ex As Exception

                                    End Try





                                End If



                            Catch ex As Exception

                            End Try

                        End If



                    End If




                End If



            Catch ex As Exception

            End Try


        Catch ex As Exception

        End Try

        Return done


    End Function






    Protected Sub yes_Click(ByVal sender As Object, ByVal e As EventArgs) Handles yes.Click
        Try

            Dim seamless As String = "0"

            Try
                If extend() = 1 Then

                    Exit Sub
                    Return
                End If
            Catch ex As Exception

            End Try



            Dim ObjUserContext As UserContext
            Dim ObjLog As LoggerService
            ObjLog = LoggerService.gtInstance
            Dim objSysConfig As New CSysConfig

            If Not Session("us") Is Nothing Then
                ObjUserContext = Session("us")
                profile = New GuestProfile(ObjUserContext.GuestID, ObjUserContext.AccessID)

                If profile.ComplimentaryPlan = 15 Or profile.ComplimentaryPlan = 16 Then

                    Try
                        Dim bt As BillService
                        bt = BillService.getInstance

                        seamless = bt.getseam(ObjUserContext.AccessID)
                    Catch ex As Exception

                    End Try


                    Try
                        'ObjLog.write2LogFile("Discount" & ObjUserContext.GuestID, "GuestId" & ObjUserContext.GuestID & "Accid" & ObjUserContext.AccessID & "Complimentary login")
                    Catch ex As Exception

                    End Try

                    Dim ObjGuest As GuestService = GuestService.getInstance


                    Try
                        ' ObjLog.write2LogFile("discount", "==== Redirect to itcdiscout Page ==========" & vbNewLine)
                        DoNewLogin(profile.ComplimentaryPlan, "1", seamless)
                    Catch ex As Exception

                    End Try

                    ' 

                ElseIf profile.ComplimentaryPlan > 0 Then


                    Try
                        Dim bt As BillService
                        bt = BillService.getInstance

                        seamless = bt.getseam(ObjUserContext.AccessID)
                    Catch ex As Exception

                    End Try



                    Try

                        'ObjLog.write2LogFile("discount" & ObjUserContext.GuestID, "GuestId" & ObjUserContext.GuestID & "Accid" & ObjUserContext.AccessID & "Discount login")
                    Catch ex As Exception

                    End Try

                    DoNewLogin(profile.ComplimentaryPlan, 1, seamless)


                Else

                    Dim planid As Long = 2

                    Try
                        Dim bt As BillService
                        bt = BillService.getInstance

                        seamless = bt.getseam(ObjUserContext.AccessID)
                    Catch ex As Exception

                    End Try



                    Try
                        planid = ExtendedUtil.GetLastPlanid(ObjUserContext.GuestID.ToString(), ObjUserContext.AccessID.ToString())
                        DoNewLogin(planid, "1", seamless)

                    Catch ex As Exception

                    End Try



                    ' 
                End If



            Else
                '  ObjLog.write2LogFile("discount", "no Session Value===" & vbNewLine)

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
                    'ObjLog.write2LogFile("discount", "no Session Value===" & vbNewLine & "Mac=" & MAC)

                    If ObjUserCred.usrId <> "" Then

                        guestid = ObjUserCred.GuestID
                        accid = ObjUserCred.ACCID
                        Try
                            Dim bt As BillService
                            bt = BillService.getInstance

                            seamless = bt.getseam(accid)
                        Catch ex As Exception

                        End Try


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

                        ElseIf profile.ComplimentaryPlan > 0 Then



                            DoNewLogin(profile.ComplimentaryPlan, 1, seamless)

                        Else
                            Dim planid As Long = 2

                            Try
                                planid = ExtendedUtil.GetLastPlanid(guestid.ToString(), accid.ToString())
                                DoNewLogin(planid, "1", seamless)

                            Catch ex As Exception

                            End Try

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


    Private Sub DoNewLoginextend(ByVal planId As Long, ByVal noDays As String, ByVal seamless As String, ByVal bid As String)

        Dim ObjUserContext As UserContext
        Dim LogRoom As String
        Dim ObjLog As LoggerService = LoggerService.gtInstance

        ObjLog.write2LogFile("DoNewLogin", "============== Inside renew New Login ===========" & vbNewLine)

        If Not Session("us") Is Nothing Then
            ObjUserContext = Session("us")
            LogRoom = ObjUserContext.roomNo

            ObjUserContext.selectedPlanId = planId
            ObjUserContext.item("noofdays") = noDays

            ObjUserContext.item("bid") = bid

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
                ObjLog.write2LogFile(LogRoom, " ====== REnew till 3PM ====" & _
                                vbCrLf & " PlanId:" & planId & _
                                 vbCrLf & " PlanId:" & planId & _
                                  vbCrLf & " Seamless Connection:" & ObjUserContext.item("seam") & _
                                vbCrLf & " TotalPlanDays:" & ObjUserContext.item("Totalnoofdays") & _
                                vbCrLf & " AccessType:" & ObjUserContext.item("accesstype") & _
                                vbCrLf & " LoginType:" & ObjUserContext.item("logintype") & _
                                vbCrLf & " MAC:" & ObjUserContext.machineId)
            Catch ex As Exception

            End Try


            LoginExtend(ObjUserContext)

        Else
            Dim commonFun As PMSCommonFun = PMSCommonFun.getInstance
            url = commonFun.BrowserQueryString(Request)
            Response.Redirect("IdentifyLogin.aspx?" & url)
        End If

        ObjLog.write2LogFile("renew", "============== Inside Plan Selection New Login ===========" & vbNewLine)

    End Sub


    Private Sub DoNewLoginextend2(ByVal planId As Long, ByVal noDays As String, ByVal seamless As String, ByVal bid As String)

        Dim ObjUserContext As UserContext
        Dim LogRoom As String
        Dim ObjLog As LoggerService = LoggerService.gtInstance

        ObjLog.write2LogFile("DoNewLogin", "============== Inside renew New Login ===========" & vbNewLine)

        If Not Session("us") Is Nothing Then
            ObjUserContext = Session("us")
            LogRoom = ObjUserContext.roomNo

            ObjUserContext.selectedPlanId = planId
            ObjUserContext.item("noofdays") = noDays

            ObjUserContext.item("bid") = bid

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
                ObjLog.write2LogFile(LogRoom, " ====== REnew till 3PM ====" & _
                                vbCrLf & " PlanId:" & planId & _
                                 vbCrLf & " PlanId:" & planId & _
                                  vbCrLf & " Seamless Connection:" & ObjUserContext.item("seam") & _
                                vbCrLf & " TotalPlanDays:" & ObjUserContext.item("Totalnoofdays") & _
                                vbCrLf & " AccessType:" & ObjUserContext.item("accesstype") & _
                                vbCrLf & " LoginType:" & ObjUserContext.item("logintype") & _
                                vbCrLf & " MAC:" & ObjUserContext.machineId)
            Catch ex As Exception

            End Try


            LoginExtend2(ObjUserContext)

        Else
            Dim commonFun As PMSCommonFun = PMSCommonFun.getInstance
            url = commonFun.BrowserQueryString(Request)
            Response.Redirect("IdentifyLogin.aspx?" & url)
        End If

        ObjLog.write2LogFile("renew", "============== Inside Plan Selection New Login ===========" & vbNewLine)

    End Sub

    Private Sub LoginExtend(ByRef UserCred As UserContext)
        Dim AAA As AAAService
        Dim ObjLog As LoggerService
        Dim OutPut As String = ""
        Dim commonFun As PMSCommonFun
        Try
            AAA = AAAService.getInstance
            commonFun = PMSCommonFun.getInstance
            url = commonFun.BrowserQueryString(Request)
            UserCred.item("radiusloginid") = Application("radiusloginid")
            OutPut = AAA.AAAA(UserCred)
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

    Private Sub LoginExtend2(ByRef UserCred As UserContext)
        Dim AAA As AAAService
        Dim ObjLog As LoggerService
        Dim OutPut As String = ""
        Dim commonFun As PMSCommonFun
        Try
            AAA = AAAService.getInstance
            commonFun = PMSCommonFun.getInstance
            url = commonFun.BrowserQueryString(Request)
            UserCred.item("radiusloginid") = Application("radiusloginid")
            OutPut = AAA.AAAAA(UserCred)
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

    Private Sub DoNewLogin(ByVal planId As Long, ByVal noDays As String, ByVal seamless As String)

        Dim ObjUserContext As UserContext
        Dim LogRoom As String
        Dim ObjLog As LoggerService = LoggerService.gtInstance

        ObjLog.write2LogFile("DoNewLogin", "============== Inside Discount New Login ===========" & vbNewLine)

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



    Protected Sub No_Click(ByVal sender As Object, ByVal e As EventArgs) Handles No.Click
        Dim qs As String = ""

        Try
            qs = Request.QueryString("encry")
        Catch ex As Exception

        End Try


        Dim mac1 As String = ""

        Try
            Dim commonFun As PMSCommonFun
            commonFun = PMSCommonFun.getInstance
            url = commonFun.BrowserQueryString(Request)

            Try
                mac1 = commonFun.DecrptQueryString("MA", qs)
                Dim objlog As LoggerService
                objlog = LoggerService.gtInstance
                ' objlog.write2LogFile("alert_" & mac1, "No button clicked")
            Catch ex As Exception

            End Try


            Response.Redirect("Instruction.aspx?" & url)

        Catch ex As Exception

        End Try
    End Sub
End Class