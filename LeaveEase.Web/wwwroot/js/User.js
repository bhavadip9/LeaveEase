var page = 1;
var pagesize = 5;
var role = "Admin";
var search = "";
var orderByDepartment = "";
var TotalCount;



//Delete User Model Open 
$(document).on("click", "#DeleteUser", function () {
    
    var id = $(this).data("user-id");
    
    $.ajax({
        url: "DeleteUser",
        method: "GET",
        data: {
            UserId: id
        },
        success: function (result) {
            if (result.success == false) {
                toastr.error(result.message)
            }
            $("#DeleteModelOpen").html(result);
            $("#DeleteUserModel").modal('show');
        }
    });
})

//Delete User Model Post Method
$(document).on("click", "#DeleteUserButton", function () {
    var id = $(this).data("user-id");
  
    $.ajax({
        url: "DeleteUserPost",
        method: "POST",
        data: {
            UserId: id
        },
        success: function (result) {  
            if (result.success == true) {
                $("#DeleteUserModel").modal('hide');
                toastr.success(result.message)

                var activeTab = $(".nav-tabs .active").attr("id");

                if (activeTab === "tab-admin") {
                    LoadUserList("Admin", search, page, pagesize, orderByDepartment);
                } else if (activeTab === "tab-employee") {
                    LoadUserList("Employee", search, page, pagesize, orderByDepartment);
                }  
            }
            else {
                toastr.error(result.message)
            }       
        }
    });
})

//close Delete User Model
$(document).on("click", ".CloseDeleteModel", function () {
    $("#DeleteUserModel").hide();
})


// Edit Page Open
//$(document).on("click", "#EditUser", function () {
//    var Id = $(this).data("user-id");
//    window.location.href = `/User/Registration/${Id}`;
//});

//$(document).on("submit", "#UserRegistrationForm", function (event) {

//    event.preventDefault();
//    var formElement = $(this)[0]; // Get the native DOM element
//    var formData = new FormData(formElement);
//    var userid = $("#UserId").val();

//    console.log(formData);
//    $.ajax({
//        url: "Registration",
//        method: "POST",
//        data: formData,
//        processData: false, 
//        contentType: false,
//        success: function (result) {
//            console.log(result)
//            if (result.success== true && userid == 0) {
//                window.location.href = `/Home/Index`;
//            }
//            else {
                
//                console.log(result);
//                toastr.success(result.message)

//                var activeTab = $(".nav-tabs .active").attr("id");

//                if (activeTab === "tab-admin") {
//                    LoadUserList("Admin", search, page, pagesize, orderByDepartment);
//                } else if (activeTab === "tab-employee") {
//                    LoadUserList("Employee", search, page, pagesize, orderByDepartment);
//                }
//            }
//            //else {
//            //    toastr.error(result.message)
//            //}
//        }
//    });

//})


//Page Load All Fuction call
$(document).ready(function () {



    //Get All User Here first load Admin Data at  page load time
    LoadUserList(role, search, page, pagesize, orderByDepartment);

    $("#UserListSearch").on("input", function () {
        search = $(this).val();
        page = 1;
        LoadUserList(role, search, page, pagesize, orderByDepartment);
    });

    // Tab switch
    $('#myTabs button[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        role = $(e.target).text().trim();
        page = 1;
        search = "";
        $("#UserListSearch").val("");

        LoadUserList(role, search, page, pagesize, orderByDepartment);
    });


    //sort by deparment name
    $(document).on("click", ".SortbyDeparment", function () {
        orderByDepartment = $(this).data('sortby');
        page = 1;
        LoadUserList(role, search, page, pagesize, orderByDepartment);
    });

});



function SelectPageSizeUserPage(role,TotalCount) {
 
    var value = parseInt($(`#usertableSizeSelect_${role}`).val());
    pagesize = value;
    let totalPagesOrder = Math.ceil(TotalCount / pagesize);
    if (page > totalPagesOrder) {
        page = totalPagesOrder > 0 ? totalPagesOrder : 1;
    }
    
    LoadUserList(role, search, page, pagesize, orderByDepartment)
}

function pageChangeUser(pageAction, TotalCount) {

    if (pageAction == 'prev') {
        page = page === 1 ? 1 : --page;
    }
    else {
        page = (page * pagesize) > TotalCount ? page : ++page;
    }
    LoadUserList(role, search, page, pagesize, orderByDepartment)
}

// Load user list using AJAX
function LoadUserList(role, search, page, pagesize, orderByDepartment) {
    const targetId = role === "Admin" ? "#UserListDivAdmin" : "#UserListDivEmployee";
    $(targetId).html('<div class="text-center py-3">Loading...</div>');

    $.ajax({
        url: "GetUserList",
        data: {
            Role: role,
            search: search,
            page: page,
            pageSize: pagesize,
            orderby: orderByDepartment
        },
        success: function (result) {
            $(targetId).html(result);
            
        },
        error: function () {
            $(targetId).html('<div class="text-danger">Error loading data.</div>');
        }
    });
}

function validateFileType() {
    var fileName = document.getElementById("fileName").value;
    var idxDot = fileName.lastIndexOf(".") + 1;
    var extFile = fileName.substr(idxDot, fileName.length).toLowerCase();
    if (extFile == "jpg" || extFile == "jpeg" || extFile == "png") {
        //TO DO
    } else {
        alert("Only jpg/jpeg and png files are allowed!");
    }
}













