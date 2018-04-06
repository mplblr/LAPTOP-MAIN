Public Class CPlan
    Dim id As Integer
    Dim name As String
    Dim time As Long 'In Seconds
    Dim validity As Long 'In Seconds
    Dim Amount As Double
    Dim PlanType As Integer = 0
    Dim IsActive As Boolean
    Dim publicip As Boolean
    Dim bandwidthUP, bandwidthDN As Long 'In KB
    Dim planuploadlimit, plandownloadlimit As Long
    Dim _PlanRemark As String

    Public Function getPlaninfo(ByVal ErPlanId As Integer) As Boolean
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim SQL_query As String
        Dim objDbase As DbaseServiceOLEDB
        Dim RefResultset As DataSet
        Dim RefDataTable As DataTable
        Dim planId As Integer
        'Variable Declaration Start ================================================

        objDbase = DbaseServiceOLEDB.getInstance
        id = ErPlanId
        planId = ErPlanId

        Try
            ' Start Getting PlanInformation ===============
            SQL_query = "SELECT * from plans where planid = " & planId
            RefResultset = objDbase.DsWithoutUpdate(SQL_query)
            RefDataTable = RefResultset.Tables(0)
            If RefDataTable.Rows.Count > 0 Then
                name = RefDataTable.Rows(0).Item("planname")
                time = RefDataTable.Rows(0).Item("PlanDuration")
                validity = RefDataTable.Rows(0).Item("PlanValidity")
                Amount = RefDataTable.Rows(0).Item("PlanAmount")
                PlanType = RefDataTable.Rows(0).Item("PlanType")
                IsActive = RefDataTable.Rows(0).Item("IsActivePlan")
                publicip = RefDataTable.Rows(0).Item("PubIPFeature")
                bandwidthUP = RefDataTable.Rows(0).Item("PlanBandup")
                bandwidthDN = RefDataTable.Rows(0).Item("PlanBanddown")
                planuploadlimit = RefDataTable.Rows(0).Item("PlanUploadLimit")
                plandownloadlimit = RefDataTable.Rows(0).Item("plandownloadlimit")
                _PlanRemark = RefDataTable.Rows(0).Item("PlanRemark")
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Return False
        Finally
            RefResultset = Nothing
            RefDataTable = Nothing
            objDbase = Nothing
        End Try

    End Function

    'Public Function getPlaninfo(ByVal Planid As Long, ByVal planType As ECOUPONTYPE) As Boolean
    '    Dim SQL_query As String
    '    Dim objDbase As DbaseService

    '    Dim RefResultset As Odbc.OdbcDataReader

    '    objDbase = DbaseService.getInstance

    '    SQL_query = "SELECT * from plans where planid = " & Planid
    '    RefResultset = objDbase.querySelect(SQL_query)
    '    id = Planid
    '    If RefResultset.Read Then
    '        name = RefResultset("planname")
    '        time = RefResultset("PlanDuration")
    '        validity = RefResultset("PlanValidity")
    '        Amount = RefResultset("PlanAmount")
    '        PlanType = RefResultset("PlanType")
    '        IsActive = RefResultset("IsActivePlan")
    '        publicip = RefResultset("PubIPFeature")
    '        bandwidthUP = RefResultset("PlanBandup")
    '        bandwidthDN = RefResultset("PlanBanddown")
    '        RefResultset.Close()
    '        Return True
    '    Else
    '        RefResultset.Close()
    '        Return False
    '    End If
    'End Function

    Public Function getAllPlans(ByVal reqPlanType As PLANTYPES, ByVal reqPlanStatus As PLANSTATUS, Optional ByVal reqPlanOrderby As PLANORDERBY = PLANORDERBY.Duration) As DataSet
        'In USE 6 AUG 2010*************************
        'Variable Declaration Start ================================================
        Dim QueryPart2 As String = ""
        Dim SQL_query As String = ""
        Dim objDbase As DbaseServiceOLEDB
        Dim refDataSet As DataSet
        'Variable Declaration End ================================================
        Try
            objDbase = DbaseServiceOLEDB.getInstance
            If reqPlanStatus = PLANSTATUS.ACTIVEONLY Then
                QueryPart2 = " And IsActivePlan = 1"
            ElseIf reqPlanStatus = PLANSTATUS.DIACTIVEONLY Then
                QueryPart2 = " And IsActivePlan = 0"
            ElseIf reqPlanStatus = PLANSTATUS.ALL Then
                QueryPart2 = ""
            End If
            Select Case reqPlanOrderby
                Case PLANORDERBY.Duration
                    SQL_query = "select * from plans where PlanType = " & reqPlanType & " " & QueryPart2 & " Order By PlanBandup"
                Case PLANORDERBY.Speed
                    SQL_query = "select * from plans where PlanType = " & reqPlanType & " " & QueryPart2 & " Order By PlanBandup Asc, PlanBanddown"
            End Select
            refDataSet = objDbase.DsWithoutUpdate(SQL_query)
        Catch ex As Exception
            refDataSet = Nothing
        End Try
        Return refDataSet
    End Function
    'Public Function getAllPlans(ByVal reqPlanType As PLANTYPES, ByVal reqPlanStatus As PLANSTATUS, ByVal planid As String, Optional ByVal reqPlanOrderby As PLANORDERBY = PLANORDERBY.Duration) As DataSet
    '    Dim QueryPart2 As String = ""
    '    Dim SQL_query As String = ""
    '    Dim objDbase As DbaseService
    '    Dim refDataSet As DataSet

    '    objDbase = DbaseService.getInstance

    '    If reqPlanStatus = PLANSTATUS.ACTIVEONLY Then
    '        QueryPart2 = " And IsActivePlan = 1"
    '        'QueryPart2 = " And IsActivePlan = True"
    '    ElseIf reqPlanStatus = PLANSTATUS.DIACTIVEONLY Then
    '        'QueryPart2 = " And IsActivePlan = False"
    '        QueryPart2 = " And IsActivePlan = 0"
    '    ElseIf reqPlanStatus = PLANSTATUS.ALL Then
    '        QueryPart2 = ""
    '    End If
    '    Select Case reqPlanOrderby
    '        Case PLANORDERBY.Duration
    '            SQL_query = "select * from plans where PlanId =" & planid & " And PlanType = " & reqPlanType & " " & QueryPart2 & " Order By PlanBandup"
    '        Case PLANORDERBY.Speed
    '            SQL_query = "select * from plans where PlanId =" & planid & " PlanType = " & reqPlanType & " " & QueryPart2 & " Order By PlanBandup Asc, PlanBanddown"
    '    End Select

    '    refDataSet = objDbase.DsWithoutUpdate(SQL_query)

    '    Return refDataSet
    'End Function

    'Public Function getAllPlans(ByVal currentPlanId As Long, ByVal reqPlanType As PLANTYPES, ByVal reqPlanStatus As PLANSTATUS, Optional ByVal reqPlanOrderby As PLANORDERBY = PLANORDERBY.Duration) As DataSet
    '    Dim QueryPart2 As String = ""
    '    Dim SQL_query As String = ""
    '    Dim objDbase As DbaseService
    '    Dim refDataSet As DataSet

    '    objDbase = DbaseService.getInstance

    '    If reqPlanStatus = PLANSTATUS.ACTIVEONLY Then
    '        'QueryPart2 = " And IsActivePlan = True"
    '        QueryPart2 = " And IsActivePlan = 1"
    '    ElseIf reqPlanStatus = PLANSTATUS.DIACTIVEONLY Then
    '        ' QueryPart2 = " And IsActivePlan = False"
    '        QueryPart2 = " And IsActivePlan = 0"
    '    ElseIf reqPlanStatus = PLANSTATUS.ALL Then
    '        QueryPart2 = ""
    '    End If

    '    Select Case reqPlanOrderby
    '        Case PLANORDERBY.Duration
    '            SQL_query = "SELECT * FROM plans WHERE PlanType = " & reqPlanType & " AND PlanBandup > (SELECT PlanBandup FROM plans WHERE planid = " & currentPlanId & ") " & QueryPart2 & " Order By PlanDuration Asc"

    '        Case PLANORDERBY.Speed
    '            SQL_query = "SELECT * FROM plans WHERE PlanType = " & reqPlanType & " AND PlanBandup > (SELECT PlanBandup FROM plans WHERE planid = " & currentPlanId & ") " & QueryPart2 & " Order By PlanBandup Asc, PlanBanddown Asc"
    '    End Select

    '    refDataSet = objDbase.DsWithoutUpdate(SQL_query)

    '    If refDataSet.Tables(0).Rows.Count <= 0 Then
    '        refDataSet = getAllPlans(reqPlanType, reqPlanStatus, reqPlanOrderby)
    '    End If

    '    Return refDataSet
    'End Function

    ReadOnly Property planId() As Integer
        Get
            Return id
        End Get
    End Property

    ReadOnly Property PlanRemark() As String
        Get
            Return _PlanRemark
        End Get
    End Property


    ReadOnly Property planName() As String
        Get
            Return name
        End Get
    End Property

    ReadOnly Property planTime() As Long
        Get
            Return time
        End Get
    End Property

    ReadOnly Property planTimeinHMS() As String
        Get
            Return GetPlanTime2(time)
        End Get
    End Property

    ReadOnly Property planValidity() As Long
        Get
            Return validity
        End Get
    End Property

    ReadOnly Property planValidityinHMS() As String
        Get
            Return GetPlanTime2(validity)
        End Get
    End Property

    ReadOnly Property planAmount() As Double
        Get
            Return Amount
        End Get
    End Property

    ReadOnly Property isCouponPlan() As Boolean
        Get
            Return PlanType
        End Get
    End Property

    ReadOnly Property PlanTypeROOM() As Integer
        Get
            If PlanType > 0 Then
                Return PlanType
            Else
                Return 0
            End If

        End Get
    End Property

    ReadOnly Property isActivePlan() As Boolean
        Get
            Return IsActive
        End Get
    End Property

    ReadOnly Property isPublicIPPlan() As Boolean
        Get
            Return publicip
        End Get
    End Property
    ReadOnly Property planBWUP() As Long
        Get
            Return bandwidthUP
        End Get
    End Property

    ReadOnly Property planBWDN() As Long
        Get
            Return bandwidthDN
        End Get
    End Property

    ReadOnly Property planBWUPLimit() As Long
        Get
            Return planuploadlimit
        End Get
    End Property

    ReadOnly Property planBWDownLimit() As Long
        Get
            Return plandownloadlimit
        End Get
    End Property

    'Returns the clocktimes to the format "HH:MM:SS"  from seconds
    Private Function Sec2HMS(ByVal Clockticks As Long) As String
        Dim HH, MM, SS, Rticks As Long
        HH = CLng(((Clockticks / 3600) * 10 - 4.9) / 10)
        Rticks = Clockticks - (HH * 3600)
        MM = CLng(((Rticks / 60) * 10 - 4.9) / 10)
        SS = Clockticks - (MM * 60 + HH * 3600)
        Return HH & "Hrs: " & MM & "Min: " & SS & "Sec"
    End Function

    Public Function GetPlanTime2(ByVal Planduration As Long) As String
        Dim lngDurationSec As Long
        lngDurationSec = Planduration
        If (lngDurationSec \ 2678400) > 0 Then
            Return CStr(lngDurationSec \ 2678400) & " Month(s)&nbsp;&nbsp;&nbsp;&nbsp;"
        ElseIf (lngDurationSec \ 86400) = 7 Or (lngDurationSec \ 86400) = 14 Or (lngDurationSec \ 86400) = 21 Or (lngDurationSec \ 86400) = 28 Then
            Return CStr(lngDurationSec \ (86400 * 7)) & " Week(s)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        ElseIf (lngDurationSec \ 86400) >= 1 Then
            Return CStr(lngDurationSec \ 86400) & " Day(s)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        ElseIf (lngDurationSec \ 3600) > 0 Then
            Return CStr(lngDurationSec \ 3600) & " Hour(s)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        ElseIf (lngDurationSec \ 60) > 0 Then
            Return CStr(lngDurationSec \ 60) & " Minute(s)"
        End If
    End Function

End Class
