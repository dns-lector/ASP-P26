document.addEventListener('submit', e => {
    const form = e.target;
    if (form.id == "product-add-form") {
        e.preventDefault();
        handleAddProduct(form);
    }
    if (form.id == "group-add-form") {
        e.preventDefault();
        handleAddProduct(form);
    }
});

function handleAddGroup(form) {
    fetch(form.action, {
        method: 'POST',
        body: new FormData(form)
    }).then(r => r.json()).then(console.log).catch(console.error);
}

function handleAddProduct(form) {
    fetch(form.action, {
        method: 'POST',
        body: new FormData(form)
    }).then(r => r.json()).then(console.log).catch(console.error);
}