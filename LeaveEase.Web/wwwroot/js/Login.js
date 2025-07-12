function togglePassword(inputId, icon) {
    const input = document.getElementById(inputId);
    if (!input) return;

    const isPassword = input.type === 'password';
    input.type = isPassword ? 'text' : 'password';

    icon.classList.toggle('bi-eye-fill', !isPassword);
    icon.classList.toggle('bi-eye-slash-fill', isPassword);
}


$('#forgetPasswordLink').click(function (e) {
   

    var email = $('input[name="Email"]').val().trim();

   
    var url = '/Login/ForgetPassword';
    if (email) {
        var encodedEmail = encodeURIComponent(email);
        url += '?email=' + encodedEmail;
    }
    window.location.href = url;
});
