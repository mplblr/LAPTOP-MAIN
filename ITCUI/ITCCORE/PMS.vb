'############## Jan 11,2011-Class Description Start ############################
'-- This class acts as an abstraction to the user
'-- The idea behind such a class is to hide PMS(Property Management Software)
'-- specific implementation details to the user
'-- so the user is only going to call the getPMSService() method passing the
'-- particular type of PMS and get a instance of that types of class
'-- It has one interface which contains three method newLogin,reLogin and reNew
'-- For example we have two different types of classes for CLS(C) and PMS(P) related 
'-- implementation. So class C and P need to implements IPMSService
'-- When user wants to have C class -- NewLogin process
'-- he needs to do as follows
'-- Dim PMS As IPMSService
'-- Dim PMSFact As PMSServiceFatory
'-- PMSFact = PMSServiceFatory.getInstance
'-- PMS = PMSFact.getPMSService(need to pass the types of PMS he intends to use)
'-- PMS.newLogin(userContext)
'-- Currently in this application we dont implement the idea such way
'############## Jan 11,2011-Class Description End   ############################
Public Interface IPMSService
    Function newLogin(ByRef userContext As UserContext) As String
    Function reLogin(ByRef userContext As UserContext) As String
    Function reNew(ByRef userContext As UserContext) As String
End Interface

Public Class PMSServiceFatory
    Private Shared PMSServiceFacInst As PMSServiceFatory

    Private Sub New()

    End Sub

    Public Shared Function getInstance() As PMSServiceFatory
        If PMSServiceFacInst Is Nothing Then PMSServiceFacInst = New PMSServiceFatory
        Return PMSServiceFacInst
    End Function

    Public Function getPMSService() As IPMSService
        Return New CLSPMSRADIUSIASV1
    End Function

End Class