<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/SHMaster.Master"
    CodeBehind="usererror.aspx.vb" Inherits="ITCUI.usererror" Title="ITC Hotels" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div class="bg">
        <div style="text-align: center; padding-top: 10%; margin-left: 3%;">
            <img src="images/error_ico.png" style="vertical-align: top; margin-bottom: 30px;"
                alt="" />
            <br />
            <div id="msgParaTitle" align="center" runat="server" class="bodytexterror1">
            </div>
            <div id="msgparaText" align="center" class="bodytexterror1" runat="server">
            </div>
        </div>
    </div>
</asp:Content>