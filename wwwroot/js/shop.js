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
    for (let btn of document.querySelectorAll("[data-cart-product-id]")) {
        btn.addEventListener('click', modifyCartQuantity);
    }
});

function modifyCartQuantity(e) {
    const btn = e.target.closest("[data-cart-product-id]");
    if (!btn) throw `modifyCartQuantity: closest("[data-cart-product-id]") not found`;
    const productId = btn.getAttribute("data-cart-product-id");
    const increment = btn.getAttribute("data-increment");
    fetch(`/api/cart/${productId}?increment=${increment}`, {
        method: 'PATCH'
    }).then(r => r.json()).then(j => {
        if (j.status.isOk) {
            window.location.reload();
        }
        else {
            alert(j.data);
        }
    });
}

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