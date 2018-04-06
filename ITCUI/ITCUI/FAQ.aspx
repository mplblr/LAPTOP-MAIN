<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FAQ.aspx.vb" Inherits="ITCUI.FAQ" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>FAQ</title>
    <link href="site.css" rel="stylesheet" type="text/css" media="screen" />
    <link rel="stylesheet" href="fonts/fonts.css" type="text/css" charset="utf-8" />

    <script type="text/javascript" src="fancybox/jquery-1.4.2.min.js"></script>

    <script type="text/javascript" language="javascript" src="js/ddaccordion.js"></script>

    <script type="text/javascript">
        ddaccordion.init({
            headerclass: "technology", //Shared CSS class name of headers group
            contentclass: "thelanguage", //Shared CSS class name of contents group
            revealtype: "click", //Reveal content when user clicks or onmouseover the header? Valid value: "click", "clickgo", or "mouseover"
            mouseoverdelay: 200, //if revealtype="mouseover", set delay in milliseconds before header expands onMouseover
            collapseprev: false, //Collapse previous content (so only one open at any time)? true/false 
            defaultexpanded: [0], //index of content(s) open by default [index1, index2, etc]. [] denotes no content.
            onemustopen: false, //Specify whether at least one header should be open always (so never all headers closed)
            animatedefault: false, //Should contents open by default be animated into view?
            persiststate: false, //persist state of opened contents within browser session?
            toggleclass: ["closedlanguage", "openlanguage"], //Two CSS classes to be applied to the header when it's collapsed and expanded, respectively ["class1", "class2"]
            togglehtml: ["prefix", "<img src='images/plus.gif' style='width:11px; height:11px' /> ", "<img src='images/minus.gif' style='width:11px; height:11px' /> "], //Additional HTML added to the header when it's collapsed and expanded, respectively  ["position", "html1", "html2"] (see docs)
            animatespeed: "fast", //speed of animation: integer in milliseconds (ie: 200), or keywords "fast", "normal", or "slow"
            oninit: function(expandedindices) { //custom code to run when headers have initalized
                //do nothing
            },
            onopenclose: function(header, index, state, isuseractivated) { //custom code to run whenever a header is opened or closed
                //do nothing
            }
        })

    </script>

</head>
<body style="background-color: #fff; background-image: none;">
    <form id="form1" runat="server">
    <div class="faq_body">
        <table width="90%" border="0" cellpadding="4" cellspacing="0">
            <tr align="left">
                <td align="left" style="vertical-align: top; padding-left: 30px; padding-right: 20px;">
                    <span class="faq_ver11_orange"><strong>FAQ</strong>
                        <br />
                        <hr />
                    </span>
                    <table width="100%" border="0" cellpadding="4" cellspacing="0" class="ver11_contents">
                        <tbody>
                            <tr>
                                <td align="left">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td width="94%" align="left">
                                    <div align="left">
                                        <span class="style1"><strong>Dear Guest,</strong><br>
                                            <h4>
                                                Please find answers to frequently asked questions below. We hope you find these
                                                useful. We provide 24x7 on-site Internet help desk assistance at this hotel. To
                                                access the helpdesk, please call WelcomAssistance by dialing "Extn.6".
                                            </h4>
                                        </span>
                                        <br />
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr align="left">
                <td align="center" class="ver10" style="vertical-align: top;">
                    <span><a href="#" class="faq_links1" onclick="ddaccordion.expandall('technology'); return false">
                        <img src="images/up_arrow1.png" alt="" width="10" height="8" border="0" style="vertical-align:middle;" />
                        Expand all</a> </span>| <a href="#" class="faq_links1" onclick="ddaccordion.collapseall('technology'); return false">
                            <img src="images/down_arrow1.png" alt="" width="9" height="8" border="0" style="vertical-align:middle;" />
                            Collapse all</a>
                </td>
            </tr>
            <tr align="left">
                <td align="center" valign="top" style="vertical-align: top; padding-left: 30px; padding-right: 20px;">
                    <div class="technology">
                        <strong>Q1. Why Internet Access Coupons? How does the coupon system work?</strong></div>
                    <div class="thelanguage">
                        <div class="faq_inner_contents">
                            <p>
                                <span>
                                    <img src="images/link_arrow.png" alt="" width="11" height="11" />
                                    <b>A1.</b> Internet Access Coupons have been introduced to prevent unauthorized
                                    Internet usage at the hotel.
                                    <p>
                                        At check-in time you are issued one Coupon which carries an Access Code. Instructions
                                        on how to use are printed at the back of the coupon.</p>
                                    <p>
                                        The first time you open a web browser from your laptop or mobile, you are requested
                                        to submit your Room Number and the AccessCode. Next you are requested
                                        to select an Internet plan and confirm, in order to have Internet activated on your
                                        device (laptop/mobile). Charges will be posted to your room bill.
                                    </p>
                                    <p>
                                        Subsequently you may use the same Access code to connect from three more devices (laptop/mobile)
                                        if required, without any additional charges.</p>
                                </span>
                            </p>
                        </div>
                    </div>
                    <div class="technology">
                        <span><strong>Q2.When I opened my browser, I saw a page asking me to enable Javascript.
                            Why?</strong></span></div>
                    <div class="thelanguage">
                        <div class="faq_inner_contents">
                            <p>
                                <span class="style1"><span style="font-weight: bold;">
                                    <img src="images/link_arrow.png" alt="" width="11" height="11" /></span> <b>A2.
                                    </b>To process your login request, this site requires Javascript to be enabled in
                                    your browser. Please follow instructions as in the webpage to get the login screen.</span>
                            </p>
                        </div>
                    </div>
                    <div class="technology">
                        <strong>Q3. What should I do if i lose my coupon?</strong></div>
                    <div class="thelanguage">
                        <div class="faq_inner_contents">
                            <p>
                                <span style="font-weight: bold;"><span>
                                    <img src="images/link_arrow.png" alt="" width="11" height="11" /></span></span>
                                <b>A3.</b> We request you to keep the Internet Access Coupon safe to prevent unauthorized
                                usage. However if you lose the coupon, please inform our Front Desk and they will
                                assist you with retrieving the Access Code. Please provide your room numer, last
                                name and check-in date to our Front Desk Assistant.
                                <br />
                            </p>
                        </div>
                    </div>
                    <div class="technology">
                        <strong>Q4. I plan to visit the hotel once again after a few days. Can I use the same
                            coupon to connect next time?</strong></div>
                    <div class="thelanguage">
                        <div class="faq_inner_contents">
                            <p>
                                <span class="style1"><span style="font-weight: bold;">
                                    <img src="images/link_arrow.png" alt="" width="11" height="11" /></span> <b>A4.</b>
                                    The Access Coupon is valid for the period you stay at the hotel starting from check-in-time.
                                    As soon as you are checked-out, the code is de-activated. When you visit our hotel
                                    once again, you will be issued a new Access Coupon.
                                    <br>
                            </p>
                        </div>
                    </div>
                    <div class="technology">
                        <strong>Q5. My Internet plan is valid for few more hours but I have checked-out of the
                            hotel. Can I have access to Internet?</strong></div>
                    <div class="thelanguage">
                        <div class="faq_inner_contents">
                            <p>
                                <span style="font-weight: bold;"><span class="style1">
                                    <img src="images/link_arrow.png" alt="" width="11" height="11" /></span></span>
                                <b>A5.</b> No, your Internet access in hotel is de-activated immediately on check-out
                                for security reasons even if you are in the hotel premises.
                            </p>
                        </div>
                    </div>
                    <div class="technology">
                        <strong>Q6. I need to use a Public IP to connect to my corporate VPN. What should I
                            do. </strong>
                    </div>
                    <div class="thelanguage">
                        <div class="faq_inner_contents">
                            <p>
                                <span style="font-weight: bold;"><span class="style1">
                                    <img src="images/link_arrow.png" alt="" width="11" height="11" /></span></span>
                                <b>A6. </b>Please dial 6 and ask WelcomAssistance for a public IP. They will arrange
                                to setup a public IP for you.
                            </p>
                        </div>
                    </div>
                    <div class="technology">
                        <strong>Q7. I'm unable to send email from my mail client.How do I proceed? </strong>
                    </div>
                    <div class="thelanguage">
                        <div class="faq_inner_contents">
                            <p>
                                <span style="font-weight: bold;"><span class="style1">
                                    <img src="images/link_arrow.png" alt="" width="11" height="11" /></span></span>
                                <b>A7. </b>Please dial 6 and inform WelcomAssistance. They may be able to help resolve
                                the problem.</p>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>