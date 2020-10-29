(function ($) {
    function index() {
        var $this = this;       

        function ExportToExcel() {
            $('#btnExcel').click(function () {
                var sDate = $("#min").val();
                var eDate = $("#max").val();
                window.location.href = domain + "NetworkReport/ExportToExcel";
            })
        }


        function initailizetable() {            
           
            var table = $('#grid-networkReportList').DataTable({
                "serverSide": true,
                "destroy": true,
                "pageLength": 10,
                "bFilter": false,
                "bSort": false,
                "bInfo": true,
                "bProcessing": true,
                "ordering": true,               
                "columnDefs":
                    [
                        { name: "rowIndex", data: "rowIndex", title: "SNO", sortable: false, searchable: false, visible: true, "targets": 0 },                        
                        { name: "FullName", data: "FullName", title: "Full Name", sortable: false, searchable: true, visible: true, "targets": 1 },
                        { name: "UserEmail", data: "UserEmail", title: "User Email", sortable: false, searchable: false, visible: true, "targets": 2 },
                        { name: "Mobile", data: "Mobile", title: "Mobile", sortable: false, searchable: false, visible: true, "targets": 3 },
                        { name: "RefferalCode", data: "ReferalID", title: "Refferal Code", sortable: false, searchable: true, visible: true, "targets": 4 },
                        {
                            name: "InActivePoints", data: "InActivePoints", title: "InActivePoints", sortable: false, searchable: false, visible: true, "targets": 5

                        },
                        { name: "RP", data: "RPOnMyPurchase", title: "RP", sortable: false, searchable: true, visible: true, "targets": 6 },
                        { name: "ERP", data: "ERP", title: "ERP", sortable: false, searchable: false, visible: true, "targets": 7 },
                        {
                            name: "LastTransaction", data: "LastTransaction", title: "Last Trx Date", sortable: false, searchable: false, visible: true, "targets": 8

                        },
                        {
                            name: "Downline", data: "Downline", title: "Downline", sortable: false, searchable: false, visible: true, "targets": 9
                        },                        
                        { name: "ParentName", data: "ParentName", title: "Parent Name", sortable: false, searchable: false, visible: true, "targets": 10 },
                        { name: "JoinDate", data: "JoinDate", title: "JoinDate", sortable: false, searchable: false, visible: true, "targets": 11 },
                        {
                            name: "ActiveStatus", data: "PendingPoints", title: "ActiveStatus", sortable: false, searchable: false, visible: true, "targets": 12,
                            render: function (data, type, row, meta) {
                                if (data == 0)
                                    return "<span style=background-color:#0ceb4a class=badge badge-table badge-success>Active</span>";
                                return "<span style=background-color:#b30707 class=badge badge-table badge-danger>Inactive</span>";
                            }
                        }
                    ],
                "ajax":
                {
                    "url": "../?Userid=" + $("#UserID").val() + "&sSearch=" + $('#txt_search').val(),
                    "type": "POST",
                    "dataType": "JSON"
                }
            });  
        }

        $("#btnSearch").click(function () {
            initailizetable();
        });

        $this.init = function () {           
            initailizetable();           
            ExportToExcel();
        };
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));