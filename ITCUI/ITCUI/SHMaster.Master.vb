Imports ITCCORE
Imports System.Web.UI.HtmlControls
Partial Public Class SHMaster
    Inherits System.Web.UI.MasterPage

    'Public ReadOnly Property GetPrintLink() As HtmlAnchor
    '    Get
    '        Return LiPrintLink
    '    End Get
    'End Property

    'Public ReadOnly Property GetFAQLink() As HtmlAnchor
    '    Get
    '        Return LiFaqLink
    '    End Get
    'End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ObjSysConfig As New CSysConfig

        If LiPrintLink.HRef = "#" Then
            LiPrintLink.HRef = ObjSysConfig.GetConfig("Print_URL")
        End If

    End Sub

End Class