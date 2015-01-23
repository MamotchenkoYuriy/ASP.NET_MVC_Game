function SetValue(btnSetValue) {
    var input = $("#input-value");
    if (input.val().length > 0) {
        $.ajax({
            method: "Post",
            url: "/Home/Set?value=" + input.val() + "&userName=" + sessionStorage.getItem("UserName"),
            data: "",
            success: function (data) {
                if (data["Status"] == 4) {
                    $("#status").text(data["Message"]);
                    alert(data["Message"]);
                }
                else if (data["Status"] == 2) {
                    $("#status").text(data["Message"]);
                    alert(data["Message"]);
                }
                else {
                    if (data["More"] == "No") {
                        alert("Введите число побольше :-)");
                    } else {
                        alert("Введите число поменьше :-)");
                    }
                }
                var htmlInnerText = "";
                for (var i = 0; i < data["History"].length; i++) {
                    htmlInnerText += "<li>" + data["History"][i] + "</li>";
                }
                $("#log").html(htmlInnerText);
            },
            error: function () {
                alert("Ошибка установки значения :-(((");
            }
        });
    } else { alert("Ну ты и гавнюк однако... ") }
}

function Update() {
    $.ajax({
        method: "Get",
        url: "/Home/Get",
        data: "",
        success: function (data) {
            if (data["Status"] == 3) {
                $("#status").text("Число отгаданно пользователем " + data["Name"] + "! Верное число было " + data["Value"] + ". Вы можете загадать число.");
            }
            else if (data["Status"] == 0) {
                $("#status").text("Игра пока что не начата. Вы можете начать ее загадав число :-)");
            }
            else {
                $("#status").text("Угадайте число :-)");
            }
            setTimeout("Update()", 1000);
        },
        error: function () {
            alert("Ошибка доступа к серверу :-(");
        }
    });
}

function Login(btnLogin, inputLogin) {
    var name = inputLogin.val();
    if (name.length <= 6) {
        alert("Имя должно быть более 6 символов в длину");
    } else {
        sessionStorage.setItem("UserName", inputLogin.val());
        document.location.reload();
    }
}

function Logout(btnLogin, inputLogin) {
    if (confirm("Вы действительно хотите высти с системы?")) {
        sessionStorage.removeItem("UserName");
        document.location.reload();
    }
}

$(document).ready(function () {
    var userName = sessionStorage.getItem("UserName");
    var btnLogin = $("#btn_login");
    var inputLogin = $("#input_name");
    var btnSetValue = $("#btn-set-value");
    if (userName != null) {
        inputLogin.hide();
        $("#NotAuthorize-block").hide();
        $("#currentUserName").text("Hello " + userName + "!");
        btnSetValue.click(function () {
            SetValue(btnSetValue);
        });
        btnLogin.val("Logout");
        btnLogin.click(function () {
            Logout(btnLogin, inputLogin);
        });
        Update();
        alert("Добро пожаловать в систему :-)");
    } else {
        $("#Authorize-block").hide();
        btnLogin.click(function () {
            Login(btnLogin, inputLogin);
        });
    }
});