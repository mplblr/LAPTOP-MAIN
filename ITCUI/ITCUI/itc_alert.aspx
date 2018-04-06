<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SHMaster.Master"
    CodeBehind="itc_alert.aspx.vb" Inherits="ITCUI.itc_alert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">


<asp:HiddenField ID="hdAccept" runat="server" Value="0" />

    <div class="bg">
        <div class="alertmsg">
            <p>
                <img src="images/renew.png" alt="" />
            </p>
            <p id="msg" runat ="server"  class="lbl_guest" style=" font-size:16px;">
           
            </p>
            <asp:Button ID="yes" runat="server" Text="Yes" CssClass="button btn_big" Style="vertical-align: middle" /> &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="No" runat="server" Text="No" CssClass="button btn_big" Style="vertical-align: middle" />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
<script type="text/javascript">
 Cufon.replace('.lbl_guest', { fontFamily: 'Myriad Pro' });

</script>



<script src="js/jquery-1.4.2.min.js" type="text/javascript"></script>
        <script type="text/javascript">
            $(function() {

                $("input[id$=yes]").click(function(e) {
                    var clickonce = $.trim($("input[id$=hdAccept]").val());
                    if (clickonce === "1" || clickonce === 1) {
                        e.preventDefault();
                    }
                    else {
                        $("input[id$=hdAccept]").val("1");
                        $("input[id$=yes]").attr("class", "btn_style_disable");
                    }
                });
            });
        </script>





</asp:Content>
