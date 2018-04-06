Imports ITCCORE.Microsense.CodeBase
Imports System.Data.Common

Public Class ExtendedUtil

    Public Shared Sub SynchronizeByAccessCode(ByVal accessCode As String)

        accessCode = accessCode.Trim()

        Try
            SynchronizeAccessUsageByAccessCode(accessCode)
        Catch ex As Exception

        End Try

        Try
            SynchronizeMacDetailsByAccessCode(accessCode)
        Catch ex As Exception

        End Try

    End Sub

    Public Shared Function NOINT(ByVal rno As String, ByVal name As String) As String

        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = ""

                sq = "select GuestCFA7 from guest where GuestRoomNo=@RM and ( upper(GuestName) = @PD   or upper(GuestFirstName)  = @PD or upper(GuestName)+upper(GuestFirstName)  = @PD or upper(GuestFirstName) + upper(GuestName)= @PD or upper(GuestName)+ ' ' + upper(GuestFirstName) = @PD or upper(GuestFirstName) + ' ' + upper(GuestName)= @PD ) and Gueststatus='A' and GuestCFA7 like '%NOINT%'  order by guestid desc"

                com.CommandText = sq

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RM", DbType.String, rno)

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@PD", DbType.String, name.ToUpper())

                '  DatabaseUtil.AddInputParameter(com, "@PD", DbType.String, String.Format("{1}{0}", "%", name.ToUpper()))



                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)


                If result.Rows.Count > 0 Then
                    Return "1"
                Else

                    Return "-1"
                End If

            Catch ex As Exception
                Return "-1"
            End Try


        Catch ex As Exception
            Return "-1"
        End Try


    End Function


    Public Shared Function iptv3(ByVal mac As String) As Integer






        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = "select  guestid from guest where gueststatus='A' and  guestroomno='" & mac & "'"

                com.CommandText = sq

                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)


                If result.Rows.Count > 0 Then
                    Return 1
                Else

                    Return 0
                End If

            Catch ex As Exception

            End Try


        Catch ex As Exception

        End Try

    End Function

    Public Shared Function iptv4(ByVal mac As String) As String






        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = "select  guestname from guest where gueststatus='A' guestroomno='" & mac & "'"

                com.CommandText = sq

                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)


                If result.Rows.Count > 0 Then
                    Return 1
                Else

                    Return 0
                End If

            Catch ex As Exception

            End Try


        Catch ex As Exception

        End Try

    End Function


    Public Shared Function iptv5(ByVal mac As String) As String






        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = "select  guestregcode from guest where gueststatus='A' and  guestroomno='" & mac & "'"

                com.CommandText = sq

                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)


                If result.Rows.Count > 0 Then
                    Return result.Rows(0)(0).ToString
                Else

                    Return "1234"
                End If

            Catch ex As Exception

            End Try


        Catch ex As Exception

        End Try

    End Function



    Public Shared Function iptv(ByVal mac As String) As Integer


       



        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = "select  * from rooms where MacAddress='" & mac & "'"

                com.CommandText = sq

                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)


                If result.Rows.Count > 0 Then
                    Return 1
                Else

                    Return 0
                End If

            Catch ex As Exception

            End Try


        Catch ex As Exception

        End Try

    End Function

    Public Shared Function iptv1(ByVal mac As String) As String






        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = "select  roomno from macdetails where Mac='" & mac & "'"

                com.CommandText = sq

                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)


                If result.Rows.Count > 0 Then
                    Return result.Rows(0)(0)
                Else

                    Return 0
                End If

            Catch ex As Exception

            End Try


        Catch ex As Exception

        End Try

    End Function



    Public Shared Function getpwd(ByVal rno As String, ByVal name As String) As String

        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = ""

                ' sq = "select pwd from guest where GuestRoomNo=@RM and (  ( upper(GuestName) = @PD   or upper(GuestFirstName)  = @PD )  or (  upper(GuestName)+upper(GuestFirstName)  = @PD ) or ( upper(GuestFirstName) +upper(GuestName)= @PD )  or ( upper(GuestName)+ ' ' + upper(GuestFirstName) = @PD ) or ( upper(GuestFirstName) + ' ' + upper(GuestName)= @PD )     ) and Gueststatus='A' and pwd is not null order by guestid desc"
                sq = "select pwd from guest where GuestRoomNo=@RM and ( upper(GuestName) = @PD   or  upper(GuestFirstName)  = @PD  or upper(GuestName)+upper(GuestFirstName)  = @PD or upper(GuestFirstName) + upper(GuestName)= @PD or upper(GuestName)+ ' ' + upper(GuestFirstName) = @PD or upper(GuestFirstName) + ' ' + upper(GuestName)= @PD   ) and Gueststatus='A' and pwd is not null order by guestid desc"

                com.CommandText = sq

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RM", DbType.String, rno)

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@PD", DbType.String, name.ToUpper())

                '  DatabaseUtil.AddInputParameter(com, "@PD", DbType.String, String.Format("{1}{0}", "%", name.ToUpper()))



                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)


                If result.Rows.Count > 0 Then
                    Return result.Rows(0)(0).ToString()
                Else

                    Return "-1"
                End If

            Catch ex As Exception
                Return "-1"
            End Try


        Catch ex As Exception
            Return "-1"
        End Try


    End Function

    Public Shared Function getname1(ByVal rno As String, ByVal name As String) As String

        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = ""

                sq = "select GuestName from guest where GuestRoomNo=@RM and pwd=@PD and Gueststatus='A' and  pwd is not null order by guestid desc"

                com.CommandText = sq

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RM", DbType.String, rno)
                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@PD", DbType.String, name)


                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)


                If result.Rows.Count > 0 Then
                    Return result.Rows(0)(0).ToString()
                Else

                    Return "-1"
                End If

            Catch ex As Exception
                Return "-1"
            End Try


        Catch ex As Exception
            Return "-1"
        End Try


    End Function


    Public Shared Function getGuestStatusLog(ByVal guestroomno As String, ByVal guestName As String)
        Try
            Dim SQL_query As String

            Dim objlog As LoggerService
            objlog = LoggerService.gtInstance

            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)
            Catch ex As Exception

            End Try


            Dim refDataSet As DataSet

            SQL_query = "select top 1 GuestId,GuestRoomNo,GuestName from Guest where ( upper(GuestName) = @GNN  or upper(GuestFirstName)  = @GNN or upper(GuestName)+upper(GuestFirstName)  = @GNN or upper(GuestFirstName) + upper(GuestName)= @GNN or upper(GuestName)+ ' ' + upper(GuestFirstName) = @GNN or upper(GuestFirstName) + ' ' + upper(GuestName)= @GNN )   and  GuestRoomNo = @RMM and GuestStatus ='D' order by GuestId desc"
            com.CommandText = SQL_query


            ' DatabaseUtil.AddInputParameter(com, "@GNN", DbType.String, String.Format("{1}{0}", "%", guestName.ToUpper().Trim()))

            Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RMM", DbType.String, guestroomno)

            Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@GNN", DbType.String, guestName.ToUpper().Trim())

            Try
                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)
            Catch ex As Exception
                objlog.write2LogFile("Guest", "GuestERRUDT =" & ex.Message)
            End Try

            Try
                'objlog.write2LogFile("guest_status" & guestroomno, "a=" & result.Rows.Count)
            Catch ex As Exception

            End Try



            '  objlog.write2LogFile("Process", SQL_query)
            If result.Rows.Count > 0 Then

                Try
                    'objlog.write2LogFile("guest_status" & guestroomno, "b=" & result.Rows.Count)
                Catch ex As Exception

                End Try



                Return 1
            Else
                Try
                    'objlog.write2LogFile("guest_status" & guestroomno, "c=" & result.Rows.Count)
                Catch ex As Exception

                End Try





                Return 0
            End If
        Catch ex As Exception
            Return 0
        End Try




    End Function


    Public Shared Function getGuestStatusLog1(ByVal guestroomno As String, ByVal guestName As String)
        Try
            Dim SQL_query As String

            Dim objlog As LoggerService
            objlog = LoggerService.gtInstance

            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)
            Catch ex As Exception

            End Try


            Dim refDataSet As DataSet

            SQL_query = "select GuestId,GuestRoomNo,GuestName from Guest where    GuestRoomNo = @RMM and GuestStatus ='A' order by GuestId desc"
            com.CommandText = SQL_query
            Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RMM", DbType.String, guestroomno)

            Try
                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)
            Catch ex As Exception
                objlog.write2LogFile("Guest", "GuestERRUDT =" & ex.Message)
            End Try
            Try
                ' objlog.write2LogFile("guest_status1" & guestroomno, "a=" & result.Rows.Count)
            Catch ex As Exception

            End Try

            '  objlog.write2LogFile("Process", SQL_query)
            If result.Rows.Count > 0 Then



                Return 0

            Else





                Return 1
            End If
        Catch ex As Exception
            Return 0
        End Try




    End Function

    Public Shared Function getname(ByVal rno As String, ByVal name As String) As String

        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = ""

                sq = "select GuestRegcode from guest where GuestRoomNo=@RM and pwd=@PD and Gueststatus='A' and  pwd is not null order by guestid desc"

                com.CommandText = sq

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RM", DbType.String, rno)
                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@PD", DbType.String, name)


                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)


                If result.Rows.Count > 0 Then
                    Return result.Rows(0)(0).ToString()
                Else

                    Return "-1"
                End If

            Catch ex As Exception
                Return "-1"
            End Try


        Catch ex As Exception
            Return "-1"
        End Try


    End Function

    Public Shared Function getGuestStatus(ByVal guestroomno As String, ByVal guestName As String) As Integer





        Try
            Dim SQL_query As String

            Dim objlog As LoggerService
            objlog = LoggerService.gtInstance

            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)
            Catch ex As Exception

            End Try


            Dim refDataSet As DataSet

            SQL_query = "select top 1 GuestId,GuestRoomNo,GuestName from Guest where ( upper(GuestName)  = @GN   or  upper(GuestFirstName)  = @GN or upper(GuestName)+upper(GuestFirstName)  = @GN or upper(GuestFirstName) + upper(GuestName)= @GN or upper(GuestName)+ ' ' + upper(GuestFirstName) = @GN or upper(GuestFirstName) + ' ' + upper(GuestName)= @GN )  and  GuestRoomNo = @RM and GuestStatus ='A' order by GuestId desc"
            com.CommandText = SQL_query

            ' DatabaseUtil.AddInputParameter(com, "@GN", DbType.String, String.Format("{1}{0}", "%", guestName.ToUpper().Trim()))

            Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RM", DbType.String, guestroomno)
            Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@GN", DbType.String, guestName.ToUpper().Trim())

            Try
                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)
            Catch ex As Exception
                objlog.write2LogFile("Guest", "GuestERRUDT =" & ex.Message)
            End Try

            '  objlog.write2LogFile("Process", SQL_query)
            If result.Rows.Count > 0 Then

                Return 1
            Else






                Return 0
            End If
        Catch ex As Exception
            Return 0
        End Try




    End Function


    Public Shared Function getAcode(ByVal regcode As String) As String

        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = ""

                sq = "select ACC_CODE from accessusage  where acc_status <> 'D' and  ACC_GuestRegCode=@RM"

                com.CommandText = sq

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RM", DbType.String, regcode)






                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)


                If result.Rows.Count > 0 Then
                    Return result.Rows(0)(0).ToString()
                Else

                    Return "-1"
                End If

            Catch ex As Exception
                Return "-1"
            End Try


        Catch ex As Exception
            Return "-1"
        End Try


    End Function


    Public Shared Function getGuestDetails(ByVal guestroomno As String, ByVal guestName As String) As DataTable

        Dim SQL_query As String

        Dim objlog As LoggerService
        objlog = LoggerService.gtInstance

        Dim con As DbConnection
        Dim com As DbCommand
        Dim result As DataTable
        Try

            con = Microsense.CodeBase.DatabaseUtil.GetConnection()
            com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)
        Catch ex As Exception

        End Try


        Dim refDataSet As DataSet

        SQL_query = "select top 1 GuestId,GuestRoomNo,GuestName, guestregcode from Guest where ( upper(GuestName) = @GNN  or upper(GuestFirstName  )   = @GNN or upper(GuestName)+upper(GuestFirstName)  = @GNN or upper(GuestFirstName) + upper(GuestName)= @GNN or upper(GuestName)+ ' ' + upper(GuestFirstName) = @GNN or upper(GuestFirstName) + ' ' + upper(GuestName)= @GNN )   and  GuestRoomNo = @RMM and GuestStatus ='A' order by GuestId desc"
        com.CommandText = SQL_query


        '  DatabaseUtil.AddInputParameter(com, "@GNN", DbType.String, String.Format("{1}{0}", "%", guestName.ToUpper().Trim())) 

        Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RMM", DbType.String, guestroomno)
        Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@GNN", DbType.String, guestName.ToUpper().Trim())

        Try
            result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)
        Catch ex As Exception
            objlog.write2LogFile("Guest", "GuestERRUDT =" & ex.Message)
        End Try

        Return result



    End Function


    Public Shared Function setpwd(ByVal rno As String, ByVal name As String, ByVal pwd As String, ByVal qns As String, ByVal ans As String) As String

        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = ""

                sq = "Update Guest set pwd=@KK, qns=@KK1 , ans=@KK2 where ( upper(GuestName) = @PD  or upper(GuestFirstName)   = @PD or upper(GuestName)+upper(GuestFirstName)  = @PD or upper(GuestFirstName) + upper(GuestName)= @PD or upper(GuestName)+ ' ' + upper(GuestFirstName) = @PD or upper(GuestFirstName) + ' ' + upper(GuestName)= @PD) and GuestRoomNo=@RM and Gueststatus='A'   "

                com.CommandText = sq

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RM", DbType.String, rno)


                ' DatabaseUtil.AddInputParameter(com, "@PD", DbType.String, String.Format("{1}{0}", "%", name.ToUpper().Trim()))

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@KK", DbType.String, pwd)

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@KK1", DbType.String, qns)

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@KK2", DbType.String, ans)

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@PD", DbType.String, name.ToUpper().Trim())

                Microsense.CodeBase.DatabaseUtil.ExecuteInsertUpdateDelete(com)


                If result.Rows.Count > 0 Then
                    Return result.Rows(0)(0).ToString()
                Else

                    Return "-1"
                End If

            Catch ex As Exception
                Return "-1"
            End Try


        Catch ex As Exception
            Return "-1"
        End Try


    End Function

    Public Shared Function getGuestStatus1(ByVal guestroomno As String, ByVal guestName As String) As String





        Try
            Dim SQL_query As String

            Dim objlog As LoggerService
            objlog = LoggerService.gtInstance

            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)
            Catch ex As Exception

            End Try


            Dim refDataSet As DataSet

            SQL_query = "select qns from Guest where ( upper(GuestName)  = @GN   or  upper(GuestFirstName)  = @GN or upper(GuestName)+upper(GuestFirstName)  = @GN or upper(GuestFirstName) + upper(GuestName)= @GN or upper(GuestName)+ ' ' + upper(GuestFirstName) = @GN or upper(GuestFirstName) + ' ' + upper(GuestName)= @GN )  and  GuestRoomNo = @RM and GuestStatus ='A' order by GuestId desc"
            com.CommandText = SQL_query

            ' DatabaseUtil.AddInputParameter(com, "@GN", DbType.String, String.Format("{1}{0}", "%", guestName.ToUpper().Trim()))

            Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RM", DbType.String, guestroomno)
            Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@GN", DbType.String, guestName.ToUpper().Trim())

            Try
                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)
            Catch ex As Exception
                objlog.write2LogFile("Guest", "GuestERRUDT =" & ex.Message)
            End Try

            '  objlog.write2LogFile("Process", SQL_query)
            If result.Rows.Count > 0 Then

                Return result.Rows(0)(0).ToString()
            Else






                Return "-1"
            End If
        Catch ex As Exception
            Return "-1"
        End Try




    End Function

    Public Shared Function getname2(ByVal rno As String, ByVal name As String) As String

        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = ""

                sq = "select GuestRegcode from guest where GuestRoomNo=@RM and ( GuestName=@PD  or  GuestFirstName=@PD or upper(GuestName)+upper(GuestFirstName)  = @PD or upper(GuestFirstName) + upper(GuestName)= @PD or upper(GuestName)+ ' ' + upper(GuestFirstName) = @PD or upper(GuestFirstName) + ' ' + upper(GuestName)= @PD ) and Gueststatus='A' and  pwd is not null order by guestid desc"

                com.CommandText = sq

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RM", DbType.String, rno)
                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@PD", DbType.String, name)


                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)


                If result.Rows.Count > 0 Then
                    Return result.Rows(0)(0).ToString()
                Else

                    Return "-1"
                End If

            Catch ex As Exception
                Return "-1"
            End Try


        Catch ex As Exception
            Return "-1"
        End Try


    End Function

    Public Shared Function getGuestStatus2(ByVal guestroomno As String, ByVal guestName As String) As String





        Try
            Dim SQL_query As String

            Dim objlog As LoggerService
            objlog = LoggerService.gtInstance

            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)
            Catch ex As Exception

            End Try


            Dim refDataSet As DataSet

            SQL_query = "select ans from Guest where ( upper(GuestName)  = @GN   or  upper(GuestFirstName)  = @GN or upper(GuestName)+upper(GuestFirstName)  = @GN or upper(GuestFirstName) + upper(GuestName)= @GN or upper(GuestName)+ ' ' + upper(GuestFirstName) = @GN or upper(GuestFirstName) + ' ' + upper(GuestName)= @GN )  and  GuestRoomNo = @RM and GuestStatus ='A' order by GuestId desc"
            com.CommandText = SQL_query

            ' DatabaseUtil.AddInputParameter(com, "@GN", DbType.String, String.Format("{1}{0}", "%", guestName.ToUpper().Trim()))

            Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RM", DbType.String, guestroomno)
            Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@GN", DbType.String, guestName.ToUpper().Trim())

            Try
                result = Microsense.CodeBase.DatabaseUtil.ExecuteSelect(com)
            Catch ex As Exception
                objlog.write2LogFile("Guest", "GuestERRUDT =" & ex.Message)
            End Try

            '  objlog.write2LogFile("Process", SQL_query)
            If result.Rows.Count > 0 Then

                Return result.Rows(0)(0).ToString()
            Else






                Return "-1"
            End If
        Catch ex As Exception
            Return "-1"
        End Try




    End Function
    Public Shared Function setpwd2(ByVal rno As String, ByVal name As String, ByVal pwd As String) As String

        Try
            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try

                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)

                Dim sq As String = ""

                sq = "Update Guest set pwd=@KK where ( upper(GuestName) = @PD  or upper(GuestFirstName)   = @PD or upper(GuestName)+upper(GuestFirstName)  = @PD or upper(GuestFirstName) + upper(GuestName)= @PD or upper(GuestName)+ ' ' + upper(GuestFirstName) = @PD or upper(GuestFirstName) + ' ' + upper(GuestName)= @PD ) and GuestRoomNo=@RM and Gueststatus='A'   "

                com.CommandText = sq

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@RM", DbType.String, rno)


                ' DatabaseUtil.AddInputParameter(com, "@PD", DbType.String, String.Format("{1}{0}", "%", name.ToUpper().Trim()))

                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@KK", DbType.String, pwd)



                Microsense.CodeBase.DatabaseUtil.AddInputParameter(com, "@PD", DbType.String, name.ToUpper().Trim())

                Microsense.CodeBase.DatabaseUtil.ExecuteInsertUpdateDelete(com)


                If result.Rows.Count > 0 Then
                    Return result.Rows(0)(0).ToString()
                Else

                    Return "-1"
                End If

            Catch ex As Exception
                Return "-1"
            End Try


        Catch ex As Exception
            Return "-1"
        End Try


    End Function
    Public Shared Sub IssueAccessCodeToGuest(ByVal issuedBy As String, ByVal serialNo As String, ByVal accessCode As String, ByVal roomNo As String, ByVal guestName As String, ByVal folio As String)

        If (guestName.Trim() = "") Then

            guestName = "N/A"
        End If

        Dim conn As DbConnection = DatabaseUtil.GetConnection()
        Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

        com.CommandText = "select seriesno from couponmaster where ACCesscode='" & accessCode & "'"
        Dim res As DataTable

        Try
            serialNo = "TTT"


            res = DatabaseUtil.ExecuteSelect(com)

            serialNo = res.Rows(0)(0)
        Catch ex As Exception

        End Try


        conn = DatabaseUtil.GetConnection()
        com = DatabaseUtil.GetCommand(conn)
        com.CommandText = "Insert Into ACCESSUSAGE (IssueDT, IssuedBy, ACC_SERIAL, ACC_CODE, ACC_ROOM, GuestName, ACC_STATUS, ACC_GuestRegCode)  Values (GETDATE(), @IssuedBy, @SerialNo, @AccessCode, @RoomNo, @GuestName, 'I', @Folio)"


        DatabaseUtil.AddInputParameter(com, "@IssuedBy", DbType.String, issuedBy)
        DatabaseUtil.AddInputParameter(com, "@SerialNo", DbType.String, serialNo)
        DatabaseUtil.AddInputParameter(com, "@AccessCode", DbType.String, accessCode)
        DatabaseUtil.AddInputParameter(com, "@RoomNo", DbType.String, roomNo)
        DatabaseUtil.AddInputParameter(com, "@GuestName", DbType.String, guestName)
        DatabaseUtil.AddInputParameter(com, "@Folio", DbType.String, folio)

        DatabaseUtil.ExecuteInsertUpdateDelete(com)





    End Sub



    Public Shared Sub PasswordHistory(ByVal roomNo As String, ByVal guestName As String, ByVal folio As String, ByVal newpwd As String)

        Dim old As String = ""

        Dim conn As DbConnection = DatabaseUtil.GetConnection()
        Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

        com.CommandText = "select pwd from guest where GuestRegCode='" & folio & "'"
        Dim res As DataTable

        Try



            res = DatabaseUtil.ExecuteSelect(com)

            old = res.Rows(0)(0)
        Catch ex As Exception

        End Try


        conn = DatabaseUtil.GetConnection()
        com = DatabaseUtil.GetCommand(conn)
        com.CommandText = "Insert Into pwdhis (LoginTime,Name,Roomno,RegCode,oldpwd,newpwd)  Values (GETDATE(), @IssuedBy, @SerialNo, @AccessCode, @RoomNo, @GuestName)"


        DatabaseUtil.AddInputParameter(com, "@IssuedBy", DbType.String, guestName)
        DatabaseUtil.AddInputParameter(com, "@SerialNo", DbType.String, roomNo)
        DatabaseUtil.AddInputParameter(com, "@AccessCode", DbType.String, folio)
        DatabaseUtil.AddInputParameter(com, "@RoomNo", DbType.String, old)
        DatabaseUtil.AddInputParameter(com, "@GuestName", DbType.String, newpwd)

        DatabaseUtil.ExecuteInsertUpdateDelete(com)





    End Sub


    Public Shared Sub SynchronizeByAccessID(ByVal accessID As String)

        accessID = accessID.Trim()

        Try
            SynchronizeAccessUsageByAccessID(accessID)
        Catch ex As Exception

        End Try

        Try
            SynchronizeMacDetailsByAccessID(accessID)
        Catch ex As Exception

        End Try

    End Sub

    Public Shared Sub SynchronizeByMac(ByVal mac As String)

        mac = mac.Trim()

        Try
            SynchronizeAccessUsageByMac(mac)
        Catch ex As Exception

        End Try

        Try
            SynchronizeMacDetailsByMac(mac)
        Catch ex As Exception

        End Try

    End Sub

    Public Shared Sub SynchronizeGuestNoDays(ByVal guestID As String)

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)
            com.CommandText = "Update Guest Set GuestNofDStay = dbo.GetNoOfDays(GuestChkInTime, GuestExpChkOutTime) " & vbNewLine & _
                              "Where GuestId = @GuestId And GuestStatus = 'A'"

            DatabaseUtil.AddInputParameter(com, "@GuestId", DbType.String, guestID)
            DatabaseUtil.ExecuteInsertUpdateDelete(com)

        Catch ex As Exception
            objLog.writeExceptionLogFile("SynchronizeGuestNoDaysExp", ex)
        End Try

    End Sub

    Private Shared Sub SynchronizeAccessUsageByAccessCode(ByVal accessCode As String)

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Update A Set A.ACC_ROOM = G.GuestRoomNo, A.GuestName = dbo.GetFullName(G.GuestTitle, G.GuestFirstName, G.GuestName) " & vbNewLine & _
                                "From Guest As G Inner Join ACCESSUSAGE As A " & vbNewLine & _
                                "	On A.ACC_GuestRegCode = G.GuestRegCode And (A.ACC_ROOM <> G.GuestRoomNo Or A.GuestName <> dbo.GetFullName(G.GuestTitle, G.GuestFirstName, G.GuestName)) " & vbNewLine & _
                                "	And G.GuestStatus <> 'D' And A.ACC_STATUS <> 'D' And ACC_CODE = @AccessCode"

            DatabaseUtil.AddInputParameter(com, "@AccessCode", DbType.String, accessCode)
            DatabaseUtil.ExecuteInsertUpdateDelete(com)

        Catch ex As Exception
            objLog.writeExceptionLogFile("SynchronizeAccessUsageByAccessCodeExp", ex)
        End Try

    End Sub

    Private Shared Sub SynchronizeAccessUsageByAccessID(ByVal accessID As String)

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Update A Set A.ACC_ROOM = G.GuestRoomNo, A.GuestName = dbo.GetFullName(G.GuestTitle, G.GuestFirstName, G.GuestName) " & vbNewLine & _
                                "From Guest As G Inner Join ACCESSUSAGE As A " & vbNewLine & _
                                "	On A.ACC_GuestRegCode = G.GuestRegCode And (A.ACC_ROOM <> G.GuestRoomNo Or A.GuestName <> dbo.GetFullName(G.GuestTitle, G.GuestFirstName, G.GuestName)) " & vbNewLine & _
                                "	And G.GuestStatus <> 'D' And A.ACC_STATUS <> 'D' And A.ID = @AccessID"

            DatabaseUtil.AddInputParameter(com, "@AccessID", DbType.String, accessID)
            DatabaseUtil.ExecuteInsertUpdateDelete(com)

        Catch ex As Exception
            objLog.writeExceptionLogFile("SynchronizeAccessUsageByAccessIDExp", ex)
        End Try

    End Sub


    Private Shared Sub SynchronizeAccessUsageByMac(ByVal mac As String)

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Update A Set A.ACC_ROOM = G.GuestRoomNo, A.GuestName = dbo.GetFullName(G.GuestTitle, G.GuestFirstName, G.GuestName) " & vbNewLine & _
                                "From Guest As G Inner Join ACCESSUSAGE As A " & vbNewLine & _
                                "	On A.ACC_GuestRegCode = G.GuestRegCode And (A.ACC_ROOM <> G.GuestRoomNo Or A.GuestName <> dbo.GetFullName(G.GuestTitle, G.GuestFirstName, G.GuestName)) " & vbNewLine & _
                                "	And G.GuestStatus <> 'D' And A.ACC_STATUS <> 'D' " & vbNewLine & _
                                "	And A.ID In (Select ACCID From MacDetails Where MAC = @Mac)"

            DatabaseUtil.AddInputParameter(com, "@Mac", DbType.String, mac)
            DatabaseUtil.ExecuteInsertUpdateDelete(com)

        Catch ex As Exception
            objLog.writeExceptionLogFile("SynchronizeAccessUsageByMacExp", ex)
        End Try

    End Sub


    Private Shared Sub SynchronizeMacDetailsByAccessCode(ByVal accessCode As String)

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Update M Set M.ROOMNO = G.GuestRoomNo " & vbNewLine & _
                                "From Guest As G Inner Join MacDetails As M " & vbNewLine & _
                                "	On M.GuestID = G.GuestID And M.ROOMNO <> G.GuestRoomNo " & vbNewLine & _
                                "	And M.ACCID In (Select ID From ACCESSUSAGE Where ACC_CODE = @AccessCode) " & vbNewLine & _
                                "	And G.GuestStatus <> 'D'"

            DatabaseUtil.AddInputParameter(com, "@AccessCode", DbType.String, accessCode)
            DatabaseUtil.ExecuteInsertUpdateDelete(com)

        Catch ex As Exception
            objLog.writeExceptionLogFile("SynchronizeMacDetailsByAccessCodeExp", ex)
        End Try

    End Sub

    Private Shared Sub SynchronizeMacDetailsByAccessID(ByVal accessID As String)

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Update M Set M.ROOMNO = G.GuestRoomNo " & vbNewLine & _
                                "From Guest As G Inner Join MacDetails As M " & vbNewLine & _
                                "	On M.GuestID = G.GuestID And M.ROOMNO <> G.GuestRoomNo " & vbNewLine & _
                                "	And G.GuestStatus <> 'D' And M.ACCID = @AccessID"

            DatabaseUtil.AddInputParameter(com, "@AccessID", DbType.String, accessID)
            DatabaseUtil.ExecuteInsertUpdateDelete(com)

        Catch ex As Exception
            objLog.writeExceptionLogFile("SynchronizeMacDetailsByAccessCodeExp", ex)
        End Try

    End Sub

    Private Shared Sub SynchronizeMacDetailsByMac(ByVal mac As String)

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Update M Set M.ROOMNO = G.GuestRoomNo " & vbNewLine & _
                                "From Guest As G Inner Join MacDetails As M " & vbNewLine & _
                                "	On M.GuestID = G.GuestID And M.ROOMNO <> G.GuestRoomNo " & vbNewLine & _
                                "	And G.GuestStatus <> 'D' And M.MAC = @Mac"

            DatabaseUtil.AddInputParameter(com, "@Mac", DbType.String, mac)
            DatabaseUtil.ExecuteInsertUpdateDelete(com)

        Catch ex As Exception
            objLog.writeExceptionLogFile("SynchronizeMacDetailsByMacExp", ex)
        End Try

    End Sub

    Public Shared Function RemoveLaptopMobileMacSettings(ByVal mac As String) As Boolean

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Begin Transaction " & vbNewLine & _
                              "Delete From LaptopMacSettings Where Mac = @mac " & vbNewLine & _
                              "Delete From MobileMacSettings Where Mac = @mac " & vbNewLine & _
                              "Commit Transaction"

            DatabaseUtil.AddInputParameter(com, "@mac", DbType.String, mac)
            DatabaseUtil.ExecuteInsertUpdateDelete(com)
            Return True

        Catch ex As Exception
            objLog.writeExceptionLogFile("RemoveLaptopMobileMacSettingsExp", ex)
            Return False
        End Try

    End Function

    Public Shared Function KeepExtraLogs() As Boolean
        Dim extraLogs As Boolean = True
        Try
            Dim keepLogs As String = ConfigurationSettings.AppSettings("KeepExtraLog").Trim()

            If keepLogs = "0" Then
                extraLogs = False
            End If

        Catch ex As Exception

        End Try
        Return extraLogs

    End Function

    Public Shared Function GetExtraLogNames() As List(Of String)
        Dim extraLogs As New List(Of String)
        extraLogs.Clear()
        extraLogs.Add("ACCDETAILS")
        extraLogs.Add("ACCESSUSAGESET")
        extraLogs.Add("ACCUSAGESET")
        extraLogs.Add("ADRES")
        extraLogs.Add("BC")
        extraLogs.Add("DoNewLogin")
        extraLogs.Add("DoReLogin")
        extraLogs.Add("HTTP_USER_AGENT")
        extraLogs.Add("LAPTOP_APP")
        extraLogs.Add("MACINSERT")
        extraLogs.Add("MOBILE_APP")
        extraLogs.Add("USER_REQUESTED_PAGE")
        extraLogs.Add("Welcome")

        Return extraLogs
    End Function


    Public Shared Function GetLastPlanid(ByVal guestID As String, ByVal accessID As String) As Integer

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Select Top 1 Billplanid as planid " & vbNewLine & _
                              "From Bill " & vbNewLine & _
                              "Where BillGrCId = @GuestID And BillAccID = @AccessID And BillType = 0  " & vbNewLine & _
                              "Order By BillID Desc"

            DatabaseUtil.AddInputParameter(com, "@GuestID", DbType.String, guestID)
            DatabaseUtil.AddInputParameter(com, "@AccessID", DbType.String, accessID)
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then
                Return result.Rows(0)(0)
            Else
                Return 2
            End If

        Catch ex As Exception
            objLog.writeExceptionLogFile("GetLastPlanid", ex)
            Return 2
        End Try

    End Function



    Public Shared Function DidLastBillGetDiscount(ByVal guestID As String, ByVal accessID As String) As Boolean

        Dim objLog As LoggerService = LoggerService.gtInstance()
        Try

            Dim conn As DbConnection = DatabaseUtil.GetConnection()
            Dim com As DbCommand = DatabaseUtil.GetCommand(conn)

            com.CommandText = "Select Top 1 Case When (BillAmount * BilltmpNoofDays) > BillPostedAmount Then 1 Else 0 End As DiscountApplied " & vbNewLine & _
                              "From Bill " & vbNewLine & _
                              "Where BillGrCId = @GuestID And BillAccID = @AccessID And BillType = 0 And BillRaiseType = 0 " & vbNewLine & _
                              "Order By BillID Desc"

            DatabaseUtil.AddInputParameter(com, "@GuestID", DbType.String, guestID)
            DatabaseUtil.AddInputParameter(com, "@AccessID", DbType.String, accessID)
            Dim result As DataTable = DatabaseUtil.ExecuteSelect(com)

            If result.Rows.Count > 0 Then
                Dim discountApplied As String = result.Rows(0)("DiscountApplied").ToString()
                If discountApplied = "1" Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If

        Catch ex As Exception
            objLog.writeExceptionLogFile("DidLastBillGetDiscountExp", ex)
            Return False
        End Try

    End Function

End Class