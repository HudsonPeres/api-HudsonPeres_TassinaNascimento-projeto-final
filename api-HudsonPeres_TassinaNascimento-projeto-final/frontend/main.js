// Configuração da API (ajuste se a porta for diferente)
const API_BASE = "http://localhost:5000/api";
let authToken = null;

// Elementos DOM
const loginSection = document.getElementById("loginSection");
const mainContent = document.getElementById("mainContent");
const loginBtn = document.getElementById("loginBtn");
const logoutBtn = document.getElementById("logoutBtn");
const productsGrid = document.getElementById("productsGrid");
const newProductBtn = document.getElementById("newProductBtn");
const inventoryBtn = document.getElementById("inventoryBtn");
const modal = document.getElementById("productModal");
const modalTitle = document.getElementById("modalTitle");
const prodId = document.getElementById("prodId");
const prodName = document.getElementById("prodName");
const prodPrice = document.getElementById("prodPrice");
const prodStock = document.getElementById("prodStock");
const modalSave = document.getElementById("modalSave");
const modalCancel = document.getElementById("modalCancel");

// Helper para chamadas autenticadas
async function apiFetch(endpoint, options = {}) {
  const headers = {
    "Content-Type": "application/json",
    Authorization: `Bearer ${authToken}`,
    ...options.headers,
  };
  const response = await fetch(`${API_BASE}${endpoint}`, {
    ...options,
    headers,
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Erro ${response.status}: ${errorText}`);
  }
  if (response.status === 204) return null;
  return await response.json();
}

// Login
async function login(username, password) {
  const response = await fetch(`${API_BASE}/Auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
  });
  if (!response.ok) {
    throw new Error("Credenciais inválidas");
  }
  const data = await response.json();
  return data.token;
}

// Produtos: listar
async function loadProducts() {
  try {
    const products = await apiFetch("/Products");
    if (!Array.isArray(products)) throw new Error("Resposta inválida");
    renderProducts(products);
  } catch (err) {
    productsGrid.innerHTML = `<div style="color:red;">Erro ao carregar produtos: ${err.message}</div>`;
  }
}

function renderProducts(products) {
  if (products.length === 0) {
    productsGrid.innerHTML =
      '<div style="grid-column:1/-1; text-align:center;">Nenhum produto cadastrado. Clique em "Novo Produto".</div>';
    return;
  }
  productsGrid.innerHTML = products
    .map(
      (p) => `
        <div class="product-card">
            <div class="product-name">${escapeHtml(p.name)}</div>
            <div class="product-price">€ ${parseFloat(p.price).toFixed(2)}</div>
            <div class="product-stock">📦 Stock: ${p.stock}</div>
            <div class="card-actions">
                <button class="btn-secondary" onclick="editProduct(${p.id}, '${escapeHtml(p.name)}', ${p.price}, ${p.stock})">✏️ Editar</button>
                <button class="btn-danger" onclick="deleteProduct(${p.id})">🗑️ Excluir</button>
            </div>
        </div>
    `,
    )
    .join("");
}

// Criar/Editar produto
async function saveProduct() {
  const id = prodId.value;
  const product = {
    name: prodName.value,
    price: parseFloat(prodPrice.value),
    stock: parseInt(prodStock.value),
  };
  try {
    if (id) {
      // Update
      await apiFetch(`/Products/${id}`, {
        method: "PUT",
        body: JSON.stringify({ ...product, id: parseInt(id) }),
      });
    } else {
      // Create
      await apiFetch("/Products", {
        method: "POST",
        body: JSON.stringify(product),
      });
    }
    closeModal();
    await loadProducts();
  } catch (err) {
    alert(`Erro ao salvar: ${err.message}`);
  }
}

window.deleteProduct = async function (id) {
  if (!confirm("Tem certeza que deseja excluir este produto?")) return;
  try {
    await apiFetch(`/Products/${id}`, { method: "DELETE" });
    await loadProducts();
  } catch (err) {
    alert(`Erro ao excluir: ${err.message}`);
  }
};

window.editProduct = function (id, name, price, stock) {
  prodId.value = id;
  prodName.value = name;
  prodPrice.value = price;
  prodStock.value = stock;
  modalTitle.innerText = "Editar Produto";
  modal.style.display = "flex";
};

function newProduct() {
  prodId.value = "";
  prodName.value = "";
  prodPrice.value = "";
  prodStock.value = "";
  modalTitle.innerText = "Novo Produto";
  modal.style.display = "flex";
}

function closeModal() {
  modal.style.display = "none";
}

// Consultar inventário (mock)
async function checkInventory() {
  const sku = prompt("Digite o SKU do produto (ex: 123):", "123");
  if (!sku) return;
  try {
    const data = await apiFetch(`/Inventory/${sku}`);
    alert(
      `📊 Inventário para SKU ${sku}:\nQuantidade disponível: ${data.quantity}\nSKU: ${data.sku}`,
    );
  } catch (err) {
    alert(`Erro ao consultar inventário: ${err.message}`);
  }
}

// Logout
function logout() {
  authToken = null;
  loginSection.style.display = "block";
  mainContent.style.display = "none";
  localStorage.removeItem("token");
  document.getElementById("loginError").innerText = "";
}

// Inicialização após login
async function initAfterLogin(token) {
  authToken = token;
  localStorage.setItem("token", token);
  loginSection.style.display = "none";
  mainContent.style.display = "block";
  await loadProducts();
}

// Eventos
loginBtn.onclick = async () => {
  const username = document.getElementById("username").value;
  const password = document.getElementById("password").value;
  const errorDiv = document.getElementById("loginError");
  errorDiv.innerText = "";
  try {
    const token = await login(username, password);
    await initAfterLogin(token);
  } catch (err) {
    errorDiv.innerText = err.message;
  }
};

logoutBtn.onclick = logout;
newProductBtn.onclick = newProduct;
inventoryBtn.onclick = (e) => {
  e.preventDefault();
  checkInventory();
};
modalSave.onclick = saveProduct;
modalCancel.onclick = closeModal;
window.onclick = (e) => {
  if (e.target === modal) closeModal();
};

// Verificar token salvo
const savedToken = localStorage.getItem("token");
if (savedToken) {
  initAfterLogin(savedToken).catch(() => logout());
}

function escapeHtml(str) {
  if (!str) return "";
  return str.replace(/[&<>]/g, function (m) {
    if (m === "&") return "&amp;";
    if (m === "<") return "&lt;";
    if (m === ">") return "&gt;";
    return m;
  });
}
