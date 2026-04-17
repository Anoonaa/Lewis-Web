import { useRef, useState } from 'react'
import { Link, NavLink, useLocation, useNavigate } from 'react-router-dom'
import { formatCurrency } from '../data/mockData'
import { useShop } from '../context/ShopContext'

export function Button({ children, to, variant = 'primary', className = '', type = 'button', ...props }) {
  const classes = `btn btn-${variant} ${className}`.trim()

  if (to) {
    return (
      <Link to={to} className={classes} {...props}>
        {children}
      </Link>
    )
  }

  return (
    <button type={type} className={classes} {...props}>
      {children}
    </button>
  )
}

export function Card({ title, children, className = '', style = {} }) {
  return (
    <article className={`card ${className}`.trim()} style={style}>
      {title && <h3 style={{ marginBottom: '1.25rem' }}>{title}</h3>}
      {children}
    </article>
  )
}

export function Badge({ children, tone = 'subtle' }) {
  const styleMap = {
    success: { background: '#dcfce7', color: '#166534' },
    info:    { background: '#dbeafe', color: '#1d4ed8' },
    warning: { background: '#fef9c3', color: '#854d0e' },
    accent:  { background: 'var(--secondary)', color: '#fff' },
    subtle:  { background: 'var(--surface-high)', color: 'var(--on-surface)' },
  }
  const s = styleMap[tone] || styleMap.subtle
  return (
    <span style={{
      display: 'inline-flex',
      alignItems: 'center',
      padding: '0.25rem 0.65rem',
      borderRadius: '4px',
      fontSize: '0.75rem',
      fontWeight: 600,
      textTransform: 'uppercase',
      letterSpacing: '0.04em',
      ...s,
    }}>
      {children}
    </span>
  )
}

export function TopNav({ links }) {
  const { cartCount, searchQuery, setSearchQuery, isAuthenticated, logoutUser } = useShop()
  const [searchOpen, setSearchOpen] = useState(false)
  const inputRef = useRef(null)
  const navigate = useNavigate()
  const location = useLocation()

  const openSearch = () => {
    setSearchOpen(true)
    requestAnimationFrame(() => inputRef.current?.focus())
  }

  const handleBlur = () => {
    if (!searchQuery) {
      setSearchOpen(false)
    }
  }

  const handleKeyDown = (e) => {
    if (e.key === 'Escape') {
      setSearchQuery('')
      setSearchOpen(false)
    }
  }

  const handleSearchChange = (e) => {
    const value = e.target.value
    setSearchQuery(value)

    if (value.trim() && location.pathname !== '/products') {
      navigate('/products')
    }
  }

  return (
    <header className="top-nav">
      {/* Lewis Logo */}
      <Link to="/" className="brand-mark" aria-label="Lewis Stores Home">
        <img src="/lewis-logo.png" alt="Lewis Stores" style={{ height: '44px', display: 'block' }} />
      </Link>

      <nav aria-label="Primary">
        {links.map((link) => (
          <NavLink
            key={link.to}
            to={link.to}
            className={({ isActive }) => (isActive ? 'nav-link active' : 'nav-link')}
          >
            {link.label}
          </NavLink>
        ))}
      </nav>

      <div className="nav-actions">
        {searchOpen ? (
          <div className="search-box search-box-open">
            <input
              ref={inputRef}
              type="text"
              className="search-input"
              aria-label="Search products"
              value={searchQuery}
              onChange={handleSearchChange}
              onBlur={handleBlur}
              onKeyDown={handleKeyDown}
            />
          </div>
        ) : (
          <button
            className="icon-btn"
            aria-label="Search products"
            onClick={openSearch}
          >
            <svg
              xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24"
              fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"
              aria-hidden="true" focusable="false"
            >
              <circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line>
            </svg>
          </button>
        )}

        <Link to="/cart" className="icon-btn" aria-label="Open cart" style={{ position: 'relative' }}>
          <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <circle cx="9" cy="21" r="1"></circle><circle cx="20" cy="21" r="1"></circle>
            <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
          </svg>
          {cartCount > 0 && <span className="cart-badge">{cartCount}</span>}
        </Link>

        <Link to="/profile" className="icon-btn" aria-label="User profile">
          <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
            <circle cx="12" cy="7" r="4"></circle>
          </svg>
        </Link>

        {isAuthenticated ? (
          <button type="button" className="btn btn-secondary" style={{ padding: '0.45rem 0.85rem', fontSize: '0.82rem' }} onClick={logoutUser}>
            Log Out
          </button>
        ) : (
          <Link to="/auth" className="btn btn-secondary" style={{ padding: '0.45rem 0.85rem', fontSize: '0.82rem', textDecoration: 'none' }}>
            Log In
          </Link>
        )}
      </div>
    </header>
  )
}

export function SidePanel({ title, items, active }) {
  return (
    <aside style={{ position: 'sticky', top: '100px' }}>
      {title && <h3 style={{ marginBottom: '1.5rem', fontSize: '1.1rem', color: 'var(--on-surface-variant)', textTransform: 'uppercase', letterSpacing: '0.06em', fontWeight: 600 }}>{title}</h3>}
      <ul className="side-menu">
        {items.map((item, index) => (
          <li key={item.label || item}>
            <Link
              to={item.to || '#'}
              className={`side-menu-link ${index === active ? 'active' : ''}`}
            >
              {item.label || item}
            </Link>
          </li>
        ))}
      </ul>
    </aside>
  )
}

export function ProductCard({ product }) {
  const { addToCart } = useShop()
  return (
    <Link to={`/products/${product.id}`} className="product-card">
      <div className="product-image-container" style={{ backgroundImage: `url("${product.image || 'https://images.unsplash.com/photo-1555041469-a586c61ea9bc?auto=format&fit=crop&q=80&w=500'}")`, backgroundSize: 'cover', backgroundPosition: 'center' }}>
        {product.tag && (
          <div style={{ position: 'absolute', top: '0.75rem', left: '0.75rem', zIndex: 1 }}>
            <Badge tone="accent">{product.tag}</Badge>
          </div>
        )}
        {product.oldPrice && (
          <div style={{ position: 'absolute', bottom: '0.75rem', left: '0.75rem', zIndex: 1, background: 'var(--secondary)', color: '#fff', fontSize: '0.7rem', fontWeight: 700, padding: '0.2rem 0.5rem', borderRadius: '3px' }}>
            SAVE {formatCurrency(product.oldPrice - product.price)}
          </div>
        )}
        <button
          className="icon-btn wishlist-btn"
          onClick={(e) => { e.preventDefault(); }}
          title="Add to wishlist"
        >
          <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"></path>
          </svg>
        </button>
      </div>

      <div style={{ padding: '0 0.25rem' }}>
        <p style={{ fontSize: '0.72rem', color: 'var(--on-surface-variant)', textTransform: 'uppercase', letterSpacing: '0.06em', marginBottom: '0.3rem', fontWeight: 600 }}>{product.category}</p>
        <h3 className="product-title">{product.title}</h3>
        <p className="product-desc">{product.description}</p>
        <div className="space-between" style={{ marginTop: '0.75rem', alignItems: 'center' }}>
          <div>
            <p className="product-price">{formatCurrency(product.price)}</p>
            {product.oldPrice && (
              <p style={{ fontSize: '0.8rem', textDecoration: 'line-through', color: 'var(--on-surface-variant)', marginTop: '0.1rem' }}>{formatCurrency(product.oldPrice)}</p>
            )}
          </div>
          <Button
            variant="primary"
            style={{ padding: '0.45rem 0.9rem', fontSize: '0.82rem', flexShrink: 0 }}
            onClick={(e) => { e.preventDefault(); addToCart({ ...product, variant: 'Standard' }, 1, 'Standard'); }}
          >
            Add to Cart
          </Button>
        </div>
      </div>
    </Link>
  )
}

export function Stepper({ steps, current = 0 }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', marginBottom: '2.5rem', background: 'var(--surface-lowest)', border: '1px solid var(--border-color)', borderRadius: '4px', padding: '1.25rem 2rem' }}>
      {steps.map((step, index) => {
        const isActive = index === current
        const isPast = index < current
        return (
          <div key={step} style={{ display: 'flex', alignItems: 'center', flex: index < steps.length - 1 ? 1 : 'none' }}>
            <div style={{
              width: '30px', height: '30px', borderRadius: '50%',
              background: isPast ? 'var(--secondary)' : isActive ? 'var(--primary)' : 'var(--surface-high)',
              color: isActive || isPast ? '#fff' : 'var(--on-surface-variant)',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              fontSize: '0.8rem', fontWeight: 700, flexShrink: 0,
              transition: 'all 0.2s'
            }}>
              {isPast ? (
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round">
                  <polyline points="20 6 9 17 4 12"></polyline>
                </svg>
              ) : index + 1}
            </div>
            <span style={{
              marginLeft: '0.6rem',
              fontSize: '0.85rem',
              fontWeight: isActive ? 700 : isPast ? 600 : 500,
              color: isActive ? 'var(--primary)' : isPast ? 'var(--on-surface-variant)' : 'var(--on-surface-variant)',
              whiteSpace: 'nowrap',
            }}>
              {step}
            </span>
            {index < steps.length - 1 && (
              <div style={{ height: '2px', background: isPast ? 'var(--secondary)' : 'var(--surface-high)', flexGrow: 1, margin: '0 1.25rem', borderRadius: '2px' }} />
            )}
          </div>
        )
      })}
    </div>
  )
}

export function OrderSummary({ title, rows, total, cta }) {
  return (
    <div className="order-summary-box">
      <h3 style={{ marginBottom: '1.5rem', fontSize: '1.1rem', borderBottom: '2px solid var(--primary)', paddingBottom: '0.75rem', color: 'var(--primary)' }}>{title}</h3>
      <div style={{ display: 'flex', flexDirection: 'column', gap: '0.85rem' }}>
        {rows.map((row) => (
          <div key={row.label} className="summary-row" style={{ margin: 0 }}>
            <span style={{ color: 'var(--on-surface-variant)' }}>{row.label}</span>
            <span style={{ fontWeight: 500, color: 'var(--on-surface)' }}>
              {row.value === 0 && row.label === 'Shipping' ? (
                <span style={{ color: '#16a34a', fontWeight: 600 }}>FREE</span>
              ) : formatCurrency(row.value)}
            </span>
          </div>
        ))}
      </div>
      <div className="summary-total">
        <span>Total</span>
        <span>{formatCurrency(total)}</span>
      </div>
      {cta && <div style={{ marginTop: '1.5rem' }}>{cta}</div>}
    </div>
  )
}
