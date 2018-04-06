<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SHMaster.Master"
    CodeBehind="itcdiscount.aspx.vb" Inherits="ITCUI.itcdiscount" %>  

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div>

</div>
    <div class="bg">
     <div class="charges_info">
          You will be charged <img src="images/rupee_symbol.png" alt="" />  <asp:Label  class="charges_info" ID="Label1" runat="server"  ></asp:Label>
          </div>
        <div id="itc_discount">
          <div id="SelectDays" runat ="server"  class="disc_section" style="position: relative;"> 
                
                <div style="text-align: left; padding-left: 10px;" id="radio_info">
                   
                    <p>
                       <asp:RadioButton ID="RadioButton1" AutoPostBack="true" Text="" runat="server" /> I would like my Internet access to be renewed <strong> automatically </strong> after every 24 hours
                    </p>
                    <p>
                       <asp:RadioButton ID="RadioButton2" AutoPostBack="true" Text="" runat="server" /> I would like <strong> myself </strong>  to renew my Internet access after every 24 hours
                    </p>
                </div>
                
            </div>
            <div id="chk1" runat="server" class="terms_section itc_dis" style=" left:0%; width:100%;">
                
                <p style="text-align: center; margin-left: 0%">
                    <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="button" />
                </p>
                
                <div style=" margin-top:2em;">
            <asp:Label ID="lblErrorMsg" runat="server" CssClass="labl_error" style="display:inline-block; width:100%; text-align:left;"></asp:Label>
            </div>
            </div>
            
        </div>
        
    </div>
     
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
</asp:Content>

