Imports System.Web.Mail
Imports ITCCORE
Imports ITCCORE.Microsense.CodeBase
Imports System.Data.Common
Imports System.Net.Mail

Public Enum MailTypes
    Login = 0
    NightAudit = 1
    BusinessCenter = 2
End Enum


Public Class Mail

    Public Enum ErrTypes
        'C = Coupon, R  = Room, L = Login, B = Bill, P = Posted, A = Added, N = Nomadix, S = Successfully, F = Failure
        'Re = Relogn, Exp = Expired, Na = NightAudit, Bc = business center, Au - Already used
        'MAC = MachineId, BK = blocked, REG = Registration Code, GT = Getname


        RLBPandANS
        RLBPandANSGT
        RLF
        RLFGT
        RLANSprvBP
        RLFbutBP
        RLFbutBPGT
        RReLANS
        RReLF
        RReLMACchgS
        RReLMACchgF
        RLFMACBK
        RREGLF

        ndxRLBPandANS
        ndxRLF
        ndxRReLANS
        ndxRReLF
        ndxRReLMACchgS
        ndxRReLMACchgF

        CLF
        CLANS
        CReLANS
        CReLF
        CnotAN
        CExp
        CReLMACchgS
        CReLMACchgF

        NaLANSBF
        NaBPS
        NaBF
        NaANF

        BcLANS
        BcLF
        BcReLANS
        BcLFAu
        BcBPS
        BcBF
        RLFGT1
    End Enum



    Private Shared gtMailServiceInst As Mail

    Dim to_address As String = ConfigurationSettings.AppSettings("to")
    Dim f_name As String = ConfigurationSettings.AppSettings("file_name")
    Dim mail_subject As String = ConfigurationSettings.AppSettings("mail_subject")

    'string from_address = ConfigurationSettings.AppSettings["from_address"];
    'string from_address = "msplteam@microsensesoftware.com";
    Dim from_address As String = "support@microsenseindia.com"
    'string from_pwd = ConfigurationSettings.AppSettings["from_pwd"];
    'string from_pwd = "sg$194$MSPL";

    Dim from_pwd As String = "Mplbb@123"
    Dim send_time As String = ConfigurationSettings.AppSettings("send_time")
    Dim d() As String

    '----------------------------------------------------------------
    ' Converted from C# to VB .NET using CSharpToVBConverter(1.2).
    ' Developed by: Kamal Patel (http://www.KamalPatel.net) 
    '----------------------------------------------------------------


    Private Sub New()
        'Nothing
    End Sub

    Public Shared Function getInstance() As Mail
        If gtMailServiceInst Is Nothing Then gtMailServiceInst = New Mail
        Return gtMailServiceInst
    End Function

    Public Function SendAdminMail(ByVal rn As String, ByVal name As String, ByVal pid As String, ByVal err As String, ByVal mac As String, ByVal typeofMail As ErrTypes) As String

    End Function

    'Public Function SendAdminMail(ByVal rn As String, ByVal name As String, ByVal pid As String, ByVal err As String, ByVal mac As String, ByVal typeofMail As ErrTypes) As String
    '    Dim Html_str, Subject, ExpiryTime, mailresult, Hotelname As String
    '    Dim Mtype As Integer

    '    Dim gn As String
    '    Dim strrm As String

    '    Dim UsrTyp As String = "ROOM"
    '    Dim objSysConfig As New CSysConfig
    '    Hotelname = objSysConfig.GetConfig("HotelName")
    '    Html_str = "Dear Associate,<br />" & _
    '      "Please find below an alert-:<br /><br /><br />"
    '    Subject = ""
    '    Select Case (typeofMail)
    '        Case ErrTypes.ndxRReLANS

    '            Dim rs As String = ""
    '            rs = name

    '            Try
    '                rs = f2(name, rn)
    '            Catch ex As Exception

    '            End Try



    '            'Re - Login Successfully
    '            Html_str = Html_str & "<table  border='1' align='center' >" & _
    '                "<tr bgcolor=#00CCFF><td colspan='2'>Information -- " & Hotelname & "</td></tr>" & _
    '                "<tr><td colspan='2'>Re-Login Added in Nomadix Successfully</td></tr>" & _
    '                "<tr><td>RoomNo: </td><td>" & rn & "</td></tr>" & _
    '                "<tr><td>Last Name/First Name: </td><td>" & rs & "</td></tr>" & _
    '                                  "<tr><td>Mac: </td><td>" & mac & "</td></tr>" & _
    '                "<tr><td>LoginTime: </td><td>" & Now() & "</td></tr>" & _
    '                                  "<tr><td>User Type: </td><td> Room </td></tr>" & _
    '               "</table>"
    '            Subject = "Re-Login Succeed:" & Hotelname & " : " & rn
    '            Mtype = MailTypes.Login

    '            Try
    '                f1(rn)
    '            Catch ex As Exception

    '            End Try

    '        Case ErrTypes.ndxRLBPandANS
    '            ' Successful(Login)

    '            Dim rs As String = ""
    '            rs = name

    '            Try
    '                rs = f2(name, rn)
    '            Catch ex As Exception

    '            End Try

    '            Html_str = Html_str & "<table  border='1' align='center' >" & _
    '                "<tr bgcolor=#00CCFF><td colspan='2'>Information -- " & Hotelname & "</td></tr>" & _
    '                "<tr><td colspan='2'>New Login -Success</td></tr>" & _
    '                "<tr><td>RoomNo: </td><td>" & rn & "</td></tr>" & _
    '                "<tr><td>Last Name/ First Name: </td><td>" & rs & "</td></tr>" & _
    '                "<tr><td>Login Type: </td><td>" & "New Login" & "</td></tr>" & _
    '                "<tr><td>Mac: </td><td>" & mac & "</td></tr>" & _
    '                "<tr><td>LoginTime: </td><td>" & Now() & "</td></tr>" & _
    '                                  "<tr><td>User Type: </td><td> Room </td></tr>" & _
    '                "</table>"
    '            Subject = "Login Succeed:" & Hotelname & " : " & rn
    '            Mtype = MailTypes.Login

    '            Try
    '                f1(rn)
    '            Catch ex As Exception

    '            End Try

    '        Case ErrTypes.ndxRLF

    '            Html_str = Html_str & "<table  border='1' align='center' >" & _
    '                 "<tr bgcolor=#00CCFF><td colspan='2'>Error Message -- " & Hotelname & "</td></tr>" & _
    '                 "<tr><td colspan='2'>Invalid Login</td></tr>" & _
    '                 "<tr><td>RoomNo: </td><td>" & rn & "</td></tr>" & _
    '                 "<tr><td>Last Name/First Name/Login Password: </td><td>" & name & "</td></tr>" & _
    '                                    "<tr><td>Mac Address: </td><td>" & mac & "</td></tr>" & _
    '                                     "<tr><td>Failure message </td><td>" & pid & "</td></tr>" & _
    '                 "<tr><td>Failure Date & Time </td><td>" & Now() & "</td></tr>" & _
    '                 "<tr><td>User Type: </td><td> Room </td></tr>" & _
    '                "</table>"
    '            Subject = "Invalid User Login:" & Hotelname & " : " & rn
    '            Mtype = MailTypes.Login










    '    End Select
    '    Call send_mail(Mtype, Subject, Html_str, rn)
    'End Function

    Public Sub send_mail(ByVal mailtype As Integer, ByVal Subject As String, ByVal Body As String, ByVal logIdentifier As String)
        Dim objlog As LoggerService
        objlog = LoggerService.gtInstance

        Try



            'Dim body As String = "<html>"
            'body += "<body>"
            'body += "<table >"
            'body += "<tr>"
            '    body += "<td height =\"200\">"
            'body += "Dear Sir,<br/><br/>"
            'body += "Please find the Hotel Guest Internet Access Report for " + DateTime.Now.AddDays(-1).ToString("dd MMMM yyyy") + "  enclosed herewith.<br/>"
            'body += "Regards,<br/><br/>"

            'body += "Microsense Support<br/><br/>"
            'body += "support@microsenseindia.com<br/><br/>"
            'body += "</td>"
            'body += "</tr>"
            'body += "</table>"
            'body += "</body>"
            'body += "</html>"

            Dim ms As System.Net.Mail.MailMessage = New System.Net.Mail.MailMessage()
            Dim ct As SmtpClient

            Try
                ct = New SmtpClient("124.153.110.194", 25)
            Catch ex As Exception

            End Try


            ms.From = New MailAddress("guestalerts@microsenseindia.com")

            ms.To.Add(New MailAddress("itcgrandcholawifi@gmail.com"))
            ms.CC.Add(New MailAddress("itcgcho_microsense@microsenseindia.com"))

            '  ms.To.Add(New MailAddress("guestalerts@microsenseindia.com"))




            Dim from_address As String = "guestalerts@microsenseindia.com"
            'string from_pwd = ConfigurationSettings.AppSettings["from_pwd"];
            'string from_pwd = "sg$194$MSPL";

            Dim from_pwd As String = "leelamail"



            ' ms.From as New MailAddress(from_address)

            Dim mailMessage As String = Body
            ms.Priority = System.Web.Mail.MailPriority.Normal
            ms.IsBodyHtml = True

            ms.Subject = Subject
            ms.Body = Body








            ct.DeliveryMethod = SmtpDeliveryMethod.Network
            ct.UseDefaultCredentials = False

            ct.Credentials = New System.Net.NetworkCredential(from_address, from_pwd)

            ct.EnableSsl = False

            ct.Send(ms)



        Catch ex As Exception

            objlog.write2LogFile("mailErr", "ex=" & ex.Message)
        End Try
    End Sub
    Public Function f2(ByVal pass As String, ByVal rno As String) As String

        Try
            Dim SQL_Query As String = ""
            Dim objDBase As DbaseServiceOLEDB
            Dim ObjLog As LoggerService
            Dim str As String

            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try
                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)
            Catch ex As Exception

            End Try

            str = "select guestname from guest where guestregcode=@A and gueststatus='A'"


            '  str = "  Insert Into GeneralLoginFailsCleared(FailId, FailTime, FailMsg, FailAccessType, FailAccId, FailMAC, FailRoomNo, FailGuestName, FailRemarks, FailAccessCode)    Select FailId, FailTime, FailMsg, FailAccessType, FailAccId, FailMAC, FailRoomNo, FailGuestName, FailRemarks, FailAccessCode From GeneralLoginFails where FailRoomNo = @FailRoomNo     Order By FailId     Delete From GeneralLoginFails where FailRoomNo = @FailRoomNo"

            DatabaseUtil.AddInputParameter(com, "@A", DbType.String, pass)


            com.CommandText = str
            result = DatabaseUtil.ExecuteSelect(com)

            Return result.Rows(0)(0).ToString()
        Catch ex As Exception

        End Try


    End Function

    Public Sub f1(ByVal FailRoomNo As String)

        Try
            Dim SQL_Query As String = ""
            Dim objDBase As DbaseServiceOLEDB
            Dim ObjLog As LoggerService
            Dim str As String

            Dim con As DbConnection
            Dim com As DbCommand
            Dim result As DataTable
            Try
                con = Microsense.CodeBase.DatabaseUtil.GetConnection()
                com = Microsense.CodeBase.DatabaseUtil.GetCommand(con)
            Catch ex As Exception

            End Try




            str = "  Insert Into GeneralLoginFailsCleared(FailId, FailTime, FailMsg, FailAccessType, FailAccId, FailMAC, FailRoomNo, FailGuestName, FailRemarks, FailAccessCode)    Select FailId, FailTime, FailMsg, FailAccessType, FailAccId, FailMAC, FailRoomNo, FailGuestName, FailRemarks, FailAccessCode From GeneralLoginFails where FailRoomNo = @FailRoomNo     Order By FailId     Delete From GeneralLoginFails where FailRoomNo = @FailRoomNo"

            DatabaseUtil.AddInputParameter(com, "@FailRoomNo", DbType.String, FailRoomNo)

            com.CommandText = str
            DatabaseUtil.ExecuteInsertUpdateDelete(com)
        Catch ex As Exception

        End Try
        

    End Sub


    '----------------------------------------------------------------
    ' Converted from C# to VB .NET using CSharpToVBConverter(1.2).
    ' Developed by: Kamal Patel (http://www.KamalPatel.net) 
    '----------------------------------------------------------------






End Class

