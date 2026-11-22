// VELOUR DESIGN — JM-Inspired Storefront & AI Stylist State Management

let allProducts = [];
let cart = [];
let currentCategory = 'All';
let isChatOpen = false;
let isChatExpanded = false;
let toastTimer = null;

const SVG_HEART_OUTLINE = `<svg viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"><path d="M12 20.5C12 20.5 4 15.2 4 9.5C4 6.8 6.1 4.8 8.6 4.8C10.2 4.8 11.4 5.7 12 6.8C12.6 5.7 13.8 4.8 15.4 4.8C17.9 4.8 20 6.8 20 9.5C20 15.2 12 20.5 12 20.5Z"/></svg>`;
const SVG_HEART_FILLED = `<svg viewBox="0 0 24 24" width="16" height="16" fill="currentColor" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"><path d="M12 20.5C12 20.5 4 15.2 4 9.5C4 6.8 6.1 4.8 8.6 4.8C10.2 4.8 11.4 5.7 12 6.8C12.6 5.7 13.8 4.8 15.4 4.8C17.9 4.8 20 6.8 20 9.5C20 15.2 12 20.5 12 20.5Z"/></svg>`;
const SVG_PLUS = `<svg viewBox="0 0 24 24" width="14" height="14" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round"><path d="M12 5V19M5 12H19"/></svg>`;
const SVG_SPARK = `<svg viewBox="0 0 24 24" width="14" height="14" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M12 3L13.8 8.2L19 10L13.8 11.8L12 17L10.2 11.8L5 10L10.2 8.2L12 3Z"/></svg>`;

document.addEventListener('DOMContentLoaded', async () => {
    await loadProducts();
    initEventListeners();
    updateCartUI();
    updateWishlistBadge();
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
    const searchInput = document.getElementById('navSearch');
    if (searchInput) {
        searchInput.addEventListener('input', (e) => {
            renderFilteredProducts(e.target.value.trim().toLowerCase());
        });
        searchInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
                triggerSearch();
            }
        });
    }

    const chatInput = document.getElementById('chatInput');
    if (chatInput) {
        chatInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
                sendChatMessage();
            }
        });
    }

    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') closeProductModal();
    });
}

function triggerSearch() {
    const kw = (document.getElementById('navSearch')?.value || '').trim().toLowerCase();
    renderFilteredProducts(kw);
    scrollToProducts();
}

function showToast(msg) {
    const el = document.getElementById('velourToast');
    if (!el) return;
    el.textContent = msg;
    el.classList.add('show');
    clearTimeout(toastTimer);
    toastTimer = setTimeout(() => el.classList.remove('show'), 2400);
}

function scrollToProducts() {
    const sec = document.getElementById('productsSection');
    if (sec) sec.scrollIntoView({ behavior: 'smooth' });
}

// ══════════════ FILTERING & SEARCH ══════════════
function filterCategory(category) {
    currentCategory = category;

    document.querySelectorAll('.cat-link').forEach(el => {
        el.classList.toggle('active', el.dataset.cat === category);
    });

    document.querySelectorAll('.circle-cat-item').forEach(el => {
        el.classList.toggle('active', el.dataset.cat === category);
    });

    const kw = (document.getElementById('navSearch')?.value || '').trim().toLowerCase();
    renderFilteredProducts(kw);

    const title = document.getElementById('productSectionTitle');
    if (title) {
        title.innerText = (category === 'All') ? 'SẢN PHẨM BÁN CHẠY' : category.toUpperCase();
    }

    if (category !== 'All') {
        scrollToProducts();
    }
}

function renderFilteredProducts(keyword = '') {
    const grid = document.getElementById('productsGrid');
    if (!grid) return;

    let filtered = allProducts;

    if (currentCategory !== 'All') {
        if (currentCategory === 'BST Mới') {
            filtered = filtered.slice(0, 4);
        } else if (currentCategory === 'Ưu đãi') {
            filtered = filtered.filter(p => p.price < 350000);
        } else if (currentCategory === 'Yêu thích') {
            filtered = filtered.filter(p => p.isWishlisted);
        } else {
            filtered = filtered.filter(p => p.category.toLowerCase() === currentCategory.toLowerCase());
        }
    }

    if (keyword) {
        filtered = filtered.filter(p =>
            p.name.toLowerCase().includes(keyword) ||
            p.category.toLowerCase().includes(keyword) ||
            p.description.toLowerCase().includes(keyword)
        );
    }

    const countEl = document.getElementById('productCountLabel');
    if (countEl) countEl.textContent = `${filtered.length} thiết kế`;

    if (filtered.length === 0) {
        grid.innerHTML = `<div style="grid-column: 1/-1; text-align: center; padding: 48px 16px; color: var(--ink-muted);">
            Không tìm thấy sản phẩm phù hợp${keyword ? ` với từ khóa "${keyword}"` : ''}.
        </div>`;
        return;
    }

    grid.innerHTML = filtered.map(p => {
        const badgeText = p.price < 350000 ? '-15%' : (p.id <= 4 ? '-5%' : 'MỚI');
        const badgeClass = badgeText === 'MỚI' ? 'jm-badge badge-new' : 'jm-badge badge-sale';

        return `
        <article class="product-card">
            <div class="product-thumb" onclick="showProductModal(${p.id})">
                <img src="${p.imageUrl}" alt="${p.name}" loading="lazy" />
                <span class="${badgeClass}">${badgeText}</span>
                <div class="product-quick-bar" onclick="event.stopPropagation()">
                    <button class="btn-quick-cart" id="btn-cart-${p.id}" onclick="addToCart(${p.id})">
                        ${SVG_PLUS}
                        <span>Thêm giỏ</span>
                    </button>
                    <button class="btn-quick-ai" onclick="askAiAboutProduct(${p.id})" title="Hỏi AI cách phối mẫu này">
                        ${SVG_SPARK}
                        <span>Phối đồ AI</span>
                    </button>
                </div>
            </div>
            <div class="product-info">
                <div class="product-title-row">
                    <h3 class="product-name" onclick="showProductModal(${p.id})" title="${p.name}">${p.name}</h3>
                    <button class="wish-inline-btn ${p.isWishlisted ? 'active' : ''}" onclick="toggleWishlist(${p.id})" aria-label="Yêu thích">
                        ${p.isWishlisted ? SVG_HEART_FILLED : SVG_HEART_OUTLINE}
                    </button>
                </div>
                <div class="product-meta-row">
                    <span class="product-price">${p.formattedPrice}</span>
                    <span class="product-cat-pill">${p.category}</span>
                </div>
            </div>
        </article>`;
    }).join('');
}

// ══════════════ PRODUCT DETAIL MODAL ══════════════
function showProductModal(productId) {
    const p = allProducts.find(x => x.id === productId);
    if (!p) return;

    document.getElementById('pmImg').src = p.imageUrl;
    document.getElementById('pmImg').alt = p.name;
    document.getElementById('pmCat').textContent = p.category;
    document.getElementById('pmName').textContent = p.name;
    document.getElementById('pmPrice').textContent = p.formattedPrice;
    document.getElementById('pmDesc').textContent = p.description;

    document.getElementById('pmAddCartBtn').onclick = () => {
        addToCart(p.id);
        closeProductModal();
    };
    document.getElementById('pmAskAiBtn').onclick = () => {
        closeProductModal();
        askAiAboutProduct(p.id);
    };

    document.getElementById('productModalBackdrop').classList.add('open');
    document.getElementById('productModal').classList.add('open');
}

function closeProductModal() {
    const bd = document.getElementById('productModalBackdrop');
    const md = document.getElementById('productModal');
    if (bd) bd.classList.remove('open');
    if (md) md.classList.remove('open');
}

function selectSize(btn) {
    document.querySelectorAll('.size-pill').forEach(el => el.classList.remove('active'));
    btn.classList.add('active');
}

// ══════════════ WISHLIST & PROMO ══════════════
function updateWishlistBadge() {
    const el = document.getElementById('wishBadge');
    if (!el) return;
    el.textContent = allProducts.filter(p => p.isWishlisted).length;
}

function toggleWishlist(productId) {
    const prod = allProducts.find(p => p.id === productId);
    if (!prod) return;
    prod.isWishlisted = !prod.isWishlisted;
    updateWishlistBadge();
    const kw = (document.getElementById('navSearch')?.value || '').trim().toLowerCase();
    renderFilteredProducts(kw);
    showToast(prod.isWishlisted ? `Đã lưu "${prod.name}" vào yêu thích.` : `Đã bỏ yêu thích "${prod.name}".`);
}

function copyPromoCode() {
    navigator.clipboard?.writeText('VELOUR15').then(() => {
        showToast("Đã sao chép mã ưu đãi VELOUR15 (giảm 15%).");
    }).catch(() => {
        showToast("Mã ưu đãi của bạn: VELOUR15");
    });
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
    showToast(`Đã thêm "${prod.name}" vào giỏ hàng.`);
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

function removeCartItem(productId) {
    cart = cart.filter(c => c.product.id !== productId);
    updateCartUI();
}

function updateCartUI() {
    const badge = document.getElementById('cartBadge');
    const list = document.getElementById('cartItemsList');
    const totalEl = document.getElementById('cartTotalDisplay');

    const totalCount = cart.reduce((sum, item) => sum + item.quantity, 0);
    const totalPrice = cart.reduce((sum, item) => sum + item.product.price * item.quantity, 0);

    if (badge) badge.textContent = totalCount;
    if (totalEl) totalEl.textContent = totalPrice.toLocaleString('vi-VN') + ' ₫';

    if (!list) return;
    if (cart.length === 0) {
        list.innerHTML = `<div class="cart-empty">Giỏ hàng của bạn hiện đang trống.<br>Hãy khám phá bộ sưu tập mới của VELOUR DESIGN.</div>`;
        return;
    }

    list.innerHTML = cart.map(item => `
        <div class="cart-item-row">
            <img src="${item.product.imageUrl}" alt="${item.product.name}" />
            <div class="cart-item-meta">
                <div>
                    <div class="cart-item-name">${item.product.name}</div>
                    <div class="cart-item-price">${item.product.formattedPrice}</div>
                </div>
                <div class="cart-qty-ctrl">
                    <div class="qty-box">
                        <button class="qty-btn" onclick="updateCartQty(${item.product.id}, -1)">-</button>
                        <span class="qty-val">${item.quantity}</span>
                        <button class="qty-btn" onclick="updateCartQty(${item.product.id}, 1)">+</button>
                    </div>
                    <button class="btn-remove-item" onclick="removeCartItem(${item.product.id})">Xóa</button>
                </div>
            </div>
        </div>
    `).join('');
}

function checkoutOrder() {
    if (cart.length === 0) {
        showToast("Giỏ hàng của bạn đang trống.");
        return;
    }
    const totalPrice = cart.reduce((sum, item) => sum + item.product.price * item.quantity, 0);
    showToast(`Đặt hàng thành công (${totalPrice.toLocaleString('vi-VN')} ₫). Cảm ơn bạn!`);
    cart = [];
    updateCartUI();
    toggleCartDrawer();
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
    const input = document.getElementById('chatInput');
    if (input) input.focus();
    scrollChatToBottom();
}

function toggleExpandChat() {
    isChatExpanded = !isChatExpanded;
    const popup = document.getElementById('chatPopup');
    if (!popup) return;

    popup.classList.toggle('expanded', isChatExpanded);
    scrollChatToBottom();
}

async function resetChatHistory() {
    try {
        await fetch('/api/chat/reset', { method: 'POST' });
    } catch { }
    const container = document.getElementById('chatMessages');
    if (container) {
        container.innerHTML = `
            <div class="msg-row bot">
                <div class="msg-bubble">
                    Đã làm mới cuộc trò chuyện. Bạn cần <strong>Velour AI Stylist</strong> tư vấn mẫu trang phục nào hôm nay?
                </div>
            </div>`;
    }
}

function askAiAboutProduct(productId) {
    const p = allProducts.find(x => x.id === productId);
    if (!p) return;

    openChat();
    const chatInput = document.getElementById('chatInput');
    if (chatInput) {
        chatInput.value = `Mẫu '${p.name}' (${p.category}, giá ${p.formattedPrice}) nên phối cùng trang phục nào và hợp mặc dịp gì?`;
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

    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 95000);

    try {
        const res = await fetch('/api/chat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ message }),
            signal: controller.signal
        });

        let data = {};
        try {
            data = await res.json();
        } catch {
            data = {};
        }

        if (typing) typing.style.display = 'none';

        if (res.ok && data.reply) {
            appendMessage('bot', data.reply);
        } else {
            appendMessage('bot', data.error || "Xin lỗi, AI tạm thời không phản hồi. Bạn vui lòng thử lại sau nhé.");
        }
    } catch (err) {
        if (typing) typing.style.display = 'none';
        if (err.name === 'AbortError') {
            appendMessage('bot', "AI đang phản hồi quá lâu (hết thời gian chờ). Bạn vui lòng thử gửi lại câu hỏi ngắn gọn hơn nhé!");
        } else {
            appendMessage('bot', "Không thể kết nối tới máy chủ web. Hãy đảm bảo ứng dụng đang chạy rồi thử lại.");
        }
    } finally {
        clearTimeout(timeoutId);
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

    const regex = /\[IMG:(\d+)\]/g;
    let match;
    let productCardsHtml = '';

    while ((match = regex.exec(text)) !== null) {
        const pid = parseInt(match[1]);
        const prod = allProducts.find(p => p.id === pid);
        if (prod) {
            productCardsHtml += `
                <div class="msg-card-product">
                    <img src="${prod.imageUrl}" alt="${prod.name}" onclick="showProductModal(${prod.id})" />
                    <div style="flex-grow: 1;">
                        <div style="font-weight: 600; font-size: 0.82rem;">${prod.name}</div>
                        <div style="color: var(--accent-dark); font-weight: 600; font-size: 0.78rem;">${prod.formattedPrice}</div>
                    </div>
                    <button class="msg-card-btn" onclick="addToCart(${prod.id})">+ Giỏ</button>
                </div>
            `;
        }
    }

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
