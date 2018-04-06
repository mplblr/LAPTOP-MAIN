'############## Jan 11,2011-Class Description Start ############################
'-- This class is used to handle all the ACCID/ACCESSCODE Related issues
'-- It mainly deals with ACCESSUSAGE table
'############## Jan 11,2011-Class Description End   ############################
Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.Common

Public Class AccessCode
    Private _AccMAC As String
    Private _AccCode As String
    Private _ACCID As Long
    Private _RoomNo As String
    Private _GuestID As Long

    '############## Jan 11,2011-Method Description Start ############################
    '-- This is a constructor which takes MAC and ACCESSCODE as parameter
    '-- This is used when require details based on the ACCESSCODE
    '############## Jan 11,2011-Method Description End   ############################
    Public Sub New(ByVal AccMAC As String, ByVal Acc_Code As String)
        _AccMAC = AccMAC
        _AccCode = Acc_Code
    End Sub
    '############## Jan 11,2011-Method Description Start ############################
    '-- This is a constructor which takes ACCID as parameter
    '-- This is used when require details based on the ACCID only
    '-- This is called when we require to fetch details inside reading vaules from cookies
    '############## Jan 11,2011-Method Description End   ############################
    Public Sub New(ByVal ACCID As Long)
        _AccMAC = ""
        _AccCode = ""
        _ACCID = ACCID
    End Sub
    '############## Jan 11,2011-Method Description Start ############################
    '-- This is a constructor which takes ROOMNO and GUESTID as parameter
    '-- This is used when require details based on the ROOMNO only
    '-- Currently it is not in use
    '############## Jan 11,2011-Method Description End   ############################
    Public Sub New(ByVal RoomNo As String, ByVal GuestId As Long)
        _RoomNo = RoomNo
        _GuestID = GuestId
        _AccMAC = ""
        _AccCode = ""
        _ACCID = 0
    End Sub
    '############## Jan 11,2011-Method Description Start ############################
    '-- This is a default constructor without any initialization parameter
    '############## Jan 11,2011-Method Description End   ############################
    Public Sub New()
        _RoomNo = ""
        _GuestID = 0
        _AccMAC = ""
        _AccCode = ""
        _ACCID = 0
    End Sub

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to collect details from ACCESSUSAGE,Guest tables 
    '-- based on the initialization parameters passed inside constructor
    '-- It takes one parameter as ACCESSTYPE ENUM to identify 
    '-- query will be based on MAC/ACCID/ACCESSCODE
    '-- It will return UserCredential object instance
    '-- This method is used to collect following details:
    '-- GuestId,AltPassword,ACCID,UserId(ROOMNO)
    '############## Jan 11,2011-Method Description End   ############################


    Public Function GetStatusID(ByVal AccessCode As String) As Integer

        AccessCode = AccessCode.Replace("'", "''").Replace("--", "")

        Dim ObjDb As DbaseServiceOLEDB
        Dim RefObjDataSet As DataSet
        Dim RefObjDataTable As DataTable
        Dim SQL_QUERY As String
        Dim ObjLog As LoggerService
        Dim ObjUserCred As UserCredential
        Dim Command As DbCommand
        'Variable Declaration End  ================================================
        ObjLog = LoggerService.gtInstance
        Try
            ObjDb = DbaseServiceOLEDB.getInstance
            Command = ObjDb.GetCommand()

            SQL_QUERY = "Select AccessID, AccessCode, Case When AccStatus Is Null Then -1 When AccStatus = 'D' Then -2 Else -1 End As AccStatus " & vbNewLine & _
                        "From " & vbNewLine & _
                        "(Select AccessID, AccessCode From CouponMaster Where AccessCode = '" & AccessCode & "') As A " & vbNewLine & _
                        "Left Outer Join " & vbNewLine & _
                        "(Select ACC_CODE, ACC_STATUS As AccStatus From ACCESSUSAGE Where ACC_CODE = '" & AccessCode & "') As B On B.ACC_CODE = A.AccessCode"


            Command.CommandText = SQL_QUERY
            RefObjDataTable = ObjDb.DsWithoutUpdateWithParam(Command)
            'ObjLog.write2LogFile("ACCDETAILS", SQL_QUERY & vbNewLine)

            If RefObjDataTable.Rows.Count > 0 Then
                'Coupon not activated through FO
                Return Integer.Parse(RefObjDataTable.Rows(0)("AccStatus").ToString())
            Else
                Return 0
            End If

        Catch ex As Exception
            Return 0
        Finally
            ObjDb = Nothing
            RefObjDataSet = Nothing
            RefObjDataTable = Nothing
        End Try

    End Function


    Public Function CollectAccDetails(ByVal DetailsType As ACCESSTYPE) As UserCredential
        'In USE 25 OCT 2010*************************
        'Variable Declaration Start ================================================
        Dim ObjDb As DbaseServiceOLEDB
        Dim RefObjDataSet As DataSet
        Dim RefObjDataTable As DataTable
        Dim SQL_QUERY As String
        Dim ObjLog As LoggerService
        Dim ObjUserCred As UserCredential
        Dim Command As DbCommand
        'Variable Declaration End  ================================================
        ObjLog = LoggerService.gtInstance


        '--------- Synchronize ACCESSUSAGE and MacDetails based on Access code or Mac or Access ID -----'
        ' This functionality will consider the Room transfer and Guest name change schenario
        Try

            If Me._AccCode <> "" Then
                '------------------------------------------------------------------------------
                '---- For Room transfer, display message to a guest differently, in case
                '---- he/she entered the old room no
                RoomTransfer.EvaluateRoomTransferByAccessCode(Me._AccCode)
                '------------------------------------------------------------------------------
                ExtendedUtil.SynchronizeByAccessCode(Me._AccCode)
            ElseIf Me._AccMAC <> "" Then
                ExtendedUtil.SynchronizeByMac(Me._AccMAC)
            ElseIf Me._ACCID > 0 Then
                ExtendedUtil.SynchronizeByAccessID(Me._ACCID.ToString())
            End If

        Catch ex As Exception

        End Try
        '---------------------------------------------------------------------------'


        Try
            ObjDb = DbaseServiceOLEDB.getInstance
            Command = ObjDb.GetCommand()

            SQL_QUERY = ""
            Select Case DetailsType
                Case ACCESSTYPE.ONLYMAC
                    'Check By MAC #####################
                    SQL_QUERY = "SELECT ID,ACC_ROOM,GuestID, ACC_GuestRegCode FROM MacDetails INNER JOIN ACCESSUSAGE ON MacDetails.ACCID=ACCESSUSAGE.ID WHERE ACC_STATUS='A' AND MAC=@PMAC"
                    ObjDb.AddInputParameter(Command, "@PMAC", DbType.String, _AccMAC)
                    'Check By MAC END #################
                Case ACCESSTYPE.ONLYCODE
                    'Check By AccessCode ##########################
                    SQL_QUERY = "SELECT ID,ACC_ROOM, ACC_GuestRegCode,GuestId FROM ACCESSUSAGE INNER JOIN Guest ON ACCESSUSAGE.ACC_GuestRegCode=Guest.GuestRegCode AND Guest.GuestStatus='A'" & _
                    vbCrLf & " AND ACCESSUSAGE.ACC_STATUS <> 'D'" & _
                    vbCrLf & " WHERE ACC_CODE=@PACCCODE"
                    'Check By AccessCode END ######################
                    ObjDb.AddInputParameter(Command, "@PACCCODE", DbType.String, _AccCode)
                Case ACCESSTYPE.ONLYID
                    'Check By ID ##################
                    SQL_QUERY = "SELECT ID,ACC_ROOM, ACC_GuestRegCode,GuestId FROM ACCESSUSAGE INNER JOIN Guest ON ACCESSUSAGE.ACC_GuestRegCode=Guest.GuestRegCode AND Guest.GuestStatus='A'" & _
                    vbCrLf & " AND ACCESSUSAGE.ACC_STATUS<>'D'" & _
                    vbCrLf & " WHERE ID=" & _ACCID
                    'Check By ID END ##############
                    ObjDb.AddInputParameter(Command, "@PACCCODE", DbType.Int64, _ACCID)
                Case ACCESSTYPE.ONLYROOM
                    SQL_QUERY = "SELECT ID,ACC_ROOM, ACC_GuestRegCode,GuestId FROM ACCESSUSAGE INNER JOIN Guest ON ACCESSUSAGE.ACC_GuestRegCode=Guest.GuestRegCode AND Guest.GuestStatus='A'" & _
                    vbCrLf & " AND ACCESSUSAGE.ACC_STATUS='I'" & _
                    vbCrLf & " WHERE ACC_ROOM=@PROOMNO AND Guest.GuestId=@PGUESTID AND ACCESSUSAGE.ACC_ADUID IS NULL"
                    ObjDb.AddInputParameter(Command, "@PROOMNO", DbType.String, _RoomNo)
                    ObjDb.AddInputParameter(Command, "@PGUESTID", DbType.Int64, _GuestID)
            End Select
            Command.CommandText = SQL_QUERY
            RefObjDataTable = ObjDb.DsWithoutUpdateWithParam(Command)
            ' ObjLog.write2LogFile("ACCDETAILS", SQL_QUERY & vbNewLine)

            If RefObjDataTable.Rows.Count > 0 Then
                ObjUserCred.ACCID = Long.Parse(RefObjDataTable.Rows(0).Item("ID").ToString())
                ObjUserCred.GuestID = Long.Parse(RefObjDataTable.Rows(0).Item("GuestID").ToString())
                ObjUserCred.passwd = RefObjDataTable.Rows(0).Item("ACC_GuestRegCode")
                ObjUserCred.usrId = RefObjDataTable.Rows(0).Item("ACC_ROOM")
            Else
                ObjUserCred.ACCID = 0
                ObjUserCred.GuestID = 0
                ObjUserCred.passwd = ""
                ObjUserCred.usrId = ""
            End If

        Catch ex As Exception
            ObjUserCred.ACCID = 0
            ObjUserCred.GuestID = 0
            ObjUserCred.passwd = ""
            ObjUserCred.usrId = ""
        Finally
            ObjDb = Nothing
            RefObjDataSet = Nothing
            RefObjDataTable = Nothing
        End Try
        Return ObjUserCred
    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '-- Split the Correct GuestName string based on spaces
    '-- loop through each substring and check if its matches with method input parameter enteredLastName
    '-- if matches return true otherwise return false
    '############## Jan 11,2011-Method Description End   ############################

    Public Function CheckLastName(ByVal GuestName As String, ByVal enteredLastName As String) As Boolean

        enteredLastName = enteredLastName.Trim().ToUpper()
        GuestName = GuestName.Trim().ToUpper()

        Dim initials(8) As String
        initials(0) = "MR"
        initials(1) = "MS"
        initials(2) = "DR"
        initials(3) = "CAPT"
        initials(4) = "CHEF"
        initials(5) = "PROF"
        initials(6) = "MRS"
        initials(7) = "COL"


        Dim tokens As List(Of String) = GuestName.Split(" ".ToCharArray()).ToList()
        Dim filteredtokens As New List(Of String)
        filteredtokens.Clear()
        For Each token As String In tokens
            If token.Trim() <> "" Then
                filteredtokens.Add(token)
            End If
        Next

        If filteredtokens.Count > 0 Then
            Dim firstword As String = filteredtokens(0).Replace(".", "")
            If initials.Contains(firstword) Then
                filteredtokens.RemoveAt(0)
            End If
        End If

        Dim nextTokenList As New List(Of String)
        nextTokenList.Clear()
        If filteredtokens.Count = 1 Then
            nextTokenList = filteredtokens(0).Split(".".ToCharArray()).ToList()
            For i As Integer = 0 To nextTokenList.Count - 1
                filteredtokens.Add(nextTokenList(i))
            Next
        End If


        For Each token As String In filteredtokens
            token = token.Trim()
            If String.Compare(enteredLastName, token, True) = 0 Then
                Return True
            End If
        Next
        Return False

    End Function


    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to update ACCESSUSAGE table with ACC_STATUS as A
    '-- and ActivationDT as currenttime
    '-- ACC_STATUS A to indicate guest logged in successfully
    '############## Jan 11,2011-Method Description End   ############################
    Public Function SetAccessDetails(ByVal ACCID As Long) As Boolean
        'In USE 25 OCT 2010*************************
        'Variable Declaration Start ================================================
        Dim ObjDb As DbaseServiceOLEDB
        Dim SQL_QUERY As String
        Dim ObjLog As LoggerService
        Dim CTime As DateTime
        Dim ObjDataSet As DataSet
        Dim Acc_code As String
        'Variable Declaration End  =================================================
        Try
            ObjLog = LoggerService.gtInstance
            'ObjLog.write2LogFile("ACCESSUSAGESET", "Update MAC for AccessId:" & ACCID)
            CTime = Now()
            SQL_QUERY = "SELECT ACC_CODE from ACCESSUSAGE WHERE ID=" & ACCID & " AND ACC_STATUS<>'A' AND ACC_STATUS<>'D'"
            ObjDb = DbaseServiceOLEDB.getInstance
            ObjDataSet = ObjDb.DsWithoutUpdate(SQL_QUERY)
            If ObjDataSet.Tables(0).Rows.Count > 0 Then
                SQL_QUERY = "UPDATE ACCESSUSAGE SET ACC_STATUS='A',ActivationDT='" & CTime & "' WHERE ID=" & ACCID & ""
                Acc_code = ObjDataSet.Tables(0).Rows(0).Item("ACC_CODE").ToString()
                ObjLog = LoggerService.gtInstance
                ' ObjLog.write2LogFile("ACCUSAGESET", SQL_QUERY)
                ObjDb.insertUpdateDelete(SQL_QUERY)
                SQL_QUERY = "UPDATE couponmaster SET Status='A' WHERE AccessCode='" & Acc_code & "'"
                'ObjLog.write2LogFile("ACCUSAGESET", SQL_QUERY)
                ObjDb.insertUpdateDelete(SQL_QUERY)
            End If
        Catch ex As Exception
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("ACCEXP", ex)
        End Try
    End Function
    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to update ACCESSUSAGE with the ADUSERID and ADPASSWORD
    '############## Jan 11,2011-Method Description END   ############################
    Public Function UpdateADUIDPWDByID(ByVal ADID As String, ByVal ADPWD As String, ByVal ACCID As Long) As Boolean
        Dim ObjDb As DbaseServiceOLEDB
        Dim SQL_QUERY As String = ""
        Try
            SQL_QUERY = "UPDATE ACCESSUSAGE SET ACC_ADUID='" & ADID & "',ACC_ADPWD='" & ADPWD & "' WHERE ID=" & ACCID
            ObjDb = DbaseServiceOLEDB.getInstance
            ObjDb.insertUpdateDelete(SQL_QUERY)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function SetRoomShiftDetails(ByVal ObjCred As UserCredential, ByVal RoomNo As String) As Boolean
        'In USE 25 OCT 2010*************************
        'Variable Declaration Start ================================================
        Dim ObjDb As DbaseServiceOLEDB
        Dim SQL_QUERY As String
        Dim ObjLog As LoggerService
        'Variable Declaration End  =================================================

        ObjLog = LoggerService.gtInstance
        Try
            ObjDb = DbaseServiceOLEDB.getInstance
            SQL_QUERY = "UPDATE ACCESSUSAGE SET ACC_ROOM='" & RoomNo & "' WHERE ID=" & ObjCred.ACCID & " AND ACC_GuestRegCode='" & ObjCred.passwd & "'"
            ' ObjLog.write2LogFile("ROOMSHIFT", "===Update AccessUsage===" & SQL_QUERY & vbNewLine)
            ObjDb.insertUpdateDelete(SQL_QUERY)

            SQL_QUERY = "UPDATE MacDetails SET ROOMNO='" & RoomNo & "' WHERE ACCID=" & ObjCred.ACCID & " AND GuestId=" & ObjCred.GuestID
            'ObjLog.write2LogFile("ROOMSHIFT", "===Update MacDetails===" & SQL_QUERY & vbNewLine)
            ObjDb.insertUpdateDelete(SQL_QUERY)
            Return True
        Catch ex As Exception
            ObjLog.writeExceptionLogFile("ROOMSHIFTEXP", ex)
            Return False
        Finally
            ObjDb = Nothing
        End Try

    End Function

    
    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to get the ACCESSCODE by passing the ACCID
    '-- it will retrieve the record from ACCESSUSAGE table
    '############## Jan 11,2011-Method Description End   ############################
    Public Function GetAccCode(ByVal ACCID As Long) As String
        Dim ObjDb As DbaseServiceOLEDB
        Dim SQL_QUERY As String = ""
        Dim ObjDataSet As DataSet
        Dim ACC_CODE As String
        SQL_QUERY = "SELECT ACC_CODE FROM ACCESSUSAGE WHERE [ID]=" & ACCID & " AND ACC_STATUS<>'D'"
        Try
            ObjDb = DbaseServiceOLEDB.getInstance
            ObjDataSet = ObjDb.DsWithoutUpdate(SQL_QUERY)
            If ObjDataSet.Tables(0).Rows.Count > 0 Then
                ACC_CODE = ObjDataSet.Tables(0).Rows(0).Item("ACC_CODE")
            Else
                ACC_CODE = ""
            End If

        Catch ex As Exception
            ACC_CODE = ""
        End Try
        Return ACC_CODE
    End Function

    Public Shared Function IsAccessCodeActive(ByVal ACCID As String) As Boolean
        Dim SQL_query As String
        Dim objDbase As DbaseServiceOLEDB
        Dim refDataSet As DataSet
        objDbase = DbaseServiceOLEDB.getInstance

        SQL_query = "Select ID From ACCESSUSAGE Where ID = '" & ACCID & "' And ACC_STATUS <> 'D'"
        refDataSet = objDbase.DsWithoutUpdate(SQL_query)

        If refDataSet.Tables(0).Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function

End Class
