class Base64 {
    static #textEncoder = new TextEncoder();
    static #textDecoder = new TextDecoder();

    // https://datatracker.ietf.org/doc/html/rfc4648#section-4
    encode = (str) => btoa(String.fromCharCode(...Base64.#textEncoder.encode(str)));
    decode = (str) => Base64.#textDecoder.decode(Uint8Array.from(atob(str), c => c.charCodeAt(0)));

    // https://datatracker.ietf.org/doc/html/rfc4648#section-5
    encodeUrl = (str) => this.encode(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    decodeUrl = (str) => this.decode(str.replace(/\-/g, '+').replace(/\_/g, '/'));

    jwtEncodeBody = (header, payload) => this.encodeUrl(JSON.stringify(header)) + '.' + this.encodeUrl(JSON.stringify(payload));
    jwtDecodePayload = (jwt) => JSON.parse(this.decodeUrl(jwt.split('.')[1]));
}

document.addEventListener('submit', e => {
    const form = e.target;
    if (form.id == 'sign-in-form') {
        e.preventDefault();
        const loginInput = form.querySelector('[name="user-login"]');
        if (!loginInput) {
            throw "'[name=user-login]' not found";
        }
        const passwordInput = form.querySelector('[name="user-password"]');
        if (!passwordInput) {
            throw "'[name=user-password]' not found";
        }
        if (loginInput.value.length == 0) {
            loginInput.classList.add('is-invalid');
            return;
        }
        // RFC 7617 
        const credentials = new Base64().encode(
            `${loginInput.value}:${passwordInput.value}`);

        fetch('/User/SignIn', {
            method: 'GET',
            headers: {
                'Authorization': `Basic ${credentials}`
            }
        }).then(r => r.json())
            .then(j => {
                if (j.status == 200) {
                    window.location.reload();
                }
                else {
                    alert("Rejected");
                }
            });
    }
});

document.addEventListener('DOMContentLoaded', () => {
    for (let btn of document.querySelectorAll('[data-nav]')) {
        btn.onclick = navigate;
    }
    const editProfileBtn = document.getElementById("edit-profile-btn");
    if (editProfileBtn) {
        editProfileBtn.addEventListener('click', editProfileBtnClick);
    }
    const deleteProfileBtn = document.getElementById("delete-profile-btn");
    if (deleteProfileBtn) {
        deleteProfileBtn.addEventListener('click', deleteProfileBtnClick);
    }
});

function deleteProfileBtnClick() {
    if (confirm(`Підтверджуєте видалення профілю?`)) {
        let login = prompt("Для підтвердження введіть свій логін:");
        console.log(login)
        if (!login || !login.trim()) {
            alert("Deletion cancelled.");
            return;
        }
        fetch("/User/Delete", {
            method: 'DELETE',
            headers: {
                'Authentication-Control': new Base64().encodeUrl(login)
            }
        }).then(r => r.json()).then(j => {
            console.log(j);
            if (j.status == 200) {
                alert("Ваш профіль видалено");
                window.location = '/';
            }
            else {
                alert("Ваш профіль НЕ видалено. Імовірно неправильне підтвердження");
            }
        });
    }
}

function editProfileBtnClick() {
    let changes = [];
    for (let elem of document.querySelectorAll('[data-editable]')) {
        if (elem.getAttribute('contenteditable')) {
            elem.removeAttribute('contenteditable');
            // console.log(elem.originalData, elem.innerText);
            if (elem.originalData != elem.innerText) {
                changes.push({
                    field: elem.getAttribute('data-editable'),
                    value: elem.innerText
                });
            }
        }
        else {
            elem.setAttribute('contenteditable', true);
            elem.originalData = elem.innerText;
        }
    }
    if (changes.length > 0) {
        const msg = changes.map(c => `${c.field}=${c.value}`).join(', ');
        if (confirm(`Підтверджуєте зміни: ${msg}`)) {
            fetch("/User/Update", {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                body: JSON.stringify(changes)
            }).then(r => r.json()).then(console.log);
        }
    }
}

function navigate(e) {
    const targetBtn = e.target.closest('[data-nav]');
    const route = targetBtn.getAttribute('data-nav');
    if (!route) throw "Attribute [data-nav] not found";
    for (let btn of document.querySelectorAll('[data-nav]')) {
        btn.classList.remove("active");
    }
    targetBtn.classList.add("active");
    showPage(route);
}

function showPage(page) {
    window.activePage = page;
    const spaContainer = document.getElementById("spa-container");
    if (!spaContainer) throw "spa-container not found";
    switch (page) {
        case 'home':    spaContainer.innerHTML = `<b>Home</b>`;    break;
        case 'privacy': spaContainer.innerHTML = `<b>privacy</b>`; break;
        case 'auth':    spaContainer.innerHTML = !!window.accessToken ? profileHtml : authHtml;    break;
        default:        spaContainer.innerHTML = `<b>404</b>`;
    }
}

const profileHtml = `<div>
<h3>Вітаємо у кабінеті</h3>
<button type="button" class="btn btn-success" onclick="emailClick()">Лист</button>
<button type="button" class="btn btn-warning" onclick="exitClick()">Вихід</button>
</div>`;

const authHtml = `<div>
    <div class="input-group mb-3">
        <span class="input-group-text" id="user-login-addon"><i class="bi bi-key"></i></span>
        <input name="user-login" type="text" class="form-control"
                placeholder="Логін" aria-label="Логін" aria-describedby="user-login-addon">
        <div class="invalid-feedback"></div>
    </div>
    <div class="input-group mb-3">
        <span class="input-group-text" id="user-password-addon"><i class="bi bi-lock"></i></span>
        <input name="user-password" type="password" class="form-control" placeholder="Пароль"
                aria-label="Пароль" aria-describedby="user-password-addon">
        <div class="invalid-feedback"></div>
    </div>
    <button type="button" class="btn btn-primary" onclick="authClick()">Вхід</button>
</div>`;

function emailClick() {
    fetch("/User/Email", {
        method: "POST",
        headers: {
            "Authorization": "Bearer " + window.accessToken + "1"  // window.accessToken.jti
        }
    }).then(r => r.json())
        .then(console.log);
}

function exitClick() {
    window.accessToken = null;
    showPage(window.activePage);
}

function authClick() {
    const login = document.querySelector('input[name="user-login"]').value;
    const password = document.querySelector('input[name="user-password"]').value;
    console.log(login, password);

    const credentials = new Base64().encode(`${login}:${password}`);
    fetch('/User/LogIn', {
        method: 'GET',
        headers: {
            'Authorization': `Basic ${credentials}`
        }
    }).then(r => r.json())
        .then(j => {
            if (j.status == 200) {
                window.accessToken = j.data;
                // console.log(window.accessToken);
                showPage(window.activePage);
            }
            else {
                alert("Rejected");
            }
        });
}
/*
Д.З. Реалізувати контроль за терміном придатності токена (Exp)
Автоматично виходити з автентифікованого режиму при добіганні
кінця терміну. Роздільна здатність - 1 секунда.
*/