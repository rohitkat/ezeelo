/*********************************************************************************************************************/
/*************************************************Facebook Login*****************************************************/
/********************************************************************************************************************/


var check=0;

    function onSignIn(googleUser) {
        // Useful data for your client-side scripts:
        var profile = googleUser.getBasicProfile();


        // The ID token you need to pass to your backend:
        var id_token = googleUser.getAuthResponse().id_token;
        console.log("ID Token: " + id_token);

        var callfrom = $("#callFrom").val();
        var externalCallBackURL = $("#externalCallBackURL").html();
        var email = profile.getEmail();
        var useName = profile.getName();

        if (check) {
        $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            url: '@Url.Action("testExternalLogin", "Login")', //+ '?email=' + profile.getEmail(),
            data: {
                'email': email, 'callFrom': callfrom, 'externalCallBackURL': externalCallBackURL, 'userName': useName

            },
            type: "GET",
            success: function (result) {
                alert(result);
                //alert("login successfull");
                window.location.href = externalCallBackURL;
            },
            error: function (data) {
                $("#lblMessage").text("We are unavailable to process your request.");
                $("#loader_img1").hide();
                window.location.href = externalCallBackURL;
            }
        });

    }
    check = 1;

    };


    function signOut() {
        var auth2 = gapi.auth2.getAuthInstance();
        auth2.signOut().then(function () {
            console.log('User signed out.');
        });
    }







/*********************************************************************************************************************/
/*************************************************google Login*****************************************************/
/********************************************************************************************************************/
//var isFirstGoogle = 0;

//function onSignIn(googleUser) {

//    // Useful data for your client-side scripts:
//    var profile = googleUser.getBasicProfile();

   
//    if (isFirstGoogle) {
//        var callfrom = $("#callFrom").val();
//        var externalCallBackURL = $("#externalCallBackURL").html();
//        var email = profile.getEmail();
//        var useName = profile.getName();

//        // The ID token you need to pass to your backend:
//        var id_token = googleUser.getAuthResponse().id_token;
//        console.log("ID Token: " + id_token);

       
     
//        $.ajax({
//            contentType: 'application/json; charset=utf-8',
//            dataType: 'json',
//            url: '@Url.Action("testExternalLogin", "Login")', //+ '?email=' + profile.getEmail(),
//            data: { 'email': email, 'callFrom': callfrom, 'externalCallBackURL': externalCallBackURL, 'userName': useName },
//            type: "GET",
//            success: function (result) {
//                alert(result);
//                //alert("login successfull");
//                window.location.href = externalCallBackURL;
//            },
//            error: function (data) {
//                $("#lblMessage").text("We are unavailable to process your request.");
//                $("#loader_img1").hide();
//                window.location.href = externalCallBackURL;
//            }
//        });
//        window.location.href = externalCallBackURL;
//    }

//    isFirstGoogle = 1
//};




//function signOut() {
//    var auth2 = gapi.auth2.getAuthInstance();
//    auth2.signOut().then(function () {
//        console.log('User signed out.');
//    });
//}



/*
Other Solution For Google Sign-in 

<html>
<head>
   <meta name="google-signin-client_id" content="YOUR_CLIENT_ID">
</head>
<body>
  <script>
    function onSignIn(googleUser) {
      var profile = googleUser.getBasicProfile();
      var user_name = profile.getName();
      alert(user_name);
    }

    function onLoad() {
      gapi.load('auth2,signin2', function() {
        var auth2 = gapi.auth2.init();
        auth2.then(function() {
          // Current values
          var isSignedIn = auth2.isSignedIn.get();
          var currentUser = auth2.currentUser.get();

          if (!isSignedIn) {
            // Rendering g-signin2 button.
            gapi.signin2.render('google-signin-button', {
              'onsuccess': 'onSignIn'  
            });
          }
        });
      });
    }
  </script>

  <div id="google-signin-button"></div>

  <script src="https://apis.google.com/js/platform.js?onload=onLoad" async defer></script>
</body>
</html>
*/



