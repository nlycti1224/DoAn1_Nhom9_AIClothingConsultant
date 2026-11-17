// Velour E-Commerce & AI Chatbot State Management

let allProducts = [];
let cart = [];
let currentCategory = 'All';
let isChatOpen = false;
let isChatExpanded = false;

document.addEventListener('DOMContentLoaded', async () => {
    await loadProducts();
    initEventListeners();
    updateCartUI();
});

async function loadProducts() {
    try {
        const res = await fetch('/api/products');
        allProducts = await res.json();
    } catch (err) {
        console.error("Lỗi tải sản phẩm:", err);
    }
}

function initEventListeners() {
    // Search Live Filter
    const searchInput = document.getElementById('navSearch');
    if (searchInput) {
        searchInput.addEventListener('input', (e) => {
            renderFilteredProducts(e.target.value.trim().toLowerCase());
        });
    }

    // Chat Enter Key
    const chatInput = document.getElementById('chatInput');
    if (chatInput) {
        chatInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
                sendChatMessage();
            }
        });
    }
}

// ══════════════ FILTERING & SEARCH ══════════════
function filterCategory(category) {
    currentCategory = category;

    // Active Tab in Nav / Cards
    document.querySelectorAll('.cat-link').forEach(el => {
        el.classList.toggle('active', el.dataset.cat === category);
    });

    document.querySelectorAll('.category-card').forEach(el => {
        el.classList.toggle('active', el.dataset.cat === category);
    });

    const searchInput = document.getElementById('navSearch');
    const kw = searchInput ? searchInput.value.trim().toLowerCase() : '';
    renderFilteredProducts(kw);

    const title = document.getElementById('productSectionTitle');
    if (title) {
        title.innerText = (category === 'All') ? 'Tất cả sản phẩm' : `Sản phẩm: ${category}`;
    }
}

function renderFilteredProducts(keyword = '') {
    const grid = document.getElementById('productsGrid');
    if (!grid) return;

    let filtered = allProducts;

    if (currentCategory !== 'All') {
        if (currentCategory === 'New Arrivals') filtered = filtered.slice(0, 4);
        else if (currentCategory === 'Women') filtered = filtered.filter(p => p.category !== 'Men');
        else if (currentCategory === 'Men') filtered = filtered.filter(p => p.category === 'Men');
        else if (currentCategory === 'Sale ♥') filtered = filtered.filter(p => p.price < 350000);
        else filtered = filtered.filter(p => p.category.toLowerCase() === currentCategory.toLowerCase());
    }

    if (keyword) {
        filtered = filtered.filter(p =>
            p.name.toLowerCase().includes(keyword) ||
            p.category.toLowerCase().includes(keyword) ||
            p.description.toLowerCase().includes(keyword)
        );
    }

    if (filtered.length === 0) {
        grid.innerHTML = `<div style="grid-column: 1/-1; text-align: center; padding: 40px; color: var(--text-muted); font-style: italic;">
            Không tìm thấy sản phẩm nào phù hợp với từ khóa "${keyword}".
        </div>`;
        return;
    }

    grid.innerHTML = filtered.map(p => `
        <div class="product-card">
            <div class="product-thumb" onclick="showProductModal(${p.id})">
                <img src="${p.imageUrl}" alt="${p.name}" />
                <button class="wish-btn" onclick="event.stopPropagation(); toggleWishlist(${p.id})">
                    ${p.isWishlisted ? '❤️' : '🤍'}
                </button>
            </div>
            <div class="product-info">
                <div class="product-cat">${p.category}</div>
                <div class="product-name" onclick="showProductModal(${p.id})">${p.name}</div>
                <div class="product-price">${p.formattedPrice}</div>
                <div class="product-actions">
                    <button class="btn-add-cart" id="btn-cart-${p.id}" onclick="addToCart(${p.id})">+ Thêm giỏ</button>
                    <button class="btn-ask-ai" onclick="askAiAboutProduct(${p.id})">💬 Stylist</button>
                </div>
            </div>
        </div>
    `).join('');
}

// ══════════════ SHOPPING CART ══════════════
function toggleCartDrawer() {
    const drawer = document.getElementById('cartDrawer');
    const overlay = document.getElementById('cartOverlay');
    if (!drawer || !overlay) return;

    drawer.classList.toggle('open');
    overlay.classList.toggle('open');
}

function addToCart(productId) {
    const prod = allProducts.find(p => p.id === productId);
    if (!prod) return;

    const item = cart.find(c => c.product.id === productId);
    if (item) {
        item.quantity++;
    } else {
        cart.push({ product: prod, quantity: 1 });
    }

    updateCartUI();

    // Feedback effect on button
    const btn = document.getElementById(`btn-cart-${productId}`);
    if (btn) {
        const originalText = btn.innerText;
        btn.innerText = "✓ Đã thêm!";
        btn.style.background = "var(--accent)";
        btn.style.color = "#FFF";
        setTimeout(() => {
            btn.innerText = originalText;
            btn.style.background = "";
            btn.style.color = "";
        }, 1200);
    }
}

function updateCartQty(productId, delta) {
    const item = cart.find(c => c.product.id === productId);
    if (!item) return;

    item.quantity += delta;
    if (item.quantity <= 0) {
        cart = cart.filter(c => c.product.id !== productId);
    }
    updateCartUI();
}

function clearCart() {
    cart = [];
    updateCartUI();
}

function updateCartUI() {
    const totalItems = cart.reduce((sum, i) => sum + i.quantity, 0);
    const totalPrice = cart.reduce((sum, i) => sum + (i.quantity * i.product.price), 0);

    const badge = document.getElementById('cartBadge');
    if (badge) badge.innerText = totalItems;

    const totalEl = document.getElementById('cartTotalDisplay');
    if (totalEl) totalEl.innerText = totalPrice.toLocaleString('vi-VN') + ' ₫';

    const itemsContainer = document.getElementById('cartItemsList');
    if (!itemsContainer) return;

    if (cart.length === 0) {
        itemsContainer.innerHTML = `<div style="text-align: center; margin-top: 60px; color: var(--text-muted); font-style: italic;">
            Giỏ hàng của bạn đang trống.<br>Hãy chọn thêm sản phẩm xinh nhé! ✨
        </div>`;
        return;
    }

    itemsContainer.innerHTML = cart.map(item => `
        <div class="cart-item-row">
            <img class="cart-item-img" src="${item.product.imageUrl}" alt="${item.product.name}" />
            <div class="cart-item-info">
                <div class="cart-item-title">${item.product.name}</div>
                <div class="cart-item-price">${item.product.formattedPrice}</div>
            </div>
            <div class="cart-qty-ctrl">
                <button class="cart-qty-btn" onclick="updateCartQty(${item.product.id}, -1)">-</button>
                <span style="font-size: 0.9rem; font-weight: 600;">${item.quantity}</span>
                <button class="cart-qty-btn" onclick="updateCartQty(${item.product.id}, 1)">+</button>
            </div>
        </div>
    `).join('');
}

function checkoutOrder() {
    if (cart.length === 0) {
        alert("Giỏ hàng của bạn đang trống!");
        return;
    }

    const total = cart.reduce((sum, i) => sum + (i.quantity * i.product.price), 0);
    const confirmOrder = confirm(`Xác nhận đặt hàng thành công!\nTổng cộng: ${total.toLocaleString('vi-VN')} ₫\n\nBạn có muốn mở Stylist AI để nhận hướng dẫn phối đồ và bảo quản trang phục không?`);

    cart = [];
    updateCartUI();
    toggleCartDrawer();

    if (confirmOrder) {
        openChat();
        const chatInput = document.getElementById('chatInput');
        if (chatInput) {
            chatInput.value = "Tôi vừa đặt mua đơn hàng tại shop, nhờ bạn gửi lời cảm ơn và hướng dẫn cách giặt ủi giữ form chuẩn nhé!";
            sendChatMessage();
        }
    }
}

function toggleWishlist(productId) {
    const prod = allProducts.find(p => p.id === productId);
    if (!prod) return;

    prod.isWishlisted = !prod.isWishlisted;
    const searchInput = document.getElementById('navSearch');
    renderFilteredProducts(searchInput ? searchInput.value.trim().toLowerCase() : '');
}

function showProductModal(productId) {
    const p = allProducts.find(x => x.id === productId);
    if (!p) return;

    alert(`👗 ${p.name}\n🏷️ Phong cách: ${p.category}\n💰 Giá: ${p.formattedPrice}\n\n📝 Mô tả: ${p.description}\n\nMẹo: Bấm nút '💬 Stylist' để nhờ AI tư vấn cách phối đồ.`);
}

function copyPromoCode() {
    navigator.clipboard.writeText("VELOUR15").then(() => {
        alert("Đã sao chép mã giảm giá: VELOUR15! Giảm 15% khi mua hàng.");
    }).catch(() => {
        alert("Mã giảm giá: VELOUR15");
    });
}

// ══════════════ AI CHAT WIDGET ══════════════
function toggleChat() {
    isChatOpen = !isChatOpen;
    const popup = document.getElementById('chatPopup');
    if (!popup) return;

    popup.classList.toggle('open', isChatOpen);
    if (isChatOpen) {
        const input = document.getElementById('chatInput');
        if (input) input.focus();
        scrollChatToBottom();
    }
}

function openChat() {
    isChatOpen = true;
    const popup = document.getElementById('chatPopup');
    if (popup) popup.classList.add('open');
    scrollChatToBottom();
}

function toggleExpandChat() {
    isChatExpanded = !isChatExpanded;
    const popup = document.getElementById('chatPopup');
    const btn = document.getElementById('btnExpandChat');
    if (!popup || !btn) return;

    popup.classList.toggle('expanded', isChatExpanded);
    btn.innerText = isChatExpanded ? '⤡' : '⤢';
    scrollChatToBottom();
}

function askAiAboutProduct(productId) {
    const p = allProducts.find(x => x.id === productId);
    if (!p) return;

    openChat();
    const chatInput = document.getElementById('chatInput');
    if (chatInput) {
        chatInput.value = `Món đồ '${p.name}' (${p.category}, giá ${p.formattedPrice}) phối với trang phục gì và đi dịp nào thì hợp?`;
        chatInput.focus();
    }
}

async function sendChatMessage() {
    const input = document.getElementById('chatInput');
    if (!input) return;

    const message = input.value.trim();
    if (!message) return;

    appendMessage('user', message);
    input.value = '';
    input.disabled = true;

    const typing = document.getElementById('chatTyping');
    if (typing) typing.style.display = 'block';
    scrollChatToBottom();

    try {
        const res = await fetch('/api/chat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ message })
        });

        const data = await res.json();
        if (typing) typing.style.display = 'none';

        if (res.ok && data.reply) {
            appendMessage('bot', data.reply);
        } else {
            appendMessage('bot', data.error || "Xin lỗi, đã xảy ra lỗi khi kết nối với AI.");
        }
    } catch (err) {
        if (typing) typing.style.display = 'none';
        appendMessage('bot', `Lỗi kết nối: ${err.message}. Hãy đảm bảo Ollama đang chạy trên máy.`);
    } finally {
        input.disabled = false;
        input.focus();
        scrollChatToBottom();
    }
}

function appendMessage(sender, text) {
    const container = document.getElementById('chatMessages');
    if (!container) return;

    const row = document.createElement('div');
    row.className = `msg-row ${sender}`;

    // Look for [IMG:id] tags to render rich product preview cards
    const regex = /\[IMG:(\d+)\]/g;
    let match;
    let productCardsHtml = '';

    while ((match = regex.exec(text)) !== null) {
        const pid = parseInt(match[1]);
        const prod = allProducts.find(p => p.id === pid);
        if (prod) {
            productCardsHtml += `
                <div class="msg-card-product">
                    <img src="${prod.imageUrl}" alt="${prod.name}" />
                    <div style="flex-grow: 1;">
                        <div style="font-weight: 600; font-size: 0.85rem;">${prod.name}</div>
                        <div style="color: var(--accent); font-weight: 700; font-size: 0.8rem;">${prod.formattedPrice}</div>
                    </div>
                    <button class="btn-add-cart" style="padding: 4px 8px; font-size: 0.75rem;" onclick="addToCart(${prod.id})">+ Giỏ</button>
                </div>
            `;
        }
    }

    // Clean tag from display text
    const cleanText = text.replace(regex, '').trim();

    row.innerHTML = `
        <div class="msg-bubble">${cleanText.replace(/\n/g, '<br>')}</div>
        ${productCardsHtml}
    `;

    container.appendChild(row);
    scrollChatToBottom();
}

function scrollChatToBottom() {
    const container = document.getElementById('chatMessages');
    if (container) {
        container.scrollTop = container.scrollHeight;
    }
}
