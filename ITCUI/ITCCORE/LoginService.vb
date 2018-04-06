'############## Jan 11,2011-Class Description Start ############################
'-- This class is used for handing logdetails, users and loginfails table
'############## Jan 11,2011-Class Description End   ############################




Public Class LoginService
    Private Shared gtLoginService As LoginService
    Private Sub New()
        'Nothing
    End Sub

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to create a instance of the class---
    '-- This method will return a object of class LoginService---
    '############## Jan 11,2011-Method Description End   ############################
    Public Shared Function getInstance() As LoginService
        If gtLoginService Is Nothing Then gtLoginService = New LoginService
        Return gtLoginService
    End Function




    Public Function GetPlanIdByMAC(ByVal MAC As String) As Integer
        'In USE 25 OCT 2010*************************
        'Variable Declaration Start ================================================
        Dim SQL_query As String
        Dim RefResultset As DataSet
        Dim RefDataTable As DataTable
        Dim objDBase As DbaseServiceOLEDB
        Dim usrcrd As New UserCredential
        Dim Ctime As DateTime
        Dim CurrPlanId As Integer
        'Variable Declaration End ==================================================
        Try
            Ctime = Now()
            SQL_query = "SELECT BillPlanID As PlanID,ACCID FROM LogDetails INNER JOIN Bill ON LogDetails.LoginBillId=Bill.BillID AND Bill.BillType=" & PMSBill.ROOM & _
                      vbCrLf & " WHERE LogDetails.LoginMac = '" & MAC & "' AND LogDetails.LoginExpTime > '" & Ctime & "' ORDER BY  LogDetails.LoginId DESC"

            objDBase = DbaseServiceOLEDB.getInstance
            RefResultset = objDBase.DsWithoutUpdate(SQL_query)
            RefDataTable = RefResultset.Tables(0)
            If RefDataTable.Rows.Count > 0 Then
                CurrPlanId = Integer.Parse(RefDataTable.Rows(0).Item("PlanID").ToString())
            Else
                CurrPlanId = 0
            End If
        Catch ex As Exception
            CurrPlanId = 0
        Finally
            objDBase = Nothing
            RefResultset = Nothing
        End Try
        Return CurrPlanId
    End Function

    Public Function WithoutCookiesGetCredential(ByVal Mac As String) As UserCredential

        'In USE 7 SEP 2010*************************
        'Variable Declaration Start ================================================
        Dim SQL_query As String
        Dim RefResultset As DataSet
        Dim RefDataTable As DataTable
        Dim objDBase As DbaseServiceOLEDB
        Dim usrcrd As New UserCredential
        Dim Ctime As DateTime
        'Variable Declaration End ==================================================
        Try
            objDBase = DbaseServiceOLEDB.getInstance
            Ctime = Now()
            SQL_query = "SELECT Guest.GuestRoomNo As RoomNo,Guest.GuestRegCode,Guest.GuestId as GuestID.Bill.BillPlanId as PlanID,Bill.ACCID As ACCID FROM ((LogDetails INNER JOIN Bill ON LogDetails.LoginBillId = Bill.Billid) INNER JOIN Guest ON Guest.Guestid = Bill.BillGrCId) " & _
                        "WHERE (Bill.BillType = " & PMSBill.ROOM & " AND Guest.GuestStatus = 'A' and LogDetails.LoginMac = '" & Mac & "' " & _
                        "AND LogDetails.LoginExpTime > '" & Ctime & "') ORDER BY  LogDetails.LoginId DESC"
            RefResultset = objDBase.DsWithoutUpdate(SQL_query)
            RefDataTable = RefResultset.Tables(0)
            If RefDataTable.Rows.Count > 0 Then
                usrcrd.usrId = RefDataTable.Rows(0).Item("RoomNo")
                usrcrd.passwd = RefDataTable.Rows(0).Item("GuestRegCode")
                usrcrd.GuestID = Long.Parse(RefDataTable.Rows(0).Item("GuestID").ToString())
                usrcrd.CurrPlanId = Integer.Parse(RefDataTable.Rows(0).Item("PlanID").ToString())
                usrcrd.ACCID = Long.Parse(RefDataTable.Rows(0).Item("ACCID").ToString())
            Else
                usrcrd.usrId = ""
                usrcrd.passwd = ""
                usrcrd.GuestID = 0
                usrcrd.CurrPlanId = 0
                usrcrd.ACCID = 0
            End If
        Catch ex As Exception
            usrcrd.usrId = ""
            usrcrd.passwd = ""
            usrcrd.GuestID = 0
            usrcrd.CurrPlanId = 0
            usrcrd.ACCID = 0
        Finally
            objDBase = Nothing
            RefResultset = Nothing
        End Try
        Return usrcrd

    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used for inserting failure log in loginfails table
    '-- This will contain all the radius-login specific error
    '-- so it will contains mostly gt error msgs
    '############## Jan 11,2011-Method Description End   ############################
    Public Sub loginfail(ByRef userContext As UserContext, ByVal msg As String)
        Dim SQL_query As String = ""
        Dim objDBase As DbaseServiceOLEDB
        Dim ObjLog As LoggerService
        Try
            SQL_query = "INSERT INTO Loginfails	(FailUserId, FailPassword, FailMAC, FailUserType, FailPlanId,FailRemarks,FailTime,FailType) VALUES ('" & userContext.roomNo & "', '" & userContext.AccessID & "', '" & userContext.machineId & "', 0, " & userContext.selectedPlanId & ", '" & msg & "', '" & Now() & "', '" & userContext.item("failtype") & "') "
            objDBase = DbaseServiceOLEDB.getInstance
            objDBase.insertUpdateDelete(SQL_query)
        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.write2LogFile("LoginFailEXP", SQL_query & vbNewLine)
            ObjLog.writeExceptionLogFile("LoginFailEXP", ex)
        End Try
    End Sub

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used for inserting failure log in GeneralLoginFails table
    '-- This will contain all the validation error user performed in Instruction Page
    '-- So generally it will contain error like invalid accesscode, roomno or last name etc
    '############## Jan 11,2011-Method Description End   ############################
    Public Sub GenericLoginFails(ByVal StrLogFails As strLoginFails)



        



        Dim SQL_Query As String = ""
        Dim objDBase As DbaseServiceOLEDB
        Dim ObjLog As LoggerService
        Try
            SQL_Query = "INSERT INTO GeneralLoginFails (FailTime,FailMsg,FailAccessType,FailAccId,FailAccessCode,FailMAC,FailRoomNo,FailGuestName,FailRemarks)" & _
                        vbCrLf & " VALUES ('" & Now() & "'," & _
                        vbCrLf & "'" & StrLogFails.FailMsg & "'," & _
                        vbCrLf & StrLogFails.FailAccessType & "," & _
                        vbCrLf & StrLogFails.FailAccId & "," & _
                        vbCrLf & "'" & StrLogFails.FailAccessCode.Replace("'", "''").ToUpper() & "'," & _
                        vbCrLf & "'" & StrLogFails.FailMac & "'," & _
                        vbCrLf & "'" & StrLogFails.FailRoomNo & "'," & _
                        vbCrLf & "'" & StrLogFails.FailGuestName.Replace("'", "''") & "'," & _
                        vbCrLf & "'" & StrLogFails.Remarks.Replace("'", "''") & "')"

            objDBase = DbaseServiceOLEDB.getInstance
            objDBase.insertUpdateDelete(SQL_Query)
        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.write2LogFile("GenericLoginFailsExp", SQL_Query & vbNewLine)
            ObjLog.writeExceptionLogFile("GenericLoginFailsExp", ex)
        End Try

        Try
            Dim objmail As Mail
            objmail = Mail.getInstance

            objmail.SendAdminMail(StrLogFails.FailRoomNo, StrLogFails.FailGuestName.Replace("'", "''"), StrLogFails.FailMsg, "", StrLogFails.FailMac, objmail.ErrTypes.ndxRLF)



        Catch exy As Exception

        End Try




    End Sub

    Public Function Timer_GetRemainingTime(ByVal Mac As String) As UserLogDetails
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim SQL_query As String
        Dim logintime, loginexptime, FirstLoginTime As DateTime
        Dim loginplantime, calcTime, Remainingtime As Long
        Dim CurrentTime As DateTime = Now
        Dim ObjElog As LoggerService
        Dim objDBase As DbaseServiceOLEDB
        Dim Refresultset As DataSet
        Dim RefDataTable As DataTable
        Dim ObjUsrLog As UserLogDetails
        Dim loginid, loginbillid As Long
        Dim BillNoOfDays, PlanID As Integer
        'Variable Declaration End ================================================
        Try
            objDBase = DbaseServiceOLEDB.getInstance
            SQL_query = "Select LoginTime, LoginPlanTime,LoginId,LoginBillId,LoginExpTime  from LogDetails where LoginId = (Select max(loginid) from LogDetails where loginmac ='" & Mac & "')"
            Refresultset = objDBase.DsWithoutUpdate(SQL_query)
            RefDataTable = Refresultset.Tables(0)
            If RefDataTable.Rows.Count > 0 Then
                logintime = RefDataTable.Rows(0).Item("LoginTime")
                loginplantime = RefDataTable.Rows(0).Item("LoginPlanTime")
                loginexptime = RefDataTable.Rows(0).Item("LoginExpTime")
                loginid = Long.Parse(RefDataTable.Rows(0).Item("LoginId").ToString())
                loginbillid = Long.Parse(RefDataTable.Rows(0).Item("LoginBillId").ToString())
                calcTime = DateDiff(DateInterval.Second, logintime, CurrentTime)
                Remainingtime = loginplantime - calcTime
                ObjUsrLog.RemainTime = Remainingtime
                ObjUsrLog.LoginId = loginid
                ObjUsrLog.ExpTime = loginexptime
                'Get NoOfDays from Bill Table #################
                Try
                    SQL_query = "select BilltmpNoofDays,BillPlanId from bill where billid=" & loginbillid
                    Refresultset = objDBase.DsWithoutUpdate(SQL_query)
                    RefDataTable = Refresultset.Tables(0)
                    If RefDataTable.Rows.Count > 0 Then
                        BillNoOfDays = Integer.Parse(RefDataTable.Rows(0).Item("BilltmpNoofDays").ToString())
                        PlanID = Integer.Parse(RefDataTable.Rows(0).Item("BillPlanId").ToString())
                        FirstLoginTime = DateAdd(DateInterval.Day, (BillNoOfDays * -1), loginexptime)
                        ObjUsrLog.FirstLoginTime = FirstLoginTime
                        ObjUsrLog.PlanId = PlanID

                    Else
                        ObjUsrLog.FirstLoginTime = #1/1/1970#
                    End If
                Catch ex As Exception
                    ObjElog = LoggerService.gtInstance
                    ObjElog.writeExceptionLogFile("RemainTimeExp", ex)
                    ObjUsrLog.FirstLoginTime = #1/1/1970#
                End Try
                'Get NoOfDays from Bill Table #################
            Else
                Remainingtime = 0
                ObjUsrLog.LoginId = 0
                ObjUsrLog.FirstLoginTime = #1/1/1970#
            End If
        Catch ex As Exception
            ObjElog = LoggerService.gtInstance
            ObjElog.writeExceptionLogFile("RemainTimeExp", ex)
            ObjUsrLog.RemainTime = 6000
            ObjUsrLog.LoginId = 0
            ObjUsrLog.FirstLoginTime = #1/1/1970#
        Finally
            objDBase = Nothing
            ObjElog = Nothing
            Refresultset = Nothing
        End Try
        Return ObjUsrLog
    End Function


    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to insert one entry in logdetails table
    '-- On successfull insert it will return newly entered LoginId
    '############## Jan 11,2011-Method Description End   ############################
    Public Function login2Days(ByRef usercontext As UserContext, ByVal accessGrant As Integer) As Long
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim SQL_query As String
        Dim planValidity As Long
        Dim loginExpiryTime, loginTime As DateTime
        Dim billId, MACAddress, accessTime, ReqURL, LoginAccessType As String
        Dim loginid As Long
        Dim bytesdown, bytesup As Long
        Dim maxdownlimit, maxuplimit As Double
        Dim SessionId As String
        Dim no_ofdays, no_Rdays As Integer
        Dim objDBase As DbaseServiceOLEDB
        Dim objPlan As New CPlan
        Dim RefResultset As DataSet
        Dim RefDataTable As DataTable
        Dim ObjLog As LoggerService
        'Variable Declaration End ================================================

        Try
            billId = usercontext.item("billid")
            loginTime = usercontext.item("logintime")
            MACAddress = usercontext.machineId
            ReqURL = usercontext.requestedPage.Replace("'", "''")

            LoginAccessType = usercontext.item("accesstype")
            bytesdown = 0
            bytesup = 0

            If usercontext.item("accessgrant") Is Nothing Or usercontext.item("logintype") = LOGINTYPE.NEWLOGIN Then
                'Start New Login Process ================================
                no_ofdays = CInt(usercontext.item("noofdays"))
                objPlan.getPlaninfo(usercontext.selectedPlanId)
                maxdownlimit = objPlan.planBWDownLimit * no_ofdays
                maxuplimit = objPlan.planBWUPLimit * no_ofdays
                If objPlan.planTime < 86400 Then
                    accessTime = objPlan.planTime
                    planValidity = objPlan.planValidity
                Else
                    accessTime = objPlan.planTime * CLng(no_ofdays)
                    planValidity = objPlan.planValidity * CLng(no_ofdays)
                End If

                Try
                    If usercontext.item("seam") = "1" Then
                        Dim guestnoofdaysstay As Integer = 1
                        guestnoofdaysstay = CInt(usercontext.item("Totalnoofdays"))
                        accessTime = objPlan.planTime * CLng(guestnoofdaysstay)
                        planValidity = objPlan.planValidity * CLng(guestnoofdaysstay)
                        maxdownlimit = objPlan.planBWDownLimit * guestnoofdaysstay
                        maxuplimit = objPlan.planBWUPLimit * guestnoofdaysstay

                    End If
                Catch ex As Exception

                End Try



                loginExpiryTime = DateAdd(DateInterval.Second, planValidity, loginTime)





            Else
                'Start Relogin Process =========================================
                objPlan.getPlaninfo(usercontext.item("planid"))  'for tier only
                no_Rdays = usercontext.item("remainingdays")
                maxdownlimit = objPlan.planBWDownLimit * no_Rdays
                maxuplimit = objPlan.planBWUPLimit * no_Rdays
                accessTime = usercontext.item("remainingtime")
                loginExpiryTime = usercontext.item("expirytime")
            End If

            Dim sip = ""

            Try
                sip = usercontext.item("sip")
                ObjLog = LoggerService.gtInstance

                ObjLog.write2LogFile("lslip", sip)
            Catch ex As Exception

            End Try



            'Start Inserting into LogDetails Table ============================
            SessionId = GetSessionId(MACAddress) 'get the sessionid from Accounting_data table
            SQL_query = "INSERT INTO LogDetails(sip,LoginBillId, LoginMAC, LoginURL, LoginPlanTime, LoginTime, LoginExpTime, LoginAccessGranted, LoginAccessType, LoginUploaded, LoginDownloaded, LoginMAXUpLimit, LoginMAXDownLimit, LoginRadSessionId, LoginRadDwnBytes, LoginRadUpBytes) VALUES ('" & sip & "'," & billId & ", '" & MACAddress & "', '" & ReqURL & "', '" & accessTime & "', '" & loginTime & "', '" & loginExpiryTime & "', " & accessGrant & ", '" & LoginAccessType & "', '" & bytesup & "', '" & bytesdown & "', '" & maxuplimit & "', '" & maxdownlimit & "', '" & SessionId & "', '0', '0') " & vbNewLine & _
                        "Select SCOPE_IDENTITY() As LoginID"
            'Component initialization start =============================
            objDBase = DbaseServiceOLEDB.getInstance
            'Component initialization End =============================
            RefResultset = objDBase.DsWithoutUpdate(SQL_query)
            RefDataTable = RefResultset.Tables(0)
            If RefDataTable.Rows.Count > 0 Then
                loginid = Long.Parse(RefDataTable.Rows(0).Item("LoginID").ToString())
                usercontext.item("loginid") = loginid
                usercontext.item("expirytime") = loginExpiryTime
            Else
                loginid = -1
            End If

        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("LogDetailsExp", ex)
            loginid = -1
        Finally
            objPlan = Nothing
            RefResultset = Nothing
            RefDataTable = Nothing
            objDBase = Nothing
        End Try
        Return loginid
    End Function


    Public Function login2Days1(ByRef usercontext As UserContext, ByVal accessGrant As Integer) As Long
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim SQL_query As String
        Dim planValidity As Long
        Dim loginExpiryTime, loginTime As DateTime
        Dim billId, MACAddress, accessTime, ReqURL, LoginAccessType As String
        Dim loginid As Long
        Dim bytesdown, bytesup As Long
        Dim maxdownlimit, maxuplimit As Double
        Dim SessionId As String
        Dim no_ofdays, no_Rdays As Integer
        Dim objDBase As DbaseServiceOLEDB
        Dim objPlan As New CPlan
        Dim RefResultset As DataSet
        Dim RefDataTable As DataTable
        Dim ObjLog As LoggerService
        'Variable Declaration End ================================================

        Try
            billId = usercontext.item("billid")
            loginTime = usercontext.item("logintime")
            MACAddress = usercontext.machineId
            ReqURL = usercontext.requestedPage.Replace("'", "''")

            LoginAccessType = usercontext.item("accesstype")
            bytesdown = 0
            bytesup = 0

            If usercontext.item("accessgrant") Is Nothing Or usercontext.item("logintype") = LOGINTYPE.NEWLOGIN Then
                'Start New Login Process ================================
                no_ofdays = CInt(usercontext.item("noofdays"))
                objPlan.getPlaninfo(usercontext.selectedPlanId)
                maxdownlimit = objPlan.planBWDownLimit * no_ofdays
                maxuplimit = objPlan.planBWUPLimit * no_ofdays
                If objPlan.planTime < 86400 Then
                    accessTime = objPlan.planTime
                    planValidity = objPlan.planValidity
                Else
                    accessTime = objPlan.planTime * CLng(no_ofdays)
                    planValidity = objPlan.planValidity * CLng(no_ofdays)
                End If


                Try
                    If usercontext.item("seam") = "1" Then
                        Dim guestnoofdaysstay As Integer = 1
                        guestnoofdaysstay = CInt(usercontext.item("Totalnoofdays"))
                        accessTime = objPlan.planTime * CLng(guestnoofdaysstay)
                        planValidity = objPlan.planValidity * CLng(guestnoofdaysstay)
                        maxdownlimit = objPlan.planBWDownLimit * guestnoofdaysstay
                        maxuplimit = objPlan.planBWUPLimit * guestnoofdaysstay

                    End If
                Catch ex As Exception

                End Try




                loginExpiryTime = DateAdd(DateInterval.Second, planValidity, loginTime)
            Else
                'Start Relogin Process =========================================
                objPlan.getPlaninfo(usercontext.item("planid"))  'for tier only
                no_Rdays = usercontext.item("remainingdays")
                maxdownlimit = objPlan.planBWDownLimit * no_Rdays
                maxuplimit = objPlan.planBWUPLimit * no_Rdays
                accessTime = usercontext.item("remainingtime")
                loginExpiryTime = usercontext.item("expirytime")
            End If



            Try
                SessionId = GetSessionId(MACAddress) 'ge
            Catch ex As Exception

            End Try

            Dim duration As String = ""
            Dim dt As DateTime

            accessTime = 0

            Try
                dt = New DateTime(Now.Year, Now.Month, Now.Day, 15, 0, 0)
                dt = dt.AddMinutes(1)
                accessTime = DateDiff(DateInterval.Second, Now, dt).ToString()

            Catch ex As Exception

            End Try
            Try
                loginExpiryTime = DateAdd(DateInterval.Second, CLng(accessTime), loginTime)
            Catch ex As Exception

            End Try

            Try

                ObjLog = LoggerService.gtInstance

                'ObjLog.write2LogFile("Logdetails", "accesstime" & accessTime & "Loginexptime" & loginExpiryTime)
            Catch ex As Exception

            End Try

            SQL_query = "Update logdetails set LogoutTime=NULL, LoginPlanTime='" & accessTime & "' , LoginExpTime='" & loginExpiryTime & "' where loginbillid='" & billId & "'"
            loginid = -1
            'Start Inserting into LogDetails Table ============================
            ' t the sessionid from Accounting_data table
            'SQL_query = "INSERT INTO LogDetails(LoginBillId, LoginMAC, LoginURL, LoginPlanTime, LoginTime, LoginExpTime, LoginAccessGranted, LoginAccessType, LoginUploaded, LoginDownloaded, LoginMAXUpLimit, LoginMAXDownLimit, LoginRadSessionId, LoginRadDwnBytes, LoginRadUpBytes) VALUES (" & billId & ", '" & MACAddress & "', '" & ReqURL & "', '" & accessTime & "', '" & loginTime & "', '" & loginExpiryTime & "', " & accessGrant & ", '" & LoginAccessType & "', '" & bytesup & "', '" & bytesdown & "', '" & maxuplimit & "', '" & maxdownlimit & "', '" & SessionId & "', '0', '0') " & vbNewLine & _
            '            "Select SCOPE_IDENTITY() As LoginID"
            'Component initialization start =============================
            objDBase = DbaseServiceOLEDB.getInstance
            'Component initialization End =============================
            objDBase.insertUpdateDelete(SQL_query)
            'RefDataTable = RefResultset.Tables(0)
            'If RefDataTable.Rows.Count > 0 Then
            '    loginid = Long.Parse(RefDataTable.Rows(0).Item("LoginID").ToString())
            '    usercontext.item("loginid") = loginid
            '    usercontext.item("expirytime") = loginExpiryTime
            'Else
            '    loginid = -1
            'End If
            Try
                SQL_query = "Update logdetails set LoginTime='" & loginTime & "'    where loginbillid='" & billId & "'"
                objDBase = DbaseServiceOLEDB.getInstance
                'Component initialization End =============================
                objDBase.insertUpdateDelete(SQL_query)
            Catch ex As Exception

            End Try




        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("LogDetailsExp", ex)
            loginid = -1
        Finally
            objPlan = Nothing
            RefResultset = Nothing
            RefDataTable = Nothing
            objDBase = Nothing
        End Try
        Return loginid
    End Function


    Public Function login2Days2(ByRef usercontext As UserContext, ByVal accessGrant As Integer) As Long
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim SQL_query As String
        Dim planValidity As Long
        Dim loginExpiryTime, loginTime As DateTime
        Dim billId, MACAddress, accessTime, ReqURL, LoginAccessType As String
        Dim loginid As Long
        Dim bytesdown, bytesup As Long
        Dim maxdownlimit, maxuplimit As Double
        Dim SessionId As String
        Dim no_ofdays, no_Rdays As Integer
        Dim objDBase As DbaseServiceOLEDB
        Dim objPlan As New CPlan
        Dim RefResultset As DataSet
        Dim RefDataTable As DataTable
        Dim ObjLog As LoggerService
        'Variable Declaration End ================================================

        Try
            billId = usercontext.item("billid")
            loginTime = usercontext.item("logintime")
            MACAddress = usercontext.machineId
            ReqURL = usercontext.requestedPage.Replace("'", "''")

            LoginAccessType = usercontext.item("accesstype")
            bytesdown = 0
            bytesup = 0

            If usercontext.item("accessgrant") Is Nothing Or usercontext.item("logintype") = LOGINTYPE.NEWLOGIN Then
                'Start New Login Process ================================
                no_ofdays = CInt(usercontext.item("noofdays"))
                objPlan.getPlaninfo(usercontext.selectedPlanId)
                maxdownlimit = objPlan.planBWDownLimit * no_ofdays
                maxuplimit = objPlan.planBWUPLimit * no_ofdays
                If objPlan.planTime < 86400 Then
                    accessTime = objPlan.planTime
                    planValidity = objPlan.planValidity
                Else
                    accessTime = objPlan.planTime * CLng(no_ofdays)
                    planValidity = objPlan.planValidity * CLng(no_ofdays)
                End If


                Try
                    If usercontext.item("seam") = "1" Then
                        Dim guestnoofdaysstay As Integer = 1
                        guestnoofdaysstay = CInt(usercontext.item("Totalnoofdays"))
                        accessTime = objPlan.planTime * CLng(guestnoofdaysstay)
                        planValidity = objPlan.planValidity * CLng(guestnoofdaysstay)
                        maxdownlimit = objPlan.planBWDownLimit * guestnoofdaysstay
                        maxuplimit = objPlan.planBWUPLimit * guestnoofdaysstay

                    End If
                Catch ex As Exception

                End Try




                loginExpiryTime = DateAdd(DateInterval.Second, planValidity, loginTime)
            Else
                'Start Relogin Process =========================================
                objPlan.getPlaninfo(usercontext.item("planid"))  'for tier only
                no_Rdays = usercontext.item("remainingdays")
                maxdownlimit = objPlan.planBWDownLimit * no_Rdays
                maxuplimit = objPlan.planBWUPLimit * no_Rdays
                accessTime = usercontext.item("remainingtime")
                loginExpiryTime = usercontext.item("expirytime")
            End If



            Try
                SessionId = GetSessionId(MACAddress) 'ge
            Catch ex As Exception

            End Try

            Dim duration As String = ""
            Dim dt As DateTime

            accessTime = 0

            Try
                dt = New DateTime(Now.Year, Now.Month, Now.Day, 23, 59, 59)
                dt = dt.AddMinutes(1)
                accessTime = DateDiff(DateInterval.Second, Now, dt).ToString()

            Catch ex As Exception

            End Try
            Try
                loginExpiryTime = DateAdd(DateInterval.Second, CLng(accessTime), loginTime)
            Catch ex As Exception

            End Try

            Try

                ObjLog = LoggerService.gtInstance

                ' ObjLog.write2LogFile("LogdetailsAf", "accesstime" & accessTime & "Loginexptime" & loginExpiryTime)
            Catch ex As Exception

            End Try

            SessionId = GetSessionId(MACAddress) 'get the sessionid from Accounting_data table
            SQL_query = "INSERT INTO LogDetails(LoginBillId, LoginMAC, LoginURL, LoginPlanTime, LoginTime, LoginExpTime, LoginAccessGranted, LoginAccessType, LoginUploaded, LoginDownloaded, LoginMAXUpLimit, LoginMAXDownLimit, LoginRadSessionId, LoginRadDwnBytes, LoginRadUpBytes) VALUES (" & billId & ", '" & MACAddress & "', '" & ReqURL & "', '" & accessTime & "', '" & loginTime & "', '" & loginExpiryTime & "', " & accessGrant & ", '" & LoginAccessType & "', '" & bytesup & "', '" & bytesdown & "', '" & maxuplimit & "', '" & maxdownlimit & "', '" & SessionId & "', '0', '0') " & vbNewLine & _
                        "Select SCOPE_IDENTITY() As LoginID"
            'Component initialization start =============================
            objDBase = DbaseServiceOLEDB.getInstance
            'Component initialization End =============================
            RefResultset = objDBase.DsWithoutUpdate(SQL_query)
            RefDataTable = RefResultset.Tables(0)
            If RefDataTable.Rows.Count > 0 Then
                loginid = Long.Parse(RefDataTable.Rows(0).Item("LoginID").ToString())
                usercontext.item("loginid") = loginid
                usercontext.item("expirytime") = loginExpiryTime
            Else
                loginid = -1
            End If


        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("LogDetailsExp", ex)
            loginid = -1
        Finally
            objPlan = Nothing
            RefResultset = Nothing
            RefDataTable = Nothing
            objDBase = Nothing
        End Try
        Return loginid
    End Function

    Public Sub Radius_Users1(ByVal usercontext As UserContext)
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim MACaddress, MACaddress1 As String
        Dim SQL_query As String
        Dim Bandwidthup, Bandwidthdown, remainingtime As Long
        Dim planuplimit, plandownlimit As Double
        Dim logintime As DateTime = Now
        Dim sqllogintime As DateTime
        Dim ObjPlan As New CPlan
        Dim objDBase As DbaseServiceOLEDB
        Dim no_ofdays, no_Rdays As Integer
        Dim ObjLog As LoggerService
        'Variable Declaration End ================================================

        Try
            objDBase = DbaseServiceOLEDB.getInstance

            If usercontext.item("accessgrant") Is Nothing Or usercontext.item("logintype") = LOGINTYPE.NEWLOGIN Then
                'Start New Login Process ===========================
                no_ofdays = CInt(usercontext.item("noofdays"))
                ObjPlan.getPlaninfo(usercontext.selectedPlanId)
                remainingtime = ObjPlan.planTime * no_ofdays
                planuplimit = ObjPlan.planBWUPLimit * no_ofdays
                plandownlimit = ObjPlan.planBWDownLimit * no_ofdays

                Try
                    If usercontext.item("seam") = "1" Then
                        Dim guestnoofdaysstay As Integer = 1
                        guestnoofdaysstay = CInt(usercontext.item("Totalnoofdays"))
                        remainingtime = ObjPlan.planTime * CLng(guestnoofdaysstay)
                        planuplimit = ObjPlan.planBWUPLimit * CLng(guestnoofdaysstay)
                        plandownlimit = ObjPlan.planBWDownLimit * CLng(guestnoofdaysstay)
                    End If
                Catch ex As Exception

                End Try



            Else
                'Start Relogin/Upgrade Process ============================
                ObjPlan.getPlaninfo(usercontext.item("planid"))  'for tier only
                no_Rdays = usercontext.item("remainingdays")
                remainingtime = usercontext.item("remainingtime")
                planuplimit = ObjPlan.planBWUPLimit * no_Rdays
                plandownlimit = ObjPlan.planBWDownLimit * no_Rdays
            End If
            'Get the Bandwidth for selected Plan =====
            With ObjPlan
                Bandwidthup = .planBWUP
                Bandwidthdown = .planBWDN
            End With

            If planuplimit <> 0 Then
                planuplimit = (planuplimit * 1024)
            End If

            If plandownlimit <> 0 Then
                plandownlimit = (plandownlimit * 1024)
            End If

            MACaddress = usercontext.machineId()
            MACaddress1 = MACaddress.Insert(2, "-")
            MACaddress1 = MACaddress1.Insert(5, "-")
            MACaddress1 = MACaddress1.Insert(8, "-")
            MACaddress1 = MACaddress1.Insert(11, "-")
            MACaddress1 = MACaddress1.Insert(14, "-")


            Dim duration As String = ""
            Dim dt As DateTime



            Try
                dt = New DateTime(Now.Year, Now.Month, Now.Day, 15, 0, 0)
                dt = dt.AddMinutes(1)
                remainingtime = DateDiff(DateInterval.Second, Now, dt)

            Catch ex As Exception

            End Try

            'first delete then Insert into Users Table=============================
            sqllogintime = Now()
            SQL_query = "delete from users where MAC='" & MACaddress1 & "'"
            objDBase.insertUpdateDelete(SQL_query, "sql")

            SQL_query = "INSERT INTO Users(MAC, login_time, up_limit, down_limit, bandwidth_up, bandwidth_down, remaining_time, next_check_time, exceed) VALUES ('" & MACaddress1 & "', '" & sqllogintime & "', '" & planuplimit & "', '" & plandownlimit & "', '" & Bandwidthup & "', '" & Bandwidthdown & "', '" & remainingtime & "', '" & sqllogintime & "', 'n') "
            objDBase.insertUpdateDelete(SQL_query, "sql")

        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("USERSEXP", ex)
        Finally
            objDBase = Nothing
            ObjPlan = Nothing
        End Try

    End Sub

    Public Sub Radius_Users2(ByVal usercontext As UserContext)
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim MACaddress, MACaddress1 As String
        Dim SQL_query As String
        Dim Bandwidthup, Bandwidthdown, remainingtime As Long
        Dim planuplimit, plandownlimit As Double
        Dim logintime As DateTime = Now
        Dim sqllogintime As DateTime
        Dim ObjPlan As New CPlan
        Dim objDBase As DbaseServiceOLEDB
        Dim no_ofdays, no_Rdays As Integer
        Dim ObjLog As LoggerService
        'Variable Declaration End ================================================

        Try
            objDBase = DbaseServiceOLEDB.getInstance

            If usercontext.item("accessgrant") Is Nothing Or usercontext.item("logintype") = LOGINTYPE.NEWLOGIN Then
                'Start New Login Process ===========================
                no_ofdays = CInt(usercontext.item("noofdays"))
                ObjPlan.getPlaninfo(usercontext.selectedPlanId)
                remainingtime = ObjPlan.planTime * no_ofdays
                planuplimit = ObjPlan.planBWUPLimit * no_ofdays
                plandownlimit = ObjPlan.planBWDownLimit * no_ofdays

                Try
                    If usercontext.item("seam") = "1" Then
                        Dim guestnoofdaysstay As Integer = 1
                        guestnoofdaysstay = CInt(usercontext.item("Totalnoofdays"))
                        remainingtime = ObjPlan.planTime * CLng(guestnoofdaysstay)
                        planuplimit = ObjPlan.planBWUPLimit * CLng(guestnoofdaysstay)
                        plandownlimit = ObjPlan.planBWDownLimit * CLng(guestnoofdaysstay)
                    End If
                Catch ex As Exception

                End Try



            Else
                'Start Relogin/Upgrade Process ============================
                ObjPlan.getPlaninfo(usercontext.item("planid"))  'for tier only
                no_Rdays = usercontext.item("remainingdays")
                remainingtime = usercontext.item("remainingtime")
                planuplimit = ObjPlan.planBWUPLimit * no_Rdays
                plandownlimit = ObjPlan.planBWDownLimit * no_Rdays
            End If
            'Get the Bandwidth for selected Plan =====
            With ObjPlan
                Bandwidthup = .planBWUP
                Bandwidthdown = .planBWDN
            End With

            If planuplimit <> 0 Then
                planuplimit = (planuplimit * 1024)
            End If

            If plandownlimit <> 0 Then
                plandownlimit = (plandownlimit * 1024)
            End If

            MACaddress = usercontext.machineId()
            MACaddress1 = MACaddress.Insert(2, "-")
            MACaddress1 = MACaddress1.Insert(5, "-")
            MACaddress1 = MACaddress1.Insert(8, "-")
            MACaddress1 = MACaddress1.Insert(11, "-")
            MACaddress1 = MACaddress1.Insert(14, "-")


            Dim duration As String = ""
            Dim dt As DateTime



            Try
                dt = New DateTime(Now.Year, Now.Month, Now.Day, 23, 59, 59)
                dt = dt.AddMinutes(1)
                remainingtime = DateDiff(DateInterval.Second, Now, dt)

            Catch ex As Exception

            End Try

            'first delete then Insert into Users Table=============================
            sqllogintime = Now()
            SQL_query = "delete from users where MAC='" & MACaddress1 & "'"
            objDBase.insertUpdateDelete(SQL_query, "sql")

            SQL_query = "INSERT INTO Users(MAC, login_time, up_limit, down_limit, bandwidth_up, bandwidth_down, remaining_time, next_check_time, exceed) VALUES ('" & MACaddress1 & "', '" & sqllogintime & "', '" & planuplimit & "', '" & plandownlimit & "', '" & Bandwidthup & "', '" & Bandwidthdown & "', '" & remainingtime & "', '" & sqllogintime & "', 'n') "
            objDBase.insertUpdateDelete(SQL_query, "sql")

        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("USERSEXP", ex)
        Finally
            objDBase = Nothing
            ObjPlan = Nothing
        End Try

    End Sub

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method will insert one record in Users table
    '-- Users table only utilized by the window service running in the server
    '############## Jan 11,2011-Method Description End   ############################
    Public Sub Radius_Users(ByVal usercontext As UserContext)
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim MACaddress, MACaddress1 As String
        Dim SQL_query As String
        Dim Bandwidthup, Bandwidthdown, remainingtime As Long
        Dim planuplimit, plandownlimit As Double
        Dim logintime As DateTime = Now
        Dim sqllogintime As DateTime
        Dim ObjPlan As New CPlan
        Dim objDBase As DbaseServiceOLEDB
        Dim no_ofdays, no_Rdays As Integer
        Dim ObjLog As LoggerService
        'Variable Declaration End ================================================

        Try
            objDBase = DbaseServiceOLEDB.getInstance

            If usercontext.item("accessgrant") Is Nothing Or usercontext.item("logintype") = LOGINTYPE.NEWLOGIN Then
                'Start New Login Process ===========================
                no_ofdays = CInt(usercontext.item("noofdays"))
                ObjPlan.getPlaninfo(usercontext.selectedPlanId)
                remainingtime = ObjPlan.planTime * no_ofdays
                planuplimit = ObjPlan.planBWUPLimit * no_ofdays
                plandownlimit = ObjPlan.planBWDownLimit * no_ofdays


                Try
                    If usercontext.item("seam") = "1" Then
                        Dim guestnoofdaysstay As Integer = 1
                        guestnoofdaysstay = CInt(usercontext.item("Totalnoofdays"))
                        remainingtime = ObjPlan.planTime * CLng(guestnoofdaysstay)
                        planuplimit = ObjPlan.planBWUPLimit * CLng(guestnoofdaysstay)
                        plandownlimit = ObjPlan.planBWDownLimit * CLng(guestnoofdaysstay)
                    End If
                Catch ex As Exception

                End Try



            Else
                'Start Relogin/Upgrade Process ============================
                ObjPlan.getPlaninfo(usercontext.item("planid"))  'for tier only
                no_Rdays = usercontext.item("remainingdays")
                remainingtime = usercontext.item("remainingtime")
                planuplimit = ObjPlan.planBWUPLimit * no_Rdays
                plandownlimit = ObjPlan.planBWDownLimit * no_Rdays
            End If
            'Get the Bandwidth for selected Plan =====
            With ObjPlan
                Bandwidthup = .planBWUP
                Bandwidthdown = .planBWDN
            End With

            If planuplimit <> 0 Then
                planuplimit = (planuplimit * 1024)
            End If

            If plandownlimit <> 0 Then
                plandownlimit = (plandownlimit * 1024)
            End If

            MACaddress = usercontext.machineId()
            MACaddress1 = MACaddress.Insert(2, "-")
            MACaddress1 = MACaddress1.Insert(5, "-")
            MACaddress1 = MACaddress1.Insert(8, "-")
            MACaddress1 = MACaddress1.Insert(11, "-")
            MACaddress1 = MACaddress1.Insert(14, "-")

            'first delete then Insert into Users Table=============================
            sqllogintime = Now()
            SQL_query = "delete from users where MAC='" & MACaddress1 & "'"
            objDBase.insertUpdateDelete(SQL_query, "sql")

            SQL_query = "INSERT INTO Users(MAC, login_time, up_limit, down_limit, bandwidth_up, bandwidth_down, remaining_time, next_check_time, exceed) VALUES ('" & MACaddress1 & "', '" & sqllogintime & "', '" & planuplimit & "', '" & plandownlimit & "', '" & Bandwidthup & "', '" & Bandwidthdown & "', '" & remainingtime & "', '" & sqllogintime & "', 'n') "
            objDBase.insertUpdateDelete(SQL_query, "sql")

        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("USERSEXP", ex)
        Finally
            objDBase = Nothing
            ObjPlan = Nothing
        End Try

    End Sub

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to get the sessionid for a particular mac
    '-- it will search in the AccountingData table for the particular mac and get the most recent Acct_Session_Id
    '-- This method need to be called after raising the Radius Login command
    '############## Jan 11,2011-Method Description End   ############################
    Private Function GetSessionId(ByVal MAC As String) As String
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim SQL_query As String
        Dim ObjElog As LoggerService
        Dim objDbase As DbaseServiceOLEDB
        Dim RefResultset As DataSet
        Dim RefDataTable As DataTable
        Dim MACaddress1 As String
        Dim SessionId As String
        'Variable Declaration End ================================================

        Try
            MACaddress1 = MAC.Insert(2, "-")
            MACaddress1 = MACaddress1.Insert(5, "-")
            MACaddress1 = MACaddress1.Insert(8, "-")
            MACaddress1 = MACaddress1.Insert(11, "-")
            MACaddress1 = MACaddress1.Insert(14, "-")

            SQL_query = "Select Acct_Session_Id from accounting_data where (Calling_Station_Id = '" & MACaddress1 & "') ORDER BY id DESC"

            objDbase = DbaseServiceOLEDB.getInstance
            RefResultset = objDbase.DsWithoutUpdate(SQL_query)
            RefDataTable = RefResultset.Tables(0)

            If RefDataTable.Rows.Count > 0 Then

                If Not IsDBNull(RefDataTable.Rows(0).Item("Acct_Session_Id")) Then
                    SessionId = RefDataTable.Rows(0).Item("Acct_Session_Id")
                Else
                    SessionId = 0
                End If

            Else
                SessionId = 0
            End If

        Catch ex As Exception
            SessionId = 0
            ObjElog = LoggerService.gtInstance
            ObjElog.writeExceptionLogFile("Getsession_" & MAC, ex)
        Finally
            RefResultset = Nothing
            RefDataTable = Nothing
            objDbase = Nothing
        End Try
        Return SessionId
    End Function

    Public Function logout(ByVal MAC As String, ByVal logoutstatus As Integer)
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim SQL_query As String
        Dim loginid As Long
        Dim objDBase As DbaseServiceOLEDB
        Dim Refresultset As DataSet
        Dim RefDataTable As DataTable
        'Variable Declaration End ================================================

        Try
            objDBase = DbaseServiceOLEDB.getInstance
            SQL_query = "select LoginId, LoginRadSessionId from logdetails where loginmac= '" & MAC & "' ORDER BY LoginId DESC"
            Refresultset = objDBase.DsWithoutUpdate(SQL_query)
            RefDataTable = Refresultset.Tables(0)
            If RefDataTable.Rows.Count > 0 Then
                loginid = RefDataTable.Rows(0).Item("loginid")
                Call UpdateOTime(logoutstatus, loginid)
            End If
        Catch ex As Exception

        End Try

    End Function

    Private Sub UpdateOTime(ByVal logstatus As Integer, ByVal loginid As Long)
        'In USE 6 AUG 2010*************************
        Dim ObjElog As LoggerService
        Dim SQL_query As String
        Dim objDBase As DbaseServiceOLEDB
        Try
            ObjElog = LoggerService.gtInstance
            ' ObjElog.write2LogFile("daylm", "Loginid:" & loginid)
            objDBase = DbaseServiceOLEDB.getInstance
            SQL_query = "update LogDetails set logoutstatus=" & logstatus & " where loginid = " & loginid & " and logoutstatus is null"
            objDBase.insertUpdateDelete(SQL_query)
        Catch ex As Exception
            ObjElog = LoggerService.gtInstance
            ObjElog.writeExceptionLogFile("DBException", ex)
        End Try

    End Sub
End Class
