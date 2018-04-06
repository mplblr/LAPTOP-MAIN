'############## Jan 11,2011-Class Description Start ############################
'-- This class contains all methods for Guest Information
'-- It mainly get or set values in Guest Tables
'############## Jan 11,2011-Class Description End   ############################
Public Class GuestService
    Private Shared gtGuestServiceinst As GuestService

    Public Sub New()
        'nothing
    End Sub
    Public Shared Function getInstance() As GuestService
        If gtGuestServiceinst Is Nothing Then gtGuestServiceinst = New GuestService
        Return gtGuestServiceinst
    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to get GuestCompCode and GuestNoOfDStay
    '-- It is used to identify the guest should get deduction on planamount or not
    '-- It takes two parameter: GuestId and GuestPara
    '-- GuestPara to identify which value the user wants to get: GuestCompCode
    '-- or GuestNofDStay
    '############## Jan 11,2011-Method Description End   ############################
    Public Function getGuestInfo(ByVal guestId As String, ByVal guestPara As String) As String
        Dim SQL_query As String
        Dim objDbase As DbaseServiceOLEDB
        Dim RefResultset As DataSet
        Dim tmpResult As String

        objDbase = DbaseServiceOLEDB.getInstance

        SQL_query = "Select * from guest where (guestId = " & guestId & ")"
        RefResultset = objDbase.DsWithoutUpdate(SQL_query)

        If RefResultset.Tables(0).Rows.Count > 0 Then
            If guestPara = "CCODE" Then
                If Not IsDBNull(RefResultset.Tables(0).Rows(0).Item("GuestCompCode")) Then
                    tmpResult = RefResultset.Tables(0).Rows(0).Item("GuestCompCode")
                Else
                    tmpResult = ""
                End If
            ElseIf guestPara = "NDS" Then
                tmpResult = RefResultset.Tables(0).Rows(0).Item("GuestNofDStay")
            Else
                tmpResult = ""
            End If
        Else
            tmpResult = ""
        End If
        Return tmpResult
    End Function


    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is used to get guestinformation based on the GuestId
    '-- It returns UserCredential Object
    '############## Jan 11,2011-Method Description End   ############################
    Public Function getGuestInfo(ByVal guestId As String) As UserCredential
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ===========================
        Dim SQL_query As String
        Dim objDbase As DbaseServiceOLEDB
        Dim RefResultset As DataSet
        Dim RefDataTable As DataTable
        Dim usrCrd As New UserCredential
        'Variable Declaration Start ===========================
        Try
            objDbase = DbaseServiceOLEDB.getInstance
            SQL_query = "Select * from guest where (guestId = " & guestId & ") and gueststatus='A'"
            RefResultset = objDbase.DsWithoutUpdate(SQL_query)
            RefDataTable = RefResultset.Tables(0)
            If RefDataTable.Rows.Count > 0 Then
                usrCrd.usrId = RefDataTable.Rows(0).Item("GuestRoomNo")
                usrCrd.passwd = RefDataTable.Rows(0).Item("GuestRegCode")
                usrCrd.GuestID = guestId
            Else
                usrCrd.usrId = ""
                usrCrd.passwd = ""
                usrCrd.GuestID = 0
            End If
        Catch ex As Exception
            usrCrd.usrId = ""
            usrCrd.passwd = ""
            usrCrd.GuestID = 0
        End Try
        Return usrCrd
    End Function

    Public Function GetGuestDaysStay(ByVal GuestID As Long) As Integer
        Dim SQL_query As String
        Dim objDbase As DbaseServiceOLEDB
        Dim RefResultset As DataSet
        Dim NoOfDayStay As Integer = 0
        Dim ObjLog As LoggerService

        'Synchronize the no. of days to stay for a guest on on GuestChkInTime and GuestExpChkOutTime
        Try
            ExtendedUtil.SynchronizeGuestNoDays(GuestID.ToString())
        Catch ex As Exception

        End Try

        'This will consider how many days the guest has remaining for his stay
        SQL_query = "SELECT dbo.GetNoOfDays(GetDate(), GuestExpChkOutTime) As GuestNofDStay FROM Guest WHERE GuestId=" & GuestID

        Try
            objDbase = DbaseServiceOLEDB.getInstance
            RefResultset = objDbase.DsWithoutUpdate(SQL_query)
            If RefResultset.Tables(0).Rows.Count > 0 Then
                NoOfDayStay = Integer.Parse(RefResultset.Tables(0).Rows(0).Item("GuestNofDStay").ToString())
            Else
                NoOfDayStay = 0
            End If
        Catch ex As Exception
            NoOfDayStay = 0
            ObjLog = LoggerService.gtInstance
            ObjLog.writeExceptionLogFile("GuestStayExp", ex)
        End Try
        Return NoOfDayStay
    End Function

    Public Function GetPlanid(ByVal guestid As String, ByVal BillMAC As String) As Integer
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ===========================
        Dim planid As Integer
        Dim SQL_query As String
        Dim objDbase As DbaseServiceOLEDB
        Dim RefResultset As DataSet
        Dim RefDataTable As DataTable
        'Variable Declaration End ===========================
        Try
            objDbase = DbaseServiceOLEDB.getInstance
            SQL_query = "select BillPlanId from bill where billgrcid = " & guestid & " and BillMAC='" & BillMAC & "' order by billid desc"
            RefResultset = objDbase.DsWithoutUpdate(SQL_query)
            RefDataTable = RefResultset.Tables(0)
            If RefDataTable.Rows.Count > 0 Then
                planid = RefDataTable.Rows(0).Item("BillPlanId")
            Else
                planid = -1
            End If
        Catch ex As Exception
            planid = -1
        Finally
            objDbase = Nothing
            RefResultset = Nothing
        End Try

        Return planid
    End Function

    Public Sub UpdateGuestADUserAndPassword(ByVal ADUserName As String, ByVal ADPassword As String, ByVal GuestId As String)
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim SQL_query As String
        Dim objDBase As DbaseServiceOLEDB
        'Variable Declaration End ================================================
        Try
            objDBase = DbaseServiceOLEDB.getInstance
            SQL_query = "update Guest set ADUserId = '" & ADUserName & "', ADPassword ='" & ADPassword & "' where GuestId = " & GuestId
            objDBase.insertUpdateDelete(SQL_query)
        Catch ex As Exception

        Finally
            objDBase = Nothing
        End Try

    End Sub

    Public Function getGuestStatus(ByVal guestroomno As String) As DataSet
        Dim SQL_query As String
        Dim objDbase As DbaseServiceOLEDB
        Dim refDataSet As DataSet
        objDbase = DbaseServiceOLEDB.getInstance

        SQL_query = "select GuestId,GuestRoomNo,GuestRegCode from Guest where GuestRoomNo = '" & guestroomno & "' and GuestStatus ='A' order by GuestId Desc"
        refDataSet = objDbase.DsWithoutUpdate(SQL_query)

        Return refDataSet

    End Function

    Public Function GetGuestName(ByVal GuestID As Long) As String
        Dim SQL_query As String
        Dim objDbase As DbaseServiceOLEDB
        Dim refDataSet As DataSet
        Dim GuestName As String
        Try
            SQL_query = "SELECT GuestName FROM Guest WHERE GuestId=" & GuestID
            objDbase = DbaseServiceOLEDB.getInstance
            refDataSet = objDbase.DsWithoutUpdate(SQL_query)
            If refDataSet.Tables(0).Rows.Count > 0 Then
                GuestName = refDataSet.Tables(0).Rows(0).Item("GuestName")
            Else
                GuestName = ""
            End If
        Catch ex As Exception
            GuestName = ""
        End Try
        Return GuestName
    End Function

End Class
