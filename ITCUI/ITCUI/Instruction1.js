$(document).ready(function() {

    DD_roundies.addRule(".access_box", "8px", true);

    var txtRoomNo = $("input[id$=txtRoomNo]");
    var txtAccCode = $("input[id$=txtAccCode]");

    var chkIAgree = $("input[id$=chkIAgree]");
    var terms_anim = $("img[id$=terms_anim1]");

    var btnLogin = $("input[id$=btnLogin]");
    var btnLoginFade = $("img[id$=btnLoginFade]");
    var connectwait = $("img[id$=connectwait]");

    var lblErrorMsg = $("span[id$=lblErrorMsg], label[id$=lblErrorMsg]");

    connectwait.hide();

    chkIAgree.attr("checked", false);

    txtRoomNo.bind("keyup", function(e) {
        if (this.value.match(/[^0-9]/g)) {
            this.value = this.value.replace(/[^0-9]/g, '');
        }
    });

    txtRoomNo.bind("cut copy paste", function(e) {
        e.preventDefault();
    });

    txtAccCode.bind("cut copy paste", function(e) {
        e.preventDefault();
    });

   

    var disable = function() {
        terms_anim.show();
        $("span#backgroundIAgree").removeClass("terms_new_enable");
        $("span#backgroundIAgree").addClass("terms_new");
        btnLogin.hide();
        btnLoginFade.show();
    };

    var enable = function() {
        terms_anim.hide();
        $("span#backgroundIAgree").removeClass("terms_new");
        $("span#backgroundIAgree").addClass("terms_new_enable");
        btnLogin.show();
        btnLoginFade.hide();
    };

    var decideButtonDisable = function() {
        var checked = chkIAgree.attr("checked");
        if (checked === false) {
            disable();
        }
        else {
            enable();
        }
    };

    decideButtonDisable();

    chkIAgree.click(function(e) {
        decideButtonDisable();
    });

    var validateInput = function() {
        var valid = true;
        lblErrorMsg.html("");
        var roomno = $.trim(txtRoomNo.val());
        var accessCode = $.trim(txtAccCode.val());

        var roomRegex = new RegExp(/^\d+$/);
        var accessRegex = new RegExp(/^[a-zA-Z0-9]{3}\-{0,1}[a-zA-Z0-9]{3}$/);

        if (roomno === "") {
            lblErrorMsg.html("Dear Guest, please enter your valid Suite Number");
            valid = false;
        }
        else if (roomRegex.test(roomno) === false) {
            lblErrorMsg.html("Dear Guest, please enter your valid Suite Number");
            valid = false;
        }
        else if (accessCode === "") {
            lblErrorMsg.html("Dear Guest, please enter your valid  password");
            valid = false;
        }
      

        return valid;
    };

    btnLogin.click(function(e) {
        if (validateInput() === false) {
            e.preventDefault();
        }
        else {
            $(this).hide();
            connectwait.show();
        }
    });

});