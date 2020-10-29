$(document).ready(fun).on('page:load', fun);

function fun() {
    pagination();
    sortable();
    searchable();
    editable();
    updatePageLink();
    exportToXLS();
    exportToPDF();
}

//////////////////////////////////////////////////////////////////////////
//////////////////////////-------Pagination-------////////////////////////
//////////////////////////////////////////////////////////////////////////

function pagination() {
    page = 1;
    $("table").each(function() {
        if ($(this).hasClass("pagination")) {
            showRows($(this), 10, page);
        }
    });
    $(".quantity").change(function() {
        showRows($(this).closest("div").find("table"), $(this).val(), page = 1);
    });
    $('.first_page, .prev_page, .next_page, .last_page').click(changePage);
}

function changePage() {
    var $div = $(this).closest('div').parent('div');
    var table = $div.find("table");
    var count = $div.find("select").val();
    var max = Math.ceil(table.find("tbody tr").length / count);
    if (page < max + 1 && page > 0) {
        var cls = $(this).attr('class');
        var pages = {"first_page": 1, "prev_page": page - 1, "next_page": page + 1, "last_page": max};
        $(this).closest('div').find('.current_page').text(page = pages[cls]);
        showRows(table, count, page);
    } else {
        return;
    }
}

function updatePageLink() {
    if ($(document).find("table.pagination").length != 0) {
        var text = "";
        var show = $(".quantity option:selected").text();
        var search = $(document).find("input.search_input").val();
        if (parseInt(show) > 10) {
            text += "show=" + show + "&";
        }
        if (search != "" && search) {
            text += "search=" + search + "&";
        }
        window.location.hash = '#';
        document.URL.indexOf('#') == -1 ? text += '#page=' + page : text += 'page=' + page;
        window.history.pushState(null, null, document.URL + text);
    }
}

function activeButtons(count, page, table) {
    var $div = table.closest('div').find("div.pagination_div");
    var $rows = table.find("tbody tr").length;
    if ($div.length === 1) {
        var elem1 = $div.find('.prev_page, .first_page');
        page === 1 ? elem1.addClass('not_active') : elem1.removeClass('not_active');
        var elem2 = $div.find('.next_page, .last_page');
        (table.prev('select').val() > $rows || page === Math.ceil($rows / count)) ? elem2.addClass('not_active') : elem2.removeClass('not_active');
    }
}

function showRows(table, count, page) {
    table.find("tbody tr").each(function(i) {
        (i >= (page - 1) * count && i < (page - 1) * count + parseInt(count)) ? $(this).show() : $(this).hide();
    });
    activeButtons(count, page, table);
    updatePageLink();
}

//////////////////////////////////////////////////////////////////////////
//////////////////////////////-------Sortable-------//////////////////////
//////////////////////////////////////////////////////////////////////////

function sortable() {
    $(".sortable > thead tr > th").click(easyTableSort);
}

function easyTableSort(newTH) {
    var $this;
    var order = 'asc';
    if ($(this).text() != "") {
        $this = $(this);
        if ($this.hasClass('sortASC')) {
            order = 'desc';
        }
    } else {
        $this = newTH;
        if ($this.hasClass('sortASC')) {
            order = 'asc';
        } else {
            order = 'desc';
        }
    }
    var table = $this.closest('table');
    var type = $this.data("sort") || null;
    var arr = ["String", "Text", "Integer", "Float", "Fixnum", "Decimal", "Date"];
    if (type === null || jQuery.inArray(type, arr) === -1) {
        return;
    }
    var index = $this.index();
    sortTable(table, order, index, type);
    stylingTh($this, order);
    var th = $this.closest('tr').find('th:nth-child(1)');
    if (th.data("sort") == "index") {
        resetIndexes(table);
    }
    if (table.hasClass("pagination")) {
        page = 1;
        table.closest('div').find('.current_page').text(page);
        showRows(table, table.closest('div').find("select").val(), page);
    }
}

function stylingTh(th, sort) {
    th.closest('tr').find('th').each(function() {
        $(this).removeClass('sortDESC sortASC');
    });
    sort === 'desc' ? th.addClass('sortDESC') : th.addClass('sortASC');
}

function resetIndexes(table) {
    var row = 1;
    $(table).find('td:nth-child(1)').each(function() {
        $(this).text(row++);
    });
}

function sortTable(table, order, column, type) {
    var tbody = table.find('tbody');
    tbody.find('tr').sort(function(a, b) {
        if (type == "String" || type == "Text" || type == "Date") {
            if (order == 'asc') {
                return $('td:eq(' + column + ')', a).text().localeCompare($('td:eq(' + column + ')', b).text());
            } else {
                return $('td:eq(' + column + ')', b).text().localeCompare($('td:eq(' + column + ')', a).text());
            }
        } else {
            if (order == 'asc') {
                return $('td:eq(' + column + ')', a).text().localeCompare($('td:eq(' + column + ')', b).text(), 'pl', {numeric: true});
            } else {
                return $('td:eq(' + column + ')', b).text().localeCompare($('td:eq(' + column + ')', a).text(), 'pl', {numeric: true});
            }
        }
    }).appendTo(tbody);
}

//////////////////////////////////////////////////////////////////////////
////////////////////////////-------Searchable-------//////////////////////
//////////////////////////////////////////////////////////////////////////

$(document).ready(function() {
    TableContent = $(document).find("table > tbody > tr");
});

function searchable() {
    $(".search_input").on("keyup", searchInTable);
    $('.clearlink').click(showWholeTable);
}

function showWholeTable() {
    $(this).prev('input').val('');
    var table = $(this).closest('div').next("table");
    var content = TableContent;
    table.find('tbody > tr').each(function() {
        $(this).remove();
    });
    content.each(function() {
        table.find('tbody').append($(this));
    });
    var th = table.find('th:nth-child(1)');
    if (th.data("sort") == "index") {
        resetIndexes(table);
    }
    if (table.hasClass("pagination")) {
        page = 1;
        table.closest('div').find('.current_page').text(page);
        showRows(table, table.closest('div').find("select").val(), page);
    }
    reSort(table);
    removeTextHighlighting(table.find('tbody tr span'));
    updatePageLink();
}

function searchInTable() {
    var text = $(this).val().toLowerCase();
    var table = $(this).closest('div').next("table");
    var content = TableContent;
    table.find('tbody > tr').each(function() {
        $(this).remove();
    });
    content.each(function() {
        var $row = $(this);
        var firstCol = table.find('thead > tr > th:first').data('sort');
        var $tdElement = $row.find("td:first");
        if (firstCol === 'index') {
            $tdElement = $row.find("td").slice(1);
        }
        var rowText = splitString($tdElement);
        if (rowText.indexOf(text) !== -1) {
            addTextHighlighting($tdElement, text);
            table.find('tbody').append($row);
        }
    });
    if (table.hasClass("pagination")) {
        page = 1;
        table.closest('div').find('.current_page').text(page);
        showRows(table, table.closest('div').find("select").val(), page);
    }
    var th = table.find('th:nth-child(1)');
    if (th.data("sort") == "index") {
        resetIndexes(table);
    }
    reSort(table);
    if (table.find('tbody > tr').length == 0) {
        table.find('tbody').append('<tr class="no_result"><td colspan="' + table.find("tr:first th").length + '">No Data Available</td></tr>');
    }
    updatePageLink();
}

function reSort(table) {
    if (table.find('.sortDESC').length != 0 || table.find('.sortASC').length != 0) {
        if (table.find('.sortDESC').index() != -1) {
            easyTableSort(table.find("th:eq(" + table.find('.sortDESC').index() + ")"));
        } else {
            easyTableSort(table.find("th:eq(" + table.find('.sortASC').index() + ")"));
        }
    }
}

function splitString($tdElement) {
    var rowText = "";
    $tdElement.each(function() {
        rowText += $(this).text() + "/";
    });
    return rowText.toLowerCase().trim();
}

function removeTextHighlighting(element) {
    element.each(function() {
        $(this).replaceWith($(this).html());
    });
}

function addTextHighlighting(columns, text) {
    columns.each(function() {
        var reg = new RegExp("(" + text + ")", "ig");
        var newText = $(this).text().replace(reg, "<span class='findElement'>$1</span>");
       // var removeAtt = $("table.searchable tbody tr td:last-child").find("span").remove();
        //var noReplece = $(".searchable tbody tr td:last-child").html();
        $(this).html(newText);
       
    });
}

//////////////////////////////////////////////////////////////////////////
//////////////////////////////-------Editable-------//////////////////////
//////////////////////////////////////////////////////////////////////////

function editable() {
    $("table.editable").on("dblclick", "th, td", function() {
        var element = $(this);
        var th = element.closest('table').find('th').eq(this.cellIndex);
        if (element.data("sort") === 'index' || th.data('sort') === 'index') {
            return;
        }
        //element.html("<input type='text' value='" + element.text() + "' id='editable_input'/>");
        //$("#editable_input").focus();
        //$(document).on('click', document, function(event) {
        //    if (event.target.id !== 'editable_input') {
        //        insertNewValue(element, event);
        //    }
        //}).keypress(function(event) {
        //    var keycode = (event.keyCode ? event.keyCode : event.which);
        //    if (keycode == '13') {
        //        insertNewValue(element, event);
        //    }
        //});
    });
}

function insertNewValue(element, event) {
    var newText = $("#editable_input").val();
    element.html(newText);
    $('html').unbind(event);
}

////////////////////////////////////////////////////////////////////////
/////////////////////////-------Save to xls-------//////////////////////
////////////////////////////////////////////////////////////////////////

function exportToXLS() {
    $(".xls_save").click(function(e) {
        var uri = 'data:application/vnd.ms-excel;base64,'
            , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--><meta http-equiv="content-type" content="text/plain; charset=UTF-8"/></head><body><table>{table}</table></body></html>'
            , base64 = function(s) {
                return window.btoa(unescape(encodeURIComponent(s)));
            }
            , format = function(s, c) {
                return s.replace(/{(\w+)}/g, function(m, p) {
                    return c[p];
                })
            }
        var table = $(this).closest('div').find("table").clone();
        table.find("tbody tr").each(function() {
            $(this).show();
        });
        table = table.html();
        window.location.href = uri + base64(format(template, {worksheet: 'Worksheet', table: table}));
        e.preventDefault();
    });
}

//////////////////////////////////////////////////////////////////////////
///////////////////////////-------Save to xls-------//////////////////////
//////////////////////////////////////////////////////////////////////////

function exportToPDF() {
    $(".pdf_save").click(function() {
        var pdf = new jsPDF('p', 'pt', 'letter');
        var source = "<table>" + $(this).closest('div').find('table').html() + "</table>";
        specialElementHandlers = {
            '#bypassme': function(element, renderer) {
                return true
            }
        };
        margins = {
            top: 80,
            bottom: 60,
            left: 40,
            width: 522
        };
        pdf.fromHTML(
            source,
            margins.left,
            margins.top, {
                'width': margins.width,
                'elementHandlers': specialElementHandlers
            },
            function(dispose) {
                pdf.save('Test.pdf');
            }, margins);
    });
}