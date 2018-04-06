
    
    
    <%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/SHMaster.Master"
    CodeBehind="logged_guest1.aspx.vb" Inherits="ITCUI.logged_guest1" Title="ITC Hotels" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="Server">
   <asp:HiddenField ID="hdAccept" runat="server" Value="0" />

   
    <div class="bg">
        <div class="title_bar_bg">
            <p class="title_bar_bg_bottom" id="bg1">
                Already logged in Guests
            </p>
            <div>
                <img src="images/title_bar_bg_bottom.png" alt="" />
            </div>
        </div>
<p class="title_bar_bg_bottom" id="bg1">
              
            </p>

<p>  </p>



        <div class="instructions">

<p class="note2">If you forget your password,&nbsp; please dial 6 for WelcomAssistance 
           
        </p>


            <p>
                <label>
                    Your Room Number</label>
                <asp:TextBox ID="txtRoomNo" runat="server" CssClass="textbox" Width="122px" MaxLength="4"
                    Font-Bold="true"></asp:TextBox>
            </p>
            <p>
                <label>
                    Your Password<%--<span style="color: #FF0000; font-size: 15px; font-weight: bold; font-family: Verdana, Arial, Helvetica, sans-serif;
                    margin-left: 2px;">*</span>--%>
                </label>
                <asp:TextBox ID="txtAccessCode"  TextMode="Password"   runat="server" CssClass="textbox" Style="width: 122px;
                    text-transform: uppercase;" MaxLength="15" Font-Bold="true"></asp:TextBox>
            </p>


 

            <div class="terms_section">
                <p style="text-align: left; margin-bottom: 10px; margin-left: 0px; height: 20px;">
                    <span id="backgroundIAgree" class="terms_new">
                        <img src="images/terms_anim1.gif" alt="" id="terms_anim1" style="vertical-align: middle;" />
                        <asp:CheckBox ID="chkIAgree"       runat="server" />
                    </span><span style="display: inline-block; vertical-align: top; font-size: 14px;
                        font-family: 'source_sans_prosemibold';">&nbsp; I accept <a id="terms" href="TermsCond.aspx">
                            <strong>Terms &amp; Conditions</strong></a> of use </span>
                </p>
                <p style="text-align: left; margin-left: 0px">
            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="button" />  
<img id="btnLoginFade" alt="" src="images/login.png" style="vertical-align: middle;
                        left: -4px; position: relative;" />
                    <img id="connectwait" alt="" src="images/please_wait.gif" style="vertical-align: middle" />


  &nbsp;
                    

 <asp:Button ID="Button1" visible="false" runat="server" Text="Forgot Password" CssClass="button" />

&nbsp;
 <asp:Button ID="Button2" runat="server" Text="Cancel" CssClass="button" />
                    
                </p>

  

            </div>
            <br />

<p style="text-align: left; clear: both; padding-top: 10px;">
                <span></span>
                    <asp:Label ID="lblErr" CssClass="labl_error" runat="server" Text=""></asp:Label>
                </p>
            <div style="text-align: center; padding-top: 100px; display: none;">
                <asp:ImageButton ID="btnEventLogin" runat="server" ImageUrl="~/images/conference_login.png" />
            </div>
        </div>
       
 <div style="text-align: left; clear: both; margin-left: 10%; padding-top: 0%;">
          
        </div>
        
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="Server">

     
  <script src="PageScript/Instruction1.js" type="text/javascript"></script>
 
</asp:Content>
