Imports ITCCORE.Microsense.CodeBase
Imports System.Data.Common

Public Class RoomTransfer

    Public Shared Sub EvaluateRoomTransferByAccessCode(ByVal accessCode As String)

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            'com.CommandText = "Select A.ID As AccessID, A.ACC_ROOM As OldRoom, G.GuestRoomNo As NewRoom " & vbNewLine & _
            '                    "From Guest As G Inner Join ACCESSUSAGE As A " & vbNewLine & _
            '                    "	On A.ACC_GuestRegCode = G.GuestRegCode And (A.ACC_ROOM Is Not Null And G.GuestRoomNo Is Not Null) And (A.ACC_ROOM <> G.GuestRoomNo) " & vbNewLine & _
            '                    "	And G.GuestStatus <> 'D' And A.ACC_STATUS <> 'D' And ACC_CODE = @AccessCode"


            com.CommandText = "Select Top 1 A.ID As AccessID, PG.OldRoom, PG.GuestRoomNo As NewRoom " & vbNewLine & _
                                "From PMSGuestData As PG Inner Join ACCESSUSAGE As A " & vbNewLine & _
                                "	On A.ACC_GuestRegCode = PG.GuestRegCode And (A.ACC_ROOM Is Not Null And PG.GuestRoomNo Is Not Null) " & vbNewLine & _
                                "	And (PG.OldRoom Is Not Null And PG.OldRoom <> '') " & vbNewLine & _
                                "	And A.ACC_STATUS <> 'D' " & vbNewLine & _
                                "	And A.ACC_CODE = @AccessCode " & vbNewLine & _
                                "Order By PG.CreatedDate Desc"

            DatabaseUtil.AddInputParameter(com, "@AccessCode", DbType.String, accessCode)
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then
                Dim accessID As String = result.Rows(0)("AccessID").ToString().Trim()
                Dim oldRoom As String = result.Rows(0)("OldRoom").ToString().Trim()
                Dim newRoom As String = result.Rows(0)("NewRoom").ToString().Trim()

                LogRoomTransfer(accessID, oldRoom, newRoom)

            End If

        Catch ex As Exception
            objLog.writeExceptionLogFile("EvaluateRoomTransferByAccessCodeExp", ex)
        End Try

    End Sub

    Private Shared Sub LogRoomTransfer(ByVal accessID As String, ByVal oldRoom As String, ByVal newRoom As String)

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Exec dbo.CheckRoomTransferModification @AccessID, @OldRoom, @NewRoom"

            DatabaseUtil.AddInputParameter(com, "@AccessID", DbType.String, accessID)
            DatabaseUtil.AddInputParameter(com, "@OldRoom", DbType.String, oldRoom)
            DatabaseUtil.AddInputParameter(com, "@NewRoom", DbType.String, newRoom)

            DatabaseUtil.ExecuteInsertUpdateDelete(com)

        Catch ex As Exception
            objLog.writeExceptionLogFile("LogRoomTransferExp", ex)
        End Try

    End Sub

    'Public Shared Function DisplayRoomTransferMessage(ByVal accessID As String, ByVal enteredRoom As String) As Boolean

    '    Dim objLog As LoggerService = LoggerService.gtInstance()
    '    Try

    '        Dim conn As DbConnection = DatabaseUtil.GetConnection()
    '        Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

    '        com.CommandText = "Select dbo.DisplayRoomTranferMessage(@AccessID, @EnteredRoom) As Accepted"

    '        DatabaseUtil.AddInputParameter(com, "@AccessID", DbType.String, accessID)
    '        DatabaseUtil.AddInputParameter(com, "@EnteredRoom", DbType.String, enteredRoom)
    '        Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

    '        If result.Rows.Count > 0 Then
    '            Dim accepted As String = result.Rows(0)("Accepted").ToString().Trim()

    '            If accepted = "1" Then
    '                'Update DisplayMessageCount
    '                IncreaseRoomTransferMessageCount(accessID)
    '                Return True
    '            Else
    '                Return False
    '            End If

    '        Else
    '            Return False
    '        End If


    '    Catch ex As Exception
    '        objLog.writeExceptionLogFile("DisplayRoomTransferMessageExp", ex)
    '        Return False
    '    End Try

    'End Function

    Public Shared Function DisplayRoomTransferMessage(ByVal accessID As String, ByVal enteredRoom As String) As Boolean

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Select AccessID From RoomTransfer Where AccessID = @AccessID And OldRoom = @EnteredRoom And NewRoom <> @EnteredRoom And MessageDisplayCount < 2"

            DatabaseUtil.AddInputParameter(com, "@AccessID", DbType.String, accessID)
            DatabaseUtil.AddInputParameter(com, "@EnteredRoom", DbType.String, enteredRoom)
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then
                'Update DisplayMessageCount
                IncreaseRoomTransferMessageCount(accessID)
                Return True

            Else
                Return False
            End If


        Catch ex As Exception
            objLog.writeExceptionLogFile("DisplayRoomTransferMessageExp", ex)
            Return False
        End Try

    End Function

    Private Shared Sub IncreaseRoomTransferMessageCount(ByVal accessID As String)

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Exec dbo.IncreaseRoomTransferMessage @AccessID"

            DatabaseUtil.AddInputParameter(com, "@AccessID", DbType.String, accessID)
            DatabaseUtil.ExecuteInsertUpdateDelete(com)

        Catch ex As Exception
            objLog.writeExceptionLogFile("IncreaseRoomTransferMessageCountExp", ex)
        End Try

    End Sub

End Class
