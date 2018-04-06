'############## Jan 10,2011-Class Description Start ############################
'---- This class deals with methods which get some specific information based on the macaddress 
'---- received from the query string
'---- Instance of this class can be created in following manner:
'---- Dim ObjMacInfo as MACINFO
'---- ObjMacInfo=MACINFO.getInstance()
'############## Jan 10,2011-Class Description End ##############################
Imports System.Data
Imports System.Data.Common

Public Class MACINFO
    Private guestid As String
    Private altPwd As String
    Private _roomno As String
    Private _MacAuthFlag As Boolean
    Private Shared gtMACServiceInst As MACINFO
    Private Sub New()
        'Nothing
    End Sub
    '############## Jan 10,2011-Method Description Start ############################
    'This method is used to create a instance of the class---
    'This method will return a object of class MACINFO---
    Public Shared Function getInstance() As MACINFO
        If gtMACServiceInst Is Nothing Then gtMACServiceInst = New MACINFO
        Return gtMACServiceInst
    End Function

    '############## Jan 10,2011-Method Description Start ############################
    '-- This method is to check using One ACCID 3 devices (configured by MaxDeviceCountForBillSharing in Web.config) can be accessible---
    '-- For only Mobile 3 device mean 3 distings macs---
    '-- This method will return LoginType(ENUM) NEWLOGIN if the ACCID does not access 3 devices yet ---
    '--- It will return LoginType APPERROR if the ACCID try to access more than 3 devices ---
    Public Function CheckMACByACCID(ByVal ACCID As Long, ByVal MAC As String, ByVal AccessType As Integer) As LOGINTYPE
        Dim SQL_QUERY As String
        Dim objDbase As DbaseServiceOLEDB
        Dim refDataSet As DataSet
        Dim refDataTable As DataTable
        Dim MacCount As Integer = 0
        Dim ObjLog As LoggerService

        ObjLog = LoggerService.gtInstance

        MAC = MAC.Trim()

        Try
            objDbase = DbaseServiceOLEDB.getInstance

            SQL_QUERY = String.Format("Select Mac, MacType From MacDetails Where ACCID = {0}", ACCID)
            refDataSet = objDbase.DsWithoutUpdate(SQL_QUERY)
            refDataTable = refDataSet.Tables(0)

            If refDataTable.Rows.Count <= 0 Then
                Return LOGINTYPE.NEWLOGIN
            Else
                Dim foundMac As String
                Dim macPresent As Boolean = False
                For i As Integer = 0 To refDataTable.Rows.Count - 1
                    foundMac = refDataTable.Rows(i)("Mac").ToString().Trim()
                    If MAC = foundMac Then
                        macPresent = True
                        Exit For
                    End If
                Next

                If macPresent = True Then
                    Return LOGINTYPE.NEWLOGIN
                Else
                    'Check for Mac Count

                    Dim WirelessMac As New List(Of String)
                    Dim WiredMac As New List(Of String)
                    Dim MobileMac As New List(Of String)

                    WirelessMac.Clear()
                    WiredMac.Clear()
                    MobileMac.Clear()

                    If AccessType = 0 Then
                        WirelessMac.Add(MAC)
                    ElseIf AccessType = 1 Then
                        WiredMac.Add(MAC)
                    ElseIf AccessType = 3 Then
                        MobileMac.Add(MAC)
                    Else
                        Throw New Exception(String.Format("Access Type {0} is not supported", AccessType))
                    End If

                    Dim cMac As String = ""
                    Dim type As Integer

                    For i As Integer = 0 To refDataTable.Rows.Count - 1
                        cMac = refDataTable.Rows(i)("Mac").ToString().Trim()
                        type = Integer.Parse(refDataTable.Rows(i)("MacType").ToString().Trim())

                        If (type = 0) And (Not WirelessMac.Contains(cMac)) Then
                            WirelessMac.Add(cMac)
                        ElseIf (type = 1) And (Not WiredMac.Contains(cMac)) Then
                            WiredMac.Add(cMac)
                        ElseIf (type = 3) And (Not MobileMac.Contains(cMac)) Then
                            MobileMac.Add(cMac)
                        Else
                            'Do nothing
                        End If

                    Next

                    Dim laptopcount As Integer
                    Dim mobilecount As Integer = MobileMac.Count


                    If WirelessMac.Count > WiredMac.Count Then
                        laptopcount = WirelessMac.Count
                    Else
                        laptopcount = WiredMac.Count
                    End If

                    Dim totalCount As Integer = laptopcount + mobilecount

                    Dim maxDeviceCountForBillSharing = CSysConfig.GetMaxDeviceCountForBillSharing()
                    If totalCount > maxDeviceCountForBillSharing Then
                        Return LOGINTYPE.APPEROR
                    Else
                        Return LOGINTYPE.NEWLOGIN
                    End If

                End If

            End If

        Catch ex As Exception
            ObjLog.writeExceptionLogFile("CheckByMACEXP", ex)
            Return LOGINTYPE.APPEROR
        End Try

    End Function



    Public Function Devcount(ByVal ACCID As Long, ByVal MAC As String, ByVal AccessType As Integer) As Integer
        Dim SQL_QUERY As String
        Dim objDbase As DbaseServiceOLEDB
        Dim refDataSet As DataSet
        Dim refDataTable As DataTable
        Dim MacCount As Integer = 0
        Dim ObjLog As LoggerService

        ObjLog = LoggerService.gtInstance

        Try
            ' ObjLog.write2LogFile("devcount", "accid" & ACCID & "mac=" & MAC)
        Catch ex As Exception

        End Try

        MAC = MAC.Trim()

        Try
            objDbase = DbaseServiceOLEDB.getInstance

            SQL_QUERY = String.Format("Select Mac, MacType From MacDetails Where ACCID = {0}", ACCID)
            refDataSet = objDbase.DsWithoutUpdate(SQL_QUERY)
            refDataTable = refDataSet.Tables(0)

            If refDataTable.Rows.Count <= 0 Then
                Return 3
            Else
               

               
                'Check for Mac Count

                Dim WirelessMac As New List(Of String)
                Dim WiredMac As New List(Of String)
                Dim MobileMac As New List(Of String)

                WirelessMac.Clear()
                WiredMac.Clear()
                MobileMac.Clear()

                'If AccessType = 0 Then
                '    WirelessMac.Add(MAC)
                'ElseIf AccessType = 1 Then
                '    WiredMac.Add(MAC)
                'ElseIf AccessType = 3 Then
                '    MobileMac.Add(MAC)
                'Else

                'End If

                Dim cMac As String = ""
                Dim type As Integer

                For i As Integer = 0 To refDataTable.Rows.Count - 1
                    cMac = refDataTable.Rows(i)("Mac").ToString().Trim()
                    type = Integer.Parse(refDataTable.Rows(i)("MacType").ToString().Trim())

                    If (type = 0) And (Not WirelessMac.Contains(cMac)) Then
                        WirelessMac.Add(cMac)
                    ElseIf (type = 1) And (Not WiredMac.Contains(cMac)) Then
                        WiredMac.Add(cMac)
                    ElseIf (type = 3) And (Not MobileMac.Contains(cMac)) Then
                        MobileMac.Add(cMac)
                    Else
                        'Do nothing
                    End If

                Next

                Dim laptopcount As Integer
                Dim mobilecount As Integer = MobileMac.Count


                If WirelessMac.Count > WiredMac.Count Then
                    laptopcount = WirelessMac.Count
                Else
                    laptopcount = WiredMac.Count
                End If

                Dim totalCount As Integer = laptopcount + mobilecount


                Try
                    ' ObjLog.write2LogFile("devcount", "total=" & totalCount)
                Catch ex As Exception

                End Try




                Return (4 - totalCount)

                'Dim maxDeviceCountForBillSharing = CSysConfig.GetMaxDeviceCountForBillSharing()
                'If totalCount > maxDeviceCountForBillSharing Then
                '    Return LOGINTYPE.APPEROR
                'Else
                '    Return LOGINTYPE.NEWLOGIN
                'End If

            End If



        Catch ex As Exception
            ObjLog.writeExceptionLogFile("CheckByMACEXP", ex)
            Return LOGINTYPE.APPEROR
        End Try

    End Function



    Public Function LogoutWirelessMACS(ByVal ACCID As Long, ByVal CurrMAC As String) As Boolean
        Dim SQL_QUERY As String
        Dim ObjDb As DbaseServiceOLEDB
        Dim ObjDataSet As DataSet
        Dim ObjMacArray As New ArrayList
        Dim count As Integer = 0
        Try
            SQL_QUERY = "SELECT MAC FROM MacDetails WHERE ACCID=" & ACCID & " AND MACTYPE in (0,3) AND MAC<>'" & CurrMAC & "'"
            ObjDb = DbaseServiceOLEDB.getInstance
            ObjDataSet = ObjDb.DsWithoutUpdate(SQL_QUERY)
            If ObjDataSet.Tables(0).Rows.Count > 0 Then
                While (count < ObjDataSet.Tables(0).Rows.Count)
                    DoLogout(ObjDataSet.Tables(0).Rows(count).Item("MAC").ToString(), 20)
                    count = count + 1
                End While
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '--- This method will insert values into macdetails iff the particular mac and the accid not
    '--- present in macdetails
    '############## Jan 11,2011-Method Description End ############################

    Public Function InsertMacDetailsByACCID(ByVal RoomNo As String, ByVal MAC As String, ByVal ACCTYPE As Integer, ByVal GuestId As Long, ByVal ACCID As Long, ByVal UsageType As Integer) As Boolean
        Dim SQL_QUERY, SQL_INSQUERY As String
        Dim objDbase As DbaseServiceOLEDB
        Dim refDataSet As DataSet
        Dim ObjLog As LoggerService
        Dim UsageCount As Integer = 0
        Dim result As Integer = 0
        ObjLog = LoggerService.gtInstance

        Try
            SQL_QUERY = "SELECT * FROM MacDetails WHERE ACCID=" & ACCID & " AND MAC='" & MAC & "'"
            objDbase = DbaseServiceOLEDB.getInstance
            refDataSet = objDbase.DsWithoutUpdate(SQL_QUERY)
            If refDataSet.Tables(0).Rows.Count <= 0 Then
                SQL_INSQUERY = "INSERT INTO MacDetails(ROOMNO,MAC,MACTYPE,GuestId,ACCID) VALUES('" & RoomNo & "','" & MAC & "'," & ACCTYPE & "," & GuestId & "," & ACCID & ")"
                'ObjLog.write2LogFile("MACINSERT", SQL_INSQUERY)
                result = objDbase.insertUpdateDelete(SQL_INSQUERY)
                If result > 0 Then
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.write2LogFile("MacInsertExp", SQL_INSQUERY)
            Return False
        End Try
    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '--- This method will return the guestid and roomno for wireless mac
    '############## Jan 11,2011-Method Description End ############################
    Public Function GetGuestInfoByMAC(ByVal MAC As String) As Boolean
        Dim Sql_Query As String
        Dim objDbase As DbaseServiceOLEDB
        Dim refDataSet As DataSet
        Sql_Query = "select * from MacDetails where MAC='" & MAC & "' and MACTYPE=0"
        Try
            objDbase = DbaseServiceOLEDB.getInstance
            refDataSet = objDbase.DsWithoutUpdate(Sql_Query)
            If refDataSet.Tables(0).Rows.Count > 0 Then
                guestid = refDataSet.Tables(0).Rows(0).Item("GuestId")
                _roomno = refDataSet.Tables(0).Rows(0).Item("RoomNo")
                altPwd = GetAltPassword(guestid)
                _MacAuthFlag = True
                Return True
            Else
                guestid = ""
                _roomno = ""
                altPwd = ""
                _MacAuthFlag = True
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try

    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '--- This method will return the altpassword for particular guestid
    '############## Jan 11,2011-Method Description End ############################

    Private Function GetAltPassword(ByVal GuestId As String) As String
        Dim Sql_Query As String
        Dim objDbase As DbaseServiceOLEDB
        Dim refDataSet As DataSet
        Dim AltPasswd As String = ""
        Sql_Query = "select GuestRegCode from guest where guestid=" & GuestId & " and gueststatus='A'"
        Try
            objDbase = DbaseServiceOLEDB.getInstance
            refDataSet = objDbase.DsWithoutUpdate(Sql_Query)
            If refDataSet.Tables(0).Rows.Count > 0 Then
                AltPasswd = refDataSet.Tables(0).Rows(0).Item("GuestRegCode")
            Else
                AltPasswd = ""
            End If
        Catch ex As Exception
            AltPasswd = ""
        End Try

        Return AltPasswd
    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '--- This method is used to do force logout for a particular mac
    '---- It will also set the logout status in logdetails table
    '############## Jan 11,2011-Method Description End ############################

    Public Function DoLogout(ByVal logoutMAC As String, ByVal LogoutStatus As Integer) As Boolean
        Dim strNASIP As String
        Dim MACaddress As String
        Dim strResponseText As String
        Dim objSysConfig As New CSysConfig
        Dim objLogInOut As LoginService

        Try
            strNASIP = objSysConfig.GetConfig("NomadixIP")
            MACaddress = logoutMAC
            strResponseText = cmdXML_RADIUS_Logout(MACaddress, strNASIP)
            If strResponseText.IndexOf("USG RESULT=""OK""") <> -1 Then
                objLogInOut = LoginService.getInstance
                objLogInOut.logout(MACaddress, LogoutStatus)
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
        Finally
            objLogInOut = Nothing
            objSysConfig = Nothing
        End Try

        Return False

    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '--- This method contains xml command to do logout
    '--- This method is called by DoLogout() method
    '############## Jan 11,2011-Method Description End ############################
    Public Function cmdXML_RADIUS_Logout(ByVal strMACID As String, ByVal strNASIP As String) As String
        Dim ObjXMLCmd As System.Xml.XmlDocument
        Dim ObjXMLHTTP As New MSXML2.XMLHTTP
        Dim strErrorMsg As String
        Dim ObjLog As LoggerService
        ObjLog = LoggerService.gtInstance

        Try
            Dim strPassparam As String
            ObjXMLCmd = New System.Xml.XmlDocument
            strPassparam = "<USG COMMAND='LOGOUT'>" & _
                "<SUB_MAC_ADDR>" & strMACID & "</SUB_MAC_ADDR>"

            strPassparam = strPassparam & "</USG>"
            Dim t = Now
            ObjXMLCmd.LoadXml(strPassparam)
            ObjXMLHTTP.open("POST", "http://" & strNASIP & ":1111/usg/command.xml", False, "", "")
            'ObjXMLHTTP.open("POST", "http://192.168.0.30:1111/usg/command.xml", False, "", "")
            ObjXMLHTTP.send(ObjXMLCmd.InnerXml)
            cmdXML_RADIUS_Logout = ObjXMLHTTP.responseText
            '------------------------Start test from MSPL---------------------

            ObjLog.write2LogFile("RLOMAC", t & "NdxCMD: " & strPassparam & vbCrLf & Now() & "NdxRes: " & cmdXML_RADIUS_Logout & vbCrLf)
            '------------------------End test from MSPL---------------------
        Catch EX As Exception
            strErrorMsg = EX.Message.ToString()
            ObjLog.writeExceptionLogFile("RLOMACEXP", EX)
        Finally
            ObjXMLHTTP = Nothing
            ObjXMLCmd = Nothing
        End Try
    End Function

    Public Function GetWiredMAC(ByVal WirelessMac As String, ByVal roomno As String) As String
        Dim SQL_getWireMac_query As String
        Dim objDbase As DbaseServiceOLEDB
        Dim refDataSet As DataSet
        Dim WiredMac As String
        objDbase = DbaseServiceOLEDB.getInstance
        'SQL_getWireMac_query = "select MAC from MacDetails where MACGROUPID=(select MACGROUPID from MacDetails where MAC='" & WirelessMac & "' and ROOMNO='" & roomno & "' and MACTYPE=0) and MACTYPE=1"
        If roomno = "" Then
            SQL_getWireMac_query = "Select MAC from MacDetails Where MACGROUPID=(select MACGROUPID from MacDetails where MAC='" & WirelessMac & "' and MACTYPE=0) and MACTYPE=1"
        Else
            SQL_getWireMac_query = "Select MAC from MacDetails Where MACGROUPID=(select MACGROUPID from MacDetails where MAC='" & WirelessMac & "' and ROOMNO='" & roomno & "' and MACTYPE=0) and MACTYPE=1"
        End If

        '--------------------------Starts test --------------------------
        Dim testlog As LoggerService
        testlog = LoggerService.gtInstance
        testlog.write2LogFile("MSPLlog", Now() & "Get Wired MAC qry: " & SQL_getWireMac_query & vbCrLf)

        '--------------------------Ends test ----------------------------
        refDataSet = objDbase.DsWithoutUpdate(SQL_getWireMac_query)
        If refDataSet.Tables(0).Rows.Count > 0 Then
            If IsDBNull(refDataSet.Tables(0).Rows(0).Item("MAC")) Then
                Return ""
            Else
                WiredMac = refDataSet.Tables(0).Rows(0).Item("MAC")
                Return WiredMac
            End If
        Else
            Return ""
        End If
        Return ""
    End Function
    'Sep7 Process Add for two Laptop End ##################################

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to get the mactype(laptop/mobile) for particular MAC
    '--- This method return ENUM MACTYPE(LAP,MOB,ND)
    '--- It will search in LaptopMacSettings for laptop macs
    '--- It will search in MobileMacSettings for mobile macs
    '--- If the requested mac not present in LaptopMacSettings or MobileMacSettings,
    '---- it will return ND( Not Defined)
    '############## Jan 11,2011-Method Description End ############################
    Public Function FindMacType(ByVal MAC As String) As MACTYPE
        Dim ObjDb As DbaseServiceOLEDB
        Dim ObjLog As LoggerService
        Dim ObjDbTable As DataTable
        Dim Command As DbCommand
        Dim SQL_QUERY As String
        Dim MacType As Integer
        SQL_QUERY = "Select Mac, 1 As Type From LaptopMacSettings" & _
                    vbCrLf & " Where Mac = @PMAC" & _
                    vbCrLf & " Union" & _
                    vbCrLf & " Select Mac, 2 As Type From MobileMacSettings" & _
                    vbCrLf & " Where Mac = @PMAC"
        ObjLog = LoggerService.gtInstance
        Try
            ObjDb = DbaseServiceOLEDB.getInstance
            Command = ObjDb.GetCommand()
            Command.CommandText = SQL_QUERY
            ObjDb.AddInputParameter(Command, "@PMAC", DbType.String, MAC)
            ObjDbTable = ObjDb.DsWithoutUpdateWithParam(Command)
            If ObjDbTable.Rows.Count > 0 Then
                MacType = Integer.Parse(ObjDbTable.Rows(0).Item("Type").ToString())
                If MacType = 1 Then
                    Return ITCCORE.MACTYPE.LAP
                ElseIf MacType = 2 Then
                    Return ITCCORE.MACTYPE.MOB
                Else
                    Return ITCCORE.MACTYPE.ND
                End If
            Else
                Return ITCCORE.MACTYPE.ND
            End If
        Catch ex As Exception
            ObjLog.writeExceptionLogFile("MAC_TYPE_EXCEPTION", ex)
        Finally
            Command = Nothing
            ObjDb = Nothing
            ObjDbTable = Nothing
        End Try

    End Function

    ReadOnly Property AltPassword() As String
        Get
            Return altPwd
        End Get
    End Property
    ReadOnly Property RoomNo() As String
        Get
            Return _roomno
        End Get
    End Property
    ReadOnly Property GrCid() As String
        Get
            Return guestid
        End Get
    End Property
    ReadOnly Property MacAllowed() As Boolean
        Get
            Return _MacAuthFlag
        End Get
    End Property
End Class
