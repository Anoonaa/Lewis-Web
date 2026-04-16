using System;
using System.Collections.Generic;

namespace LewisStores.Api.Models
{
    /// <summary>
    /// Product entity shown in catalog and product listing views.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Unique product identifier.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Display title for the product.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Product description text.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Current selling price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Previous price used for promotions, when available.
        /// </summary>
        public decimal? OldPrice { get; set; }

        /// <summary>
        /// Optional promotional label, such as New or Sale.
        /// </summary>
        public string? Tag { get; set; }

        /// <summary>
        /// Average customer rating.
        /// </summary>
        public double Rating { get; set; }
    }

    /// <summary>
    /// Category used to organize product catalog sections.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Unique category identifier.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Category display name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Category description text.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// UI route destination associated with the category.
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Visual tone identifier used by the frontend.
        /// </summary>
        public string Tone { get; set; } = string.Empty;
    }

    /// <summary>
    /// Cart line item representing a selected product and quantity.
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// Internal database key for the cart item.
        /// </summary>
        public int InternalId { get; set; } // DB PK

        /// <summary>
        /// Product identifier.
        /// </summary>
        public string Id { get; set; } = string.Empty; // Product ID

        /// <summary>
        /// Product title at the time it was added to cart.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Selected product variant.
        /// </summary>
        public string Variant { get; set; } = string.Empty;

        /// <summary>
        /// Number of units in the cart.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price at time of cart update.
        /// </summary>
        public decimal Price { get; set; }
    }

    /// <summary>
    /// Customer order summary model.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Order identifier.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable order date.
        /// </summary>
        public string Date { get; set; } = string.Empty;

        /// <summary>
        /// Current order status.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Final order total amount.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Associated user identifier.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Human-readable order item summary.
        /// </summary>
        public string Items { get; set; } = string.Empty;
    }

    /// <summary>
    /// Application user model used for mock authentication.
    /// </summary>
    public class User
    {
        /// <summary>
        /// User identifier.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// User email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User password for mock login only.
        /// </summary>
        public string Password { get; set; } = string.Empty; // Mock, not hashed

        /// <summary>
        /// User role used in authorization claims.
        /// </summary>
        public string Role { get; set; } = "Customer";

        /// <summary>
        /// User display name.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// User contact phone.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// User primary address.
        /// </summary>
        public string Address { get; set; } = string.Empty;
    }

    /// <summary>
    /// Stored payment method linked to a user account.
    /// </summary>
    public class PaymentMethod
    {
        /// <summary>
        /// Numeric primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Associated user identifier.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Cardholder full name.
        /// </summary>
        public string CardholderName { get; set; } = string.Empty;

        /// <summary>
        /// Last four card digits.
        /// </summary>
        public string Last4 { get; set; } = string.Empty;

        /// <summary>
        /// Payment brand label.
        /// </summary>
        public string Brand { get; set; } = "Card";

        /// <summary>
        /// Expiry month and year in MM/YY format.
        /// </summary>
        public string Expiry { get; set; } = string.Empty;

        /// <summary>
        /// True when this is the default payment method.
        /// </summary>
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// Credit application submitted by a user.
    /// </summary>
    public class CreditApplication
    {
        /// <summary>
        /// Numeric database identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Associated user identifier.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Current credit application status.
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// UTC timestamp when the application was submitted.
        /// </summary>
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// National ID number supplied by the applicant.
        /// </summary>
        public string IdNumber { get; set; } = string.Empty;

        /// <summary>
        /// Applicant employment status.
        /// </summary>
        public string EmploymentStatus { get; set; } = string.Empty;

        /// <summary>
        /// Applicant's monthly income amount.
        /// </summary>
        public decimal MonthlyIncome { get; set; }

        /// <summary>
        /// Applicant's monthly expenses amount.
        /// </summary>
        public decimal MonthlyExpenses { get; set; }
    }
}
