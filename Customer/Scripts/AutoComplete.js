

/*function CallAutoComplete() {
    alert("CallAutoComplete");
}*/
function CallAutoComplete() {
    //alert("CallAutoComplete");
    //********* Multiple Franchise in same city By Ashish***********//
    var source = [];
    var jsonGloble = [];
    $("#ddlSelectCity").change(function () {
        if ($("#ddlSelectCity").val() == 0) {
            $("#txtSelectPincode").val('');
            $("#txtSelectPincode").hide();
            $("#ddlSelectArea").val('');
            $("#ddlSelectArea").hide();
        }
        $("#txtSelectPincode").show();
        /// calling 1st API is not working here
    });
    $("#ddlSelectArea").change(function () {

        //////////////////////////////////////
        //******************** Call 3rd API ****************//
        var ID2 = $("#ddlSelectCity").val();
        var Name2 = $("#ddlSelectCity option:selected").text();
        var FranchiseID2 = $("#ddlSelectArea").val();
        var HelpLineContact2 = "GetHelpLineNumber?franchiseid=" + FranchiseID2;
        var urlAPIHelpLineNo2 = "" + (new URLsFromConfig()).GetURL("API") + "" + HelpLineContact2;
        $.getJSON(urlAPIHelpLineNo2, { format: "json" })
           .done(function (jsonfcontact) {
               if (jsonfcontact.HTTPStatusCode == "200") {
                   var countfc = jsonfcontact.Data.length;
                   if (parseInt(countfc) > 0) {
                       var HelpLineNumber2 = jsonfcontact.Data[0].HelpLineNumber;
                       // alert("HelpNo: 200" + HelpLineNumber2 + "===" + FranchiseID2 + "===" + Name2);

                       var changeUrl2 = '@(Html.Raw(Url.RouteUrl("ChangeCity", new { city = cityName, CityID = "_CityID_", CityName = "_CityName_", FranchiseID = "_FranchiseID_", HelpLineNumber = "_HelpLineNumber_" })))';
                       var url2 = changeUrl2.replace("_CityID_", ID2).replace("_CityName_", Name2).replace("_FranchiseID_", FranchiseID2).replace("_HelpLineNumber_", HelpLineNumber2);
                       if (url2 != undefined) {
                           window.location.href = url2;
                       }
                   }
               }
               else if (json.HTTPStatusCode == "204") {
                   //// alert("No Record found.");

                   var HelpLineNumber = "Contact not found.";
                   //alert("HelpNo: 204" + HelpLineNumber2 + "===" + FranchiseID2 + "===" + Name2);

                   var changeUrl2 = '@(Html.Raw(Url.RouteUrl("ChangeCity", new { city = cityName, CityID = "_CityID_", CityName = "_CityName_", FranchiseID = "_FranchiseID_", HelpLineNumber = "_HelpLineNumber_" })))';
                   var url2 = changeUrl2.replace("_CityID_", ID2).replace("_CityName_", Name2).replace("_FranchiseID_", FranchiseID2).replace("_HelpLineNumber_", HelpLineNumber2);
                   if (url2 != undefined) {
                       window.location.href = url2;
                   }
               }
               else if (json.HTTPStatusCode == "400") {
                   //// alert("Failed.");
                   // alert("aaa else 400:" + jsonfcontact.UserMessage);
               }
           })
           .fail(function (jqXHR, textStatus, error) {
               //  var err = textStatus + ", " + error;
           });

        //******************** End Call 3rd API ****************//

    });

    $("#txtSelectPincode").keydown(function () {

        var ID = $("#ddlSelectCity").val();
        var Name = $("#ddlSelectCity option:selected").text();
        var fLocation = "GetFranchiseAllottedLocation?cityid=" + ID;
        var urlAPILocation = "" + (new URLsFromConfig()).GetURL("API") + "" + fLocation;


        //******************** Call 1st API ****************//
        //Call the REST program/method returns: JSONP
        $.getJSON(urlAPILocation, { format: "json" })
        .done(function (json) {
            //  loading...
            if (json.HTTPStatusCode == "200") {
                // $("#txtSelectPincode").show();
                //////////////
                // alert("kp: "+$("#txtSelectPincode").val());
                if ($("#txtSelectPincode").val() != '') {
                    var input = $("#txtSelectPincode").val();
                    var integer_str = parseInt(input);
                    if (isNaN(integer_str)) {
                        // alert('string');

                        source = [];
                        source = $.map(json.Data, function (item) {
                            return {
                                label: item.Area,
                                value: item.AreaID
                            };
                        });
                        if (input.length == 0) {
                            $("#lblarea").text('');
                        }

                    }
                    else if (!isNaN(integer_str)) {
                        // alert('integer');
                        source = [];
                        source = $.map(json.Data, function (item) {
                            return {
                                label: item.Pincode + " - " + item.Area,
                                value: item.PincodeID
                            };
                        });
                        /* if (input.length > 6 ) {
                              $("#lblarea").text('Invalid pincode...');
                          }
                          else {
                              $("#lblarea").text('');
                          }*/
                        if (input.length == 0) {
                            $("#lblarea").text('');
                        }
                    }
                }

                //////////////////////////////
                var __response = $.ui.autocomplete.prototype._response;
                $.ui.autocomplete.prototype._response = function (content) {
                    __response.apply(this, [content]);
                    this.element.trigger("autocompletesearchcomplete", [content]);
                };
                ///////////////////////////////
                // alert(source.lenght);
                $("#txtSelectPincode").autocomplete({
                    // delay: 500,
                    source: source,
                    focus: function (event, ui) {
                        $('#txtSelectPincode').val(ui.item.label);
                        return false;
                    },
                    // appendTo: ee,
                    // Once a value in the drop down list is selected, do the following:
                    select: function (event, ui) {
                        // place the person.given_name value into the textfield called 'select_origin'...
                        $('#txtSelectPincode').val(ui.item.label);
                        // and place the person.id into the hidden textfield called 'link_origin_id'.
                        //// $('#hdd_origin_id').val(ui.item.value);

                        //******************** Call 2nd API ****************//
                        var AreaID = ui.item.value;
                        var Area_Pin_ID = "GetFranchiseID?cityid=" + ID + "&" + "pincide_areaid=" + AreaID;
                        var urlAPIFranciseID = "" + (new URLsFromConfig()).GetURL("API") + "" + Area_Pin_ID;
                        $.getJSON(urlAPIFranciseID, { format: "json" })

                        .done(function (jsonfid) {
                            //  loading...
                            if (jsonfid.HTTPStatusCode == "200") {

                                var count = jsonfid.Data.length;
                                if (parseInt(count) > 1) {
                                    var ddlArea = $("[id*=ddlSelectArea]");
                                    ddlArea.empty().append('<option selected="selected" value="0">Select Area</option>');
                                    $.each(jsonfid.Data, function (index, item) {
                                        var City = this['CityName'] + " - " + this['FranchiseOfficePincode'];
                                        ddlArea.append($("<option></option>").val(this['FranchiseIDs']).html(City));
                                    });
                                    $("#ddlSelectArea").show();
                                }
                                else {
                                    var FranchiseID = jsonfid.Data[0].FranchiseIDs;
                                    // alert("ID:" + FranchiseID);

                                    //******************** Call 3rd API ****************//
                                    var HelpLineContact = "GetHelpLineNumber?franchiseid=" + FranchiseID;
                                    var urlAPIHelpLineNo = "" + (new URLsFromConfig()).GetURL("API") + "" + HelpLineContact;
                                    $.getJSON(urlAPIHelpLineNo, { format: "json" })
                                       .done(function (jsonfcontact) {
                                           if (jsonfcontact.HTTPStatusCode == "200") {
                                               var countfc = jsonfcontact.Data.length;
                                               //alert("hhh:" + parseInt(countfc));
                                               if (parseInt(countfc) > 0) {
                                                   var HelpLineNumber = jsonfcontact.Data[0].HelpLineNumber;
                                                   // alert("HelpNo...200" + HelpLineNumber + "===" + FranchiseID + "===" + Name+ "===" +ID);

                                                   var changeUrl = '@(Html.Raw(Url.RouteUrl("ChangeCity", new { city = cityName, CityID = "_CityID_", CityName = "_CityName_", FranchiseID = "_FranchiseID_", HelpLineNumber = "_HelpLineNumber_" })))';
                                                   var url = changeUrl.replace("_CityID_", ID).replace("_CityName_", Name).replace("_FranchiseID_", FranchiseID).replace("_HelpLineNumber_", HelpLineNumber);
                                                   if (url != undefined) {
                                                       window.location.href = url;
                                                   }
                                               }
                                           }
                                           else if (jsonfcontact.HTTPStatusCode == "204") {
                                               //// alert("No Record found.");
                                               var HelpLineNumber = "Contact not found.";
                                               // alert("HelpNo...204" + HelpLineNumber + "===" + FranchiseID + "===" + Name);

                                               var changeUrl = '@(Html.Raw(Url.RouteUrl("ChangeCity", new { city = cityName, CityID = "_CityID_", CityName = "_CityName_", FranchiseID = "_FranchiseID_", HelpLineNumber = "_HelpLineNumber_" })))';
                                               var url = changeUrl.replace("_CityID_", ID).replace("_CityName_", Name).replace("_FranchiseID_", FranchiseID).replace("_HelpLineNumber_", HelpLineNumber);
                                               if (url != undefined) {
                                                   window.location.href = url;
                                               }

                                           }
                                           else if (jsonfcontact.HTTPStatusCode == "400") {
                                               //// alert("Failed.");
                                               // alert("hhh else 400:" + jsonfcontact.UserMessage);

                                           }
                                       })
                                            .fail(function (jqXHR, textStatus, error) {
                                                var err = textStatus + ", " + error;
                                            });

                                    //******************** End Call 3rd API ****************//
                                    if (url != undefined) {
                                        window.location.href = url;//urlglobal;
                                    }
                                }
                            }
                            else if (json.HTTPStatusCode == "204") {
                                //// alert("No Record found.");
                            }
                            else if (json.HTTPStatusCode == "400") {
                                //// alert("Failed.");
                            }
                        })
                              .fail(function (jqXHR, textStatus, error) {
                                  var err = textStatus + ", " + error;
                                  //// alert('Unable to Connect to Server.\n Try again Later.\n Request Failed: ' + err);

                              });

                        //******************** End Call 2nd API ****************//
                        return false;
                    }
                })
                         .bind("autocompletesearchcomplete", function (event, contents) { // $("#txtSelectPincode")
                             // alert(contents.length);
                             // $("#lblarea").text(contents.length);
                             // alert($("#lblarea").text());

                             if ($("#txtSelectPincode").val() != '') {
                                 var input = $("#txtSelectPincode").val();
                                 var integer_str = parseInt(input);
                                 if (!isNaN(integer_str)) {
                                     if (input.length > 6) {
                                         $("#lblarea").text('Invalid pincode...');
                                     }
                                     else {
                                         $("#lblarea").text("");
                                     }
                                 }
                                 /*else if (isNaN(integer_str)) {
                                     if (contents.length == 0) {
                                         $("#lblarea").text('Area not found...');
                                         //$("#lblarea").text(contents.length);
                                      //   return false;
                                     }
                                     else if (input.length == 0) {
                                         $("#lblarea").text("");
                                     }
                                 }*/

                                 /* if (contents.length !== 0) {
                                      $("#lblarea").text('');
                                  }*/
                             }
                             /*if (contents.length !== 0) {
                                 $("#lblarea").html('');
                             }*/
                             /* else
                              {
                                  $("#lblarea").text('');
                              }*/
                             // alert(contents.length)
                         });

                /* $("#txtSelectPincode").autocomplete("result", function (event, source) {

                     if (!source) { alert('nothing found!'); }

                 })*/

            }
            else if (json.HTTPStatusCode == "204") {
                //// alert("No Record found.");
            }
            else if (json.HTTPStatusCode == "400") {
                //// alert("Failed.");
            }

        })
            .fail(function (jqXHR, textStatus, error) {
                var err = textStatus + ", " + error;
                //// alert('Unable to Connect to Server.\n Try again Later.\n Request Failed: ' + err);

            });
        //******************** End Call 1st API ****************//



    });

    //********* End Multiple Franchise in same city ***********//
};