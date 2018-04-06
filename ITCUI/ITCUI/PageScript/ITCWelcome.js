$(document).ready(function() {

    DD_roundies.addRule(".nights_bg", "0px 0px 8px 8px", true);
    DD_roundies.addRule(".wlcm_note", "8px 8px 0px 0px", true);
    DD_roundies.addRule(".wlcm_note_right", "8px 8px 0px 0px", true);
    Cufon.replace('.wlcm_note p', { fontFamily: 'Myriad Pro' });
    Cufon.replace('.txt1,.txt2,.rate', { fontFamily: 'Myriad Pro' });

    var SelectDays = $("div[id$=SelectDays]");
    var DrpDwnNoofDays = $("select[id$=DrpDwnNoofDays]");

    var PlanConfirm = $("div[id$=PlanConfirm]");

    var HtmlBtnPurchase = $("input[id$=HtmlBtnPurchase]");
    var HtmlPurchaseWait = $("img[id$=HtmlPurchaseWait]");

    var HtmlBtnContinue = $("input[id$=HtmlBtnContinue]");
    var HtmlContinueWait = $("img[id$=HtmlContinueWait]");

    var lblBillToPost = $("span[id$=lblBillToPost], label[id$=lblBillToPost]");
    var hdDiscount = $("input[id$=hdDiscount]");

    var lblErrorMsg = $("label[id$=lblErrorMsg], span[id$=lblErrorMsg]");

    var calculateBill = function() {
        var checked = $("table[id$=rdoplan] input:checked");
        if (checked.size() === 1) {
            var discount = parseFloat($.trim(hdDiscount.val()));
            var plan = $.trim(checked.val());
            var amount = parseInt($("span[id$=forPlan_" + plan + "]").attr("accesskey"), 10);
            var nights = $.trim(DrpDwnNoofDays.val());

            var totalBill = (amount * (100.00 - discount) / 100) * nights;
            if (totalBill > 0) {
                lblBillToPost.text("INR " + totalBill + "/-");
            }
            else {
                lblBillToPost.text("Complimentary");
            }
            Cufon.replace('.rate', { fontFamily: 'Myriad Pro' });
        }
    };

    var showHideContent = function() {
        var checked = $("table[id$=rdoplan] input:checked");
        var selplancount = checked.size();
        if (selplancount === 0) {
            SelectDays.hide();
            PlanConfirm.hide();
        }
        else {
            var plan = $.trim(checked.val());
            var checkclass = $.trim($("span[id$=forPlan_" + plan + "]").attr("class"));
            checkclass = checkclass.toLowerCase();
            if (checkclass === "nonight") {
                DrpDwnNoofDays.val("1");
                SelectDays.hide();
            }
            else {
                SelectDays.show();
            }

            var selectedNights = $.trim(DrpDwnNoofDays.val());
            if (selectedNights === "0") {
                PlanConfirm.hide();
            }
            else {
                PlanConfirm.show();
                calculateBill();
            }
        }
        fixFrame();
    };

    showHideContent();

    var changePlanConfirmationButton = function() {
        var checked = $("table[id$=rdoplan] input:checked");
        if (checked.size() === 1) {
            var plan = $.trim(checked.val());
            var amount = parseInt($("span[id$=forPlan_" + plan + "]").attr("accesskey"), 10);
            if (amount > 0) {
                HtmlBtnPurchase.val("Confirm Purchase");
            }
            else {
                HtmlBtnPurchase.val("Connect");
            }
        }
    };

    $("table[id$=rdoplan] input").click(function(e) {
        lblErrorMsg.text("");
        DrpDwnNoofDays.val("0");
        changePlanConfirmationButton();
        showHideContent();
    });

    DrpDwnNoofDays.change(function(e) {
        lblErrorMsg.text("");
        showHideContent();
    });

    HtmlContinueWait.hide();
    HtmlBtnContinue.click(function() {
        $(this).hide();
        HtmlContinueWait.show();
    });

    HtmlPurchaseWait.hide();
    HtmlBtnPurchase.click(function() {
        $(this).hide();
        HtmlPurchaseWait.show();
    });

    $("table[id$=rdoplan] input").css({ "vertical-align": "middle"});
    var trcount = 0
    $("table[id$=rdoplan] tr").each(function() {
        if (trcount % 2 === 0) {
            $(this).css({ "background-color": "#F5F5F5" });
        }
        else {
            $(this).css({ "background-color": "#FFF" });
        }
        trcount = trcount + 1;
    });

});