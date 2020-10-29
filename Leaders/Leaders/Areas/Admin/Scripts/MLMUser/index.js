(function ($) {
    function index() {
        var $this = this;

        function getfilter() {
            var dateRange = $('#mlmDateRange').val();
            var dateFrom = '', dateTo = '';

            if (dateRange && dateRange != '') {
                dateFrom = dateRange.split(localeOpts.separator)[0];
                dateTo = dateRange.split(localeOpts.separator)[1];
            }

            var data = {
                sSearch: $('#txt_search').val(),
                startDate: dateFrom,
                endDate: dateTo
            };

            return data;
        }

        function ExportToExcel() {
            $('#btnExcel').click(function () {
                var sDate = $("#min").val();
                var eDate = $("#max").val();
                window.location.href = domain + "MLMUser/ExportToExcel?toDate=" + eDate + "&fromDate=" + sDate;
            })
        }


        function initailizetable() {
            var table = $('#grid-leaderList').DataTable({
                "serverSide": true,
                "destroy": true,
                "pageLength": 10,
                "bFilter": false,
                "bSort": false,
                "bInfo": true,
                "bProcessing": true,
                "ordering": true,
                language: {
                    searchPlaceholder: "Search by Name/RefId"
                },
                "columnDefs":
                    [
                        { name: "rowindex", data: "rowIndex", title: "SNO", sortable: false, searchable: false, visible: true, "targets": 0 },
                        {
                            name: "NetworkReport", data: "UserId", title: "Network Report", sortable: false, searchable: false, visible: true, "targets": 1,
                            render: function (data, type, row, meta) {
                                console.log(data);
                                return "<a href='" + domain +'networkReport/index/' + row.UserId + "'>Network Report</a>";
                            }
                        },
                        { name: "FullName", data: "FullName", title: "Full Name", sortable: false, searchable: true, visible: true, "targets": 2 },
                        { name: "Designation", data: "Designation", title: "Designation", sortable: false, searchable: false, visible: true, "targets": 3 },
                        { name: "Email", data: "Email", title: "Email", sortable: false, searchable: false, visible: true, "targets": 4 },
                        { name: "Mobile", data: "Mobile", title: "Mobile", sortable: false, searchable: false, visible: true, "targets": 5 },
                        {
                            name: "CreateDate", data: "CreateDate", title: "Create Date", sortable: false, searchable: false, visible: true, "targets": 6

                        },
                        { name: "ReferalId", data: "Ref_Id", title: "Referal ID", sortable: false, searchable: false, visible: true, "targets": 7 },
                        { name: "Level", data: "Level", title: "Level", sortable: false, searchable: false, visible: true, "targets": 8 },
                        {
                            name: "JoinDate", data: "Join_ref_date", title: "Join Date", sortable: false, searchable: false, visible: true, "targets": 9

                        },
                        {
                            name: "ActivateDateRef", data: "Active_ref_date", title: "Activate Date Ref", sortable: false, searchable: false, visible: true, "targets": 10
                        },
                        { name: "ReferedId", data: "Refered_Id_ref", title: "Refered By", sortable: false, searchable: false, visible: true, "targets": 11 },
                        { name: "ParentName", data: "ParentName", title: "Parent Name", sortable: false, searchable: false, visible: true, "targets": 12 }
                    ],
                "ajax":
                {
                    "url": "../Admin/MLMUser/leaderList",
                    "type": "POST",
                    "dataType": "JSON",
                    "data": getfilter()
                }
            });

            //$('#txt_search').keypress(function (event) {
            //    var keycode = (event.keyCode ? event.keyCode : event.which);
            //    if (keycode == '13') {
            //        initailizetable();
            //    }
            //});
        }

        function initializeControl() {

            localeOpts = {
                format: "DD/MM/YYYY",
                separator: " to "
            };

            function rangeChangeCB(start, end) {
                $('#mlmDateRange').val(start.format(localeOpts.format) + localeOpts.separator + end.format(localeOpts.format));
            }

            $('#mlmDateRange').daterangepicker({
                "locale": localeOpts,
                startDate: moment().subtract(29, 'days'),
                endDate: moment(),
                autoUpdateInput: false,
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                }
            }, rangeChangeCB);
        }

        $("#btnSearch").click(function () {
            initailizetable();
        });

        $("#clrFilterDate").click(function () {
            $("#mlmDateRange").val('');
        });

        $this.init = function () {
            initializeControl();
            initailizetable();           
            ExportToExcel();
        };
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));