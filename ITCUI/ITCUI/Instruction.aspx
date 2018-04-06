<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/SHMaster.Master"
    CodeBehind="Instruction.aspx.vb" Title="ITC Hotels" Inherits="ITCUI.Instruction" %>

<%@ MasterType VirtualPath="~/SHMaster.Master" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div class="bg">
        <div class="title_bar_bg">
            <p class="title_bar_bg_bottom" id="bg1">
                For Resident Guests
            </p>
            <div>
                <img src="images/title_bar_bg_bottom.png" alt="" />
            </div>
        </div>
        <div class="instructions">
            <p class="note1">
                For secure Internet access, please create your own password
            </p>
            <p>
                <label >
                    Your Room Number</label>
                <asp:TextBox ID="txtRoomNo" runat="server" CssClass="textbox" Width="122px" MaxLength="4"
                    Font-Bold="true"></asp:TextBox>
            </p>
            <p>
                <label >
                    Your First/Last Name<%--<span style="color: #FF0000; font-size: 15px; font-weight: bold; font-family: Verdana, Arial, Helvetica, sans-serif;
                    margin-left: 2px;">*</span>--%>
                </label>
                <asp:TextBox ID="txtAccCode"  runat="server" CssClass="textbox" Style="width: 122px;text-transform: uppercase;" MaxLength="25" Font-Bold="true"></asp:TextBox>
            </p>
            <br />
            <div class="terms_section">
                <p style="text-align: left; margin-bottom: 10px; margin-left: 0px; height: 20px;">
                    <span id="backgroundIAgree" class="terms_new">
                        <img src="images/terms_anim1.gif" alt="" id="terms_anim1" style="vertical-align: middle;" />
                        <asp:CheckBox ID="chkIAgree" runat="server" />
                    </span><span style="display: inline-block; vertical-align: top; font-size: 14px;
                        font-family: 'source_sans_prosemibold';">&nbsp; I accept <a id="terms" href="TermsCond.aspx">
                            <strong>Terms &amp; Conditions</strong></a> of use </span>
                </p>
                <p style="text-align: left; margin-left: 3px">
                    <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="button" />
                    <img id="btnLoginFade" alt="" src="images/login.png" style="vertical-align: middle;
                        left: -4px; position: relative;" />
                    <img id="connectwait" alt="" src="images/please_wait.gif" style="vertical-align: middle" />
              <asp:Button ID="Button2" runat="server" Text="Cancel" CssClass="button" />
              
                </p>

<p>  </p>


                
                
            </div>
<p style="text-align: left; clear: both; padding-top: 10px;">
                <span></span>
                    <asp:Label ID="lblErrorMsg" CssClass="labl_error" runat="server" Text=""></asp:Label>
                </p>
            <div style="text-align: center; padding-top: 100px; display: none;">
                <asp:ImageButton ID="btnEventLogin" runat="server" ImageUrl="~/images/conference_login.png" />
            </div>
        </div>
        <div class="access_code">
             <img src="images/user_icon.png" />
            <p>
                   If you have already created your password, please
               
                <asp:Button ID="Button1"  runat="server" Text="Click here" CssClass="button" />
            </p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ScriptContent" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">

    <script src="PageScript/Instruction.js" type="text/javascript"></script>

</asp:Content>
