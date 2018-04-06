Imports System.Threading

'############## Jan 11,2011-Class Description Start ############################
'-- This class is used to encapsulate the Nomadix command response
'-- and present the result in user readable format
'-- This mostly used by addMAC,Radius_Login() methods
'############## Jan 11,2011-Class Description End   ############################
Public Class NdxGatewayResults
    Inherits GatewayResults

    Dim nasId As String
    Dim respIP As String

    Sub New()
        MyBase.New()

        nasId = ""
        respIP = ""
    End Sub

    Sub New(ByVal XMLResponse As String)

        Me.New()

        Dim ndxGtResponse As Hashtable

        ndxGtResponse = ndxResponseParser(XMLResponse, nomadixCommand.ADD)

        initResults(ndxGtResponse)
        ndxGtResponse.Clear()
    End Sub

    Protected Overrides Sub initResults(ByRef ndxGtResponse As Hashtable)
        nasId = ndxGtResponse.Item("nomadixId")
        respIP = ndxGtResponse.Item("ndxResponseIP")

        'ndxGtResponse.Remove("nomadixId")
        'ndxGtResponse.Remove("nomadixIP")

        MyBase.initResults(ndxGtResponse)
    End Sub


    ReadOnly Property ndxNasId() As String
        Get
            Return nasId
        End Get
    End Property

    ReadOnly Property ndxResponseIP() As String
        Get
            Return respIP
        End Get
    End Property

    Public Shared Function ndxResponseParser(ByVal ndxXMLResponse As String, ByVal ndxCommand As nomadixCommand) As Hashtable
        Dim rStatus, rNasId, rRespIP, rErrorCode, rErrorMessage, rDataVal, rMAC, _
                rRoomNo, rPassword, rUserName, rExpireTime, rExpTimeUnits, rPayMethod, rUserStatus As String

        Dim ndxGtResponse As New Hashtable

        rStatus = "" : rErrorCode = "" : rErrorMessage = "" : rDataVal = "-1" : rNasId = "" : rRespIP = "" : rMAC = ""
        rRoomNo = "" : rPassword = "" : rUserName = "" : rExpireTime = "-1" : rExpTimeUnits = "-1" : rPayMethod = ""

        If ndxXMLResponse = "TimedOut" Then
            rStatus = ndxXMLResponse
            rErrorMessage = ndxXMLResponse
            rErrorCode = "-1"

        ElseIf ndxXMLResponse <> "" Then
            Dim ObjXMLParser As New MSXML.DOMDocument
            ObjXMLParser.async = False
            ObjXMLParser.loadXML(ndxXMLResponse)

            rStatus = ObjXMLParser.documentElement.attributes(0).text
            rNasId = ObjXMLParser.documentElement.attributes(1).text
            rRespIP = ObjXMLParser.documentElement.attributes(2).text

            If UCase(rStatus) = "OK" Then

                Select Case ndxCommand
                    Case nomadixCommand.AUTHORIZE
                        rUserStatus = ObjXMLParser.getElementsByTagName("STATUS").item(0).text
                        rPayMethod = ObjXMLParser.getElementsByTagName("PAYMENT_METHOD").item(0).text
                        rErrorCode = "0"
                        rErrorMessage = rUserStatus

                    Case nomadixCommand.QUERY
                        rMAC = ObjXMLParser.getElementsByTagName("MAC_ADDR").item(0).text
                        rUserName = ObjXMLParser.getElementsByTagName("USER_NAME").item(0).text
                        rPassword = ObjXMLParser.getElementsByTagName("PASSWORD").item(0).text
                        rExpireTime = ObjXMLParser.getElementsByTagName("EXPIRY_TIME").item(0).text
                        rExpTimeUnits = ObjXMLParser.getElementsByTagName("EXPIRY_TIME").item(0).attributes(0).text
                        rRoomNo = ObjXMLParser.getElementsByTagName("ROOM_NUMBER").item(0).text
                        rPayMethod = ObjXMLParser.getElementsByTagName("PAYMENT_METHOD").item(0).text
                        rDataVal = ObjXMLParser.getElementsByTagName("DATA_VOLUME").item(0).text
                        rErrorCode = "0"
                        rErrorMessage = "Success"
                    Case Else
                        rErrorCode = "0"
                        rErrorMessage = "Success"

                End Select


            Else
                rErrorCode = ObjXMLParser.getElementsByTagName("ERROR_NUM").item(0).text
                rErrorMessage = ObjXMLParser.getElementsByTagName("ERROR_DESC").item(0).text
            End If

        Else
            rStatus = "Nomdix Response is Empty"

        End If

        ndxGtResponse.Add("gtStatus", rStatus)
        ndxGtResponse.Add("gtErrorNo", rErrorCode)
        ndxGtResponse.Add("gtErrorMessage", rErrorMessage)
        ndxGtResponse.Add("gtDataVal", "-1")
        ndxGtResponse.Add("nomadixId", rNasId)
        ndxGtResponse.Add("ndxResponseIP", rRespIP)
        ndxGtResponse.Add("ndxUserStatus", rUserStatus)
        ndxGtResponse.Add("ndxPayMethod", rPayMethod)
        ndxGtResponse.Add("ndxUserName", rUserName)
        ndxGtResponse.Add("ndxRoomNo", rRoomNo)
        ndxGtResponse.Add("ndxPassword", rPassword)
        ndxGtResponse.Add("ndxExpireTime", rExpireTime)
        ndxGtResponse.Add("ndxExpTimeUnits", rExpTimeUnits)
        ndxGtResponse.Add("ndxDataValume", rDataVal)

        Return ndxGtResponse

    End Function
End Class

Public Class NdxAuthGatewayResults
    Inherits NdxGatewayResults

    Dim userStatus As String
    Dim payMethod As String

    Sub New()
        MyBase.New()

        userStatus = ""
        payMethod = ""
    End Sub

    Sub New(ByVal XMLResponse As String)
        Me.New()

        Dim ndxGtResponse As Hashtable

        ndxGtResponse = ndxResponseParser(XMLResponse, nomadixCommand.AUTHORIZE)
        userStatus = ndxGtResponse.Item("ndxUserStatus")
        payMethod = ndxGtResponse.Item("ndxPayMethod")

        'ndxGtResponse.Remove("ndxUserStatus")
        'ndxGtResponse.Remove("ndxPayMethod")

        MyBase.initResults(ndxGtResponse)
        ndxGtResponse.Clear()

    End Sub

    ReadOnly Property ndxUserStatus() As String
        Get
            Return userStatus
        End Get
    End Property

    ReadOnly Property ndxPayMethod() As String
        Get
            Return payMethod
        End Get
    End Property
End Class

'############## Jan 11,2011-Class Description Start ############################
'-- This class is used to encapsulate the Nomadix command response
'-- and present the result in user readable format
'-- This mostly used by RoomQuery method
'############## Jan 11,2011-Class Description End   ############################
Public Class NdxQueryGatewayResults
    Inherits NdxGatewayResults

    Dim MAC As String
    Dim userName As String
    Dim password As String
    Dim expTime, expUnits As String
    Dim roomNo As String
    Dim payMethod As String
    Dim dataval As String

    Sub New()
        MyBase.New()
        MAC = ""
        userName = ""
        password = ""
        expTime = ""
        expUnits = ""
        roomNo = ""
        payMethod = ""
        dataval = ""
    End Sub

    Sub New(ByVal XMLResponse As String)

        Me.New()
        Dim ndxGtResponse As Hashtable

        ndxGtResponse = ndxResponseParser(XMLResponse, nomadixCommand.QUERY)
        userName = ndxGtResponse.Item("ndxUserName")
        password = ndxGtResponse.Item("ndxPassword")
        expTime = ndxGtResponse.Item("ndxExpireTime")
        expUnits = ndxGtResponse.Item("ndxExpTimeUnits")
        roomNo = ndxGtResponse.Item("ndxRoomNo")
        payMethod = ndxGtResponse.Item("ndxPayMethod")
        dataval = ndxGtResponse.Item("ndxDataValume")

        MyBase.initResults(ndxGtResponse)
        ndxGtResponse.Clear()
    End Sub

    ReadOnly Property ndxMachineId()
        Get
            Return MAC
        End Get
    End Property

    ReadOnly Property ndxUserName() As String
        Get
            Return userName
        End Get
    End Property

    ReadOnly Property ndxPassword() As String
        Get
            Return password
        End Get
    End Property

    ReadOnly Property ndxExpireTime() As String
        Get
            Return expTime
        End Get
    End Property

    ReadOnly Property ndxExpUnits() As String
        Get
            Return expUnits
        End Get
    End Property

    ReadOnly Property ndxRoomNo() As String
        Get
            Return roomNo
        End Get
    End Property

    ReadOnly Property ndxPayMethod() As String
        Get
            Return payMethod
        End Get
    End Property

    ReadOnly Property ndxDateVolume() As String
        Get
            Return dataval
        End Get
    End Property
End Class

'############## Jan 11,2011-Class Description Start ############################
'-- This class contains all the nomadix related methods
'-- and methods to execute nomadix related command
'############## Jan 11,2011-Class Description End   ############################
Public Class Nomadix
    Private nomadixIPAddress As String
    Private nomadixPortNo As String

    'Logging purpose
    Protected logroom As String

    Sub New()
        Dim ObjSysConfig As New CSysConfig
        nomadixIPAddress = ObjSysConfig.GetConfig("NomadixIP")
        nomadixPortNo = ObjSysConfig.GetConfig("NomadixPort")
    End Sub

    Sub New(ByVal ndxIP As String, Optional ByVal ndxPort As String = "1111")
        nomadixIPAddress = ndxIP
        nomadixPortNo = ndxPort
    End Sub

    Public ReadOnly Property NomadixIP() As String
        Get
            Return nomadixIPAddress
        End Get
    End Property

    Public ReadOnly Property NomadixPort() As String
        Get
            Return nomadixPortNo
        End Get
    End Property

    Public ReadOnly Property NomadixCMDURL() As String
        Get
            Return "http://" & nomadixIPAddress & ":" & NomadixPort & "/usg/command.xml"
        End Get
    End Property
    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to execute any nomadix related command
    '-- it will take the xmlcommand in string format as parameter
    '-- it will return the xml response in string format
    '############## Jan 11,2011-Method Description End   ############################
    Protected Function execute(ByVal XMLCommand As String) As String
        Dim ObjXMLParser As New MSXML.DOMDocument
        Dim ObjXMLCom As New MSXML2.XMLHTTP
        Dim ObjElog As LoggerService

        ObjXMLParser.async = False
        ObjXMLParser.loadXML(XMLCommand)

        'Logging Nomadix Transcation Started(1/2)
        ObjElog = LoggerService.gtInstance
        ObjElog.write2LogFile(logroom, "================== Nomadix Command Started ==================" & vbCrLf _
                                & Now() & " Nomadix URL: " & NomadixCMDURL & vbCrLf _
                                & "--------------------  Nomadix Command: " & XMLCommand)
        'Logging Nomadix Transcation Stopped(1/2)

        Try
            ObjXMLCom.open("POST", NomadixCMDURL, False)
            ObjXMLCom.send(ObjXMLParser)

            'Logging Nomadix Transcation Started(2/2)
            ObjElog.write2LogFile(logroom, Now() & "--Nomadix Response: " & ObjXMLCom.responseText & vbCrLf & vbCrLf _
                                    & "================== Nomadix Command Ended ==================" & vbCrLf & vbCrLf)
            'Logging Nomadix Transcation Stopped(2/2)

            Return ObjXMLCom.responseText

        Catch ex As Exception
            ObjElog.writeExceptionLogFile(logroom, ex)
            Return "TimedOut"

        Finally
            ObjXMLParser = Nothing
            ObjXMLCom = Nothing
        End Try
    End Function

    Protected Function addMAC(ByVal userContext As UserContext) As NdxGatewayResults
        Dim XMLCommand, IPType, planTime, planAmount, userName, ndxResponse, ExtraTime As String
        Dim ObjPlan As New CPlan
        Dim Bandwidthup, Bandwidthdown As Long
        Dim objSysConfig As New CSysConfig

        'userName = userContext.guestName & Now.ToString("ddMMyyyyHHmmssfff")
        'userName = userContext.userId
        userName = userContext.userId & Now.ToString("ddMMyyyyHHmmssfff")


        If UCase(IPType) = "PRIVATE" Then
            IPType = ""
        Else
            IPType = UCase(IPType)
        End If

        If userContext.item("accessgrant") Is Nothing Then
            ObjPlan.getPlaninfo(userContext.selectedPlanId)
            With ObjPlan
                planTime = .planTime
                planAmount = .planAmount
                Bandwidthup = .planBWUP
                Bandwidthdown = .planBWDN

            End With
            '------------START Extra Time Get from Config Table

            ExtraTime = objSysConfig.GetConfig("ExtraTime")
            planTime = Convert.ToInt32(planTime) + Convert.ToInt32(ExtraTime)

            '------------END Extra Time Get from Config Table

        Else

            If userContext.item("accessgrant") Then
                planTime = userContext.item("remainingtime")
                planAmount = 0.0
            Else
                planTime = userContext.item("remainingtime")
                planAmount = 0.0
            End If

            ObjPlan.getPlaninfo(userContext.item("planid"))
            With ObjPlan
                Bandwidthup = .planBWUP
                Bandwidthdown = .planBWDN
            End With

        End If

        Dim mac As String

        Try
            mac = userContext.item("mac")
        Catch ex As Exception

        End Try

        If mac = "" Then
            mac = userContext.machineId
        End If


        XMLCommand = "<USG COMMAND='USER_ADD' MAC_ADDR='" & mac & "'>" & _
                            "<USER_NAME >" & userName & "</USER_NAME>" & _
                            "<PASSWORD></PASSWORD>" & _
                            "<EXPIRY_TIME UNITS='SECONDS'>" & planTime & "</EXPIRY_TIME>" & _
                            "<ROOM_NUMBER>" & userContext.roomNo & "</ROOM_NUMBER>" & _
                            "<PAYMENT_METHOD>ROOM_OPEN</PAYMENT_METHOD>" & _
                            "<IP_TYPE>" & IPType & "</IP_TYPE>" & _
                            "<CONFIRMATION></CONFIRMATION>" & _
                            "<PAYMENT>" & planAmount & "</PAYMENT>" & _
                      "</USG>"
        ndxResponse = execute(XMLCommand)

        Return New NdxGatewayResults(ndxResponse)
    End Function

    Protected Function RadiusLogin(ByVal userContext As UserContext) As NdxGatewayResults
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim XMLCommand, userName, password, ndxResponse As String
        Dim ObjPlan As New CPlan
        Dim IPType As String = ""
        Dim objSysConfig As New CSysConfig
        Dim Objlog As LoggerService
        'Variable Declaration End ================================================

        Try
            userName = userContext.item("aduserid")
            password = userContext.item("adpassword")
            If UCase(IPType) = "PRIVATE" Then
                IPType = ""
            Else
                IPType = UCase(IPType)
            End If

            XMLCommand = "<USG COMMAND='RADIUS_LOGIN'>" & _
                                "<SUB_USER_NAME>" & userName & "</SUB_USER_NAME>" & _
                                "<SUB_PASSWORD>" & password & "</SUB_PASSWORD>" & _
                                "<SUB_MAC_ADDR>" & userContext.machineId & "</SUB_MAC_ADDR>" & _
                                "<PORTAL_SUB_ID>" & userContext.item("radiusloginid") & "</PORTAL_SUB_ID>" & _
                         "</USG>"
            ndxResponse = execute(XMLCommand)
            Return New NdxGatewayResults(ndxResponse)
        Catch ex As Exception
            Objlog = LoggerService.gtInstance
            Objlog.writeExceptionLogFile(logroom, ex)
        End Try

    End Function

    Protected Function UserPurchase_1WAY(ByVal NDXdata As structNomadixData, ByVal NDXitemData As structNomadixItem) As NdxGatewayResults

        Dim XMLCommand, ndxResponse As String
        Dim tmpItemTotAmt As Double
        Dim Objlog As LoggerService

        Try
            Objlog = LoggerService.gtInstance
            tmpItemTotAmt = NDXitemData.itemAmount + NDXitemData.itemTaxAmount
            XMLCommand = "<USG COMMAND='USER_PURCHASE' ROOM_NUMBER='" & NDXdata.RoomNo & "'>" & _
                                "<ITEM_CODE>" & NDXitemData.itemCode & "</ITEM_CODE>" & _
                                "<ITEM_DESCRIPTION>" & NDXitemData.itemDesc & "</ITEM_DESCRIPTION>" & _
                                "<ITEM_AMOUNT>" & NDXitemData.itemAmount.ToString & "</ITEM_AMOUNT>" & _
                                "<ITEM_TAX>" & NDXitemData.itemTaxAmount.ToString & "</ITEM_TAX>" & _
                                "<ITEM_TOTAL>" & tmpItemTotAmt.ToString & "</ITEM_TOTAL>" & _
                                "<REAL_NAME></REAL_NAME>" & _
                                "<MAC_ADDRESS></MAC_ADDRESS>" & _
                                "<REG_NUMBER></REG_NUMBER>" & _
                         "</USG>"

            ndxResponse = execute(XMLCommand)
            Return New NdxGatewayResults(ndxResponse)
        Catch ex As Exception
            Objlog = LoggerService.gtInstance
            Objlog.writeExceptionLogFile(NDXdata.RoomNo, ex)
        End Try

    End Function

    Protected Function RadiusLogoutByMAC(ByVal MACaddress As String) As NdxGatewayResults
        Dim XMLCommand, ndxResponse As String

        XMLCommand = "<USG COMMAND='LOGOUT'>" & _
                        "<SUB_MAC_ADDR>" & MACaddress & "</SUB_MAC_ADDR>" & _
                        "<SUB_USER_NAME></SUB_USER_NAME>" & _
                     "</USG>"


        ndxResponse = execute(XMLCommand)

        Return New NdxGatewayResults(ndxResponse)
    End Function

    Protected Function deleteByMAC(ByVal MACaddress As String) As NdxGatewayResults
        Dim XMLCommand, ndxResponse As String

        XMLCommand = "<USG COMMAND='USER_DELETE'>" & _
                        "<USER ID_TYPE='MAC_ADDR'>" & MACaddress & "</USER>" & _
                     "</USG>"


        ndxResponse = execute(XMLCommand)

        Return New NdxGatewayResults(ndxResponse)
    End Function

    Protected Function deleteByUser(ByVal userName As String) As NdxGatewayResults
        Dim XMLCommand, ndxResponse As String

        XMLCommand = "<USG COMMAND='USER_DELETE'>" & _
                        "<USER ID_TYPE='USER_NAME'>" & userName & "</USER>" & _
                     "</USG>"

        ndxResponse = execute(XMLCommand)

        Return New NdxGatewayResults(ndxResponse)

    End Function

    Protected Function queryByMAC(ByVal MACaddress As String) As NdxQueryGatewayResults
        Dim XMLCommand, ndxResponse As String

        XMLCommand = "<USG COMMAND='USER_QUERY'>" & _
                        "<USER ID_TYPE='MAC_ADDR'>" & MACaddress & "</USER>" & _
                     "</USG>"

        ndxResponse = execute(XMLCommand)

        Return New NdxQueryGatewayResults(ndxResponse)

    End Function

    Protected Function queryByUser(ByVal userName As String) As NdxQueryGatewayResults
        Dim XMLCommand, ndxResponse As String

        XMLCommand = "<USG COMMAND='USER_QUERY'>" & _
                        "<USER ID_TYPE='USER_NAME'>" & userName & "</USER>" & _
                     "</USG>"

        ndxResponse = execute(XMLCommand)

        Return New NdxQueryGatewayResults(ndxResponse)

    End Function

    Protected Function authroise(ByVal MACaddress As String) As NdxAuthGatewayResults
        Dim XMLCommand, ndxResponse As String

        XMLCommand = "<USG COMMAND='USER_AUTHORIZE' MAC_ADDR='" & MACaddress & "'></USG>"

        ndxResponse = execute(XMLCommand)

        Return New NdxAuthGatewayResults(ndxResponse)

    End Function

    Protected Function updateCache(ByVal MACaddress As String) As NdxGatewayResults
        Dim XMLCommand, ndxResponse As String

        XMLCommand = "<USG COMMAND='CACHE_UPDATE' MAC_ADDR='" & MACaddress & "'>" & _
                        "<PAYMENT_METHOD>ROOM_OPEN</PAYMENT_METHOD>" & _
                     "</USG>"

        ndxResponse = execute(XMLCommand)

        Return New NdxGatewayResults(ndxResponse)

    End Function

    Protected Function setBandWidthUp(ByVal MACaddress As String, ByVal SpeedinKbps As String) As NdxGatewayResults
        Dim XMLCommand, ndxResponse As String

        XMLCommand = "<USG COMMAND='SET_BANDWIDTH_UP' SUBSCRIBER='" & MACaddress & "'>" & _
                            "<BANDWIDTH_UP>" & SpeedinKbps & "</BANDWIDTH_UP>" & _
                     "</USG>"

        ndxResponse = execute(XMLCommand)

        Return New NdxGatewayResults(ndxResponse)

    End Function

    Protected Function setBandWidthDown(ByVal MACaddress As String, ByVal SpeedinKbps As String) As NdxGatewayResults
        Dim XMLCommand, ndxResponse As String

        XMLCommand = "<USG COMMAND='SET_BANDWIDTH_DOWN' SUBSCRIBER='" & MACaddress & "'>" & _
                            "<BANDWIDTH_DOWN>" & SpeedinKbps & "</BANDWIDTH_DOWN>" & _
                     "</USG>"

        ndxResponse = execute(XMLCommand)

        Return New NdxGatewayResults(ndxResponse)

    End Function

    'Protected Function UserPayment_PMS(ByVal userContext As UserContext) As NdxGatewayResults

    '    Dim XMLCommand, IPType, planTime, planAmount, userName, ndxResponse As String
    '    Dim ObjPlan As New CPlan

    '    userName = userContext.roomNo & Now.ToString("ddMMyyyyHHmmssfff")

    '    If UCase(IPType) = "PRIVATE" Then
    '        IPType = ""
    '    Else
    '        IPType = UCase(IPType)
    '    End If

    '    If userContext.item("accessgrant") Is Nothing Then

    '        If userContext.item("coupontype") = ECOUPONTYPE.BULKEXTRA Then
    '            ObjPlan.getPlaninfo(userContext.selectedPlanId, ECOUPONTYPE.BULKEXTRA)

    '        Else
    '            ObjPlan.getPlaninfo(userContext.selectedPlanId)

    '        End If

    '        With ObjPlan
    '            planTime = .planTime
    '            planAmount = .planAmount
    '        End With
    '    Else

    '        If userContext.item("accessgrant") Then
    '            planTime = userContext.item("remainingtime")
    '            planAmount = 0.0
    '        Else
    '            planTime = userContext.item("remainingtime")
    '            planAmount = 0.0

    '        End If

    '    End If

    '    XMLCommand = "<USG COMMAND='USER_PAYMENT' PAYMENT_METHOD='PMS'>" & _
    '                    "<USER_NAME>" & userName & "</USER_NAME>" & _
    '                    "<PASSWORD ENCRYPT='FALSE'></PASSWORD>" & _
    '                    "<EXPIRY_TIME UNITS='SECONDS'>" & planTime & "</EXPIRY_TIME>" & _
    '                    "<ROOM_NUMBER>" & userContext.roomNo & "</ROOM_NUMBER>" & _
    '                    "<PAYMENT>" & planAmount & "</PAYMENT>" & _
    '                "</USG>"

    '    ndxResponse = execute(XMLCommand)

    '    Return New NdxGatewayResults(ndxResponse)
    'End Function

    'Protected Function addMAC_PMS(ByVal userContext As UserContext) As NdxGatewayResults

    '    Dim XMLCommand, IPType, planTime, planAmount, userName, ndxResponse As String
    '    Dim ObjPlan As New CPlan

    '    userName = userContext.guestName & Now.ToString("ddMMyyyyHHmmssfff")

    '    If UCase(IPType) = "PRIVATE" Then
    '        IPType = ""
    '    Else
    '        IPType = UCase(IPType)
    '    End If

    '    If userContext.item("accessgrant") Is Nothing Then

    '        If userContext.item("coupontype") = ECOUPONTYPE.BULKEXTRA Then
    '            ObjPlan.getPlaninfo(userContext.selectedPlanId, ECOUPONTYPE.BULKEXTRA)

    '        Else
    '            ObjPlan.getPlaninfo(userContext.selectedPlanId)

    '        End If

    '        With ObjPlan
    '            planTime = .planTime
    '            planAmount = .planAmount
    '        End With
    '    Else

    '        If userContext.item("accessgrant") Then
    '            planTime = userContext.item("remainingtime")
    '            planAmount = 0.0
    '        Else
    '            planTime = userContext.item("remainingtime")
    '            planAmount = 0.0

    '        End If

    '    End If

    '    XMLCommand = "<USG COMMAND='USER_PAYMENT' PAYMENT_METHOD='PMS'>" & _
    '                    "<USER_NAME>" & userContext.machineId & "</USER_NAME>" & _
    '                    "<REAL_NAME>" & userContext.guestName & "</REAL_NAME>" & _
    '                    "<PASSWORD ENCRYPT='FALSE'></PASSWORD>" & _
    '                    "<EXPIRY_TIME UNITS='SECONDS'>" & planTime & "</EXPIRY_TIME>" & _
    '                    "<ROOM_NUMBER>" & userContext.roomNo & "</ROOM_NUMBER>" & _
    '                    "<PAYMENT>" & planAmount & "</PAYMENT>" & _
    '                "</USG>"

    '    ndxResponse = execute(XMLCommand)

    '    Return New NdxGatewayResults(ndxResponse)
    'End Function

    Protected Sub TdelaySec(ByVal DelaySeconds As Integer)
        Dim SecCount, Sec2, Sec1
        SecCount = 0
        Sec2 = 0
        While SecCount < DelaySeconds + 1
            Sec1 = Second(Date.Now)
            If Sec1 <> Sec2 Then
                Sec2 = Second(Date.Now)
                SecCount = SecCount + 1
            End If
        End While
    End Sub

End Class

'############## Jan 11,2011-Class Description Start ############################
'-- This class contains current application related commands
'-- This class inherited from Nomadix
'-- It can be consider as a subset of Nomadix class
'-- It contains only those methods which need to be called by the current app
'############## Jan 11,2011-Class Description End   ############################
Public Class RadiusGtService
    Inherits Nomadix
    Implements IGatewayService
    Private processDelay As Integer

    Sub New()
        MyBase.New()

    End Sub

    Sub New(ByVal ndxIP As String, Optional ByVal ndxPort As String = "1111")
        MyBase.New(ndxIP, ndxPort)

    End Sub

    ReadOnly Property ndxProcessDelay() As String
        Get
            Return processDelay
        End Get
    End Property

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method will be called for RadiusLogin
    '-- It will do following tasks:
    '-- It will call the RadiusLogin method passing the userContext
    '-- If the status is OK then it will check in Radius_Login_logout table RADIUS_LOGIN_ACCEPT 
    '-- has received for the particular Portal Sub ID and mac
    '-- If the response OK then it will return
    '-- If response not OK and contains ErrorNo 444 or 999 then
    '-- do one one repeat of radiuslogin process
    '-- if still error received then return from the method
    '############## Jan 11,2011-Method Description End   ############################
    Public Function add(ByVal userContext As UserContext) As GatewayResults Implements IGatewayService.add
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim addResp, tres As NdxGatewayResults
        Dim RadiusLoginTime As DateTime
        Dim ObjLog As LoggerService
        Dim ObjAdCred As StructADCred
        'Variable Declaration End ================================================
        logroom = userContext.roomNo
        'Start Radius Login Process ###########################################
        Try
            RadiusLoginTime = Now
            addResp = addMAC(userContext)
            Dim mac As String = ""
            Try
                mac = userContext.item("mac")
            Catch ex As Exception

            End Try

            If mac = "" Then
                mac = userContext.machineId
            End If



            Try
                updateCache(mac)
            Catch ex As Exception

            End Try



            If UCase(addResp.gtStatus) = "OK" Then
                '-----------------------Starts RADIUS Login STATUS Check by using NOMADIX Data in MS-SQL Data--------------------
                '  addResp = Find_ValidUser(userContext.item("radiusloginid"), RadiusLoginTime, userContext.machineId)
                '-----------------------Ends RADIUS Login STATUS Check by using NOMADIX Data in MS-SQL Data--------------------
                If UCase(addResp.gtStatus) = "OK" Then
                    Return addResp
                End If

            End If




        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("NDXEXP", ex)
        End Try
        Return addResp
        'End Radius Login Process ###########################################
    End Function

    Private Function AddADUser(ByVal MAC As String, ByVal roomno As String) As StructADCred
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim ADRes() As String
        Dim ObjElog As LoggerService
        Dim ADCred As New StructADCred
        'Variable Declaration End ================================================
        Try
            ADRes = ActiveUtil.ActiveUser.AddAdUser(MAC, roomno).Split(",")
            ObjElog = LoggerService.gtInstance
            ObjElog.write2LogFile("ADRES", Now & "---------" & String.Join(",", ADRes))

            If Not ADRes Is Nothing Then
                If UCase(ADRes(0)) = "TRUE" Then
                    ADCred.ADuserid = ADRes(1)
                    ADCred.ADpassword = ADRes(2)
                Else
                    ADCred.ADuserid = "itcguest"
                    ADCred.ADpassword = "Pp123456"

                    ObjElog = LoggerService.gtInstance
                    ObjElog.write2LogFile(roomno, String.Join(" ,", ADRes))

                End If
            Else
                ADCred.ADuserid = "itcguest"
                ADCred.ADpassword = "Pp123456"
            End If
            ActiveUtil.ActiveUser.closeEntry()
            Return ADCred
        Catch ex As Exception
            ObjElog = LoggerService.gtInstance
            ObjElog.writeExceptionLogFile("ADEXCEPTION", ex)
            ADCred.ADuserid = "itcguest"
            ADCred.ADpassword = "Pp123456"
            ActiveUtil.ActiveUser.closeEntry()
            Return ADCred
        End Try

    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to check sub_status in Radius_login_logout table for particular
    '-- macaddress and portal_sub_id
    '-- If no result coming it will continue to check the same for 60 times with 1000 ms delay in each
    '-- substatus can be any of the following:
    '-- RADIUS_LOGIN_ACCEPT
    '-- RADIUS_LOGIN_REJECT
    '-- RADIUS_LOGOUT_USER_REQUEST
    '-- RADIUS_LOGOUT_PORTAL_RESET
    '-- RADIUS_LOGOUT_ADMIN_RESET
    '-- RADIUS_LOGOUT_IDLE_TIMEOUT
    '-- Based on the substatus it will generate the XML response with ERROR_NUM and ERROR_DESC
    '-- it will return that response to the caller
    '############## Jan 11,2011-Method Description End   ############################
    Private Function Find_ValidUser(ByVal PortalSubId As String, ByVal RadiusLoginTime As DateTime, ByVal MachineId As String) As NdxGatewayResults
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim i As Integer = 0
        Dim SQL_query As String = ""
        Dim NomdaixRes As String = ""
        Dim TmpResponse As String = ""
        Dim RefResultset As DataSet
        Dim objDbase As DbaseServiceOLEDB
        Dim ObjLog As LoggerService
        'Variable Declaration End ================================================

        Try
            objDbase = DbaseServiceOLEDB.getInstance
            SQL_query = "Select sub_status from radius_login_logout where (portal_sub_id = '" & PortalSubId & "' and sub_mac_addr = '" & MachineId & "' and logtime >= '" & RadiusLoginTime & "')"
            While i <= 60
                RefResultset = objDbase.DsWithoutUpdate(SQL_query)
                If RefResultset.Tables(0).Rows.Count > 0 Then
                    NomdaixRes = UCase(RefResultset.Tables(0).Rows(0).Item("sub_status"))
                    If NomdaixRes = "RADIUS_LOGIN_ACCEPT" Then
                        TmpResponse = "<USG RESULT='OK' ID='0257C6' IP='192.168.2.9'>" & _
                                        "<ERROR_NUM>555</ERROR_NUM>" & _
                                        "<ERROR_DESC>Login Successful</ERROR_DESC>" & _
                                      "</USG>"
                        Exit While
                    ElseIf NomdaixRes = "RADIUS_LOGIN_TIMEOUT" Then
                        TmpResponse = "<USG RESULT='ERROR' ID='0257C6' IP='192.168.2.9'>" & _
                                        "<ERROR_NUM>444</ERROR_NUM>" & _
                                        "<ERROR_DESC>Radius Login TimeOut.</ERROR_DESC>" & _
                                      "</USG>"
                        Exit While
                    ElseIf NomdaixRes = "RADIUS_LOGIN_ERROR" Then
                        TmpResponse = "<USG RESULT='ERROR' ID='0257C6' IP='192.168.2.9'>" & _
                                        "<ERROR_NUM>333</ERROR_NUM>" & _
                                        "<ERROR_DESC>An error occurred</ERROR_DESC>" & _
                                      "</USG>"
                        Exit While

                    ElseIf NomdaixRes = "RADIUS_LOGIN_REJECT" Then
                        TmpResponse = "<USG RESULT='ERROR' ID='0257C6' IP='192.168.2.9'>" & _
                                        "<ERROR_NUM>777</ERROR_NUM>" & _
                                        "<ERROR_DESC>Login Reject</ERROR_DESC>" & _
                                      "</USG>"
                        Exit While

                    Else
                        TmpResponse = "<USG RESULT='ERROR' ID='0257C6' IP='192.168.2.9'>" & _
                                        "<ERROR_NUM>888</ERROR_NUM>" & _
                                        "<ERROR_DESC>Login Error</ERROR_DESC>" & _
                                      "</USG>"
                    End If
                Else 'Invalid UserName and Password
                    TmpResponse = "<USG RESULT='ERROR' ID='0257C6' IP='192.168.2.9'>" & _
                                    "<ERROR_NUM>999</ERROR_NUM>" & _
                                    "<ERROR_DESC>Login Response Error.</ERROR_DESC>" & _
                                  "</USG>"
                End If

                Thread.Sleep(1000)
                i = i + 1
            End While

        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("RadiusEXP", ex)
        End Try

        Return New NdxGatewayResults(TmpResponse)

    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used for logout a particular mac from nomadix
    '-- This method can be called when we require to have force logout of any particular mac which
    '-- is previously validated using RadiusLogin
    '############## Jan 11,2011-Method Description End   ############################
    Public Function delete(ByVal userContext As UserContext) As GatewayResults Implements IGatewayService.delete
        Dim logoutResp As NdxGatewayResults
        logoutResp = RadiusLogoutByMAC(userContext.machineId)

    End Function

    Public Function query(ByVal userContext As UserContext) As Object Implements IGatewayService.query
        logroom = userContext.roomNo
        Return queryByMAC(userContext.machineId)
    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is called to raise bandwidth up and down command for specific MAC
    '-- It is used to do bandwidth capping for specific MAC
    '-- It will carry out following tasks:
    '-- Get the planinformation(planBWUP) for particular planid in Plans table
    '-- serially call setBandWidthUp and setBandWidthDown passing the MAC,BandWidthUp,setBandWidthDown
    '############## Jan 11,2011-Method Description End   ############################
    Public Function update(ByVal userContext As UserContext) As GatewayResults Implements IGatewayService.update


        Dim addResp As NdxGatewayResults
        Dim Bandwidthup As String = ""
        Dim Bandwidthdown As String = ""

        '=================18-SEP-2008 Bandwidth management purpose added the following Starts==================
        Dim objPlan As New CPlan

        If userContext.item("accessgrant") Is Nothing Or userContext.item("logintype") = LOGINTYPE.NEWLOGIN Then
            objPlan.getPlaninfo(userContext.selectedPlanId)
        Else
            objPlan.getPlaninfo(userContext.item("planid"))

        End If

        'bandwidth capping for 22 DEC NEW POC ######################
        Bandwidthup = objPlan.planBWUP.ToString()
        Bandwidthdown = objPlan.planBWUP.ToString()
        'bandwidth capping for 22 DEC NEW POC ######################

        logroom = userContext.roomNo
        Dim mac As String = ""

        Try
            mac = userContext.item("mac")
        Catch ex As Exception

        End Try

        If mac = "" Then
            mac = userContext.machineId
        End If

        addResp = setBandWidthUp(mac, Bandwidthup)

        If UCase(addResp.gtStatus) = "TIMEDOUT" Then
            Return addResp

        ElseIf UCase(addResp.gtStatus) = "OK" Then
            addResp = setBandWidthDown(mac, Bandwidthdown)

        End If

        Return addResp


    End Function

    Public Function payment(ByVal userContext As UserContext) As GatewayResults Implements IGatewayService.payment
        logroom = userContext.roomNo
        Dim addResp As NdxGatewayResults
        Dim Obj_NomadixData As New structNomadixData
        Dim Obj_NomadixItem As New structNomadixItem
        Dim ObjSysConfig As New CSysConfig

        Obj_NomadixData.RoomNo = userContext.roomNo
        Obj_NomadixItem.itemCode = userContext.roomNo   'ObjSysConfig.GetConfig("InternetItemCode")
        Obj_NomadixItem.itemDesc = ObjSysConfig.GetConfig("InternetItemDesc")
        Obj_NomadixItem.itemAmount = userContext.item("itemamount")
        Obj_NomadixItem.itemTaxAmount = 0.0 'ObjSysConfig.GetConfig("InternetItemTax")

        addResp = UserPurchase_1WAY(Obj_NomadixData, Obj_NomadixItem)
        Return addResp

    End Function

End Class