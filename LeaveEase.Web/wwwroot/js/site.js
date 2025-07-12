

//Get ProfileData 
$(document).ready(function () {
    $.ajax({
        url: "/Home/ProfileData",
        type: "GET",
        success: function (response) {

            $("#profileimg").attr("src", response.profileImage);
            $("#Username").text(response.firstName)
        },
    });
})




// Toggle sidebar visibility
function toggleSidebar() {
    const sidebar = document.getElementById("sidebar");
    sidebar?.classList.toggle("active");
}

//Toggle password visibility with eye icon
function togglePassword(inputId, icon) {
    const input = document.getElementById(inputId);
    if (!input) return;

    const isPassword = input.type === 'password';
    input.type = isPassword ? 'text' : 'password';

    icon.classList.toggle('bi-eye-fill', !isPassword);
    icon.classList.toggle('bi-eye-slash-fill', isPassword);
}



// Live preview of profile image upload
function loadFile(event) {
    const output = document.getElementById('profile-image');
    if (!output || !event.target.files?.length) return;

    output.src = URL.createObjectURL(event.target.files[0]);
    output.onload = () => URL.revokeObjectURL(output.src);
}




//Logout Request GET Method
$(document).on("click", "#logoutbutton", function () {
    $.ajax({
        url: "/Home/LogoutRequest",
        method: "GET",
        success: function (result) {
            if (result.success == false) {
                toastr.error(result.message);
            }
            $("#LogoutDiv").html(result);
            $('#LogoutModel').modal('show');
        }
    });
})

// Confirm logout (POST method)
$(document).on("click", "#LogoutModelbutton", function () {
    $.ajax({
        url: "/Login/Logout",
        method: "POST",
        success: function () {
            $("#LogoutModel").modal('hide');
            location.reload();
        },
        error: function () {
            toastr.error("Logout failed.");
        }
    });
});