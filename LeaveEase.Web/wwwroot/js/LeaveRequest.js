

$(document).on("click", ".DeleteLeaveRequest", function () {

    var id = $(this).data("leave-id");

    $.ajax({
        url: "DeleteLeaveRequest",
        method: "GET",
        data: {
            LeaveId: id
        },
        success: function (result) {
            if (result.success == false) {
                toastr.error(result.message)
            }
            $("#LeaveRequestDeleteDiv").html(result);
            $('#DeleteLeaveRequest').modal('show');

        }
    });
})

$(document).on("click", "#DeleteLeaveRequestButton", function () {

    var id = $(this).data("leave-id");
    $.ajax({
        url: "DeleteLeaveRequestPost",
        method: "POST",
        data: {
            LeaveId: id
        },
        success: function (result) {
            if (result.success == true) {
                $("#DeleteLeaveRequest").modal('hide');
                toastr.success(result.message)
                LoadLeaveReqestList(page, pagesize, LeaveStatus, LeaveDates, SortbyFromdate, SortbyTodate, SortbyLeavetype, AppliedDate)
            }
            else {
                toastr.error(result.message)
            }

        }
    });
})


var page = 1;
var pagesize = 5;
var LeaveStatus = "";
var LeaveDates = "all";
var SortbyFromdate = " ";
var SortbyTodate = " ";
var SortbyLeavetype = " ";
var AppliedDate = " ";

$(document).ready(function () {


    updateLeaveDays();

    $('#FromDate').on('change', function () {
        updateLeaveDays();
    });

    $('#ToDate').on('change', function () {
        updateLeaveDays();
    });

    LoadLeaveReqestList(page, pagesize, LeaveStatus, LeaveDates, SortbyFromdate, SortbyTodate, SortbyLeavetype, AppliedDate)

    $(document).on("change", "#leave-status", function () {

        LeaveStatus = $(this).val();
        page = 1;
        LoadLeaveReqestList(page, pagesize, LeaveStatus, LeaveDates, SortbyFromdate, SortbyTodate, SortbyLeavetype, AppliedDate)
    });

    $(document).on("change", "#leave-time", function () {
        LeaveDates = $(this).val();
        LoadLeaveReqestList(page, pagesize, LeaveStatus, LeaveDates, SortbyFromdate, SortbyTodate, SortbyLeavetype, AppliedDate)
    });

    $(document).on("click", ".SortByAppliedDate", function () {
        SortbyFromdate = "";
        SortbyTodate = "";
        SortbyLeavetype = "";
        AppliedDate = $(this).data("sortbyapplieddate");
        LoadLeaveReqestList(page, pagesize, LeaveStatus, LeaveDates, SortbyFromdate, SortbyTodate, SortbyLeavetype, AppliedDate)
    });

    $(document).on("click", ".SortByFromDate", function () {
        AppliedDate = "";
        SortbyTodate = "";
        SortbyLeavetype = "";
        SortbyFromdate = $(this).data("sortbyfromdate");
        LoadLeaveReqestList(page, pagesize, LeaveStatus, LeaveDates, SortbyFromdate, SortbyTodate, SortbyLeavetype, AppliedDate)
    });

    $(document).on("click", ".SortByToDate", function () {
        AppliedDate = "";
        SortbyFromdate = "";
        SortbyLeavetype = "";
        SortbyTodate = $(this).data("sortbytodate");
        LoadLeaveReqestList(page, pagesize, LeaveStatus, LeaveDates, SortbyFromdate, SortbyTodate, SortbyLeavetype, AppliedDate)
    });
    $(document).on("click", ".SortByLeaveType", function () {
        AppliedDate = "";
        SortbyFromdate = "";
        SortbyTodate = "";
        SortbyLeavetype = $(this).data("sortbyleavetype");
        LoadLeaveReqestList(page, pagesize, LeaveStatus, LeaveDates, SortbyFromdate, SortbyTodate, SortbyLeavetype, AppliedDate)
    });

});



function updateLeaveDays() {
    var fromDate = $('#FromDate').val();
    var toDate = $('#ToDate').val();

    if (fromDate) {
        $('#ToDate').attr('min', fromDate);
    }

    if (fromDate && toDate) {
        var totalLeave = daysBetween(fromDate, toDate);

        if (totalLeave > 20) {
            toastr.error("You can request a maximum of 20 days leave.");
            $('#ToDate').val('');
            $("#ActualLeave").val('');
            $("#TotalLeave").val('');
            $("#ActualLeaveDisplay").val('');
            $("#TotalLeaveDisplay").val('');
            return;
        }

        if (new Date(toDate) < new Date(fromDate)) {
            $('#ToDate').val('');
            $("#ActualLeave").val('');
            $("#TotalLeave").val('');
            $("#ActualLeaveDisplay").val('');
            $("#TotalLeaveDisplay").val('');
        } else {
            var actualDay = calcBusinessDays(fromDate, toDate);
            $("#ActualLeave").val(actualDay);
            $("#ActualLeaveDisplay").val(actualDay);

            $("#TotalLeave").val(totalLeave);
            $("#TotalLeaveDisplay").val(totalLeave);
        }
    }
}

function daysBetween(date1, date2) {

    if (!(date1 instanceof Date)) date1 = new Date(date1);
    if (!(date2 instanceof Date)) date2 = new Date(date2);
    const date1Ms = date1.getTime();
    const date2Ms = date2.getTime();
    const diffMs = date2Ms - date1Ms;
    const diffDays = Math.round(diffMs / (1000 * 60 * 60 * 24));
    return (diffDays + 1);
}





function paginationLeaveRequest(TotalCount) {

    var pagesize = parseInt($("#LeaveRequesttableSizeSelect").val());

    let totalPagesOrder = Math.ceil(TotalCount / pagesize);

    if (page > totalPagesOrder) {
        page = totalPagesOrder > 0 ? totalPagesOrder : 1;
    }
    LoadLeaveReqestList(page, pagesize, LeaveStatus, LeaveDates, SortbyFromdate, SortbyTodate, SortbyLeavetype, AppliedDate)
}

function pageChangeLeaveRequest(pageAction, TotalCount) {

    var pagesize = parseInt($("#LeaveRequesttableSizeSelect").val());
    if (pageAction == 'prev') {
        page = page === 1 ? 1 : --page;
    }
    else {
        page = (page * pagesize) > TotalCount ? page : ++page;
    }
    LoadLeaveReqestList(page, pagesize, LeaveStatus, LeaveDates, SortbyFromdate, SortbyTodate, SortbyLeavetype, AppliedDate)
}



function calcBusinessDays(dDate1, dDate2) {
    if (!(dDate1 instanceof Date)) dDate1 = new Date(dDate1);
    if (!(dDate2 instanceof Date)) dDate2 = new Date(dDate2);

    if (dDate2 < dDate1) return -1;

    let count = 0;
    let currentDate = new Date(dDate1);

    while (currentDate <= dDate2) {
        const day = currentDate.getDay();
        if (day !== 0 && day !== 6) { // not Sunday (0) or Saturday (6)
            count++;
        }
        currentDate.setDate(currentDate.getDate() + 1);
    }

    return count;
}




function LoadLeaveReqestList(page, pagesize, LeaveStatus, LeaveDates, SortbyFromdate, SortbyTodate, SortbyLeavetype, AppliedDate) {


    $.ajax({
        url: "LeaveReqestList",
        method:"GET",
        data: {
            page: page,
            pagesize: pagesize,
            Status: LeaveStatus,
            LeaveDates: LeaveDates,
            SortbyFromdate: SortbyFromdate,
            SortbyTodate: SortbyTodate,
            SortbyLeavetype: SortbyLeavetype,
            AppliedDate: AppliedDate
        },
        success: function (result) {
            $("#LeaveRequestListDiv").html(result);

        },
        error: function () {
            $("#LeaveRequestListDiv").html('<div class="text-danger">Error loading data.</div>');
        }
    });
}