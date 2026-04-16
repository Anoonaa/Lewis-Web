import { createContext, useContext, useState } from 'react';

const ShopContext = createContext();

export function useShop() {
  return useContext(ShopContext);
}

const INITIAL_CREDIT_FORM = {
  // Step 1 – Personal Info
  fullName: '',
  idNumber: '',
  email: '',
  phone: '',
  grossIncome: '',
  netIncome: '',
  expenses: '',
  // Step 2 – Employment
  employer: '',
  industry: '',
  yearsAtJob: '',
  employmentType: '',
  // Result
  submitted: false,
  approved: null,
  referenceNumber: '',
  creditLimit: 0,
};

export function ShopProvider({ children }) {
  const [cartItems, setCartItems] = useState([]);
  const [toastMessage, setToastMessage] = useState(null);
  const [creditForm, setCreditForm] = useState(INITIAL_CREDIT_FORM);
  const [searchQuery, setSearchQuery] = useState('');

  const cartCount = cartItems.reduce((count, item) => count + item.quantity, 0);
  const cartSubtotal = cartItems.reduce((total, item) => total + (item.price * item.quantity), 0);
  const tax = cartItems.length > 0 ? Math.round(cartSubtotal * 0.15) : 0;
  const shipping = cartSubtotal > 0 && cartSubtotal < 5000 ? 499 : 0;
  const cartTotal = cartSubtotal + tax + shipping;

  const showToast = (message) => {
    setToastMessage(message);
    setTimeout(() => {
      setToastMessage(null);
    }, 3500);
  };

  const addToCart = (product, quantity = 1, variant = 'Standard') => {
    setCartItems(prev => {
      const existing = prev.find(item => item.id === product.id && item.variant === variant);
      if (existing) {
        return prev.map(item => item.id === product.id && item.variant === variant
          ? { ...item, quantity: item.quantity + quantity }
          : item
        );
      }
      return [...prev, { ...product, variant, quantity }];
    });
    showToast(`${product.title} added to cart`);
  };

  const removeFromCart = (id, variant) => {
    setCartItems(prev => prev.filter(i => !(i.id === id && i.variant === variant)));
  };

  const updateQuantity = (id, variant, delta) => {
    setCartItems(prev =>
      prev.map(item => {
        if (item.id === id && item.variant === variant) {
          const newQ = item.quantity + delta;
          if (newQ > 0) return { ...item, quantity: newQ };
        }
        return item;
      })
    );
  };

  const clearCart = () => {
    setCartItems([]);
  };

  // Credit form handlers
  const updateCreditForm = (fields) => {
    setCreditForm(prev => ({ ...prev, ...fields }));
  };

  const submitCreditApplication = () => {
    const net = parseFloat(String(creditForm.netIncome).replace(/[^0-9.]/g, '')) || 0;
    const expenses = parseFloat(String(creditForm.expenses).replace(/[^0-9.]/g, '')) || 0;
    const disposable = net - expenses;
    const approved = net >= 3000 && disposable >= 1000 && creditForm.yearsAtJob >= 1;
    const creditLimit = approved ? Math.min(Math.round(disposable * 3), 60000) : 0;
    const refNum = `LWS-${Math.floor(1000 + Math.random() * 9000)}-${String.fromCharCode(65 + Math.floor(Math.random() * 26))}${String.fromCharCode(65 + Math.floor(Math.random() * 26))}`;

    setCreditForm(prev => ({
      ...prev,
      submitted: true,
      approved,
      creditLimit,
      referenceNumber: refNum,
    }));
  };

  const resetCreditForm = () => {
    setCreditForm(INITIAL_CREDIT_FORM);
  };

  return (
    <ShopContext.Provider value={{
      cartItems,
      cartCount,
      cartSubtotal,
      cartTotal,
      tax,
      shipping,
      addToCart,
      removeFromCart,
      updateQuantity,
      clearCart,
      showToast,
      creditForm,
      updateCreditForm,
      submitCreditApplication,
      resetCreditForm,
      searchQuery,
      setSearchQuery,
    }}>
      {children}
      {toastMessage && (
        <div style={{
          position: 'fixed',
          bottom: '2rem',
          right: '2rem',
          background: 'var(--primary)',
          color: '#fff',
          padding: '1rem 1.5rem',
          borderRadius: '4px',
          boxShadow: '0 8px 24px rgba(0,31,92,0.25)',
          zIndex: 9999,
          animation: 'slideIn 0.3s ease-out forwards',
          fontWeight: 500,
          display: 'flex',
          alignItems: 'center',
          gap: '0.75rem',
          maxWidth: '360px',
        }}>
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
            <polyline points="20 6 9 17 4 12"></polyline>
          </svg>
          {toastMessage}
        </div>
      )}
    </ShopContext.Provider>
  );
}
