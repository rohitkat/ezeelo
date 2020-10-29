var app = new Vue({
    el: '#app',
    data: {
        scanner: null,
        activeCameraId: null,
        cameras: [],
        scans: []
    },
    mounted: function () {
        var self = this;
        self.scanner = new Instascan.Scanner({ video: document.getElementById('preview'), scanPeriod: 5 });
        self.scanner.addListener('scan', function (content, image) {           
            var userloginId = getUserLoginId();
            $("#ScannerDiv").hide();
            $("#LoaderDiv").show();             
            const Url = "https://api.ezeelo.com/api/ReadMerchantQRCode?Code=" + content + "&UserLoginId=" + userloginId;
            $.ajax({
                type: "GET",
                url: Url,
                success: function (data) {
                    $("#LoaderDiv").hide();
                    if (data.Success == "1") {
                        $("#MerchantFoundDiv").show();
                        $("#MerchantName").text(data.data.Name);
                        $("#CityName").text(data.data.City);
                        $("#MerchantName_").text(data.data.Name);
                        $("#CityName_").text(data.data.City);
                        $("#MerchantId").val(data.data.Id);
                        $("#UserLoginId").val(userloginId);
                    } else {
                        $("#MerchantNotFoundDiv").show();
                    }
                },
                failure: function (data) {
                    alert("fail");
                },
                error: function (data) {
                    alert("error");
                }
            });

            self.scans.unshift({ date: +(Date.now()), content: content });
        });
        Instascan.Camera.getCameras().then(function (cameras) {
            self.cameras = cameras;
            if (cameras.length > 0) {
                self.activeCameraId = cameras[1].id;
                self.scanner.start(cameras[1]);                
            } else {
                console.error('No cameras found.');
            }
        }).catch(function (e) {
            console.error(e);
        });
    },
    methods: {
        formatName: function (name) {
            return name || '(unknown)';
        },
        selectCamera: function (camera) {
            this.activeCameraId = camera.id;
            this.scanner.start(camera);
        }        
    }
});


function getUserLoginId() {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === "uli") {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
};

$(document).ready(function () {
    $(".link").click(function () {
        window.location.reload();
    });
    $("#submit").click(function () {
        var BillAmount = $("#txtBillAmt").val();
        if (BillAmount.length == 0) {
            alert("Please Enter Bill Amount !!!");
        } else if (BillAmount.length > 7) {
            alert("Please Enter Correct Bill Amount !!!");
        }
        else {
            $("#LoaderDiv").show(); 
            $("#MerchantFoundDiv").hide();           
            $("#BillAmt").text(BillAmount);
            var MerchantId = $("#MerchantId").val(); 
            var userloginId = $("#UserLoginId").val();
            const Url = "https://api.ezeelo.com/api/SubmitTransViaQRCode?MerchantId=" + MerchantId + "&UserLoginId=" + userloginId + "&BillAmount=" + BillAmount;
            $.ajax({
                type: "GET",
                url: Url,
                success: function (data) {
                    $("#LoaderDiv").hide();
                    if (data.Success == "1") {
                        $("#TransId").text(data.data);
                        $("#SuccedDiv").show();
                    } else {
                        $("#FailedDiv").show();
                    }
                },
                failure: function (data) {
                    alert("Transaction Failed!!!");
                    $("#LoaderDiv").hide();
                    $("#FailedDiv").show();
                },
                error: function (data) {
                    alert("Something Went Wrong!!!");
                    $("#LoaderDiv").hide();
                    $("#FailedDiv").show();
                }
            });
        }
    });
    
})