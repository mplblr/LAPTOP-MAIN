Imports System.Configuration
Imports System.Data
Imports System.Data.Common
Imports security

Namespace Microsense.CodeBase

    Public Class DatabaseUtil

        Private Shared factory As DbProviderFactory

        Public Shared Function GetConnection() As DbConnection


            If factory Is Nothing Then
                factory = DbProviderFactories.GetFactory("System.Data.SqlClient")
            End If
            Dim connection As DbConnection = factory.CreateConnection()

            Dim connectionString As String = ConfigurationSettings.AppSettings("Databaseconnection")
            connection.ConnectionString = connectionString
            Return connection

        End Function

        Public Shared Function GetCommand(ByVal connection As DbConnection) As DbCommand
            Dim command As DbCommand = connection.CreateCommand()
            Return command
        End Function

        Public Shared Sub AddInputParameter(ByVal command As DbCommand, ByVal parameterName As String, ByVal parameterType As DbType, ByVal value As String)
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

        Public Shared Sub AddInputParameter(ByVal command As DbCommand, ByVal parameterName As String, ByVal parameterType As DbType, ByVal value As String, ByVal size As Integer)
            Dim parameter As DbParameter = command.CreateParameter()
            parameter.ParameterName = parameterName
            parameter.DbType = parameterType

            If value = "" Or value = Nothing Then
                parameter.Value = DBNull.Value
            Else
                parameter.Value = value
            End If

            parameter.Size = size
            command.Parameters.Add(parameter)
        End Sub

        Public Shared Sub AddOutputParameter(ByVal command As DbCommand, ByVal parameterName As String, ByVal parameterType As DbType)
            Dim parameter As DbParameter = command.CreateParameter()
            parameter.ParameterName = parameterName
            parameter.DbType = parameterType
            parameter.Direction = ParameterDirection.Output
            command.Parameters.Add(parameter)
        End Sub

        Public Shared Sub AddOutputParameter(ByVal command As DbCommand, ByVal parameterName As String, ByVal parameterType As DbType, ByVal size As Integer)
            Dim parameter As DbParameter = command.CreateParameter()
            parameter.ParameterName = parameterName
            parameter.DbType = parameterType
            parameter.Direction = ParameterDirection.Output
            parameter.Size = size
            command.Parameters.Add(parameter)
        End Sub

        Public Shared Function ExecuteSelect(ByVal selectCommand As DbCommand) As DataTable
            Try
                selectCommand.Connection.Open()
                Dim reader As DbDataReader = selectCommand.ExecuteReader()
                Dim table As New DataTable()
                table.Load(reader)
                reader.Close()
                Return table
            Catch ex As Exception
                Throw ex
            Finally
                selectCommand.Connection.Close()
            End Try
        End Function


        Public Shared Function ExecuteInsertUpdateDelete(ByVal command As DbCommand) As Integer
            Try
                command.Connection.Open()
                Return command.ExecuteNonQuery()
            Catch ex As Exception
                Throw ex
            Finally
                command.Connection.Close()
            End Try
        End Function

    End Class

End Namespace

