$(document).ready(function() {

    $('#registerRedirect').click(function (e) {
        location.href = "/Account/Register";
    });

    $('#login').click(function (e) {
        $('#userNameError').html("");
        $('#userEmailError').html("");
        $('#userSummaryError').html("");

        var user = {
            UserName: $('#userName').val(),
            UserEmail: $('#userEmail').val()
        }

        if (user.UserName == "") {
            $('#userNameError').html("User name cann't be empty");
            return false;
        }

        if (user.UserEmail == "") {
            $('#userEmailError').html("User email cann't be empty");
            return false;
        }

        if (!validateEmail(user.UserEmail)) {
            $('#userEmailError').html("Please, enter valid email");
                return false;
            }

        $.ajax({
            url: "/Account/Login",
            method: "POST",
            data: JSON.stringify(user),
            contentType: "application/json",
            success: function (response) {
                if (response != null && response.success) {
                    location.href = "/Book/List";
                } else {
                    $('#userSummaryError').html("User does not exist in system. Please, register user");
                }
            }
        });
    });


    function validateEmail(email) {
        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        return emailReg.test(email);
    }

    $('#register').click(function (e) {
        $('#userSummaryRegistryError').html("");
        $('#userEmailRegistryError').html("");
        $('#userNameRegistryError').html("");

        var user = {
            UserName: $('#userRegister').val(),
            UserEmail: $('#emailRegister').val()
        }

        if (user.UserName == "") {
            $('#userNameRegistryError').html("User name cann't be empty");
            return false;
        }

        if (user.UserEmail == "") {
            $('#userEmailRegistryError').html("User email cann't be empty");
            return false;
        }

        if (!validateEmail(user.UserEmail)) {
            $('#userEmailRegistryError').html("Please, enter valid email");
            return false;
        }

        $.ajax({
            url: "/Account/Register",
            method: "POST",
            data: JSON.stringify(user),
            contentType: "application/json",
            success: function (response) {
                if (response != null && response.success) {
                    location.href = "/Book/List";
                } else {
                    $('#userSummaryRegistryError').html("user with such name or email exist in system. Please, enter another name and email");
                }
            }
        });
    });

    $('#logOff').click(function (e) {
        location.href = "/Account/Login";
    });
});