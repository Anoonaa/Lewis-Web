const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || ''

async function request(path, options = {}) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      'Content-Type': 'application/json',
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
