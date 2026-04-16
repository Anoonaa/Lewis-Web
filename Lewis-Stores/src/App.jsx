import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { TopNav } from './components/UI'
import { navLinks } from './data/mockData'
import { ShopProvider } from './context/ShopContext'
import {
  CartPage,
  CheckoutPage,
  CreditFinancialsPage,
  CreditFormOnePage,
  CreditFormTwoPage,
  CreditInfoPage,
  CreditReviewPage,
  CreditStatusPage,
  HomePage,
  OrderHistoryPage,
  OrderTrackingPage,
  ProductDetailPage,
  ProductListingPage,
  ProfilePage,
} from './pages/Pages'
import './App.css'

function App() {
  return (
    <ShopProvider>
      <BrowserRouter>
        <div className="app-shell">
          <TopNav links={navLinks} />
          <Routes>
            <Route path="/"                   element={<HomePage />} />
            <Route path="/products"           element={<ProductListingPage />} />
            <Route path="/products/:id"       element={<ProductDetailPage />} />
            <Route path="/cart"               element={<CartPage />} />
            <Route path="/checkout"           element={<CheckoutPage />} />
            <Route path="/profile"            element={<ProfilePage />} />
            <Route path="/orders"             element={<OrderHistoryPage />} />
            <Route path="/orders/tracking"    element={<OrderTrackingPage />} />
            <Route path="/credit"             element={<CreditInfoPage />} />
            <Route path="/credit/form-1"      element={<CreditFormOnePage />} />
            <Route path="/credit/form-2"      element={<CreditFormTwoPage />} />
            <Route path="/credit/financials"  element={<CreditFinancialsPage />} />
            <Route path="/credit/review"      element={<CreditReviewPage />} />
            <Route path="/credit/status"      element={<CreditStatusPage />} />
            <Route path="*"                   element={<Navigate to="/" replace />} />
          </Routes>
        </div>
      </BrowserRouter>
    </ShopProvider>
  )
}

export default App
