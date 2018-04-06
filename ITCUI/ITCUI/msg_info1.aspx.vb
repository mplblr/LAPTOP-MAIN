
Imports ITCCORE
Partial Public Class msg_info1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim r1 As String = ""
            r1 = Request.QueryString("total")

            Try
                Dim str1() = r1.Split(",")
                Dim ind As Integer = 0
                ind = str1.Length - 1

                r1 = str1(ind)
            Catch ex As Exception

            End Try







            If r1 = "4" Then
                lblerr1.Text = "You can connect upto 4 devices."

            ElseIf r1 = "0" Then
                lblerr1.Visible = False

            Else

                lblerr1.Text = "You can connect " & r1 & "  more devices."
            End If


        Catch ex As Exception

        End Try

        Try
            Dim qs As String

            Dim commonFun As PMSCommonFun

            qs = Request.QueryString("encry")
            commonFun = PMSCommonFun.getInstance

            Dim sip As String
            sip = commonFun.DecrptQueryString("MA", qs)

            If ExtendedUtil.iptv(sip) = 1 Then


                Dim roomno As String

                roomno = ExtendedUtil.iptv1(sip)
                Dim device_count_str As String = ConfigurationManager.AppSettings("IPTVURL")

                Response.Redirect(device_count_str & roomno)

            End If
        Catch ex As Exception

        End Try




    End Sub

    Protected Sub Button2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button2.Click
        Dim qs As String

        Dim commonFun As PMSCommonFun

        qs = Request.QueryString("encry")
        commonFun = PMSCommonFun.getInstance

        Dim sip As String
        sip = commonFun.DecrptQueryString("MA", qs)



        Dim roomno As String

        roomno = ExtendedUtil.iptv1(sip)
        Dim device_count_str As String = ConfigurationManager.AppSettings("IPTVURL")

        Response.Redirect(device_count_str & roomno)
    End Sub
End Class