Imports System.Data.Common
Imports ITCCORE.Microsense.CodeBase

'############## Jan 11,2011-Class Description Start ############################
'-- This class is used for handling all Bill related issues
'############## Jan 11,2011-Class Description End   ############################
Public Class BillService
    Private Shared gtBillServiceInst As BillService

    Private Sub New()
        'Nothing
    End Sub

    Public Shared Function getInstance() As BillService
        If gtBillServiceInst Is Nothing Then gtBillServiceInst = New BillService
        Return gtBillServiceInst
    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '-- This Method is used insert one record in bill table
    '-- It will do following tasks:
    '-- It collects all the necessary details(billtype,BillAccessType,grCId,planId,MACAddress,ACCID from
    '-- userContext
    '-- It will calculate the posted amount by calling CalSplGuestAmount
    '-- It will insert all the above collected record in Bill table
    '-- On successful insertion of bill table it will return the newly created billid
    '############## Jan 11,2011-Method Description End   ############################
    'Public Function RaiseBill2Days(ByVal userContext As UserContext, Optional ByVal accessStatus As Boolean = False) As Long
    '    'In USE 6 AUG 2010*************************
    '    'Variable Declaration Start ================================================
    '    Dim billdate As DateTime = Now
    '    Dim SQL_query As String
    '    Dim planId As Integer
    '    Dim grCId, billId, ACCID As Long
    '    Dim planAmount, planPostAmount, MACAddress As String
    '    Dim BillAccessType As String = ""
    '    Dim billtype, BillRaiseType As Integer
    '    Dim objDbase As DbaseServiceOLEDB
    '    Dim no_ofdays As Integer
    '    Dim objPlan As New CPlan
    '    Dim RefResultset As DataSet
    '    Dim RefDataTable As DataTable
    '    Dim ObjLog As LoggerService
    '    'Variable Declaration End ================================================
    '    Try
    '        'Bill Process start =============================
    '        billtype = PMSBill.ROOM
    '        BillAccessType = userContext.item("accesstype")
    '        grCId = userContext.GuestID
    '        planId = userContext.selectedPlanId
    '        MACAddress = userContext.machineId
    '        ACCID = userContext.AccessID
    '        BillRaiseType = 0

    '        objPlan.getPlaninfo(planId) ' Get Selected Plan Information ==================

    '        If userContext.item("noofdays") Is Nothing Then
    '            no_ofdays = 1
    '        Else
    '            no_ofdays = CInt(userContext.item("noofdays"))
    '        End If
    '        ' Process For Renew / New Login ==================================
    '        planAmount = objPlan.planAmount

    '        planPostAmount = CDbl(planAmount) * no_ofdays

    '        '========= Latest code snippet: June 11, 2012 =========================
    '        '========= By: Subhadeep Ray ==========================================
    '        '========= Reason: Incorporating discount on ITC FIAS =================
    '        '------------------------------------------------------------------------------------------
    '        Dim profile As New GuestProfile(userContext.GuestID, userContext.AccessID)

    '        'Check if guest need to auto-connect a plan with an auto-connect discount
    '        'Else if the guest selected plan is matched with the complementary plan assigned with the
    '        'current guest profile and for the current coupon the complimentary plan is application 
    '        'then guest will enjoy internet free.
    '        Dim actionOnBill As String = ""
    '        Dim appliedDiscount As Double = 0

    '        Dim considerAutoConnect As Boolean = False
    '        If Not profile.ExceptionalAutoConnect Is Nothing Then
    '            If profile.ExceptionalAutoConnect.AutoConnectPlan = planId Then
    '                considerAutoConnect = True
    '            End If
    '        End If

    '        If considerAutoConnect = True Then
    '            appliedDiscount = profile.ExceptionalAutoConnect.AutoConnectDiscount
    '            planPostAmount = Math.Floor(CDbl(planAmount) * ((100.0 - appliedDiscount) / 100)) * no_ofdays
    '            actionOnBill = String.Format("{0}% {1} discount for {2}", appliedDiscount, "Auto-connect", objPlan.planName)

    '        ElseIf (profile.ComplimentaryPlan = planId) And (profile.IsComplimentaryConsiderable = True) Then
    '            appliedDiscount = 100
    '            planPostAmount = 0
    '            actionOnBill = String.Format("Complimentary access on {0} for {1}", profile.ComplimentaryTypeApplicable, objPlan.planName)
    '        Else
    '            appliedDiscount = profile.DiscountApplicable
    '            planPostAmount = Math.Floor(CDbl(planAmount) * ((100.0 - appliedDiscount) / 100)) * no_ofdays
    '            actionOnBill = String.Format("{0}% {1} discount for {2}", appliedDiscount, profile.DiscountTypeApplicable, objPlan.planName)
    '        End If

    '        '------------------------------------------------------------------------------------------
    '        '======================================================================

    '        userContext.item("itemamount") = planPostAmount
    '        Dim PlanPostAmountInt As Double
    '        PlanPostAmountInt = Convert.ToDouble(planPostAmount)

    '        '========= Modification: June 14, 2012 ================================
    '        '========= By: Subhadeep Ray ==========================================
    '        '========= Reason: Allowing zero bill =================================
    '        SQL_query = "Insert Into Bill (BillGrCId, BillPlanId, BillAmount, BillType, BillTime, BillMAC, BillPostedAmount,BillAccessType, BilltmpNoofDays,BillACCId,BillRaiseType) VALUES (" & grCId & ", " & planId & ", " & planAmount & ", " & billtype & ", '" & billdate & "', '" & MACAddress & "', " & planPostAmount & "," & BillAccessType & ", " & no_ofdays & "," & ACCID & "," & BillRaiseType & ")" & vbNewLine & _
    '                    "Select SCOPE_IDENTITY() As BillID"
    '        '======================================================================

    '        objDbase = DbaseServiceOLEDB.getInstance
    '        RefResultset = objDbase.DsWithoutUpdate(SQL_query)
    '        RefDataTable = RefResultset.Tables(0)
    '        If RefDataTable.Rows.Count > 0 Then
    '            billId = Long.Parse(RefDataTable.Rows(0).Item("BillID").ToString())

    '            '========= Latest code snippet: June 11, 2012 =========================
    '            '========= By: Subhadeep Ray ==========================================
    '            '========= Reason: Incorporating discount on ITC FIAS =================
    '            'After bill posting insert the bill and guest profile in the 
    '            'database table "BillGuestProfileMappings"

    '            GuestProfile.LogBillWithGuestProfile(billId, planId, planAmount, profile, appliedDiscount, actionOnBill)
    '            '----------------------------------------------------------------
    '            '======================================================================

    '        Else
    '            billId = -1
    '        End If
    '        'Bill Process End ===============================
    '    Catch ex As Exception
    '        ObjLog = LoggerService.gtInstance
    '        ObjLog.writeExceptionLogFile("BILLEXP", ex)
    '        billId = -1
    '    Finally
    '        objPlan = Nothing
    '        RefResultset = Nothing
    '        RefDataTable = Nothing
    '        objDbase = Nothing
    '    End Try
    '    Return billId

    'End Function

    Public Function RaiseBill2Days(ByVal userContext As UserContext, Optional ByVal accessStatus As Boolean = False) As Long
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim billdate As DateTime = Now
        Dim planId As Integer
        Dim grCId, billId, ACCID As Long
        Dim planAmount, planPostAmount, MACAddress As String
        Dim BillAccessType As String = ""
        Dim billtype, BillRaiseType As Integer
        Dim objDbase As DbaseServiceOLEDB
        Dim no_ofdays As Integer
        Dim objPlan As New CPlan
        Dim RefDataTable As DataTable
        Dim ObjLog As LoggerService
        'Variable Declaration End ================================================
        Try
            'Bill Process start =============================
            billtype = PMSBill.ROOM
            BillAccessType = userContext.item("accesstype")
            grCId = userContext.GuestID
            planId = userContext.selectedPlanId
            MACAddress = userContext.machineId
            ACCID = userContext.AccessID
            BillRaiseType = 0

            objPlan.getPlaninfo(planId) ' Get Selected Plan Information ==================

            If userContext.item("noofdays") Is Nothing Then
                no_ofdays = 1
            Else
                no_ofdays = CInt(userContext.item("noofdays"))
            End If

            no_ofdays = 1

            ' Process For Renew / New Login ==================================
            planAmount = objPlan.planAmount

            planPostAmount = CDbl(planAmount) * no_ofdays

            '========= Latest code snippet: June 11, 2012 =========================
            '========= By: Subhadeep Ray ==========================================
            '========= Reason: Incorporating discount on ITC FIAS =================
            '------------------------------------------------------------------------------------------
            Dim profile As New GuestProfile(userContext.GuestID, userContext.AccessID)

            'Check if guest need to auto-connect a plan with an auto-connect discount
            'Else if the guest selected plan is matched with the complementary plan assigned with the
            'current guest profile and for the current coupon the complimentary plan is application 
            'then guest will enjoy internet free.
            Dim actionOnBill As String = ""
            Dim appliedDiscount As Double = 0

            Dim considerAutoConnect As Boolean = False
            If Not profile.ExceptionalAutoConnect Is Nothing Then
                If profile.ExceptionalAutoConnect.AutoConnectPlan = planId Then
                    considerAutoConnect = True
                End If
            End If

           
            If planId = 15 Or planId = 16 Then
                appliedDiscount = 100
                planPostAmount = 0
                actionOnBill = String.Format("Complimentary access on {0} for {1}", profile.ComplimentaryTypeApplicable, objPlan.planName)
            ElseIf planId = 17 Then
                appliedDiscount = profile.DiscountApplicable
                planPostAmount = Math.Floor(CDbl(planAmount) * ((100.0 - appliedDiscount) / 100)) * no_ofdays
                actionOnBill = String.Format("{0}% {1} discount for {2}", appliedDiscount, profile.DiscountTypeApplicable, objPlan.planName)
            End If

            '------------------------------------------------------------------------------------------
            '======================================================================

            Try
                ObjLog = LoggerService.gtInstance
                ObjLog.write2LogFile("billtest", "Guestid=" & grCId & "amt" & planAmount & "amt post" & planPostAmount)
            Catch ex As Exception

            End Try



            userContext.item("itemamount") = planPostAmount
            Dim PlanPostAmountInt As Double
            PlanPostAmountInt = Convert.ToDouble(planPostAmount)

            '========= Modification: June 14, 2012 ================================
            '========= By: Subhadeep Ray ==========================================
            '========= Reason: Preventing duplicate bill =================================
            planAmount = 299

            RefDataTable = RaiseDatabaseBill(grCId, planId, planAmount, billtype, billdate, MACAddress, planPostAmount, BillAccessType, no_ofdays, ACCID, BillRaiseType)
            '======================================================================

            If RefDataTable Is Nothing Then
                billId = -1
                Return billId
            End If

            If RefDataTable.Rows.Count > 0 Then
                billId = Long.Parse(RefDataTable.Rows(0).Item("BillID").ToString())



                Try
                    Try
                        ObjLog = LoggerService.gtInstance
                        ObjLog.write2LogFile("billtest", "Guest no of days stay" & userContext.item("Totalnoofdays") & "Seamless" & userContext.item("seam"))





                    Catch ex As Exception

                    End Try

                    Try
                        If userContext.item("seam") = "1" Then
                            Dim guestnoofdaysstay As Integer = 1
                            guestnoofdaysstay = CInt(userContext.item("Totalnoofdays"))

                            Try
                                setseam(ACCID, 1)
                            Catch ex As Exception

                            End Try

                            BillTrack(grCId, billId, planPostAmount, ACCID, guestnoofdaysstay)

                        Else
                            Try
                                setseam(ACCID, 0)
                            Catch ex As Exception

                            End Try
                        End If
                    Catch ex As Exception

                    End Try

                Catch ex As Exception

                End Try









                Dim newBill As String = RefDataTable.Rows(0).Item("NewBill").ToString().Trim()

                '========= Latest code snippet: June 11, 2012 =========================
                '========= By: Subhadeep Ray ==========================================
                '========= Reason: Incorporating discount on ITC FIAS =================
                'After bill posting insert the bill and guest profile in the 
                'database table "BillGuestProfileMappings"

                If newBill = "1" Then
                    GuestProfile.LogBillWithGuestProfile(billId, planId, planAmount, profile, appliedDiscount, actionOnBill)
                End If
                '----------------------------------------------------------------
                '======================================================================

            Else
                billId = -1
            End If
            'Bill Process End ===============================
        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("BILLEXP", ex)
            billId = -1
        Finally
            objPlan = Nothing
            RefDataTable = Nothing
            objDbase = Nothing
        End Try
        Return billId

    End Function

    Public Function RaiseBill2Days2(ByVal userContext As UserContext, Optional ByVal accessStatus As Boolean = False) As Long
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim billdate As DateTime = Now

        billdate = New DateTime(Now.Year, Now.Month, Now.Day)

        Dim planId As Integer
        Dim grCId, billId, ACCID As Long
        Dim planAmount, planPostAmount, MACAddress As String
        Dim BillAccessType As String = ""
        Dim billtype, BillRaiseType As Integer
        Dim objDbase As DbaseServiceOLEDB
        Dim no_ofdays As Integer
        Dim objPlan As New CPlan
        Dim RefDataTable As DataTable
        Dim ObjLog As LoggerService
        'Variable Declaration End ================================================
        Try
            'Bill Process start =============================
            billtype = PMSBill.ROOM
            BillAccessType = userContext.item("accesstype")
            grCId = userContext.GuestID
            planId = userContext.selectedPlanId
            MACAddress = userContext.machineId
            ACCID = userContext.AccessID
            BillRaiseType = 0

            objPlan.getPlaninfo(planId) ' Get Selected Plan Information ==================

            If userContext.item("noofdays") Is Nothing Then
                no_ofdays = 1
            Else
                no_ofdays = CInt(userContext.item("noofdays"))
            End If

            no_ofdays = 1

            ' Process For Renew / New Login ==================================
            planAmount = objPlan.planAmount

            planPostAmount = CDbl(planAmount) * no_ofdays

            '========= Latest code snippet: June 11, 2012 =========================
            '========= By: Subhadeep Ray ==========================================
            '========= Reason: Incorporating discount on ITC FIAS =================
            '------------------------------------------------------------------------------------------
            Dim profile As New GuestProfile(userContext.GuestID, userContext.AccessID)

            'Check if guest need to auto-connect a plan with an auto-connect discount
            'Else if the guest selected plan is matched with the complementary plan assigned with the
            'current guest profile and for the current coupon the complimentary plan is application 
            'then guest will enjoy internet free.
            Dim actionOnBill As String = ""
            Dim appliedDiscount As Double = 0

         

           

            If planId = 15 Or planId = 16 Then
                appliedDiscount = 100
                planPostAmount = 0
                actionOnBill = String.Format("Complimentary access on {0} for {1}", profile.ComplimentaryTypeApplicable, objPlan.planName)
            ElseIf planId = 17 Then
                appliedDiscount = profile.DiscountApplicable
                planPostAmount = Math.Floor(CDbl(planAmount) * ((100.0 - appliedDiscount) / 100)) * no_ofdays
                actionOnBill = String.Format("{0}% {1} discount for {2}", appliedDiscount, profile.DiscountTypeApplicable, objPlan.planName)
            End If

            '------------------------------------------------------------------------------------------
            '======================================================================

            Try
                ObjLog = LoggerService.gtInstance
                ObjLog.write2LogFile("billtest", "Guestid=" & grCId & "amt" & planAmount & "amt post" & planPostAmount)
            Catch ex As Exception

            End Try



            userContext.item("itemamount") = planPostAmount
            Dim PlanPostAmountInt As Double
            PlanPostAmountInt = Convert.ToDouble(planPostAmount)

            PlanPostAmountInt = PlanPostAmountInt / 2

            planPostAmount = PlanPostAmountInt


            '========= Modification: June 14, 2012 ================================
            '========= By: Subhadeep Ray ==========================================
            '========= Reason: Preventing duplicate bill =================================
            planAmount = 299

            RefDataTable = RaiseDatabaseBill2(grCId, planId, planAmount, billtype, billdate, MACAddress, planPostAmount, BillAccessType, no_ofdays, ACCID, BillRaiseType)
            '======================================================================

            If RefDataTable Is Nothing Then
                billId = -1
                Return billId
            End If

            If RefDataTable.Rows.Count > 0 Then
                billId = Long.Parse(RefDataTable.Rows(0).Item("BillID").ToString())
                ' billId = Long.Parse(RefDataTable.Rows(0).Item("BillID").ToString())
                Dim newBill As String = "1"


                Try
                    Try
                        ObjLog = LoggerService.gtInstance
                        ObjLog.write2LogFile("billtest", "Guest no of days stay" & userContext.item("Totalnoofdays") & "Seamless" & userContext.item("seam"))





                    Catch ex As Exception

                    End Try

                    Try
                        'If userContext.item("seam") = "1" Then
                        '    Dim guestnoofdaysstay As Integer = 1
                        '    guestnoofdaysstay = CInt(userContext.item("Totalnoofdays"))

                        '    Try
                        '        setseam(ACCID, 1)
                        '    Catch ex As Exception

                        '    End Try

                        '    BillTrack(grCId, billId, planPostAmount, ACCID, guestnoofdaysstay)

                        'Else
                        '    Try
                        '        setseam(ACCID, 0)
                        '    Catch ex As Exception

                        '    End Try
                        'End If
                    Catch ex As Exception

                    End Try

                Catch ex As Exception

                End Try









                ' Dim newBill As String = RefDataTable.Rows(0).Item("NewBill").ToString().Trim()

                '========= Latest code snippet: June 11, 2012 =========================
                '========= By: Subhadeep Ray ==========================================
                '========= Reason: Incorporating discount on ITC FIAS =================
                'After bill posting insert the bill and guest profile in the 
                'database table "BillGuestProfileMappings"

                If newBill = "1" Then
                    GuestProfile.LogBillWithGuestProfile(billId, planId, planAmount, profile, appliedDiscount, actionOnBill)
                End If
                '----------------------------------------------------------------
                '======================================================================

            Else
                billId = -1
            End If
            'Bill Process End ===============================
        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("BILLEXP", ex)
            billId = -1
        Finally
            objPlan = Nothing
            RefDataTable = Nothing
            objDbase = Nothing
        End Try
        Return billId

    End Function

    Public Function getExpifo2(ByVal bid As Long) As String
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "select convert(nvarchar,CAST(loginexptime as time),108)  from logdetails where loginbillid=@PPPPP and DAY(loginexptime)=DAY(getdate()) and MONTH(loginexptime)=MONTH(getdate()) and YEAR (loginexptime) = YEAR(getdate())"
            DatabaseUtil.AddInputParameter(com, "@PPPPP", DbType.String, bid.ToString())
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then
                Return result.Rows(0)(0).ToString()
            Else
                Return ""

            End If



        Catch ex As Exception
            Return 0
        End Try

    End Function




    Public Function getExpifo1(ByVal bid As Long) As String
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "select loginexptime  from logdetails where loginbillid=@PPPP and DAY(loginexptime)=DAY(getdate()) and MONTH(loginexptime)=MONTH(getdate()) and YEAR (loginexptime) = YEAR(getdate())"
            DatabaseUtil.AddInputParameter(com, "@PPPP", DbType.String, bid.ToString())
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then
                Return result.Rows(0)(0).ToString()
            Else
                Return ""

            End If



        Catch ex As Exception
            Return 0
        End Try

    End Function


    Public Function getExpifo(ByVal gid As Long) As String
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "select GuestExpChkOutTime  from guest where GuestId=@PPP and DAY(GuestExpChkOutTime)=DAY(getdate()) and MONTH(GuestExpChkOutTime)=MONTH(getdate()) and YEAR (GuestExpChkOutTime) = YEAR(getdate())"
            DatabaseUtil.AddInputParameter(com, "@PPP", DbType.String, gid.ToString())
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then
                Return result.Rows(0)(0).ToString()
            Else
                Return ""

            End If



        Catch ex As Exception
            Return 0
        End Try

    End Function


    Public Function getbid(ByVal aid As Long) As Long
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "Select billid from bill where billaccid=@QQQ  order by BillId desc"
            DatabaseUtil.AddInputParameter(com, "@QQQ", DbType.String, aid.ToString())
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then
                Return result.Rows(0)(0).ToString()
            Else
                Return 0

            End If



        Catch ex As Exception
            Return 0
        End Try

    End Function

    Public Function renewal(ByVal accid As Long, ByVal guestid As Long, ByVal exptime As String, ByVal duration As String, ByVal ck As String, ByVal bid As Long) As Integer
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            Dim result As DataTable

            Dim ga As String = ""
            Dim gr As String = ""

            com.CommandText = "Select guestname , guestroomno from guest where guestid=@HH"
            DatabaseUtil.AddInputParameter(com, "@HH", DbType.String, guestid.ToString())


            Try
                result = DatabaseUtil.ExecuteSelect(com)

                If result.Rows.Count > 0 Then

                    ga = result.Rows(0)(0)

                    gr = result.Rows(0)(1)



                End If



            Catch ex As Exception

            End Try





            com.CommandText = "insert into renewal(guestid,accid,exptime,logintime,duration,chkouttime,guestname,Guestroomno,billid) values(@J1, @J2,@J3,@J4,@J5,@J6, @J7, @J8 , @J9)"
            DatabaseUtil.AddInputParameter(com, "@J1", DbType.String, guestid.ToString())
            DatabaseUtil.AddInputParameter(com, "@J2", DbType.String, accid.ToString())
            DatabaseUtil.AddInputParameter(com, "@J3", DbType.String, exptime)
            DatabaseUtil.AddInputParameter(com, "@J4", DbType.String, Now.ToString())
            DatabaseUtil.AddInputParameter(com, "@J5", DbType.String, duration)
            DatabaseUtil.AddInputParameter(com, "@J6", DbType.String, ck)
            DatabaseUtil.AddInputParameter(com, "@J7", DbType.String, ga)
            DatabaseUtil.AddInputParameter(com, "@J8", DbType.String, gr)
            DatabaseUtil.AddInputParameter(com, "@J9", DbType.String, bid.ToString())

            DatabaseUtil.ExecuteInsertUpdateDelete(com)





        Catch ex As Exception
            Return 0
        End Try

    End Function

    Public Function renewalpay(ByVal accid As Long, ByVal guestid As Long, ByVal exptime As String, ByVal duration As String, ByVal ck As String, ByVal bid As Long) As Integer
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            Dim result As DataTable

            Dim ga As String = ""
            Dim gr As String = ""

            Dim bamt As String = ""

            Dim bill As Long = 0
            com.CommandText = "Select guestname , guestroomno from guest where guestid=@HH"
            DatabaseUtil.AddInputParameter(com, "@HH", DbType.String, guestid.ToString())


            Try
                result = DatabaseUtil.ExecuteSelect(com)

                If result.Rows.Count > 0 Then

                    ga = result.Rows(0)(0)

                    gr = result.Rows(0)(1)



                End If



            Catch ex As Exception

            End Try



            com.CommandText = "Select billpostedamount from bill where billid=@MM"
            DatabaseUtil.AddInputParameter(com, "@MM", DbType.String, bid.ToString())


            Try
                result = DatabaseUtil.ExecuteSelect(com)

                If result.Rows.Count > 0 Then

                    bamt = result.Rows(0)(0)

                    Try

                        bill = bamt

                        bill = bamt / 2

                    Catch ex As Exception

                    End Try


                End If



            Catch ex As Exception

            End Try







            com.CommandText = "insert into renewalpay(guestid,accid,exptime,logintime,duration,chkouttime,guestname,Guestroomno,billid, billpostedamount) values(@J1, @J2,@J3,@J4,@J5,@J6, @J7, @J8 , @J9, @J10)"
            DatabaseUtil.AddInputParameter(com, "@J1", DbType.String, guestid.ToString())
            DatabaseUtil.AddInputParameter(com, "@J2", DbType.String, accid.ToString())
            DatabaseUtil.AddInputParameter(com, "@J3", DbType.String, exptime)
            DatabaseUtil.AddInputParameter(com, "@J4", DbType.String, Now.ToString())
            DatabaseUtil.AddInputParameter(com, "@J5", DbType.String, duration)
            DatabaseUtil.AddInputParameter(com, "@J6", DbType.String, ck)
            DatabaseUtil.AddInputParameter(com, "@J7", DbType.String, ga)
            DatabaseUtil.AddInputParameter(com, "@J8", DbType.String, gr)
            DatabaseUtil.AddInputParameter(com, "@J9", DbType.String, bid.ToString())
            DatabaseUtil.AddInputParameter(com, "@J10", DbType.String, Bill.tostring())

            DatabaseUtil.ExecuteInsertUpdateDelete(com)





        Catch ex As Exception
            Return 0
        End Try

    End Function

    Public Function getseam(ByVal accid As Long) As Integer
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "select * from seam where accid=@PPP"
            DatabaseUtil.AddInputParameter(com, "@PPP", DbType.String, accid.ToString())
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then
                Return result.Rows(0)(0)
            Else
                Return 0

            End If



        Catch ex As Exception
            Return 0
        End Try

    End Function
    Public Sub setseam(ByVal accid As String, ByVal status As Integer)

        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "select * from seam where accid=" & accid
            Dim sdt As DataTable
            sdt = DatabaseUtil.ExecuteSelect(com)

            If sdt.Rows.Count > 0 Then
                com = DatabaseUtil.GetCommand(conn)
                com.CommandText = "update seam set seamless=@KA2 where accid=@KA1"
                DatabaseUtil.AddInputParameter(com, "@KA1", DbType.String, accid.ToString())
                DatabaseUtil.AddInputParameter(com, "@KA2", DbType.String, status.ToString())
                DatabaseUtil.ExecuteInsertUpdateDelete(com)
            Else
                com = DatabaseUtil.GetCommand(conn)
                com.CommandText = "Insert into seam(accid,seamless) values(@K1,@K2)"
                DatabaseUtil.AddInputParameter(com, "@K1", DbType.String, accid.ToString())
                DatabaseUtil.AddInputParameter(com, "@K2", DbType.String, status.ToString())
                DatabaseUtil.ExecuteInsertUpdateDelete(com)


            End If




        Catch ex As Exception

        End Try


    End Sub

    Private Function BillTrack(ByVal GuestID As String, ByVal billid As Long, ByVal BillAmount As String, ByVal accid As String, ByVal total As Integer)

        Dim val As Integer = 0
        val = 86400



        Dim objlog As LoggerService
        objlog = LoggerService.gtInstance

        Try
            
            objlog.write2LogFile("billtest", "GuestID" & GuestID & "billid" & billid & "Billamt" & BillAmount & "accid=" & accid & "total=" & total)





        Catch ex As Exception

        End Try


        Dim conn As DbConnection = DatabaseUtil.GetConnection()
        Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
        Dim billt As Long = 1


       


       



        Try


            com.CommandText = "select max(BillT) from billtrack"
            Dim res As DataTable

            Try
                res = DatabaseUtil.ExecuteSelect(com)
                If res.Rows.Count > 0 Then

                    billt = CLng(res.Rows(0)(0).ToString())
                    billt = billt + 1

                End If
            Catch ex As Exception
                objlog.write2LogFile("billtest", "Biit err" & ex.Message)
            End Try




           

            


            Dim cnt As Integer = 1
            cnt = total - 1

            Dim index As Integer = 0

            Dim st As DateTime
            st = Now

            st = DateAdd(DateInterval.Second, val, st)

            Dim ed As DateTime
            ed = Now
            ed = DateAdd(DateInterval.Second, val * 2, ed)


            Try
                objlog.write2LogFile("billtest", "BillTNO" & billt & "Total=" & total & "cnt=" & cnt)
            Catch ex As Exception

            End Try

            Try

                'com.CommandText = "INSERT INTO BillTrack(Billid,BillT,BillGrCId,StartTime,EndTime,BillPostedAmount,BillStatus,BillACCId,noofstay) values(@A1,@A2,@A3,@A4,@A5,@A6,@A7,@A8,@A9)"
            Catch ex As Exception

            End Try


            While index < cnt

                Try
                    com = DatabaseUtil.GetCommand(conn)
                    com.CommandText = "INSERT INTO BillTrack(Billid,BillT,BillGrCId,StartTime,EndTime,BillPostedAmount,BillStatus,BillACCId,noofstay) values(@A1,@A2,@A3,@A4,@A5,@A6,@A7,@A8,@A9)"
                    DatabaseUtil.AddInputParameter(com, "@A1", DbType.String, billid.ToString())
                    DatabaseUtil.AddInputParameter(com, "@A2", DbType.String, billt.ToString())
                    DatabaseUtil.AddInputParameter(com, "@A3", DbType.String, GuestID)
                    DatabaseUtil.AddInputParameter(com, "@A4", DbType.String, st.ToString())
                    DatabaseUtil.AddInputParameter(com, "@A5", DbType.String, ed.ToString())
                    DatabaseUtil.AddInputParameter(com, "@A6", DbType.String, BillAmount)
                    DatabaseUtil.AddInputParameter(com, "@A7", DbType.String, "A")
                    DatabaseUtil.AddInputParameter(com, "@A8", DbType.String, accid)
                    DatabaseUtil.AddInputParameter(com, "@A9", DbType.String, total.ToString())

                    DatabaseUtil.ExecuteInsertUpdateDelete(com)
                Catch ex As Exception
                    objlog.write2LogFile("billtest", "Biit err1" & ex.Message)
                End Try
                Try
                    st = DateAdd(DateInterval.Second, val, st)
                    ed = DateAdd(DateInterval.Second, val, ed)
                Catch ex As Exception

                End Try


                index = index + 1

            End While







        Catch ex As Exception

        End Try


    End Function

    Private Function RaiseDatabaseBill2(ByVal GuestID As String, ByVal BillPlanID As String, ByVal BillAmount As String, ByVal BillType As String, ByVal BillTime As String, ByVal BillMac As String, ByVal BillPostAmount As String, ByVal BillAccessType As String, ByVal BillDays As String, ByVal BillAccID As String, ByVal BillRaiseType As String) As DataTable
        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Insert Into Bill (BillGrCId, BillPlanId, BillAmount, BillType, BillTime, BillMAC, BillPostedAmount, BillAccessType, BilltmpNoofDays, BillACCId, BillRaiseType) " & vbNewLine & _
                                "VALUES (@BillGrcID, @BillPlanID, @BillAmount, @BillType, @BillTime, @BillMac, @BillPostedAmount, @BillAccessType, @BillDays, @BillAccID, @BillRaiseType) "



            'com.CommandText = "Begin Transaction " & vbNewLine & _
            '                    "Declare @BillID int " & vbNewLine & _
            '                    "Declare @CurrentBillID int " & vbNewLine & _
            '                    "Declare @NewBill int " & vbNewLine & _
            '                    "Set @NewBill = 0 " & vbNewLine & _
            '                    "Insert Into Bill (BillGrCId, BillPlanId, BillAmount, BillType, BillTime, BillMAC, BillPostedAmount, BillAccessType, BilltmpNoofDays, BillACCId, BillRaiseType) " & vbNewLine & _
            '                    "VALUES (@BillGrcID, @BillPlanID, @BillAmount, @BillType, @BillTime, @BillMac, @BillPostedAmount, @BillAccessType, @BillDays, @BillAccID, @BillRaiseType) " & vbNewLine & _
            '                    "Set @CurrentBillID = SCOPE_IDENTITY() " & vbNewLine & _
            '                    "Set @NewBill = 1 " & vbNewLine & _
            '                    "Select Top 1 @BillID = BillID " & vbNewLine & _
            '                    "From " & vbNewLine & _
            '                    "( " & vbNewLine & _
            '                    "Select BillID, DateAdd(Second, ((Select Top 1 PlanDuration From Plans Where PlanId = BillPlanID) * BilltmpNoofDays), BillTime) As BillExpiryTime " & vbNewLine & _
            '                    "From Bill " & vbNewLine & _
            '                    "Where BillID < @CurrentBillID And BillType = 0 And BillPlanID = @BillPlanID " & vbNewLine & _
            '                    "      And BillACCId = @BillAccID And BillRaiseType = 0 " & vbNewLine & _
            '                    ") As B " & vbNewLine & _
            '                    "Where BillExpiryTime > (Select Top 1 BillTime From Bill Where BillId = @CurrentBillID) " & vbNewLine & _
            '                    "Order By BillID Desc " & vbNewLine & _
            '                    "If @BillID Is Null " & vbNewLine & _
            '                    "Begin " & vbNewLine & _
            '                    "	Set @BillID = @CurrentBillID " & vbNewLine & _
            '                    "End " & vbNewLine & _
            '                    "If @CurrentBillID <> @BillID " & vbNewLine & _
            '                    "Begin " & vbNewLine & _
            '                    "	Delete From Bill Where BillID = @CurrentBillID " & vbNewLine & _
            '                    "	Set @NewBill = 0 " & vbNewLine & _
            '                    "End " & vbNewLine & _
            '                    "Select @BillID As BillID, @NewBill As NewBill " & vbNewLine & _
            '                    "Commit Transaction"


            DatabaseUtil.AddInputParameter(com, "@BillGrcID", DbType.String, GuestID)
            DatabaseUtil.AddInputParameter(com, "@BillPlanID", DbType.String, BillPlanID)
            DatabaseUtil.AddInputParameter(com, "@BillAmount", DbType.String, BillAmount)
            DatabaseUtil.AddInputParameter(com, "@BillType", DbType.String, BillType)
            DatabaseUtil.AddInputParameter(com, "@BillTime", DbType.String, BillTime)
            DatabaseUtil.AddInputParameter(com, "@BillMac", DbType.String, BillMac)
            DatabaseUtil.AddInputParameter(com, "@BillPostedAmount", DbType.String, BillPostAmount)
            DatabaseUtil.AddInputParameter(com, "@BillAccessType", DbType.String, BillAccessType)
            DatabaseUtil.AddInputParameter(com, "@BillDays", DbType.String, BillDays)
            DatabaseUtil.AddInputParameter(com, "@BillAccID", DbType.String, BillAccID)
            DatabaseUtil.AddInputParameter(com, "@BillRaiseType", DbType.String, BillRaiseType)

            Try
                DatabaseUtil.ExecuteInsertUpdateDelete(com)

            Catch ex As Exception

            End Try

            com.CommandText = "select max(billid) as BillID from bill where billgrcid=@KKK  and BillACCId=@MMM"
            DatabaseUtil.AddInputParameter(com, "@KKK", DbType.String, GuestID)
            DatabaseUtil.AddInputParameter(com, "@MMM", DbType.String, BillAccID)
            Dim result As DataTable
            result = DatabaseUtil.ExecuteSelect(com)

            Return result

        Catch ex As Exception
            objLog.writeExceptionLogFile("RaiseDatabaseBillExp", ex)
            Return Nothing
        End Try
    End Function



    Private Function RaiseDatabaseBill(ByVal GuestID As String, ByVal BillPlanID As String, ByVal BillAmount As String, ByVal BillType As String, ByVal BillTime As String, ByVal BillMac As String, ByVal BillPostAmount As String, ByVal BillAccessType As String, ByVal BillDays As String, ByVal BillAccID As String, ByVal BillRaiseType As String) As DataTable
        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "Begin Transaction " & vbNewLine & _
                                "Declare @BillID int " & vbNewLine & _
                                "Declare @CurrentBillID int " & vbNewLine & _
                                "Declare @NewBill int " & vbNewLine & _
                                "Set @NewBill = 0 " & vbNewLine & _
                                "Insert Into Bill (BillGrCId, BillPlanId, BillAmount, BillType, BillTime, BillMAC, BillPostedAmount, BillAccessType, BilltmpNoofDays, BillACCId, BillRaiseType) " & vbNewLine & _
                                "VALUES (@BillGrcID, @BillPlanID, @BillAmount, @BillType, @BillTime, @BillMac, @BillPostedAmount, @BillAccessType, @BillDays, @BillAccID, @BillRaiseType) " & vbNewLine & _
                                "Set @CurrentBillID = SCOPE_IDENTITY() " & vbNewLine & _
                                "Set @NewBill = 1 " & vbNewLine & _
                                "Select Top 1 @BillID = BillID " & vbNewLine & _
                                "From " & vbNewLine & _
                                "( " & vbNewLine & _
                                "Select BillID, DateAdd(Second, ((Select Top 1 PlanDuration From Plans Where PlanId = BillPlanID) * BilltmpNoofDays), BillTime) As BillExpiryTime " & vbNewLine & _
                                "From Bill " & vbNewLine & _
                                "Where BillID < @CurrentBillID And BillType = 0 And BillPlanID = @BillPlanID " & vbNewLine & _
                                "      And BillACCId = @BillAccID And BillRaiseType = 0 " & vbNewLine & _
                                ") As B " & vbNewLine & _
                                "Where BillExpiryTime > (Select Top 1 BillTime From Bill Where BillId = @CurrentBillID) " & vbNewLine & _
                                "Order By BillID Desc " & vbNewLine & _
                                "If @BillID Is Null " & vbNewLine & _
                                "Begin " & vbNewLine & _
                                "	Set @BillID = @CurrentBillID " & vbNewLine & _
                                "End " & vbNewLine & _
                                "If @CurrentBillID <> @BillID " & vbNewLine & _
                                "Begin " & vbNewLine & _
                                "	Delete From Bill Where BillID = @CurrentBillID " & vbNewLine & _
                                "	Set @NewBill = 0 " & vbNewLine & _
                                "End " & vbNewLine & _
                                "Select @BillID As BillID, @NewBill As NewBill " & vbNewLine & _
                                "Commit Transaction"


            DatabaseUtil.AddInputParameter(com, "@BillGrcID", DbType.String, GuestID)
            DatabaseUtil.AddInputParameter(com, "@BillPlanID", DbType.String, BillPlanID)
            DatabaseUtil.AddInputParameter(com, "@BillAmount", DbType.String, BillAmount)
            DatabaseUtil.AddInputParameter(com, "@BillType", DbType.String, BillType)
            DatabaseUtil.AddInputParameter(com, "@BillTime", DbType.String, BillTime)
            DatabaseUtil.AddInputParameter(com, "@BillMac", DbType.String, BillMac)
            DatabaseUtil.AddInputParameter(com, "@BillPostedAmount", DbType.String, BillPostAmount)
            DatabaseUtil.AddInputParameter(com, "@BillAccessType", DbType.String, BillAccessType)
            DatabaseUtil.AddInputParameter(com, "@BillDays", DbType.String, BillDays)
            DatabaseUtil.AddInputParameter(com, "@BillAccID", DbType.String, BillAccID)
            DatabaseUtil.AddInputParameter(com, "@BillRaiseType", DbType.String, BillRaiseType)

            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)
            Return result

        Catch ex As Exception
            objLog.writeExceptionLogFile("RaiseDatabaseBillExp", ex)
            Return Nothing
        End Try
    End Function


End Class
