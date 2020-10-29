﻿// Js file use for desktop push notification for SEO use
//Added by Tejaswee suggested by Bhavna and Tarun



if (typeof (sourceName) == "undefined") {
    var sourceName = "subscribeNow";
}

infVObj = {};

function testMob() {
    var check = false;
    (function (a) { if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true })(navigator.userAgent || navigator.vendor || window.opera);
    return check;
}

scrollFlag = 0;

addIframe();


window.addEventListener("message", receiveMessage, false);

function addIframe() {
    if (typeof (web) != "undefined") {
        if (testMob()) {
            var perWin = "https://" + web + ".informvisitors.com/permissionMob.html";
        }
        else {
            var perWin = "https://" + web + ".informvisitors.com/permission.html";
        }
        var ifrm = document.createElement("iframe");
        ifrm.setAttribute("src", perWin);
        ifrm.style.width = "0px";
        ifrm.style.height = "0px";
        if (document.body) {
            document.body.appendChild(ifrm);
            var fileref = document.createElement("link");
            fileref.setAttribute("rel", "stylesheet");
            fileref.setAttribute("type", "text/css");
            fileref.setAttribute("href", "https://www.informvisitors.com/resources/popupFinal.css");
            document.getElementsByTagName("head")[0].appendChild(fileref);
            checkTags();
        }
        else {
            setTimeout(function () { addIframe(); }, 100);
        }
    }
    else {
        setTimeout(function () { addIframe(); }, 100);
    }
}

function checkTags() {
    var fileref = document.createElement("script");
    fileref.setAttribute("type", "text/javascript");
    fileref.setAttribute("src", "https://" + web + ".informvisitors.com/rulesJson.js");
    document.getElementsByTagName("head")[0].appendChild(fileref);
    startTagging();
}

function startTagging() {
    if (typeof (dataTagg) == "undefined") {
        setTimeout(function () { startTagging(); }, 2000);
    }
    else {
        if (dataTagg.urlRules.length == 0 && dataTagg.keyRules.length == 0 && dataTagg.pathRules.length == 0 && dataTagg.triggerRules.length == 0) {
            // console.log("No Rules available");
        }
        else {
            var fileref = document.createElement("script");
            fileref.setAttribute("type", "text/javascript");
            fileref.setAttribute("src", "https://" + web + ".informvisitors.com/tagging.js");
            document.getElementsByTagName("head")[0].appendChild(fileref);
        }
    }
}

function showBell() {
    var ua = navigator.userAgent;
    var status = 0;
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini|Mobile|mobile|CriOS/i.test(ua)) {
        status = 1;
    }
    if (/Chrome/i.test(ua)) {
        if (status == 0) {
            tryBlackOut();
        }
        status++;
    }
    if (status == 2) {
        // addBell();
        //showOld();

        // scrollLock();
    }

}



function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + "; " + expires + "; path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) != -1) return c.substring(name.length, c.length);
    }
    return "";
}

function child_open() {

    popupWindow = window.open('https://' + web + '.informvisitors.com/?notYet=1&clid=' + sourceName, "_blank", "directories=no, status=no, menubar=no, scrollbars=yes, resizable=no,width=600, height=600,top=200,left=200");


}

function child_close() {
    popupWindow.close();
}

function bellReady() {
    setTimeout(function () {
        requestAnimationFrame(function () {
            ivBell.classList.add('ivNotif-animated', 'ivNotif-bounceInDownBell', 'iv-bell--animate');
        })
        setTimeout(function () {
            requestAnimationFrame(function () {
                ivBellOuterCirc.classList.add('iv-widg-bell__outerCirc--dash');
            })
            setTimeout(function () {
                requestAnimationFrame(function () {
                    ivBell.classList.remove('iv-bell--animate');
                })
            }, 1200);

        }, 800);
    }, 5000);
}

function addBell() {
    var node = document.createElement("div");
    node.setAttribute('id', 'addHere_infV2');
    document.body.appendChild(node);
    document.getElementById('addHere_infV2').innerHTML = '<div class="iv-bell"><div class="iv-bell__text">Subscribe with just one tap!</div><svg xmlns="http://www.w3.org/2000/svg" class="iv-widg-bell" viewBox="0 0 68 68"><circle cx="34" cy="34" r="32" class="iv-widg-bell__outerCirc" fill="#ED1C24" stroke="#FBB03B" stroke-width="4" stroke-miterlimit="10"/><path fill="#FBB03B" d="M38 48.3c0 1.9-1.8 3.4-4 3.4s-4-1.5-4-3.4h8z"/><path fill="#FFCD2D" d="M48.3 41.8h-2V30.2c0-6.3-4.2-11.6-10.2-12.7 0-.5-.1-1.1-.6-1.5-.9-.9-2.4-.9-3.3 0-.4.4-.5.9-.5 1.4-6 1.1-10.4 6.4-10.4 12.7v11.7h-1.7c-1.5 0-3.3 1.1-3.3 2.6v2c0 .3.8.4 1.1.4h33c.3 0 .8-.1.8-.4v-2c.2-1.5-1.4-2.6-2.9-2.6z"/><path fill="#FBB03B" d="M34.4 16.8v3H34v27h16.6c.3 0 .8-.1.8-.4v-2c0-1.5-1.5-2.6-3-2.6h-2V30.2c0-6.3-4.2-11.6-10.2-12.7-.1-.5 0-1.1-.4-1.5-.5-.5-1.4-.7-1.4-.7v1.5" opacity=".5"/><circle cx="46" cy="15.5" r="8.2" fill="#C1272D" stroke="#FFF" stroke-width="3"  class="iv-bell__plus_wrap" stroke-miterlimit="10"/><g class="iv-bell__plus"><path fill="none" stroke="#FFF" stroke-width="2" stroke-miterlimit="10" d="M46 11.5v8M50 15.5h-8"/></g></svg></div>';
    ivBell = document.querySelector('.iv-bell'),
    ivBellOuterCirc = document.querySelector('.iv-widg-bell__outerCirc');
    bellReady();
    document.getElementsByClassName('iv-bell')[0].onclick = (function () {
        popupWindow = window.open('https://' + web + '.informvisitors.com/?notYet=1&clid=' + sourceName, "_blank", "directories=no, status=no, menubar=no, scrollbars=yes, resizable=no,width=600, height=600,top=200,left=200");
        document.body.scrollTop = document.documentElement.scrollTop = 0;
    });
}

function tryBlackOut() {
    if (web != "mrvoonik" && web != "voonik" && web != "yellowfashion") {
        var node = document.createElement("div");
        node.setAttribute('class', 'black-overlay-infV');
        document.body.appendChild(node);
    }

}


function removeBlackOut() {
    if (document.getElementsByClassName('black-overlay-infV').length > 0) {
        for (var l = 0; l < document.getElementsByClassName('black-overlay-infV').length; l++) {
            document.getElementsByClassName('black-overlay-infV')[l].style.display = "none";
        }
    }
}

function checkFinish() {
    // console.log("called");
    if (getCookie('finishShow') == 1 && !(testMob())) {
        addPopup_infV();
    }
}



function startChat() {
    var ifrm = document.createElement("iframe");
    ifrm.setAttribute("src", "https://" + web + ".informvisitors.com/swComm.html");
    ifrm.style.width = "0px";
    ifrm.style.height = "0px";
    document.body.appendChild(ifrm);
}

function receiveMessage(event) {
    // console.log("Data is " + event.data);
    if (event.origin == "https://" + web + ".informvisitors.com" && event.data == "someData") {
        addPopup_infV();
    }
    if (event.origin == "https://" + web + ".informvisitors.com" && event.data == "notSubs") {
        setCookie('finishShow', 1, 1000);
        if (testMob()) {
            addPopup_infV_Start();
        }
        showBell();
        // console.log("Not subscribed");
    }
    if (event.origin == "https://" + web + ".informvisitors.com" && event.data == "notAvail") {
        // console.log("Already blocked");
    }
    if (event.origin == "https://" + web + ".informvisitors.com" && event.data == "available") {
        // console.log("Already subscribed");
        // scrollOpen();
        checkFinish();
        startChat();
    }
    if (event.origin == "https://" + web + ".informvisitors.com" && event.data == "removeBlackOut") {
        removeBlackOut();
        // scrollOpen();
        // console.log("Action taken");
    }
    if (event.origin == "https://" + web + ".informvisitors.com" && event.data.split("valuesPass").length > 1) {
        var dataValues = event.data.split("valuesPass~");
        dataValues = dataValues[1];
        dataValues = dataValues.split("**");
        infVObj[dataValues[0]] = dataValues[1];
    }
    return;
}



var popupWindow = null;

function child_open() {

    popupWindow = window.open('https://' + web + '.informvisitors.com/?clid=' + sourceName, "_blank", "directories=no, status=no, menubar=no, scrollbars=yes, resizable=no,width=600, height=600,top=200,left=200");
    setTimeout(function () { child_close(); }, 5000);

}

function child_close() {
    popupWindow.close();
}

var splStyle = "";


function addPopup_infV() {
    var node = document.createElement("div");
    node.setAttribute('id', 'addHere_infV');
    document.body.appendChild(node);
    document.getElementById('addHere_infV').innerHTML = '<div id="ivNotifDone" class="ivNotif-hidden informvisitors-notif__done ivNotif-animated ivNotif-bounceInDownCenter ivNotif-notif--hasSticky"><div class="ivNotif-wrapper"><div class="ivNotif_status-img__wrap iv-margBot-2"><svg class="iv_status-img--placeholder" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 65 65"><circle cx="32.5" cy="32.5" r="32" fill="none" stroke="#ddd" stroke-miterlimit="10"/><path fill="#ddd" d="M50.8 24l-6.5-6.5-17 17-6.6-6.6-6.5 6.5 13.1 13.1"/></svg><svg class="ivNotif-flipInY iv_status-img" id="ivStatusImg" viewBox="0 0 116.7 116.7"><circle cx="58.3" cy="58.3" r="58.3" fill="#009245"/><path fill="#FFF" d="M91.7 42.8L79.9 31 48.8 62l-12-12-11.9 11.9 23.9 23.8h.1"/><path fill="#1A1A1A" d="M86.8 37.9L43.9 80.8l32.9 32.9c20.5-6.8 36-24.7 39.3-46.6L86.8 37.9z" opacity=".24"/></svg></div><div class="ivNotif-text__wrap"><div class="ivNotif-tw-text__huge iv-margBot-2">Awesome!</div><div class="ivNotif-tw-text__main iv-margBot-1">Thank You for Subscribing to Push Notifications</div><div class="ivNotif-tw-text__para iv-margBot-2">' + titlePop + '</div></div><div class="ivNotif-powered iv-margBot-2">Powered By<a class="ivNotif-powered__link" href="https://informvisitors.com?utm_source=' + web + '&utm_medium=thankYouPop" target="_blank"><img height="20" src="https://informvisitors.com/images/logo-med.png" class="ivNotif-powered__img" alt="Buyhatke"></a></div><div class="ivNotif-btn__wrap ivNotif-btn__wrap--sticky"><a href="#" class="ivNotif-btn--sticky ivNotif-btn--close">FINISH</a></div></div></div><div id="ivNotifBg" class="ivNotif-bg ivNotif-hide"></div>';
    setTimeout(function () { document.getElementById('ivNotifDone').classList.remove('ivNotif-hidden'); document.getElementById('ivStatusImg').classList.add('ivNotif-animated'); document.getElementById('ivNotifBg').classList.remove('ivNotif-hide') }, 2);

    document.getElementsByClassName('ivNotif-btn--close')[0].onclick = (function () {
        setCookie('finishShow', 2, 1000);
        child_open();
        setTimeout(function () { document.getElementById('ivNotifDone').classList.add('ivNotif-hidden') }, 100);
        document.getElementById('ivNotifBg').style.display = 'none';
    });
    setTimeout(function () { document.getElementById('ivNotifDone').classList.remove('ivNotif-hidden') }, 100);
}


function addPopup_infV_Start() {
    if (getCookie('notToShow') == 1) {
        return;
    }
    var node = document.createElement("div");
    node.setAttribute('id', 'addHere_infV_Start');
    document.body.appendChild(node);
    var currentURL = "https://" + web + ".informvisitors.com/?notYet=1" + "&clid=" + sourceName;
    // document.getElementById('addHere_infV_Start').innerHTML = '<div id="ivNotifDone" class="ivNotif-hidden informvisitors-notif__done ivNotif-animated ivNotif-bounceInDownCenter ivNotif-notif--hasSticky"><div class="ivNotif-wrapper"><div class="ivNotif_status-img__wrap iv-margBot-2"><svg class="iv_status-img--placeholder" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 65 65"><circle cx="32.5" cy="32.5" r="32" fill="none" stroke="#ddd" stroke-miterlimit="10"/><path fill="#ddd" d="M50.8 24l-6.5-6.5-17 17-6.6-6.6-6.5 6.5 13.1 13.1"/></svg><svg class="ivNotif-flipInY iv_status-img" id="ivStatusImg" viewBox="0 0 116.7 116.7"><circle cx="58.3" cy="58.3" r="58.3" fill="#009245"/><path fill="#FFF" d="M91.7 42.8L79.9 31 48.8 62l-12-12-11.9 11.9 23.9 23.8h.1"/><path fill="#1A1A1A" d="M86.8 37.9L43.9 80.8l32.9 32.9c20.5-6.8 36-24.7 39.3-46.6L86.8 37.9z" opacity=".24"/></svg></div><div class="ivNotif-text__wrap"><div class="ivNotif-tw-text__huge iv-margBot-2">Awesome!</div><div class="ivNotif-tw-text__main iv-margBot-1">Just click subscribe to get latest updates</div><div class="ivNotif-tw-text__para iv-margBot-2">' + titlePop + '</div></div><div class="ivNotif-powered iv-margBot-2">Powered By<a class="ivNotif-powered__link" href="https://informvisitors.com?utm_source=' + web + '&utm_medium=thankYouPop" target="_blank"><img height="20" src="https://informvisitors.com/images/logo-med.png" class="ivNotif-powered__img" alt="Buyhatke"></a></div><div class="ivNotif-btn__wrap ivNotif-btn__wrap--sticky"><a target="_blank" href="' + currentURL + '" class="ivNotif-btn--sticky ivNotif-btn--close ivNotif-btn--subs">SUBSCRIBE</a></div></div></div><div id="ivNotifBg" class="ivNotif-bg ivNotif-hide"></div>';
    document.getElementById('addHere_infV_Start').innerHTML = '<div id="ivNotifDone" class="ivNotif-hidden informvisitors-notif__done ivNotif-animated ivNotif-bounceInDownCenter ivNotif-notif--hasSticky"><div class="ivNotif__close">&times;</div><div class="ivNotif-wrapper"><div class="ivNotif_status-img__wrap iv-margBot-2"><svg class="ivNotif-swing iv_status-img" id="ivStatusImg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 64 64"><circle cx="32" cy="32" r="32" fill="#ED1C24"/><path fill="#FBB03B" d="M36 46.3c0 1.9-1.8 3.4-4 3.4s-4-1.5-4-3.4h8z"/><path fill="#FFCD2D" d="M46.4 39.8h-1.9V28.2c0-6.3-4.3-11.6-10.3-12.7 0-.5-.2-1.1-.6-1.5-.9-.9-2.4-.9-3.4 0-.4.4-.4.9-.5 1.4-6 1.1-10.3 6.4-10.3 12.7v11.7h-1.8c-1.5 0-3.2 1.1-3.2 2.6v2c0 .3.7.4 1 .4h33c.3 0 .9-.1.9-.4v-2c.2-1.5-1.4-2.6-2.9-2.6z"/><path fill="#FBB03B" d="M32.5 14.8v3H32v27h16.6c.3 0 .9-.1.9-.4v-2c0-1.5-1.6-2.6-3.1-2.6h-1.9V28.2c0-6.3-4.3-11.6-10.3-12.7-.1-.5.1-1.1-.3-1.5-.5-.5-1.4-.7-1.4-.7v1.5" opacity=".5"/></svg></div><div class="ivNotif-text__wrap"><div class="ivNotif-tw-text__huge iv-margBot-2">Awesome!</div><div class="ivNotif-tw-text__main iv-margBot-1">Just click subscribe to get latest updates</div><div class="ivNotif-tw-text__para iv-margBot-2">' + titlePop + '</div></div><div class="ivNotif-powered iv-margBot-2">Powered By<a class="ivNotif-powered__link" href="https://informvisitors.com?utm_source=' + web + '&utm_medium=thankYouPop" target="_blank"><img height="20" src="https://informvisitors.com/images/logo-med.png" class="ivNotif-powered__img" alt="Buyhatke"></a></div><div class="ivNotif-btn__wrap ivNotif-btn__wrap--sticky"><a target="_blank" href="' + currentURL + '" class="ivNotif-btn--sticky ivNotif-btn--subs">SUBSCRIBE</a></div></div></div><div id="ivNotifBg" class="ivNotif-bg ivNotif-hide"></div>';
    sendTrigger(0);

    setTimeout(function () { document.getElementById('ivNotifDone').classList.remove('ivNotif-hidden'); document.getElementById('ivStatusImg').classList.add('ivNotif-animated'); document.getElementById('ivNotifBg').classList.remove('ivNotif-hide') }, 2);
    document.getElementsByClassName('ivNotif-btn--subs')[0].onclick = (function () {
        setCookie('finishShow', 2, 1000);
        setTimeout(function () { document.getElementById('ivNotifDone').classList.add('ivNotif-hidden') }, 100);
        document.getElementById('ivNotifBg').style.display = 'none';
        sendTrigger(1);
    });
    //   document.getElementsByClassName('ivNotif-btn--close')[0].onclick = (function(){
    //      setCookie('notToShow', 1, 3);
    //      setTimeout(function(){document.getElementById('ivNotifDone').classList.add('ivNotif-hidden')},100);
    //      document.getElementById('ivNotifBg').style.display = 'none';
    //      sendTrigger(2);
    // });
    document.getElementsByClassName('ivNotif__close')[0].onclick = (function () {
        setCookie('notToShow', 1, 3);
        setTimeout(function () { document.getElementById('ivNotifDone').classList.add('ivNotif-hidden') }, 100);
        document.getElementById('ivNotifBg').style.display = 'none';
        sendTrigger(2);
    });
    setTimeout(function () { document.getElementById('ivNotifDone').classList.remove('ivNotif-hidden') }, 100);
}

var tokenAvail = 0;

function checkValues() {
    if (tokenAvail == 0) {
        setTimeout(function () { checkValues(); }, 3000);
    }
    if (!(infVObj.dev_id == undefined) && tokenAvail == 0) {
        if (infVObj.dev_id == "Error in getting token") {
            tokenAvail = 1;
            // addPopup_infV();
        }
        else {
            tokenAvail = 1;
        }
    }
}

// checkValues();

function getXMLHTTPRequestNew() {
    try {
        req = new XMLHttpRequest();
    } catch (err1) {
        try {
            req = new ActiveXObject("Msxml2.XMLHTTP");
        } catch (err2) {
            try {
                req = new ActiveXObject("Microsoft.XMLHTTP");
            } catch (err3) {
                req = false;
            }
        }
    }
    return req;
}


function sendTrigger(type) {
    var http3 = getXMLHTTPRequestNew();
    var myurl = "https://www.informvisitors.com/resources/triggerActions.php";
    var myurl1 = myurl;
    myRand = parseInt(Math.random() * 999999999999999);
    var modurl = myurl1 + "?rand=" + myRand + "&web=" + web + "&type=" + type;

    http3.open("GET", modurl, true);
    http3.onreadystatechange = function () {
        if (http3.readyState == 4) {
            if (http3.status == 200) {
                var mytext = http3.responseText;
            }
        }
    };
    http3.send(null);
}