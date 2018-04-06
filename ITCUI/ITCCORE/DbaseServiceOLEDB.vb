Imports Security
Imports System.Data
Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Configuration
Public Class DbaseServiceOLEDB
    Private Shared gtDbaseSreviceInst As DbaseServiceOLEDB
    Private Shared factory As DbProviderFactory

    Private Sub New()
        'nothing
    End Sub

    Public Shared Function getInstance() As DbaseServiceOLEDB
        If gtDbaseSreviceInst Is Nothing Then gtDbaseSreviceInst = New DbaseServiceOLEDB
        Return gtDbaseSreviceInst
    End Function

    Public Function GetCommand() As DbCommand
        Dim RefCon As DbConnection = GetConnection()
        Dim command As DbCommand = RefCon.CreateCommand()

        Return command
    End Function

    Public Sub AddInputParameter(ByVal command As DbCommand, ByVal parameterName As String, ByVal parameterType As DbType, ByVal value As String)
        Dim parameter As DbParameter = command.CreateParameter()
        parameter.ParameterName = parameterName
        parameter.DbType = parameterType

        If value = "" Or value = Nothing Then
            parameter.Value = DBNull.Value
        Else
            parameter.Value = value
        End If
        command.Parameters.Add(parameter)
    End Sub

    Private Function GetConnection() As DbConnection

        If factory Is Nothing Then
            factory = DbProviderFactories.GetFactory("System.Data.SqlClient")
        End If
        Dim connection As DbConnection = factory.CreateConnection()

        Dim connectionString As String = ConfigurationManager.AppSettings("Databaseconnection")
        connection.ConnectionString = connectionString
        Return connection

    End Function

    'Close the database connection
    Private Sub closeconnection(ByVal conn As DbConnection)
        If conn.State = ConnectionState.Open Then
            conn.Close()
        End If
    End Sub

    Public Function insertUpdateDelete(ByVal str As String, Optional ByVal ConCount As String = "") As Integer

        Dim RefCon As DbConnection = GetConnection()
        Dim ObjCmd As DbCommand = RefCon.CreateCommand()

        Dim ObjLog As LoggerService

        ObjCmd.CommandText = str
        ObjCmd.Connection = RefCon

        ObjLog = LoggerService.gtInstance
        Try
            RefCon.Open()
            insertUpdateDelete = ObjCmd.ExecuteNonQuery

        Catch ex As Exception
            'Throw ex
            If ex.Message.ToUpper().Contains("THREAD") And ex.Message.ToUpper().Contains("ABORT") Then
                'Do nothing
            Else
                ObjLog.write2LogFile("DBEXP", ObjCmd.CommandText)
                ObjLog.writeExceptionLogFile("DBEXP", ex)
            End If

        Finally
            RefCon.Close()

        End Try
    End Function

    'Returns the dataset for the input query to fill the data in DataGrid
    Public Function DsWithoutUpdate(ByVal str As String, Optional ByVal ConCount As String = "") As DataSet

        Dim ObjDAdapter As DbDataAdapter
        Dim ObjDs As New DataSet
        Dim RefCon As DbConnection = GetConnection()
        Dim ObjLog As LoggerService
        ObjLog = LoggerService.gtInstance
        Try
            RefCon.Open()
            ObjDAdapter = New SqlDataAdapter(str, RefCon)

            ObjDAdapter.Fill(ObjDs)
            Return ObjDs

        Catch ex As Exception
            'Throw ex
            ObjLog.write2LogFile("DBEXP", str)
            ObjLog.writeExceptionLogFile("DBEXP", ex)
        Finally
            RefCon.Close()
        End Try
    End Function

    Public Function DsWithoutUpdateWithParam(ByVal selectCommand As DbCommand) As DataTable
        Dim ObjLog As LoggerService
        ObjLog = LoggerService.gtInstance
        Try
            selectCommand.Connection.Open()
            Dim reader As DbDataReader = selectCommand.ExecuteReader()
            Dim table As New DataTable()
            table.Load(reader)
            reader.Close()
            Return table
        Catch ex As Exception
            'Throw ex
            ObjLog.write2LogFile("DBEXP", selectCommand.CommandText)
            ObjLog.writeExceptionLogFile("DBEXP", ex)
        Finally
            selectCommand.Connection.Close()
        End Try
    End Function

End Class

