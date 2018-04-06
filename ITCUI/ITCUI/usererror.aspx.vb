Imports ITCCORE

Partial Public Class usererror
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'If Session("Title") Is Nothing Then
        '    msgParaTitle.InnerHtml = "Authentication"
        'End If

        If Session("Message") Is Nothing Then
            msgparaText.InnerHtml = "Sorry! Access Denied."
        End If

        'If Not Session("Title") Is Nothing Then
        '    msgParaTitle.InnerHtml = Session("Title")
        'End If

        If Not Session("Message") Is Nothing Then
            msgparaText.InnerHtml = Session("Message").ToString().Replace(Messaging.LineSeperator, "<br />")
        End If


    End Sub

End Class