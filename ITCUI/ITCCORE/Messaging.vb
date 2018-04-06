Public Class Messaging

    Public Shared LineSeperator As String = "|"

    Public Shared CouponDeactivatedMessage As String = ""
    Public Shared TechnicalErrorMessage As String = ""
    Public Shared NomadixNotRegisteredMessage As String = ""
    Public Shared DeviceExceededMessage As String = ""
    Public Shared BCInternetServiceNotActivatedMessage As String = ""
    Public Shared AccessDenied As String = ""

    Public Shared IncorrectAccessCodeMessage As String = ""
    Public Shared CouponNotActivatedMessage As String = ""


    Public Shared noauth As String = ""

    Shared Sub New()
        CouponDeactivatedMessage = String.Format("Dear Guest,{0}The Internet service De-activated.", LineSeperator)
        TechnicalErrorMessage = String.Format("Dear Guest,{0}We are sorry there is a technical error at this time.{0}Kindly inform WelcomAssistance by dialling 6.", LineSeperator)
        NomadixNotRegisteredMessage = "Your Nomadix is not registered to work with this billing system."
        DeviceExceededMessage = String.Format("Dear Guest,{0}{1}{0}Kindly dial 6 for assistance.{0}Thank you!", LineSeperator, CSysConfig.DeviceExceededMessage)
        BCInternetServiceNotActivatedMessage = String.Format("Dear Guest,{0}Your Internet service is not yet activated.{0}Please contact your Business Centre Associate.{0}Thank you!", LineSeperator)
        AccessDenied = "Sorry! Access Denied."

        IncorrectAccessCodeMessage = String.Format("Dear Guest,{0}You have entered an Incorrect Room number or Last Name/FirstName  . Kindly re-enter correct Room number or Last Name/FirstName.", LineSeperator)
        CouponNotActivatedMessage = String.Format("Dear Guest,{0}Your Room number or Last Name/FirstName not Activated. Kindly inform WelcomAssistance.", LineSeperator)
        noauth = String.Format("Kindly call the Duty Manager for access to Internet Usage", LineSeperator)
    End Sub

End Class