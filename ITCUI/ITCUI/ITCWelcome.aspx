<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/SHMaster.Master"
    CodeBehind="ITCWelcome.aspx.vb" Inherits="ITCUI.ITCWelcome" Title="ITC Hotels" %>

<%@ MasterType VirtualPath="~/SHMaster.Master" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:HiddenField ID="hdDiscount" runat="server" Value="0" />
    <div id="PlanSelectionDiv" runat="server" class="bg">
        <div class="wlcm_note_wrapper" id="welcomeNoteWrapper" runat="server">
            <div class="wlcm_note" style="display: none;">
                <p>
                    <asp:Label ID="lblWelcomeAddress" runat="server" Text=""></asp:Label>
                </p>
                <p>
                    <asp:Label ID="lblWelcomeGuestName" runat="server" Text=""></asp:Label>
                </p>
            </div>
            <div class="wlcm_note_right" id="welcomeNote" runat="server">
                <div>
                    <asp:Label ID="lblFirstDiscountInfo" runat="server" Text="" class="txt1"></asp:Label>
                </div>
                <div>
                    <asp:Label ID="lblSecondDiscountInfo" runat="server" class="txt2" Text=""></asp:Label>
                </div>
            </div>
        </div>
        <div class="upgrade_note">
            <strong>Dear Guest,</strong>
            <br />
            <p>
                For upgrade of Internet plans, please visit <span style="color: #0136cf; font-weight: bold;">
                    http://itcinternet.in</span>.</p>



        </div>
        <div class="title_bar_bg" style="width: 35%;">
            <p class="title_bar_bg_bottom" id="bg1">
                <span>Please select an Internet plan</span>
                <img src="images/arrow2.png" alt="" /></p>
            <div>
                <img src="images/title_bar_bg_bottom.png" alt="" />
            </div>
        </div>
        <div id="pid" runat="server"  class="plan_selection">
            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                <tr>
                    <td align="left" style="font-weight: bold; font-size: 15px; line-height: 25px">
                        <div id="divstart" runat="server">
                        </div>
                        <asp:RadioButtonList ID="rdoplan" runat="server" CellPadding="0" CellSpacing="0"
                            RepeatColumns="1" Width="100%">
                        </asp:RadioButtonList>
                        <div id="divend" runat="server">
                        </div>
                    </td>
                </tr>
            </table>
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td height="25" style="background-color: #F4F0BB">
                        <p style="font-size: 12px; padding-left: 10px;">
                            * Taxes extra as applicable.
                        </p>
                    </td>
                </tr>
            </table>
            <div id="SelectDays" class="nights_bg" style="position: relative;">
                <span></span>
                <div style="text-align: left; padding-left: 10px;" id="radio_info">
                   
                    <p>
                       <asp:RadioButton ID="RadioButton1" AutoPostBack="true" Text="" runat="server" /> I would like my Internet access to be renewed <strong> automatically </strong> after every 24 hours.
                    </p>
                    <p>
                       <asp:RadioButton ID="RadioButton2" AutoPostBack="true" Text="" runat="server" /> I would like <strong> myself </strong>  to renew my Internet access after every 24 hours.
                    </p>
                </div>
            </div>
        </div>
        <div style="width: 80%; margin: 0 auto; margin-top: 15px; margin-bottom: 15px; padding-top: 5px;">
            <asp:Label ID="lblErrorMsg" runat="server" CssClass="labl_error"></asp:Label>
        </div>
        <div id="PlanConfirm" runat="server" class="purchase_bg">
            <div style="text-align: center">
                <img alt="" src="images/divider2.png" style="vertical-align: middle" />
            </div>
            <div class=" btn_section">
                <div class="rate">
                    <asp:Label ID="lblAmountTitle" Text="Total Amount:" runat="server"></asp:Label>
                    <asp:Label ID="lblBillToPost" runat="server" Text=""></asp:Label>
                </div>
                <p style="text-align: center; vertical-align: top;">
                    <asp:Button ID="HtmlBtnPurchase" runat="server" Text="Confirm Purchase" CssClass="button"
                        Style="vertical-align: middle" />
                    <img id="HtmlPurchaseWait" alt="" src="images/please_wait.gif" style="vertical-align: middle" />
                </p>
            </div>
            <div style="text-align: center">
                <img alt="" src="images/divider2.png" style="vertical-align: middle" />
            </div>
        </div>
    </div>
    <div id="ContinueDiv" runat="server" class="" style="margin-top: 11%; padding: 10px 0px;">
        <p style="padding-bottom: 10px; text-align: center; font: 13px verdana;">
            <strong>Welcome back!</strong>
            <br />
            To Login please click on Continue.
        </p>
        <div style="text-align: center;">
            <asp:Button ID="HtmlBtnContinue" runat="server" Text="Continue" CssClass="button"
                Style="vertical-align: middle" />
            <img id="HtmlContinueWait" alt="" src="images/please_wait.gif" style="vertical-align: middle" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="ScriptContent" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">

    <script src="PageScript/ITCWelcome.js" type="text/javascript"></script>

</asp:Content>
