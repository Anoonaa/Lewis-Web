const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || ''

function getAuthToken() {
  return localStorage.getItem('ls_token') || ''
}

async function request(path, options = {}) {
  const token = getAuthToken()

  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers || {}),
    },
    ...options,
  })

  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}`)
  }

  if (response.status === 204) {
    return null
  }

  return response.json()
}

export function getProducts() {
  return request('/api/Products')
}

export function getCategories() {
  return request('/api/Categories')
}

export function login(email, password) {
  return request('/api/Auth/login', {
    method: 'POST',
    body: JSON.stringify({ email, password }),
  })
}

export function register(payload) {
  return request('/api/Auth/register', {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}

export function getCurrentUser() {
  return request('/api/Auth/me')
}

export function updateCurrentUser(payload) {
  return request('/api/Auth/me', {
    method: 'PUT',
    body: JSON.stringify(payload),
  })
}

export function getOrders() {
  return request('/api/Orders')
}

export function createOrder(payload) {
  return request('/api/Orders', {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}

export function getPaymentMethods() {
  return request('/api/PaymentMethods')
}

export function addPaymentMethod(payload) {
  return request('/api/PaymentMethods', {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}

export function removePaymentMethod(id) {
  return request(`/api/PaymentMethods/${id}`, {
    method: 'DELETE',
  })
}
