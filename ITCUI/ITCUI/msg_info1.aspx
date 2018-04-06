<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/SHMaster.Master"
    CodeBehind="msg_info1.aspx.vb" Inherits="ITCUI.msg_info1" Title="ITC Hotels" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

  
    <div class="msg_info"  >
  <p  style="text-align:center">
    <img     src="images/alert.png" />
    </p>
      
           <p> 
            Dear Guest,</br> You are now successfully connected to the Internet.</br>
          <asp:Label ID="lblerr1"  runat="server" Text=""></asp:Label>
       </p>
       
       <p style="margin-bottom:10px;margin-top:10px">
       
<a id="terms" href="http://192.168.8.20/speedtest/index.html">
                             <img src="images/speed_bg.gif" alt="" /></a> 


       </p>


<div class="speedtst">
        Also available on <a href="http://speedtest.in">speedtest.in</a>
    </div>


<div> <asp:Button ID="Button2" runat="server" Text="VOD" CssClass="button" /></div>


    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
</asp:Content>
