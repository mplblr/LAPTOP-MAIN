'############## Jan 11,2011-Class Description Start ############################
'-- The idea behind having generic GatewayServiceFactory is 
'-- same as discussed in PMSServiceFatory
'############## Jan 11,2011-Class Description End   ############################
Public Class GatewayResults
    Dim status As String
    Dim errorNo As String
    Dim errorMessage As String
    Dim dataVal As String

    Sub New()
        status = ""
        errorNo = ""
        errorMessage = ""
        dataVal = ""
    End Sub

    Protected Overridable Sub initResults(ByRef gtResponse As Hashtable)
        status = gtResponse.Item("gtStatus")
        errorNo = gtResponse.Item("gtErrorNo")
        errorMessage = gtResponse.Item("gtErrorMessage")
        dataVal = gtResponse.Item("gtDataVal")
    End Sub


    ReadOnly Property gtStatus()
        Get
            Return status
        End Get
    End Property

    ReadOnly Property gtErrorNo()
        Get
            Return errorNo
        End Get
    End Property

    ReadOnly Property gtErrorMsg()
        Get
            Return errorMessage
        End Get
    End Property

    ReadOnly Property gtDataVal()
        Get
            Return dataVal
        End Get
    End Property
End Class

Public Interface IGatewayService
    Function add(ByVal userContext As UserContext) As GatewayResults
    Function delete(ByVal userContext As UserContext) As GatewayResults
    Function update(ByVal userContext As UserContext) As GatewayResults
    Function query(ByVal userContext As UserContext) As Object
    Function payment(ByVal userContext As UserContext) As GatewayResults
End Interface

Public Class GatewayServiceFactory
    Private Shared GatewayServiceInst As GatewayServiceFactory

    Private Sub New()
        'Nothing
    End Sub

    Public Shared Function getInstance() As GatewayServiceFactory
        If GatewayServiceInst Is Nothing Then GatewayServiceInst = New GatewayServiceFactory
        Return GatewayServiceInst
    End Function

    Public Function getGatewayService(ByVal pmsName As String, ByVal pmsVersion As String) As IGatewayService
        Return New RadiusGtService
    End Function

End Class