'############## Jan 11,2011-Class Description Start ############################
'-- This class is the entry point for all kind of login process
'-- The idea of such a class to encapsulate the login process from the user interface
'-- It exposes two public method AAA and GetOnlyRemainTime
'-- It contains some private method getRemainingTime, getRoomRemainTime and remainingTime
'############## Jan 11,2011-Class Description End   ############################

Imports System.Data.Common
Imports ITCCORE.Microsense.CodeBase
Public Class AAAService
    Private Shared gtAAAServiceInst As AAAService = Nothing
    Dim mac, guestid, ReceiveBytes As String
    Dim ADCred As New StructADCred

    Private Sub New()
        'nothing
    End Sub

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to call getRoomRemainTime
    '-- In later version this method can be made obsolute and called 
    '-- getRoomRemainTime directly itself
    Private Function getRemainingTime(ByRef usercontext As UserContext) As Long
        Return getRoomRemainTime(usercontext)
    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to create a instance of the class
    '-- This method will return a object of class AAA
    Public Shared Function getInstance() As AAAService
        If gtAAAServiceInst Is Nothing Then gtAAAServiceInst = New AAAService
        Return gtAAAServiceInst
    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is the staring point for all login process
    '-- It will first check if the particular ACCID wants to access more than 3 devices (configured by MaxDeviceCountForBillSharing in Web.config)
    '-- It will call getRemainingTime to get the remaining time for particular user details
    '-- If the remainingtime is less than 0 then it will consider it as new login and PMS.newLogin will be called
    '-- If the remainingtime is less than 0 then it will call PMS.ReLogin method
    '-- It will return the response based on what PMS.newLogin/ReLogin returns
    '############## Jan 11,2011-Method Description End   ############################
    Public Function AAA(ByRef userContext As UserContext) As String
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start =====================================
        Dim Rtime As Long
        Dim PMS As IPMSService
        Dim PMSFact As PMSServiceFatory
        Dim LogRoomNo As String
        Dim ObjLog As LoggerService
        Dim ObjMacInfo As MACINFO
        'Variable Declaration End =====================================

        Try
            ObjMacInfo = MACINFO.getInstance
            If ObjMacInfo.CheckMACByACCID(userContext.AccessID, userContext.machineId, userContext.item("accesstype")) = LOGINTYPE.APPEROR Then
                Return "ACCEXCEED"
            End If

            Rtime = getRemainingTime(userContext) ' Get RemainTime For Particular Guest
            'Get Proper PMS Service start ======================
            PMSFact = PMSServiceFatory.getInstance
            PMS = PMSFact.getPMSService()
            'Get Proper PMS Service End ======================

            ObjLog = LoggerService.gtInstance
            LogRoomNo = userContext.roomNo
            If Rtime <= 0 Then
                ObjLog.write2LogFile(LogRoomNo, "====== NEW Login Process start ======" & vbNewLine)
                userContext.item("logintype") = LOGINTYPE.NEWLOGIN
                Return PMS.newLogin(userContext)
            Else
                ObjLog.write2LogFile(LogRoomNo, "====== ReLogin Process start for GuestId:" & userContext.item("grcid") & "::======" & vbNewLine)
                userContext.item("logintype") = LOGINTYPE.RELOGIN
                Return PMS.reLogin(userContext)
            End If
        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("AAAEXP", ex)
            Return "APPERROR"
        End Try
    End Function


    Public Function AAAA(ByRef userContext As UserContext) As String

        Dim objlog As LoggerService
        objlog = LoggerService.gtInstance

        Dim iGateway As IGatewayService
        Dim iGatewayFact As GatewayServiceFactory
        Dim iGatewayResult As GatewayResults
        Dim objBill As BillService
        Dim objLogInOut As LoginService
        Dim objSysConfig As New CSysConfig
        Dim objPlan As New CPlan
        ' Dim Objlog As LoggerService
        Dim billId, loginId As Long
        Dim ObjMacInfo As MACINFO
        Dim ObjAccCode As AccessCode
        Const PMSName = "CLSPMSRADIUSIAS"  'CLS PMS RADIUS IAS
        Const PMSVersion = "4.0.0"
        Dim logroom As String
        Dim ADCred As New StructADCred

        logroom = userContext.roomNo
        Try
            Dim isActive As Boolean = AccessCode.IsAccessCodeActive(userContext.AccessID)
            If isActive = False Then
                Return "COUPONDEACTIVE"
            End If

        Catch ex As Exception

        End Try
        Try
            '---------
            If DoADCheck(userContext) = True Then
                '########## Start NEW LOGIN PROCESS #################
                iGatewayFact = GatewayServiceFactory.getInstance
                iGateway = iGatewayFact.getGatewayService(PMSName, PMSVersion)

                iGatewayResult = iGateway.add(userContext)

                ObjAccCode = New AccessCode()
                If UCase(iGatewayResult.gtStatus) = "OK" Then '----- Login SuccessFul-----
                    userContext.item("logintime") = Now

                    objBill = BillService.getInstance



                    Try
                        billId = userContext.item("bid")
                    Catch ex As Exception

                    End Try
                    Try
                        ' objlog.write2LogFile("Bill_a", billId)
                    Catch ex As Exception

                    End Try


                    If billId > 0 Then
                        userContext.item("billid") = billId
                        objLogInOut = LoginService.getInstance
                        ObjMacInfo = MACINFO.getInstance
                        loginId = objLogInOut.login2Days1(userContext, 1)
                        objLogInOut.Radius_Users1(userContext)
                        'Dim AccessType As Integer
                        'AccessType = CInt(userContext.item("accesstype"))
                        'ObjMacInfo.InsertMacDetailsByACCID(userContext.roomNo, userContext.machineId, AccessType, userContext.GuestID, userContext.AccessID, 1)
                        'ObjAccCode.SetAccessDetails(userContext.AccessID)

                        iGatewayFact = GatewayServiceFactory.getInstance
                        iGateway = iGatewayFact.getGatewayService("CLSPMSRADIUSIAS", "4.0.0")
                        iGatewayResult = iGateway.update(userContext)
                        Return "Success"
                    Else
                        Return "APPERROR"
                    End If
                ElseIf UCase(iGatewayResult.gtStatus) = "TIMEDOUT" Then '--- Login Timeout----
                    'LOGIN ERROR LOG Start -------------------------------------------------------------
                    Objlog = LoggerService.gtInstance
                    Objlog.write2LogFile(logroom & "NDxResErr", Now() & "NDX TIMEOUT NewLogin: " & userContext.machineId & _
                                             vbCrLf & "------Room No: " & logroom & _
                                             vbCrLf & "------GtErrorNo: " & iGatewayResult.gtErrorNo & _
                                             vbCrLf & "---GtMessage: " & iGatewayResult.gtErrorMsg)
                    'LOGIN ERROR LOG End -------------------------------------------------------------
                    objLogInOut = LoginService.getInstance
                    objLogInOut.loginfail(userContext, "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg)
                    Return "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg
                ElseIf UCase(iGatewayResult.gtStatus) = "ERROR" Then '--- LoginError--------

                    If UCase(iGatewayResult.gtErrorMsg) = "USER IS ALREADY VALID" Then '--- Login SuccessFull -------------
                        userContext.item("logintime") = Now
                        objBill = BillService.getInstance
                        Try
                            billId = userContext.item("bid")
                        Catch ex As Exception

                        End Try
                        Try
                            'objlog.write2LogFile("Bill_b", billId)
                        Catch ex As Exception

                        End Try



                        If billId > 0 Then
                            userContext.item("billid") = billId
                            objLogInOut = LoginService.getInstance
                            ObjMacInfo = MACINFO.getInstance
                            loginId = objLogInOut.login2Days1(userContext, 1)
                            objLogInOut.Radius_Users1(userContext)
                            'Dim AccessType As Integer
                            'AccessType = CInt(userContext.item("accesstype"))
                            'ObjMacInfo.InsertMacDetailsByACCID(userContext.roomNo, userContext.machineId, AccessType, userContext.GuestID, userContext.AccessID, 1)
                            'ObjAccCode.SetAccessDetails(userContext.AccessID)

                            iGatewayFact = GatewayServiceFactory.getInstance
                            iGateway = iGatewayFact.getGatewayService("CLSPMSRADIUSIAS", "4.0.0")
                            iGatewayResult = iGateway.update(userContext)
                            Return "Success"
                        Else
                            Return "APPERROR"
                        End If
                    End If

                    'LOGIN ERROR LOG Start -------------------------------------------------------------
                    Objlog = LoggerService.gtInstance
                    Objlog.write2LogFile(logroom & "NDxResErr", Now() & "NDX ERROR NewLogin: " & userContext.machineId & _
                                             vbCrLf & "------Room No: " & logroom & _
                                             vbCrLf & "------GtErrorNo: " & iGatewayResult.gtErrorNo & _
                                             vbCrLf & "---GtMessage: " & iGatewayResult.gtErrorMsg)
                    'LOGIN ERROR LOG End -------------------------------------------------------------
                    objLogInOut = LoginService.getInstance
                    objLogInOut.loginfail(userContext, "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg)
                    Return "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg

                Else
                    'LOGIN ERROR LOG Start -------------------------------------------------------------
                    Objlog = LoggerService.gtInstance
                    Objlog.write2LogFile(logroom & "NDxResErr", Now() & "NDX ERROR NewLogin: " & userContext.machineId & _
                                             vbCrLf & "------Room No: " & logroom & _
                                             vbCrLf & "------GtErrorNo: " & iGatewayResult.gtErrorNo & _
                                             vbCrLf & "---GtMessage: " & iGatewayResult.gtErrorMsg)
                    'LOGIN ERROR LOG End -------------------------------------------------------------
                    objLogInOut = LoginService.getInstance
                    objLogInOut.loginfail(userContext, "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg)
                    Return "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg
                End If
                '########## End NEW LOGIN PROCESS ###################
            Else
                '################# AD Authentication Fail#######################
                'DoAuth Failure ========================
                Dim tmsg As String
                tmsg = "Invalid Roomno or Password"
                'End If
                'LOGIN ERROR LOG Start -------------------------------------------------------------
                Objlog = LoggerService.gtInstance
                Objlog.write2LogFile(userContext.roomNo & "NDxResErr", Now() & "Authentication Error NewLogin: " & userContext.machineId & _
                         vbCrLf & "------Room No: " & userContext.roomNo & _
                         vbCrLf & "------Ad UserID: " & userContext.item("aduserid") & _
                         vbCrLf & "---Ad Password: " & userContext.item("adpassword"))
                'LOGIN ERROR LOG End -------------------------------------------------------------
                objLogInOut = LoginService.getInstance
                objLogInOut.loginfail(userContext, tmsg)
                Return tmsg
            End If

        Catch ex As Exception
            Objlog = LoggerService.gtInstance
            Objlog.writeExceptionLogFile(logroom & "_EXP", ex)
        Finally
            objBill = Nothing
            objLogInOut = Nothing
            iGatewayResult = Nothing
            iGatewayFact = Nothing
            ObjMacInfo = Nothing
        End Try




    End Function

    Public Function getamt(ByVal bid As Long) As Integer
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "select billpostedamount from bill where billid=@PPP"
            DatabaseUtil.AddInputParameter(com, "@PPP", DbType.String, bid.ToString())
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

    Public Function updaterenew(ByVal bid As Long, ByVal old As Integer) As Integer
        Try
            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "update  renewalpay set billid =@HHH where billid=@PPP "
            DatabaseUtil.AddInputParameter(com, "@PPP", DbType.String, old.ToString())
            DatabaseUtil.AddInputParameter(com, "@HHH", DbType.String, bid.ToString())

            DatabaseUtil.ExecuteInsertUpdateDelete(com)




        Catch ex As Exception
            Return 0
        End Try

    End Function

    Public Function AAAAA(ByRef userContext As UserContext) As String

        Dim objlog As LoggerService
        objlog = LoggerService.gtInstance

        Dim iGateway As IGatewayService
        Dim iGatewayFact As GatewayServiceFactory
        Dim iGatewayResult As GatewayResults
        Dim objBill As BillService
        Dim objLogInOut As LoginService
        Dim objSysConfig As New CSysConfig
        Dim objPlan As New CPlan
        ' Dim Objlog As LoggerService
        Dim billId, loginId As Long
        Dim ObjMacInfo As MACINFO
        Dim ObjAccCode As AccessCode
        Const PMSName = "CLSPMSRADIUSIAS"  'CLS PMS RADIUS IAS
        Const PMSVersion = "4.0.0"
        Dim logroom As String
        Dim ADCred As New StructADCred
        Dim RefDataTable As DataTable
        Dim billpostedamount As Long = 0
        Dim oldbillid As Long = 0

        logroom = userContext.roomNo
        Try
            Dim isActive As Boolean = AccessCode.IsAccessCodeActive(userContext.AccessID)
            If isActive = False Then
                Return "COUPONDEACTIVE"
            End If

        Catch ex As Exception

        End Try
        Try
            '---------
            If DoADCheck(userContext) = True Then
                '########## Start NEW LOGIN PROCESS #################
                iGatewayFact = GatewayServiceFactory.getInstance
                iGateway = iGatewayFact.getGatewayService(PMSName, PMSVersion)

                iGatewayResult = iGateway.add(userContext)

                ObjAccCode = New AccessCode()
                If UCase(iGatewayResult.gtStatus) = "OK" Then '----- Login SuccessFul-----
                    userContext.item("logintime") = Now

                    objBill = BillService.getInstance



                    Try
                        billId = userContext.item("bid")

                        billpostedamount = getamt(billId)
                        oldbillid = billId

                        billId = objBill.RaiseBill2Days2(userContext)

                    Catch ex As Exception

                    End Try
                    Try
                        'objlog.write2LogFile("Bill_a", billId)
                    Catch ex As Exception

                    End Try


                    If billId > 0 Then

                        Try
                            updaterenew(billId, oldbillid)
                        Catch ex As Exception

                        End Try

                        userContext.item("billid") = billId
                        objLogInOut = LoginService.getInstance
                        ObjMacInfo = MACINFO.getInstance
                        loginId = objLogInOut.login2Days2(userContext, 1)
                        objLogInOut.Radius_Users2(userContext)
                        'Dim AccessType As Integer
                        'AccessType = CInt(userContext.item("accesstype"))
                        'ObjMacInfo.InsertMacDetailsByACCID(userContext.roomNo, userContext.machineId, AccessType, userContext.GuestID, userContext.AccessID, 1)
                        'ObjAccCode.SetAccessDetails(userContext.AccessID)

                        iGatewayFact = GatewayServiceFactory.getInstance
                        iGateway = iGatewayFact.getGatewayService("CLSPMSRADIUSIAS", "4.0.0")
                        iGatewayResult = iGateway.update(userContext)
                        Return "Success"
                    Else
                        Return "APPERROR"
                    End If
                ElseIf UCase(iGatewayResult.gtStatus) = "TIMEDOUT" Then '--- Login Timeout----
                    'LOGIN ERROR LOG Start -------------------------------------------------------------
                    objlog = LoggerService.gtInstance
                    objlog.write2LogFile(logroom & "NDxResErr", Now() & "NDX TIMEOUT NewLogin: " & userContext.machineId & _
                                             vbCrLf & "------Room No: " & logroom & _
                                             vbCrLf & "------GtErrorNo: " & iGatewayResult.gtErrorNo & _
                                             vbCrLf & "---GtMessage: " & iGatewayResult.gtErrorMsg)
                    'LOGIN ERROR LOG End -------------------------------------------------------------
                    objLogInOut = LoginService.getInstance
                    objLogInOut.loginfail(userContext, "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg)
                    Return "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg
                ElseIf UCase(iGatewayResult.gtStatus) = "ERROR" Then '--- LoginError--------

                    If UCase(iGatewayResult.gtErrorMsg) = "USER IS ALREADY VALID" Then '--- Login SuccessFull -------------
                        userContext.item("logintime") = Now
                        objBill = BillService.getInstance
                        Try
                            billId = userContext.item("bid")
                            billpostedamount = getamt(billId)
                            oldbillid = billId

                            billId = objBill.RaiseBill2Days2(userContext)
                        Catch ex As Exception

                        End Try
                        Try
                            'objlog.write2LogFile("Bill_b", billId)
                        Catch ex As Exception

                        End Try



                        If billId > 0 Then

                            Try
                                updaterenew(billId, oldbillid)
                            Catch ex As Exception

                            End Try

                            userContext.item("billid") = billId
                            objLogInOut = LoginService.getInstance
                            ObjMacInfo = MACINFO.getInstance
                            loginId = objLogInOut.login2Days2(userContext, 1)
                            objLogInOut.Radius_Users2(userContext)
                            'Dim AccessType As Integer
                            'AccessType = CInt(userContext.item("accesstype"))
                            'ObjMacInfo.InsertMacDetailsByACCID(userContext.roomNo, userContext.machineId, AccessType, userContext.GuestID, userContext.AccessID, 1)
                            'ObjAccCode.SetAccessDetails(userContext.AccessID)

                            iGatewayFact = GatewayServiceFactory.getInstance
                            iGateway = iGatewayFact.getGatewayService("CLSPMSRADIUSIAS", "4.0.0")
                            iGatewayResult = iGateway.update(userContext)
                            Return "Success"
                        Else
                            Return "APPERROR"
                        End If
                    End If

                    'LOGIN ERROR LOG Start -------------------------------------------------------------
                    objlog = LoggerService.gtInstance
                    objlog.write2LogFile(logroom & "NDxResErr", Now() & "NDX ERROR NewLogin: " & userContext.machineId & _
                                             vbCrLf & "------Room No: " & logroom & _
                                             vbCrLf & "------GtErrorNo: " & iGatewayResult.gtErrorNo & _
                                             vbCrLf & "---GtMessage: " & iGatewayResult.gtErrorMsg)
                    'LOGIN ERROR LOG End -------------------------------------------------------------
                    objLogInOut = LoginService.getInstance
                    objLogInOut.loginfail(userContext, "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg)
                    Return "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg

                Else
                    'LOGIN ERROR LOG Start -------------------------------------------------------------
                    objlog = LoggerService.gtInstance
                    objlog.write2LogFile(logroom & "NDxResErr", Now() & "NDX ERROR NewLogin: " & userContext.machineId & _
                                             vbCrLf & "------Room No: " & logroom & _
                                             vbCrLf & "------GtErrorNo: " & iGatewayResult.gtErrorNo & _
                                             vbCrLf & "---GtMessage: " & iGatewayResult.gtErrorMsg)
                    'LOGIN ERROR LOG End -------------------------------------------------------------
                    objLogInOut = LoginService.getInstance
                    objLogInOut.loginfail(userContext, "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg)
                    Return "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg
                End If
                '########## End NEW LOGIN PROCESS ###################
            Else
                '################# AD Authentication Fail#######################
                'DoAuth Failure ========================
                Dim tmsg As String
                tmsg = "Invalid Roomno or Password"
                'End If
                'LOGIN ERROR LOG Start -------------------------------------------------------------
                objlog = LoggerService.gtInstance
                objlog.write2LogFile(userContext.roomNo & "NDxResErr", Now() & "Authentication Error NewLogin: " & userContext.machineId & _
                         vbCrLf & "------Room No: " & userContext.roomNo & _
                         vbCrLf & "------Ad UserID: " & userContext.item("aduserid") & _
                         vbCrLf & "---Ad Password: " & userContext.item("adpassword"))
                'LOGIN ERROR LOG End -------------------------------------------------------------
                objLogInOut = LoginService.getInstance
                objLogInOut.loginfail(userContext, tmsg)
                Return tmsg
            End If

        Catch ex As Exception
            objlog = LoggerService.gtInstance
            objlog.writeExceptionLogFile(logroom & "_EXP", ex)
        Finally
            objBill = Nothing
            objLogInOut = Nothing
            iGatewayResult = Nothing
            iGatewayFact = Nothing
            ObjMacInfo = Nothing
        End Try




    End Function

    Private Function DoADCheck(ByRef ucred As UserContext) As Boolean
        Dim SQL_query As String
        Dim objDbase As DbaseServiceOLEDB
        Dim ObjAccCode As AccessCode
        Dim RefResultset As DataSet
        Dim RefResultTable As DataTable
        Dim ACCCODE As String
        Try
            SQL_query = "SELECT ACC_ADUID,ACC_ADPWD,ACC_CODE FROM ACCESSUSAGE WHERE ID=" & ucred.AccessID
            objDbase = DbaseServiceOLEDB.getInstance
            RefResultset = objDbase.DsWithoutUpdate(SQL_query)
            RefResultTable = RefResultset.Tables(0)
            If RefResultTable.Rows.Count > 0 Then
                If IsDBNull(RefResultTable.Rows(0).Item("ACC_ADUID")) Or IsDBNull(RefResultTable.Rows(0).Item("ACC_ADPWD")) Then
                    ACCCODE = RefResultTable.Rows(0).Item("ACC_CODE").ToString()
                    ACCCODE = ACCCODE.Replace("-", "")

                    '------- Adding the Active Directory User ---------------------------'


                    Try
                        AddADUser(ACCCODE, ucred.roomNo)
                    Catch ex As Exception

                    End Try


                    ucred.item("aduserid") = ADCred.ADuserid
                    ucred.item("adpassword") = ADCred.ADpassword
                    '------------- START Update The ADUsername And ADPassword in ACCESSUSAGE
                    ObjAccCode = New AccessCode()
                    ObjAccCode.UpdateADUIDPWDByID(ADCred.ADuserid, ADCred.ADpassword, ucred.AccessID)
                Else
                    ucred.item("aduserid") = RefResultTable.Rows(0).Item("ACC_ADUID")
                    ucred.item("adpassword") = RefResultTable.Rows(0).Item("ACC_ADPWD")
                End If
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function AddADUser(ByVal MAC As String, ByVal roomno As String) As StructADCred
        Dim ADRes() As String
        Dim ObjElog As LoggerService
        Try
            ADRes = ActiveUtil.ActiveUser.AddAdUser(MAC, roomno).Split(",")
            'changes from MicrosenseMPL--------------------
            ObjElog = LoggerService.gtInstance
            ObjElog.write2LogFile("ADRES", Now & "---------" & String.Join(",", ADRes))

            If Not ADRes Is Nothing Then
                If UCase(ADRes(0)) = "TRUE" Then
                    ADCred.ADuserid = ADRes(1)
                    ADCred.ADpassword = ADRes(2)
                Else
                    ADCred.ADuserid = "itcguest"
                    ADCred.ADpassword = "Pp123456"
                End If
            Else
                ADCred.ADuserid = "itcguest"
                ADCred.ADpassword = "Pp123456"
            End If

            ActiveUtil.ActiveUser.closeEntry()

            Return ADCred

        Catch ex As Exception
            ObjElog = LoggerService.gtInstance
            ObjElog.writeExceptionLogFile(roomno, ex)
            ADCred.ADuserid = "itcguest"
            ADCred.ADpassword = "Pp123456"
            Return ADCred
        End Try
    End Function



    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to get the remainingtime based on the usecontext values
    '-- This is only used for In Room Guest
    '-- If the remaining time is available then it will populate following details in usercontext
    '-- expirytime,remainingtime,LoginPlanTime(remainingtime),remainingdays,GuestID,loginid
    '-- usercontext is used as pass by reference here to add some more info in current usercontext.
    '############## Jan 11,2011-Method Description End   ############################
    Private Function getRoomRemainTime(ByRef usercontext As UserContext) As Long
        'In USE 7 SEP 2010*************************
        'Variable Declaration Start =====================================
        Dim OldMAC As String
        Dim SQL_query As String = ""
        Dim Ctime, Itime, Otime, Etime As DateTime
        Dim Accesstime, GrCId, loginid, Rtime, BillId, PlanId As Long
        Dim AccessGranted As Boolean
        Dim logroom As String = "_"
        Dim RefResultset As DataSet
        Dim objDbase As DbaseServiceOLEDB
        Dim Objlog As LoggerService
        Dim no_ofdays As Integer
        'Variable Declaration End =====================================

        Try
            logroom = usercontext.roomNo
            Ctime = Now
            'Get Values from DB #########################################
            SQL_query = "SELECT GuestId,LogoutTime,LoginTime,LoginExpTime,LoginPlanTime,LoginBillId,BillPlanId,LoginAccessGranted," & _
                        " LoginMAC,LoginId,BilltmpNoofDays" & _
                        " FROM ((LogDetails INNER JOIN Bill ON LogDetails.LoginBillId = Bill.Billid) INNER JOIN Guest ON Guest.Guestid = Bill.BillGrCId) " & _
                        " WHERE (Bill.BillType = " & PMSBill.ROOM & " AND Guest.GuestRegCode = '" & usercontext.password & "' AND Bill.BillACCId=" & usercontext.AccessID & _
                        " AND Guest.GuestRoomNo = '" & usercontext.roomNo & "' AND LogDetails.LoginExpTime > '" & Ctime & "') ORDER BY  LogDetails.LoginId DESC"

            objDbase = DbaseServiceOLEDB.getInstance
            RefResultset = objDbase.DsWithoutUpdate(SQL_query)

            Try
                Objlog = LoggerService.gtInstance
                Objlog.write2LogFile("ss,", SQL_query)
            Catch ex As Exception

            End Try



            If RefResultset.Tables(0).Rows.Count > 0 Then
                GrCId = RefResultset.Tables(0).Rows(0).Item("GuestId")
                If Not IsDBNull(RefResultset.Tables(0).Rows(0).Item("LogoutTime")) Then
                    Otime = RefResultset.Tables(0).Rows(0).Item("LogoutTime")
                Else
                    Otime = #1/1/1970#
                End If
                Itime = RefResultset.Tables(0).Rows(0).Item("LoginTime")
                Etime = RefResultset.Tables(0).Rows(0).Item("LoginExpTime")
                Accesstime = RefResultset.Tables(0).Rows(0).Item("LoginPlanTime")
                BillId = RefResultset.Tables(0).Rows(0).Item("LoginBillId")
                PlanId = RefResultset.Tables(0).Rows(0).Item("BillPlanId")
                AccessGranted = RefResultset.Tables(0).Rows(0).Item("LoginAccessGranted")
                OldMAC = RefResultset.Tables(0).Rows(0).Item("LoginMAC")
                loginid = RefResultset.Tables(0).Rows(0).Item("LoginId")
                no_ofdays = RefResultset.Tables(0).Rows(0).Item("BilltmpNoofDays")

                'If AccessGranted Then
                '    Rtime = remainingTime(Ctime, Itime, Otime, Etime, Accesstime)
                'Else
                '    Rtime = Accesstime
                'End If

                Rtime = remainingTime(Ctime, Itime, Otime, Etime, Accesstime)

                Try
                    Try

                        Objlog = LoggerService.gtInstance
                        'Objlog.write2LogFile(usercontext.roomNo, "Rtime=" & Rtime)
                    Catch ex As Exception

                    End Try
                Catch ex As Exception

                End Try







                If Rtime > 0 Then
                    With usercontext
                        .item("lastlogintime") = Itime
                        .item("expirytime") = Etime
                        .item("remainingtime") = Rtime
                        .item("logintime") = Ctime
                        '---------Calculate Remaining Days Starts----------
                        Dim Rdays As Integer = Rtime \ 86400L
                        Dim tmprdr As Integer = Rtime Mod 86400L
                        If tmprdr > 0 Then Rdays = Rdays + 1
                        .item("remainingdays") = Rdays
                        .item("initnoofdays") = no_ofdays
                        .item("billid") = BillId
                        .GuestID = GrCId
                        .item("accessgrant") = AccessGranted
                        .item("oldmac") = OldMAC
                        .item("planid") = PlanId
                        .item("loginid") = loginid
                    End With
                End If

                Return Rtime
            Else
                Return 0
            End If
        Catch ex As Exception
            Objlog = LoggerService.gtInstance
            Objlog.writeExceptionLogFile(logroom, ex)
        Finally
            objDbase = Nothing
            RefResultset = Nothing
        End Try
    End Function

    Public Function GetOnlyRemainTime(ByRef userContext As UserContext) As Long
        Dim RTime As Long
        Dim SQL_query As String = ""
        Dim Ctime, Itime, Otime, Etime As DateTime
        Dim RefResultset As DataSet
        Dim objDbase As DbaseServiceOLEDB
        Dim Accesstime As Long
        Dim ObjLog As LoggerService
        Dim ObjMacInfo As MACINFO
        Ctime = Now()

        Try
            ObjMacInfo = MACINFO.getInstance
            If ObjMacInfo.CheckMACByACCID(userContext.AccessID, userContext.machineId, userContext.item("accesstype")) = LOGINTYPE.APPEROR Then
                userContext.item("logintype") = LOGINTYPE.APPEROR
                Return 0
            End If
            SQL_query = "SELECT LogoutTime,LoginTime,LoginExpTime,LoginPlanTime" & _
                        " FROM ((LogDetails INNER JOIN Bill ON LogDetails.LoginBillId = Bill.Billid) INNER JOIN Guest ON Guest.Guestid = Bill.BillGrCId) " & _
                        " WHERE (Bill.BillType = " & PMSBill.ROOM & " AND Guest.GuestRegCode = '" & userContext.password & "' AND Bill.BillACCId=" & userContext.AccessID & _
                        " AND Guest.GuestRoomNo = '" & userContext.roomNo & "' AND LogDetails.LoginExpTime > '" & Ctime & "') ORDER BY  LogDetails.LoginId DESC"


            objDbase = DbaseServiceOLEDB.getInstance
            RefResultset = objDbase.DsWithoutUpdate(SQL_query)
            If RefResultset.Tables(0).Rows.Count > 0 Then
                If Not IsDBNull(RefResultset.Tables(0).Rows(0).Item("LogoutTime")) Then
                    Otime = RefResultset.Tables(0).Rows(0).Item("LogoutTime")
                Else
                    Otime = #1/1/1970#
                End If
                Itime = RefResultset.Tables(0).Rows(0).Item("LoginTime")
                Etime = RefResultset.Tables(0).Rows(0).Item("LoginExpTime")
                Accesstime = RefResultset.Tables(0).Rows(0).Item("LoginPlanTime")
                RTime = remainingTime(Ctime, Itime, Otime, Etime, Accesstime)
            Else
                RTime = 0
            End If

            Return RTime

        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("GetOnlyRemianEXP", ex)
            RTime = 0
        End Try
    End Function

    Private Function remainingTime(ByVal Ctime As DateTime, ByVal Itime As DateTime, ByVal Otime As DateTime, ByVal Etime As DateTime, ByVal Ptime As Long) As Long
        Dim I2CSec, CrO2Esec, RSec, UsageSec As Long
        If Otime = #1/1/1970# Then
            I2CSec = DateDiff(DateInterval.Second, Itime, Ctime)
            RSec = Ptime - I2CSec
        Else
            UsageSec = DateDiff(DateInterval.Second, Itime, Otime)
            CrO2Esec = DateDiff(DateInterval.Second, Ctime, Etime)
            RSec = Ptime - UsageSec
            If RSec > CrO2Esec Then RSec = CrO2Esec
        End If
        If RSec < 0 Then Return 0
        Return RSec
    End Function
End Class
