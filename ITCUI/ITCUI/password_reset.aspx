<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/SHMaster.Master"
    CodeBehind="password_reset.aspx.vb" Title="ITC Hotels" Inherits="ITCUI.password_reset" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        #dialog
        {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="Server">
    <asp:HiddenField ID="hdAccept" runat="server" Value="0" />
    <asp:HiddenField ID="ppp" runat="server" Value="0" />
    <div class="bg">
        <div class="title_bar_bg">
            <p class="title_bar_bg_bottom" id="bg1">
                Create your own password
            </p>
            <div>
                <img src="images/title_bar_bg_bottom.png" alt="" />
            </div>
        </div>
        <div class="instructions logged_gst pwd_section pwd_reset">
           
            <div class="pwd_wrapper">

 <p class="note2">
                Your password must have minimum 6 letters and / or numbers
            </p>
            <p class="note2">
                
            </p>
                <p class="pwd_info">
                    <label class="pwd_fields">
                        Password<%--<span style="color: #FF0000; font-size: 15px; font-weight: bold; font-family: Verdana, Arial, Helvetica, sans-serif;
                    margin-left: 2px;">*</span>--%>
                    </label>
                    <asp:TextBox ID="txtAccCode" textmode="password" runat="server" CssClass="textbox" Style="width: 150px;"
                        MaxLength="15" Font-Bold="true"></asp:TextBox> <br />
                    <label>
                        Confirm Password<%--<span style="color: #FF0000; font-size: 15px; font-weight: bold; font-family: Verdana, Arial, Helvetica, sans-serif;
                    margin-left: 2px;">*</span>--%>
                    </label>
                    <asp:TextBox ID="TextBox1" textmode="password" runat="server" CssClass="textbox" Style="width: 150px;"
                        MaxLength="15" Font-Bold="true"></asp:TextBox>
                </p>
               

<p>     

<h2>
                    Please remember your password for further use
                </h2>
</p>

<p>  </p>
                   <p>
                    <asp:Button ID="btnLogin" runat="server" Text="Continue" CssClass="button btn_popup" />
                </p>
                
                  <p>
                  <asp:Label ID="lblerr1" CssClass="labl_error" runat="server" Text=""></asp:Label>
                
                
                </p>
<p>  </p>

<p>  </p>
<br/>

<p class="note2"> If you forget your password,&nbsp; please dial 6 for WelcomAssistance  </p>
                 <div class="device_info">
                
                <h2>
                    You may connect upto four devices
                </h2>
            </div>
            </div>
          
            
            <div class="device_info"></div>
           
            <br />
            <div style="text-align: center; padding-top: 100px; display: none;">
                <asp:ImageButton ID="btnEventLogin" runat="server" ImageUrl="~/images/conference_login.png" />
            </div>
        </div>
       
        <div id="dialog" title="Basic dialog">
            <div style="text-align: center;">
                <img src="images/popup_icon.png" alt="" />
            </div>
            <h1 style="color: #000;">
                Please remember your password for further use.
            </h1>
        </div>
    </div>
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="Server">
    <%--<script src="js/Layout.js" type="text/javascript"></script>--%>
    <link rel="stylesheet" href="jquery-ui.css" media="all" />
    <link href="js/css/ui-lightness/jquery-ui-1.8.1.custom.css" rel="stylesheet" type="text/css" />

    <script src="js/jquery-1.4.2.min.js" type="text/javascript"></script>

    <script type="text/javascript">
            $(function() {

                $("input[id$=btnLogin]").click(function(e) {
                    var clickonce = $.trim($("input[id$=hdAccept]").val());
                    if (clickonce === "1" || clickonce === 1) {
                        e.preventDefault();
                    }
                    else {
                        $("input[id$=btnLogin]").val("Please Wait");
                        $("input[id$=btnLogin]").attr("class", "btn_style_disable");
                    }
                });
            });
    </script>

    <script src="js/jquery-ui.min.js" type="text/javascript"></script>

    <script src="js/jquery-ui-1.8.1.custom.min.js" type="text/javascript"></script>

    <script type="text/javascript">


$(function(){
$("input[id$=btnLogin]").click(function(e) {
    
  
    
    $('#DropDownList1').trigger('focus');
    
     $('#DropDownList1').css("background" , "#ccc")
    
    


    
    });

});





    </script>

</asp:Content>
