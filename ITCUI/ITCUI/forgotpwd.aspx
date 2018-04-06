
    
    
    <%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/SHMaster.Master"
    CodeBehind="forgotpwd.aspx.vb" Inherits="ITCUI.forgotpwd" Title="ITC Hotels" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="Server">
   <asp:HiddenField ID="hdAccept" runat="server" Value="0" />

   
    <div class="bg">
        <div class="title_bar_bg">
            <p class="title_bar_bg_bottom" id="bg1">
               Reset Login Password
            </p>
            <div>
                <img src="images/title_bar_bg_bottom.png" alt="" />
            </div>
        </div>
<p class="title_bar_bg_bottom" id="bg1">
              
            </p>

<p>  </p>



        <div class="instructions logged_gst">

<p>
          <strong id="kk" runat="server"> Please answer your Security Question.</strong>
          
          <strong id="kk1" runat="server"> Please change your login password.</strong>
        </p>


            <p id="a1" runat ="server">
                <label  >
                    Your Room Number</label>
                <asp:TextBox ID="txtRoomNo" runat="server" CssClass="textbox" Width="122px" MaxLength="4"
                    Font-Bold="true"></asp:TextBox>
            </p>
            <p id="a2" runat ="server">
                <label>
                      Your First/Last Name
                </label>
                <asp:TextBox ID="txtAccessCode"    runat="server" CssClass="textbox" Style="width: 122px;
                    text-transform: uppercase;" MaxLength="10" Font-Bold="true"></asp:TextBox>
            </p>


  <p id="P1" runat ="server">
               
               
              <asp:Label ID="p2" runat ="server" >  </asp:Label>
               
                <asp:TextBox ID="TextBox1" runat="server" CssClass="textbox" Width="122px" MaxLength="30"
                    Font-Bold="true"></asp:TextBox>
            </p>
           
  <p id="P3" runat ="server">
                <label  >
                    Password</label>
                <asp:TextBox ID="TextBox2" textmode="password" runat="server" CssClass="textbox" Width="122px" MaxLength="10"
                    Font-Bold="true"></asp:TextBox>
            </p>
            <p id="P4" runat ="server">
                <label>
                       Confirm Password<%--<span style="color: #FF0000; font-size: 15px; font-weight: bold; font-family: Verdana, Arial, Helvetica, sans-serif;
                    margin-left: 2px;">*</span>--%>
                </label>
                <asp:TextBox ID="TextBox3"  textmode="password"  runat="server" CssClass="textbox" Style="width: 122px;"
                    MaxLength="10" Font-Bold="true"></asp:TextBox>
            </p>

 

            <div class="terms_section">
               
                <p style="text-align: left; margin-left: 0px">
            

                    

 <asp:Button ID="Button1" runat="server" Text="Continue" CssClass="button" />

 <asp:Button ID="Button2" runat="server" Text="Continue" CssClass="button" />

 <asp:Button ID="Button3" runat="server" Text="Continue" CssClass="button" />
 
 <asp:Button ID="Button4" runat="server" Text="Cancel" CssClass="button" />


                    
                </p>
            </div>
            <br />
            <div style="text-align: center; padding-top: 100px; display: none;">
                <asp:ImageButton ID="btnEventLogin" runat="server" ImageUrl="~/images/conference_login.png" />
            </div>
        </div>
        <div style="text-align: left; clear: both; margin-left: 10%; padding-top: 0%;">
            <asp:Label ID="lblErrorMsg" CssClass="labl_error" runat="server" Text=""></asp:Label>
            
              <h2 id="kk2" runat="server">
                    If you have forgotten your Security Answer, please dial 6 for WelcomAssistance
                </h2>
                
                 <h2 id="kk3" runat="server">
                    Please remember your password for further use.
                </h2>
                
                
        </div>
        
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="Server">

     
  <script src="PageScript/Instruction1.js" type="text/javascript"></script>
 
</asp:Content>
