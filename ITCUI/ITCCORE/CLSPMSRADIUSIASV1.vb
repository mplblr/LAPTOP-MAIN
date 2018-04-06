'############## Jan 11,2011-Class Description Start ############################
'-- This class is responsible for all the relogin/newlogin and renew process
'-- it exposes 3 methods for the same.
'-- It has one private method called DoADCheck to check AD user
'-- It has another private method to add active directory user
'############## Jan 11,2011-Class Description End ############################
Public Class CLSPMSRADIUSIASV1
    Implements IPMSService
    Const PMSName = "CLSPMSRADIUSIAS"  'CLS PMS RADIUS IAS
    Const PMSVersion = "4.0.0"
    Dim logroom As String
    Dim ADCred As New StructADCred

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is called for NEW Login purpose
    '-- The logic are as follows
    '-- First call the DoADCheck to create ad user if its not already created
    '-- Call the GatewayServiceFactory.Add method to raise radius login using the aduser and password
    '-- This method will return instance of GatewayResults
    '-- If the Gatewayresults.gtstatus is OK then go insie other wise return error
    '-- Raise Bill by calling BillService.RaiseBill2Days
    '-- Insert records in LogDetails by calling login2Days
    '-- Insert records in Users by calling Radius_Users
    '-- Insert record in Macdetails by calling InsertMacDetailsByACCID
    '-- Update ACCESSUSAGE status as A by calling SetAccessDetails
    '-- Update bandwidthup and bandwidthdown by calling GatewayService update method
    '-- If all successfull return Success
    '############## Jan 11,2011-Method Description End ##############################

    Public Function newLogin(ByRef userContext As UserContext) As String Implements IPMSService.newLogin
        Dim iGateway As IGatewayService
        Dim iGatewayFact As GatewayServiceFactory
        Dim iGatewayResult As GatewayResults
        Dim objBill As BillService
        Dim objLogInOut As LoginService
        Dim objSysConfig As New CSysConfig
        Dim objPlan As New CPlan
        Dim Objlog As LoggerService
        Dim billId, loginId As Long
        Dim ObjMacInfo As MACINFO
        Dim ObjAccCode As AccessCode

        objlog = LoggerService.gtInstance


        Try
            Objlog.write2LogFile("sip3=", userContext.item("sip"))
        Catch ex As Exception

        End Try


        'New Login Process Start ##########################

        '$$$$$$$$$$$$$$$$ Start With Only ACCESSCODE 26 OCT $$$$$$$$$$$$$$$$$$$$$$
        Try
            logroom = userContext.roomNo
            If userContext.selectedPlanId <= 0 Then Return "NOPLAN"
            userContext.item("logintype") = LOGINTYPE.NEWLOGIN

            '--------------------------------------------------------------------------------
            'Check whether the Access Code is Active
            Try
                Dim isActive As Boolean = AccessCode.IsAccessCodeActive(userContext.AccessID)
                If isActive = False Then
                    Return "COUPONDEACTIVE"
                End If

            Catch ex As Exception

            End Try
            '--------------------------------------------------------------------------------

            If DoADCheck(userContext) = True Then
                '########## Start NEW LOGIN PROCESS #################
                iGatewayFact = GatewayServiceFactory.getInstance
                iGateway = iGatewayFact.getGatewayService(PMSName, PMSVersion)

                iGatewayResult = iGateway.add(userContext)

                ObjAccCode = New AccessCode()
                If UCase(iGatewayResult.gtStatus) = "OK" Then '----- Login SuccessFul-----
                    userContext.item("logintime") = Now

                    objBill = BillService.getInstance

                    billId = objBill.RaiseBill2Days(userContext)
                    If billId > 0 Then
                        userContext.item("billid") = billId
                        objLogInOut = LoginService.getInstance
                        ObjMacInfo = MACINFO.getInstance


                        Try
                            loginId = objLogInOut.login2Days(userContext, 1)
                        Catch ex As Exception

                        End Try






                        objLogInOut.Radius_Users(userContext)
                        Dim AccessType As Integer
                        AccessType = CInt(userContext.item("accesstype"))
                        ObjMacInfo.InsertMacDetailsByACCID(userContext.roomNo, userContext.machineId, AccessType, userContext.GuestID, userContext.AccessID, 1)
                        ObjAccCode.SetAccessDetails(userContext.AccessID)

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
                        billId = objBill.RaiseBill2Days(userContext)
                        If billId > 0 Then
                            userContext.item("billid") = billId
                            objLogInOut = LoginService.getInstance
                            ObjMacInfo = MACINFO.getInstance
                            loginId = objLogInOut.login2Days(userContext, 1)
                            objLogInOut.Radius_Users(userContext)
                            Dim AccessType As Integer
                            AccessType = CInt(userContext.item("accesstype"))
                            ObjMacInfo.InsertMacDetailsByACCID(userContext.roomNo, userContext.machineId, AccessType, userContext.GuestID, userContext.AccessID, 1)
                            ObjAccCode.SetAccessDetails(userContext.AccessID)

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
        '$$$$$$$$$$$$$$$$ End With Only ACCESSCODE 26 OCT $$$$$$$$$$$$$$$$$$$$$$
        'New Login Process End ############################
    End Function


    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is called for Re Login purpose
    '-- The logic are as follows
    '-- First call the DoADCheck to create ad user if its not already created
    '-- Call the GatewayServiceFactory.Add method to raise radius login using the aduser and password
    '-- This method will return instance of GatewayResults
    '-- If the Gatewayresults.gtstatus is OK then go insie other wise return error
    '-- Insert records in LogDetails by calling login2Days
    '-- Insert records in Users by calling Radius_Users
    '-- Insert record in Macdetails by calling InsertMacDetailsByACCID
    '-- Update bandwidthup and bandwidthdown by calling GatewayService update method
    '-- If all successfull return Success
    '############## Jan 11,2011-Method Description End ##############################

    Public Function reLogin(ByRef userContext As UserContext) As String Implements IPMSService.reLogin

        Dim loginId As String
        Dim iGateway As IGatewayService
        Dim iGatewayFact As GatewayServiceFactory
        Dim iGatewayResult As GatewayResults
        Dim objLogInOut As LoginService
        Dim Objlog As LoggerService
        Dim ObjMacInfo As MACINFO

        Try
            Objlog = LoggerService.gtInstance
            Objlog.write2LogFile("sip3=", userContext.item("sip"))
        Catch ex As Exception

        End Try
        '$$$$$$$$$$$$$$$$ Start With Only ACCESSCODE 26 OCT $$$$$$$$$$$$$$$$$$$$$$
        Try
            logroom = userContext.roomNo
            userContext.item("logintype") = LOGINTYPE.RELOGIN

            '--------------------------------------------------------------------------------
            'Check whether the Access Code is Active
            Try
                Dim isActive As Boolean = AccessCode.IsAccessCodeActive(userContext.AccessID)
                If isActive = False Then
                    Return "COUPONDEACTIVE"
                End If

            Catch ex As Exception

            End Try
            '--------------------------------------------------------------------------------

            If DoADCheck(userContext) = True Then
                '########## Start NEW LOGIN PROCESS #################
                iGatewayFact = GatewayServiceFactory.getInstance
                iGateway = iGatewayFact.getGatewayService(PMSName, PMSVersion)
                iGatewayResult = iGateway.add(userContext)
                If UCase(iGatewayResult.gtStatus) = "OK" Then '----- Login SuccessFul-----
                    userContext.item("logintime") = Now
                    objLogInOut = LoginService.getInstance
                    ObjMacInfo = MACINFO.getInstance
                    loginId = objLogInOut.login2Days(userContext, 1)
                    objLogInOut.Radius_Users(userContext)
                    Dim AccessType As Integer
                    AccessType = CInt(userContext.item("accesstype"))
                    ObjMacInfo.InsertMacDetailsByACCID(userContext.roomNo, userContext.machineId, AccessType, userContext.GuestID, userContext.AccessID, 1)

                    iGatewayFact = GatewayServiceFactory.getInstance
                    iGateway = iGatewayFact.getGatewayService("CLSPMSRADIUSIAS", "4.0.0")
                    iGatewayResult = iGateway.update(userContext)
                    Return "Success"

                ElseIf UCase(iGatewayResult.gtStatus) = "TIMEDOUT" Then '--- Login Timeout----
                    'LOGIN ERROR LOG Start -------------------------------------------------------------
                    Objlog = LoggerService.gtInstance
                    Objlog.write2LogFile(logroom & "NDxResErr", Now() & "NDX TIMEOUT ReLogin: " & userContext.machineId & _
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
                        objLogInOut = LoginService.getInstance
                        ObjMacInfo = MACINFO.getInstance
                        loginId = objLogInOut.login2Days(userContext, 1)
                        objLogInOut.Radius_Users(userContext)
                        Dim AccessType As Integer
                        AccessType = CInt(userContext.item("accesstype"))
                        ObjMacInfo.InsertMacDetailsByACCID(userContext.roomNo, userContext.machineId, AccessType, userContext.GuestID, userContext.AccessID, 1)

                        iGatewayFact = GatewayServiceFactory.getInstance
                        iGateway = iGatewayFact.getGatewayService("CLSPMSRADIUSIAS", "4.0.0")
                        iGatewayResult = iGateway.update(userContext)
                        Return "Success"
                    End If
                    'LOGIN ERROR LOG Start -------------------------------------------------------------
                    Objlog = LoggerService.gtInstance
                    Objlog.write2LogFile(logroom & "NDxResErr", Now() & "NDX ERROR ReLogin: " & userContext.machineId & _
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
                    Objlog.write2LogFile(logroom & "NDxResErr", Now() & "NDX ERROR ReLogin: " & userContext.machineId & _
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
                Objlog.write2LogFile(userContext.roomNo & "NDxResErr", Now() & "Authentication Error ReLogin: " & userContext.machineId & _
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
            objLogInOut = Nothing
            iGatewayResult = Nothing
            iGatewayFact = Nothing
            ObjMacInfo = Nothing
        End Try
        '$$$$$$$$$$$$$$$$ End With Only ACCESSCODE 26 OCT $$$$$$$$$$$$$$$$$$$$$$
    End Function


    Public Function reNew(ByRef userContext As UserContext) As String Implements IPMSService.reNew

        'Dim objNdxGatewayResults As New NdxGatewayResults
        'Dim loginId, billId As Long
        'Dim iGateway As IGatewayService
        'Dim iGatewayFact As GatewayServiceFactory
        'Dim iGatewayResult As GatewayResults
        'Dim objBill As BillService
        'Dim objLogInOut As LoginService
        'Dim Objlog As LoggerService

        'Try
        '    logroom = userContext.roomNo
        '    userContext.item("accessgrant") = Nothing
        '    iGatewayFact = GatewayServiceFactory.getInstance
        '    iGateway = iGatewayFact.getGatewayService(PMSName, PMSVersion)

        '    If userContext.item("logintype") = LOGINTYPE.RENEW Then

        '        '----------------- START Re-new  ----------------------------------------------

        '        If DoAuthentication(userContext) Then
        '            iGatewayFact = GatewayServiceFactory.getInstance
        '            iGateway = iGatewayFact.getGatewayService(PMSName, PMSVersion)
        '            iGatewayResult = iGateway.add(userContext)
        '            If UCase(iGatewayResult.gtStatus) = "OK" Then '----- Login SuccessFul-----
        '                userContext.item("logintime") = Now
        '                objBill = BillService.getInstance
        '                billId = objBill.RaiseBill2Days(userContext)
        '                If billId > 0 Then
        '                    userContext.item("billid") = billId
        '                    objLogInOut = LoginService.getInstance
        '                    loginId = objLogInOut.login2Days(userContext, 1)
        '                    objLogInOut.Radius_Users(userContext)
        '                    Return "Success"
        '                Else
        '                    Return "APPERROR"
        '                End If
        '            ElseIf UCase(iGatewayResult.gtStatus) = "TIMEDOUT" Then '--- Login Timeout----

        '                'LOGIN ERROR LOG Start -------------------------------------------------------------
        '                Objlog = LoggerService.gtInstance
        '                Objlog.write2LogFile(logroom & "NDxResErr", Now() & "NDX TIMEOUT ReNew: " & userContext.machineId & _
        '                                         vbCrLf & "------Room No: " & logroom & _
        '                                         vbCrLf & "------GtErrorNo: " & iGatewayResult.gtErrorNo & _
        '                                         vbCrLf & "---GtMessage: " & iGatewayResult.gtErrorMsg)
        '                'LOGIN ERROR LOG End -------------------------------------------------------------
        '                objLogInOut = LoginService.getInstance
        '                objLogInOut.loginfail(userContext, "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg)
        '                Return "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg

        '            ElseIf UCase(iGatewayResult.gtStatus) = "ERROR" Then '--- LoginError--------
        '                If UCase(iGatewayResult.gtErrorMsg) = "USER IS ALREADY VALID" Then '--- Login SuccessFull -------------
        '                    userContext.item("logintime") = Now
        '                    objBill = BillService.getInstance
        '                    billId = objBill.RaiseBill2Days(userContext)
        '                    If billId > 0 Then
        '                        userContext.item("billid") = billId
        '                        objLogInOut = LoginService.getInstance
        '                        loginId = objLogInOut.login2Days(userContext, 1)
        '                        objLogInOut.Radius_Users(userContext)
        '                        Return "Success"
        '                    Else
        '                        Return "APPERROR"
        '                    End If
        '                    'LOGIN ERROR LOG Start -------------------------------------------------------------
        '                    Objlog = LoggerService.gtInstance
        '                    Objlog.write2LogFile(logroom & "NDxResErr", Now() & "NDX ERROR ReNew: " & userContext.machineId & _
        '                                             vbCrLf & "------Room No: " & logroom & _
        '                                             vbCrLf & "------GtErrorNo: " & iGatewayResult.gtErrorNo & _
        '                                             vbCrLf & "---GtMessage: " & iGatewayResult.gtErrorMsg)
        '                    'LOGIN ERROR LOG End -------------------------------------------------------------
        '                    objLogInOut = LoginService.getInstance
        '                    objLogInOut.loginfail(userContext, "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg)
        '                    Return "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg
        '                Else '---- Error Unknown -----------------

        '                    'LOGIN ERROR LOG Start -------------------------------------------------------------
        '                    Objlog = LoggerService.gtInstance
        '                    Objlog.write2LogFile(logroom & "NDxResErr", Now() & "NDX ERROR ReNew: " & userContext.machineId & _
        '                                             vbCrLf & "------Room No: " & logroom & _
        '                                             vbCrLf & "------GtErrorNo: " & iGatewayResult.gtErrorNo & _
        '                                             vbCrLf & "---GtMessage: " & iGatewayResult.gtErrorMsg)
        '                    'LOGIN ERROR LOG End -------------------------------------------------------------
        '                    objLogInOut = LoginService.getInstance
        '                    objLogInOut.loginfail(userContext, "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg)
        '                    Return "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg

        '                End If
        '            End If
        '        Else
        '            'DoAuth Failure ========================
        '            Dim tmsg As String
        '            tmsg = "Invalid Roomno or Password"
        '            'End If
        '            'LOGIN ERROR LOG Start -------------------------------------------------------------
        '            Objlog = LoggerService.gtInstance
        '            Objlog.write2LogFile(userContext.roomNo & "NDxResErr", Now() & "Authentication Error ReNew: " & userContext.machineId & _
        '                     vbCrLf & "------Room No: " & userContext.roomNo & _
        '                     vbCrLf & "------Ad UserID: " & userContext.item("aduserid") & _
        '                     vbCrLf & "---Ad Password: " & userContext.item("adpassword"))
        '            'LOGIN ERROR LOG End -------------------------------------------------------------
        '            objLogInOut = LoginService.getInstance
        '            objLogInOut.loginfail(userContext, tmsg)
        '            Return tmsg
        '        End If
        '        '------------------ END Re-new  ------------------------------------------
        '    Else
        '        iGatewayResult = iGateway.update(userContext)
        '        If UCase(iGatewayResult.gtStatus) = "OK" Then
        '            userContext.item("logintime") = Now
        '            userContext.item("accessgrant") = 1
        '            userContext.item("planid") = userContext.selectedPlanId
        '            objBill = BillService.getInstance
        '            billId = objBill.RaiseBill2Days(userContext)
        '            If billId = -1 Then
        '                Return "ERROR"
        '            End If
        '            userContext.item("billid") = billId

        '            objLogInOut = LoginService.getInstance
        '            loginId = objLogInOut.login2Days(userContext, 1)
        '            objLogInOut.Radius_Users(userContext)
        '            Return "Success"

        '        ElseIf UCase(iGatewayResult.gtStatus) = "TIMEDOUT" Then
        '            'LOGIN ERROR LOG Start -------------------------------------------------------------
        '            Objlog = LoggerService.gtInstance
        '            Objlog.write2LogFile(userContext.roomNo & "NDxResErr", Now() & "TIMEOUT UpGrade: " & userContext.machineId & _
        '                                     vbCrLf & "------Room No: " & userContext.roomNo & _
        '                                     vbCrLf & "------GtErrorNo: " & iGatewayResult.gtErrorNo & _
        '                                     vbCrLf & "---GtMessage: " & iGatewayResult.gtErrorMsg)
        '            'LOGIN ERROR LOG End -------------------------------------------------------------
        '            Return "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg

        '        ElseIf UCase(iGatewayResult.gtStatus) = "ERROR" Then
        '            'LOGIN ERROR LOG Start -------------------------------------------------------------
        '            Objlog = LoggerService.gtInstance
        '            Objlog.write2LogFile(userContext.roomNo & "NDxResErr", Now() & "ERROR UpGrade: " & userContext.machineId & _
        '                                     vbCrLf & "------Room No: " & userContext.roomNo & _
        '                                     vbCrLf & "------GtErrorNo: " & iGatewayResult.gtErrorNo & _
        '                                     vbCrLf & "---GtMessage: " & iGatewayResult.gtErrorMsg)
        '            'LOGIN ERROR LOG End -------------------------------------------------------------
        '            Return "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg
        '        Else
        '            Return "GtErrorNo: " & iGatewayResult.gtErrorNo & " GtMessage: " & iGatewayResult.gtErrorMsg
        '        End If

        '    End If
        'Catch ex As Exception
        '    Objlog = LoggerService.gtInstance
        '    Objlog.writeExceptionLogFile(logroom, ex)
        'End Try
        Return "ERROR"
    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method deals with aduser and adpassword
    '-- It does following tasks:
    '-- First it checks ACC_ADUID,ACC_ADPWD present in ACCESSUSAGE based on the ACCID
    '-- If present then update the usercontext aduserid and adpassword item with corresponding values
    '-- If not then call AddADUser to create an Aduser same as AccessCode without -
    '-- Update the ACCESSUSAGE ACC_ADUID,ACC_ADPWD columns with the newly created aduserid and password
    '-- update the usercontext aduserid and adpassword item with corresponding values
    '############## Jan 11,2011-Method Description End   ############################
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

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to create Active Directory User 
    '-- It does the following tasks:
    '-- call ActiveUtil.ActiveUser.AddAdUser with MAC and roomno as parameter
    '-- if it returns true then return the newly created AduserId and Password
    '-- otherwise return itcguest as aduser and Pp123456 as password as default
    '############## Jan 11,2011-Method Description End   ############################
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



End Class



