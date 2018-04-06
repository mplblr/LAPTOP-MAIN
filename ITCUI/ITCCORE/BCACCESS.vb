'############## Jan 11,2011-Class Description Start ############################
'-- This class is Used for BC Machine related issues
'-- It mainly deals with two table BcMachine and BcMachineUsage
'-- It exposes Following properties
'-- isBCMachine: To identify requested MAC is BC Machine mac or not
'-- isActiveBCMachine: Particular BC Machine MAC is in Active State or not
'-- isNSG: If the particular guest assigned to use particular bc machine is Non Staying Guest or not
'-- isRequiredValidity: If the default 30 minutes free use is edited by BC Admin or not
'-- DefaultPlanID: get the default planid(for free access)
'-- FREEACCTIME: To get the no of minutes edited by the BC Admin for free access
'-- RoomNo: Return the RoomNo of the guest allowed to use the bc machine
'-- GuestId: Return the GuestId of the guest allowed to use the bc machine
'############## Jan 11,2011-Class Description End   ############################
Public Class BCACCESS
    Private BCMAC As String
    Private BCBILLROOMNO As String
    Private BCACCESSID As Long
    Private BCID As Long
    Private BCSTATUS As String
    Private BCGUESTID As Long
    Private BCNSGID As Integer
    Private BCUSEFREEVALIDITY As Integer
    Private BCFREEACCTIME As Long
    Public Sub New(ByVal MAC As String)
        BCMAC = Nothing
        BCBILLROOMNO = Nothing
        BCACCESSID = 0L
        BCID = 0L
        BCGUESTID = 0L
        BCSTATUS = Nothing
        BCNSGID = 0
        BCUSEFREEVALIDITY = 0
        BCFREEACCTIME = 0L
        Find_BCDetails(MAC)
    End Sub

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method is called inside the Class constructor
    '-- This will do the following tasks:
    '-- It will first check in BCMachine table that requested MAC present or not
    '-- if present that indicates the requested MAC is BC Machine MAC and get the BCID
    '-- it will get the most recent details from the BCMchineUsage table for particular BCID
    '-- It will populate local private variables with all the collected details
    '############## Jan 11,2011-Method Description End   ############################
    Private Sub Find_BCDetails(ByVal MAC As String)
        Dim SQL_query As String
        Dim BCMasterDetails, BCDetails As DataSet
        Dim objDBase As DbaseServiceOLEDB
        Dim ObjLog As LoggerService
        Dim AccessTime As Long
        AccessTime = 0L

        ObjLog = LoggerService.gtInstance
        SQL_query = "select BCMachineId,BCMachineMAC from BCMachine where BCMachineMAC='" & MAC & "'"
        ObjLog.write2LogFile("BC", "GetBCID:" & SQL_query)
        Try
            objDBase = DbaseServiceOLEDB.getInstance
            BCMasterDetails = objDBase.DsWithoutUpdate(SQL_query)
            If BCMasterDetails.Tables(0).Rows.Count > 0 Then
                BCID = BCMasterDetails.Tables(0).Rows(0).Item("BCMachineId")
                BCMAC = BCMasterDetails.Tables(0).Rows(0).Item("BCMachineMAC")
            Else
                BCID = 0L
                BCMAC = Nothing
            End If

            If BCID > 0L Then
                SQL_query = "select RoomNo,Status,PlanID,GuestID,NSVisitorID,Validity from BCMachineUsage where ID=(select Max(ID) from BCMachineUsage where BCID=" & BCID & " and Status='A')"
                ObjLog.write2LogFile("BC", "GetBCDetails:" & SQL_query)

                BCDetails = objDBase.DsWithoutUpdate(SQL_query)
                If BCDetails.Tables(0).Rows.Count > 0 Then
                    BCBILLROOMNO = BCDetails.Tables(0).Rows(0).Item("RoomNo")
                    BCACCESSID = BCDetails.Tables(0).Rows(0).Item("PlanID")
                    BCSTATUS = BCDetails.Tables(0).Rows(0).Item("Status")
                    BCGUESTID = BCDetails.Tables(0).Rows(0).Item("GuestID")
                    BCNSGID = BCDetails.Tables(0).Rows(0).Item("NSVisitorID")
                    AccessTime = BCDetails.Tables(0).Rows(0).Item("Validity")
                    If AccessTime > 0L Then
                        BCUSEFREEVALIDITY = 1
                        BCFREEACCTIME = AccessTime
                    Else
                        BCUSEFREEVALIDITY = 0
                        BCFREEACCTIME = 1800
                    End If
                Else
                    BCBILLROOMNO = Nothing
                    BCACCESSID = 0L
                    BCGUESTID = 0L
                    BCSTATUS = Nothing
                    BCNSGID = 0
                    BCFREEACCTIME = 0L
                    BCUSEFREEVALIDITY = 0
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub
    '############## Jan 11,2011-Method Description Start ############################
    '-- This method will return the altpassword for particular GuestId
    '############## Jan 11,2011-Method Description End   ############################
    Public Function GetAltPassword(ByVal GuestId As Integer) As String
        Dim SQL_query As String
        Dim GuestDetails As DataSet
        Dim objDBase As DbaseServiceOLEDB
        Dim GuestAltPassowrd As String
        GuestAltPassowrd = ""
        Try
            SQL_query = "select GuestRegCode from guest where guestid=" & GuestId
            objDBase = DbaseServiceOLEDB.getInstance
            GuestDetails = objDBase.DsWithoutUpdate(SQL_query)
            If GuestDetails.Tables(0).Rows.Count > 0 Then
                GuestAltPassowrd = GuestDetails.Tables(0).Rows(0).Item("GuestRegCode")
            End If
        Catch ex As Exception
            GuestAltPassowrd = ""
        End Try
        Return GuestAltPassowrd
    End Function

    '############## Jan 11,2011-Method Description Start ############################
    '-- This method will check if the particular guest allowed to get the default free time or not
    '-- The rule for getting default free access is:
    '-- Any particular guest will get first 30 minutes free for any particular day
    '-- This implies if Guest A comes in today first time he will get 30 minutes free
    '-- Any more access in the same day will be chargeable
    '-- If A comes tomorrow : first 30 minutes access will be free
    '-- BC Admin can change the default 30 minutes free access and increase the free access time
    '-- for any particular guest
    '############## Jan 11,2011-Method Description End   ############################
    Public Function isDefaultPlan() As Boolean
        Dim SQL_query As String
        Dim PlanDetails As DataSet
        Dim objDBase As DbaseServiceOLEDB
        Dim DefaultPlan As Boolean
        Dim CurrentTime, BillTime As DateTime
        Dim BILLPLANID As Integer
        Dim objSysConfig As New CSysConfig
        Dim DefaultPlanDuration As Long

        DefaultPlanDuration = CLng(objSysConfig.GetConfig("DefaultBCDuration")) * 86400

        BILLPLANID = 0
        DefaultPlan = False
        SQL_query = "select BillTime,billPlanId from bill where billid=(select Max(billid) from bill where billgrcid=" & BCGUESTID & " and billtype=2 and billmac='" & BCMAC & "')"
        objDBase = DbaseServiceOLEDB.getInstance
        Try
            CurrentTime = Now
            PlanDetails = objDBase.DsWithoutUpdate(SQL_query)
            If PlanDetails.Tables(0).Rows.Count > 0 Then
                BillTime = PlanDetails.Tables(0).Rows(0).Item("BillTime")
                BILLPLANID = PlanDetails.Tables(0).Rows(0).Item("billPlanId")
                'If BILLPLANID = 6 Then
                '    If DateDiff(DateInterval.Minute, BillTime, CurrentTime) >= 30 And DateDiff(DateInterval.Day, BillTime, CurrentTime) <= 1 Then
                '        DefaultPlan = False
                '    End If
                'Else
                '    DefaultPlan = True
                'End If
                If DateDiff(DateInterval.Second, BillTime, CurrentTime) > DefaultPlanDuration Then
                    DefaultPlan = True
                ElseIf DateDiff(DateInterval.Second, BillTime, CurrentTime) <= BCFREEACCTIME Then
                    DefaultPlan = True
                End If
            Else
                DefaultPlan = True
            End If
        Catch ex As Exception

        End Try
        Return DefaultPlan
    End Function
    ReadOnly Property isBCMachine() As Boolean
        Get
            If BCID > 0L Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    ReadOnly Property isActiveBCMachine() As Boolean
        Get
            If BCSTATUS Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property
    ReadOnly Property isNSG() As Boolean
        Get
            If BCNSGID > 0 Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    ReadOnly Property isRequiredValidity() As Boolean
        Get
            If BCUSEFREEVALIDITY > 0 Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    ReadOnly Property FREEACCTIME() As Long
        Get
            If BCFREEACCTIME > 0L Then
                Return BCFREEACCTIME
            Else
                Return 0
            End If
        End Get
    End Property

    ReadOnly Property DefaultPlanID() As Long
        Get
            Dim ObjSysConfig As New CSysConfig
            If BCACCESSID > 0L Then
                Return BCACCESSID
            Else
                Return ObjSysConfig.GetConfig("DftBCPlanId")
            End If
        End Get
    End Property

    ReadOnly Property RoomNo() As String
        Get
            If BCBILLROOMNO Is Nothing Then
                Return ""
            Else
                Return BCBILLROOMNO
            End If
        End Get
    End Property

    ReadOnly Property GuestID() As Long
        Get
            Dim ObjSysConfig As New CSysConfig
            If BCGUESTID > 0L Then
                Return BCGUESTID
            Else
                Return 0
            End If
        End Get
    End Property

End Class
