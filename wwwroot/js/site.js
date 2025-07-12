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

/* Д.З. Реєстрація: доповнити валідацію даних перевіркою 
всіх необхідних даних.
Автентифікація: забезпечити перевірку логіна/паролю на 
порожність, виводити invalid-feedback повідомлення 
відповідного змісту.
Реалізувати зміну статусів полів введення (валідний/невалідний/неперевірений)
при змінах їх статусів.
        */