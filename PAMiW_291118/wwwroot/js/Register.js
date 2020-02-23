function showPassword(id) {
    var x = document.getElementById(id);
    spanId = id + "Span";
    if (x.type === "password") {
        x.type = "text";
        document.getElementById(spanId).setAttribute("src", "/images/openEyeSmall.png");
    } else {
        x.type = "password";
        document.getElementById(spanId).setAttribute("src", "/images/closedEyeSmall.png");
    }
}
var timeouts = {};
function onFunction(i) {
    clearTimeout(timeouts[i]);
    timeouts[i] = setTimeout(
        function () {
            switch (i) {
                case 1:
                    validateEmail();
                    break;
                case 2:
                    validateLogin(0);
                    getFromServerAsync();
                    break;
                case 3:
                    validatePassword();
                    break;
                case 4:
                    validateRepeatPassword();
                    break;
                default:
                    console.log('Sorry, we are out of ' + expr + '.');
            }
        }, 1500);
}
function validateEmail() {
    if (document.contains(document.getElementById("errorEmail"))) {
        document.getElementById("errorEmail").remove();
    } 
    var emailNode = document.getElementById("email");
    var email = emailNode.value;
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (!re.test(email)) {
        emailNode.insertAdjacentHTML('afterend', '<div id="errorEmail" class="errorText">Błędny adres email.</div>');
        return false;
    }
    else
        return true;
}
function validateLogin(i) {
    if (document.contains(document.getElementById("errorLogin"))) {
        document.getElementById("errorLogin").remove();
    } 
    var loginNode = document.getElementById("login");
    var login = loginNode.value;
    if (login.length < 3 || login.length > 12) {
        loginNode.insertAdjacentHTML('afterend', '<div id="errorLogin" class="errorText">Niepoprawna długość loginu. Login musi mieć więcej niż 2 oraz mniej niż 13 znaków.</div>');
        return false;
    }
    else {
        return true;
    }
}
var isPasswordGood = false;
function validatePassword() {
    if (document.contains(document.getElementById("errorPassword"))) {
        document.getElementById("errorPassword").remove();
    } 
    if (document.contains(document.getElementById("errorRepeatPassword"))) {
        document.getElementById("errorRepeatPassword").remove();
    }
    var passwordSpanNode = document.getElementById("passwordSpan");
    var password = document.getElementById("password").value;
    var repeatPassword = document.getElementById("repeatPassword").value;
    var repeatPasswordSpanNode = document.getElementById("repeatPasswordSpan");
    if (password.length < 8) {
        passwordSpanNode.insertAdjacentHTML('afterend', '<div id="errorPassword" class="errorText">Hasło musi mieć więcej niż 8 znaków.</div>');
        isPasswordGood = false;
        return false;
    }
    else if (!hasNumber(password)) {
        passwordSpanNode.insertAdjacentHTML('afterend', '<div id="errorPassword" class="errorText">Hasło musi zawierać liczbę.</div>');
        isPasswordGood = false;
        return false;
    }
    else if (!hasLowerCase(password)) {
        passwordSpanNode.insertAdjacentHTML('afterend', '<div id="errorPassword" class="errorText">Brak małych liter.</div>');
        isPasswordGood = false;
        return false;
    }
    else if (!hasUpperCase(password)) {
        passwordSpanNode.insertAdjacentHTML('afterend', '<div id="errorPassword" class="errorText">Brak wielkich liter.</div>');
        isPasswordGood = false;
        return false;
    }
    else if (password !== repeatPassword && repeatPassword.length !== 0) {
        isPasswordGood = true;
        if (document.contains(document.getElementById("errorRepeatPassword"))) {
            document.getElementById("errorRepeatPassword").remove();
        }
        repeatPasswordSpanNode.insertAdjacentHTML('afterend', '<div id="errorRepeatPassword" class="errorText">Hasła różnią się.</div>');
    }
    else {
        isPasswordGood = true;
        return true;
    }

}
function validateRepeatPassword() {
    if (document.contains(document.getElementById("errorRepeatPassword"))) {
        document.getElementById("errorRepeatPassword").remove();
    }
    var repeatPasswordSpanNode = document.getElementById("repeatPasswordSpan");
    var password = document.getElementById("password").value;
    var repeatPassword = document.getElementById("repeatPassword").value;
    if (!isPasswordGood) {
        repeatPasswordSpanNode.insertAdjacentHTML('afterend', '<div id="errorRepeatPassword" class="errorText">Hasło nieprawidłowe. Popraw najpierw hasło.</div>');
        return false;
    }
    else if (repeatPassword !== password || repeatPassword === "") {
        repeatPasswordSpanNode.insertAdjacentHTML('afterend', '<div id="errorRepeatPassword" class="errorText">Hasła różnią się.</div>');
        return false;
    }
    else
        return true;
}
function validatePhoto(i) {
    if (document.contains(document.getElementById("errorPhoto"))) {
        document.getElementById("errorPhoto").remove();
    }
    var photoNode = document.getElementById("photo");
    var photoImgNode = document.getElementById("menu_image");
    var photo = photoNode.value;
    if (photo === "") {
        photoImgNode.insertAdjacentHTML('afterend', '<div id="errorPhoto" class="errorText">Brak dodanego zdjęcia.</div>');
        return false;
    }
    else {
        var reader = new FileReader();
        if (photoNode.files[0].type.indexOf("image") === -1) {
            photoImgNode.insertAdjacentHTML('afterend', '<div id="errorPhoto" class="errorText">Zły typ pliku.</div>');
            var x = document.getElementById("menu_image");
            x.setAttribute("src", "");
            x.style.display = "none";

            return false;
        }
        else if (photoNode.files[0].size > 1048576) {
            photoImgNode.insertAdjacentHTML('afterend', '<div id="errorPhoto" class="errorText">Rozmiar zdjęcia przekracza 1MB.</div>');
            var y = document.getElementById("menu_image");
            y.setAttribute("src", "");
            y.style.display = "none";
            return false;
        }
        else {
            if (i === 1) {
                reader.onload = function (e) {
                    var z = document.getElementById("menu_image");
                    z.src = e.target.result;
                    z.style.display = "";
                };
                reader.readAsDataURL(photoNode.files[0]);
                return true;
            }
            else
                return true;
        }
    }
}
function validatePDF() {
    var pdfNode = document.getElementById("pdfInput");
    var pdf = pdfNode.value;
    if (pdf === "") {
        document.getElementById("errorPDF").innerHTML = "Brak dodanego pliku PDF.";
        return false;
    }
    else {
        var reader = new FileReader();
        if (pdfNode.files[0].type.indexOf("pdf") === -1) {
            document.getElementById("errorPDF").innerHTML = "Zły typ pliku";
            return false;
        }
        else if (pdfNode.files[0].size > 10485760) {
            document.getElementById("errorPDF").innerHTML = "Rozmiar PDF przekracza 10MB";
            return false;
        }
        else {
            document.getElementById("errorPDF").innerHTML = "";
            return true;
        }
    }
}
function validateFile() {
    var pdfNode = document.getElementById("pdfInput");
    var pdf = pdfNode.value;
    if (pdf === "") {
        document.getElementById("errorPDF").innerHTML = "Brak dodanego pliku PDF.";
        $("#submit").attr("disabled", true);
        return false;
    }
    else {
        var reader = new FileReader();
        if (pdfNode.files[0].type.indexOf("pdf") === -1) {
            document.getElementById("errorPDF").innerHTML = "Zły typ pliku";
            $("#submit").attr("disabled", true);
            return false;
        }
        else if (pdfNode.files[0].size > 10485760) {
            document.getElementById("errorPDF").innerHTML = "Rozmiar PDF przekracza 10MB";
            $("#submit").attr("disabled", true);
            return false;
        }
        else {
            document.getElementById("errorPDF").innerHTML = "";
            $("#submit").attr("disabled", false);
            return true;
        }
    }
} 
function validateForm() {
    var button = document.getElementById("submit");
    button.disabled = true;
    var errors = 0;
    if (!validateEmail())
        errors += 1;
    if (!validateLogin(1))
        errors += 1;
    if (!validatePassword())
        errors += 1;
    if (!validateRepeatPassword())
        errors += 1;
    if (errors !== 0) {
        button.disabled = false;
        return false;
    }
}
function validatePDFForm() {
    var button = document.getElementById("submit");
    button.disabled = true;
    var errors = 0;
    if (!validatePDF())
        errors += 1;
    if (errors !== 0) {
        button.disabled = false;
        return false;
    }
}
function getFromServerAsync() {
    var loginNode = document.getElementById("login");
    var login = loginNode.value;
    if (login.length === 0)
        return false;
    var url = "https://pi.iem.pw.edu.pl/user/" + login;
    var req = new XMLHttpRequest();
    req.open('GET', url, true);
    req.onreadystatechange = function (aEvt) {
        if (req.readyState === 4) {
            if (req.status === 200) {
                loginNode.insertAdjacentHTML('afterend', '<div id="errorLogin" class="errorText">Istnieje już użytkownik o podanym loginie.</div>');
                return false;
            }
            else
                return true;
        }
    };
    req.send(null); 
}
function getFromServerSync(url) {
    var req = new XMLHttpRequest();
    req.open('GET', url, false);
    req.send(null);
    if (req.status === 200) {
        return false;
    }
    else
        return true;
}
function hasLowerCase(str) {
    return (/[a-z]/.test(str));
}
function hasOnlyLowerCase(str) {
    return (/^[a-z]+$/ .test(str));
}
function hasUpperCase(str) {
    return (/[A-Z]/.test(str));
}
function hasNumber(str) {
    return /\d/.test(str);
}
