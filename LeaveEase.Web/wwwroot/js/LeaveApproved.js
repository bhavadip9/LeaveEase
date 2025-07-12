
var search = "";
var page = 1;
var pagesize = 5;
var LeaveStatus = "";
var LeaveDates = "all";
var SortbyName = " ";
var SortbyFromdate = " ";
var SortbyTodate = " ";
var SortbyLeavetype=" "

$(document).ready(function () {

  
    LoadLeaveApproveList(search, page, pagesize, LeaveStatus, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype);
    updateLeaveDays();

    //Status Like Pending ,Reject ....
    $(document).on("change", "#leave-status", function () {

        LeaveStatus = $(this).val();
        page = 1;
        LoadLeaveApproveList(search, page, pagesize, LeaveStatus, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype)
    });


    
    $(document).on("change", "#leave-time", function () {
       
        LeaveDates = $(this).val();
        LoadLeaveApproveList(search, page, pagesize, LeaveStatus, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype)
    });

    $(document).on("input", "#nameinput", function () {
     
        search = $(this).val();
        LoadLeaveApproveList(search, page, pagesize, LeaveStatus, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype)
    });


    $(document).on("click", ".SortbyName", function () {
        
        SortbyFromdate = null;
        SortbyTodate = null;
        SortbyLeavetype = null;
        SortbyName = $(this).data("sortbyname");
        LoadLeaveApproveList(search, page, pagesize, LeaveStatus, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype)
    });

    $(document).on("click", ".SortByFromDate", function () {
      
        SortbyName = null;
        SortbyTodate = null; 
        SortbyLeavetype = null;
        SortbyFromdate = $(this).data("sortbyfromdate");
        LoadLeaveApproveList(search, page, pagesize, LeaveStatus, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype)
    });

    $(document).on("click", ".SortByToDate", function () {
       
        SortbyName = "";
        SortbyFromdate = "";
        SortbyLeavetype = "";
        SortbyTodate = $(this).data("sortbytodate");
        LoadLeaveApproveList(search, page, pagesize, LeaveStatus, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype)
    });
    $(document).on("click", ".SortByLeaveType", function () {
        SortbyName = "";
        SortbyFromdate = "";
        SortbyTodate = "";
        SortbyLeavetype = $(this).data("sortbyleavetype");
        LoadLeaveApproveList(search, page, pagesize, LeaveStatus, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype)
    });
});

function updateLeaveDays() {
    var fromDate = $('#FromDate').val();
    var toDate = $('#ToDate').val();

    if (fromDate) {

        $('#ToDate').attr('min', fromDate);
    }

    if (fromDate && toDate) {
        if (new Date(toDate) < new Date(fromDate)) {
            //  toastr.error("To Date cannot be before From Date.");
            $('#ToDate').val('');
            $("#ActualLeave").val('');
            $("#TotalLeave").val('');
            $("#ActualLeaveDisplay").val('');
            $("#TotalLeaveDisplay").val('');
        } else {
            var ActualDay = calcBusinessDays(fromDate, toDate);
            $("#ActualLeave").val(ActualDay);
            $("#ActualLeaveDisplay").val(ActualDay);

            var TotalDay = daysBetween(fromDate, toDate);
            $("#TotalLeave").val(TotalDay);
            $("#TotalLeaveDisplay").val(TotalDay);
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

function paginationLeaveApprove(TotalCount) {


    var pagesize = parseInt($("#LeaveApprovetableSizeSelect").val());

    let totalPagesOrder = Math.ceil(TotalCount / pagesize);

    if (page > totalPagesOrder) {
        page = totalPagesOrder > 0 ? totalPagesOrder : 1;
    }
    LoadLeaveApproveList(search, page, pagesize, LeaveStatus, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype)
   
}

function pageChangeLeaveApprove(pageAction, TotalCount) {

    var pagesize = parseInt($("#LeaveApprovetableSizeSelect").val());
    
    if (pageAction == 'prev') {
        page = page === 1 ? 1 : --page;
    }
    else {
        page = (page * pagesize) > TotalCount ? page : ++page;
    }
    LoadLeaveApproveList(search, page, pagesize, LeaveStatus, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype)
}
function LoadLeaveApproveList(search, page, pagesize, LeaveStatus, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype) {


    $.ajax({
        url: "LeaveApproveList",
        data: {
            search: search,
            page: page,
            pagesize: pagesize,
            Status: LeaveStatus,
            LeaveDates: LeaveDates,
            SortbyName: SortbyName,
            SortbyFromdate: SortbyFromdate,
            SortbyTodate: SortbyTodate,
            SortbyLeavetype: SortbyLeavetype
        },
        success: function (result) {
            $("#LeaveApprovedListDiv").html(result);

        },
        error: function () {
            $("#LeaveApprovedListDiv").html('<div class="text-danger">Error loading data.</div>');
        }
    });
}


$(document).on("click", ".LeaveCancel", function () {

    var Id = $(this).data('leave-id');
   
    $.ajax({
        url: "LeaveCancel",
        method:"GET",
        data: {
            LeaveId:Id
        },
        success: function (result) {
            if (result.success == false) {
                toastr.error(result.message);
            }
            $("#CancelDiv").html(result);
            $("#CancelLeaveRequest").modal('show');
        },
        error: function () {
            $("#CancelDiv").html('<div class="text-danger">Error loading data.</div>');
        }
    })
})

$(document).on("click", "#CancelLeaveRequestButton", function () {
    var Id = $(this).data('leave-id');
    
    $.ajax({
        url: "LeaveCancelPost",
        method: "GET",
        data: {
            LeaveId: Id
        },

        success: function (result) {
            if (result.success == true) {
                $("#CancelLeaveRequest").modal('hide');
                toastr.success(result.message)
                LoadLeaveApproveList(search, page, pagesize, LeaveStatus, LeaveDates, SortbyName, SortbyFromdate, SortbyTodate, SortbyLeavetype)
            }
            else {
                toastr.error(result.message)
            } 
          
        },
        error: function () {
            $("#CancelDiv").html('<div class="text-danger">Error loading data.</div>');
        }
    })
})