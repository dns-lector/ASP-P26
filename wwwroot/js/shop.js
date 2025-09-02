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

document.addEventListener('DOMContentLoaded', e => {
    for (let btn of document.querySelectorAll("[data-product-id]")) {
        btn.addEventListener('click', addToCartClick);
    }
});

function addToCartClick(e) {
    const btn = e.target.closest("[data-product-id]");
    if (!btn) throw `addToCartClick: closest("[data-product-id]") not found`;
    const productId = btn.getAttribute("data-product-id");
    fetch("/api/cart/" + productId, {
        method: 'POST'
    }).then(r => r.json()).then(console.log);
}

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