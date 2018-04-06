


  <%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/SHMaster.Master"
    CodeBehind="password.aspx.vb" Inherits="ITCUI.password" Title="ITC Hotels" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="Server">
    <div class="bg">
        <div class="title_bar_bg">
            <p class="title_bar_bg_bottom" id="bg1">
                Already logged in Guests
            </p>
            <div>
                <img src="images/title_bar_bg_bottom.png" alt="" />
            </div>


        </div>

         <div class="instructions">

<p class="note2">
          <strong>If you forget your password,&nbsp; please dial 6 for WelcomAssistance </strong>
        </p>
            <p>
                <label>
                    Your Password<%--<span style="color: #FF0000; font-size: 15px; font-weight: bold; font-family: Verdana, Arial, Helvetica, sans-serif;
                    margin-left: 2px;">*</span>--%>
                </label>
                <asp:TextBox ID="txtAccessCode" TextMode="Password"   runat="server" CssClass="textbox" Style="width: 122px;
                    text-transform: uppercase;" MaxLength="15" Font-Bold="true"></asp:TextBox>

            </p>


<p>   </p>

<p>   </p>

  
            <div   style="text-align:left; margin-left:13.6em;">
                 
             
                     <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="button" />&nbsp;
<asp:Button ID="Button1" runat="server" Text="Cancel" CssClass="button" />
 
            </div>
            <br>

<p style="text-align: left; clear: both; padding-top: 10px;">
                <span></span>
                    <asp:Label ID="lblErr" CssClass="labl_error" runat="server" Text=""></asp:Label>
                </p>
            <div style="text-align: center; padding-top: 100px; display: none;">
                <asp:ImageButton ID="btnEventLogin" runat="server" ImageUrl="~/images/conference_login.png" />
            </div>
        </div>
        <div style="text-align: left; clear: both; margin-left: 3%; padding-top: 0%;">
            
        </div>
        <div class="dial_info">
            
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="Server">


 
 
 
</asp:Content>
