# Design System Specification: The Architectural Retail Experience

## 1. Overview & Creative North Star: "The Curated Gallery"
This design system moves beyond traditional e-commerce "grids" to embrace a **Curated Gallery** aesthetic. For the Lewis Group, retail is not just a transaction; it is an architectural experience. We prioritize breathing room, intentional asymmetry, and tonal depth over rigid lines and cluttered interfaces.

**The Creative North Star** is "Editorial Authority." We treat every product like a piece of art in a high-end showroom. By utilizing significant white space, bold typography scales, and layered surfaces, we create a digital environment that feels premium, trustworthy, and effortless to navigate.

---

## 2. Colors & Surface Philosophy
The palette utilizes a foundation of **Deep Prussian Blue (`primary`)** and **Heritage Red (`secondary`)**, balanced by a sophisticated range of cool grays.

### The "No-Line" Rule
**Borders are strictly prohibited for sectioning.** To define boundaries, designers must use background color shifts or tonal transitions. A section on `surface` should be distinguished from a section on `surface-container-low` purely through color.

### Surface Hierarchy & Nesting
Depth is created through "The Stack." UI elements should feel like physical layers of fine paper or frosted glass.
- **Background (`#f9f9fc`):** The canvas.
- **Surface Container Lowest (`#ffffff`):** Reserved for primary content cards or focused input areas.
- **Surface Container High (`#e8e8ea`):** Reserved for utility bars or secondary navigation drawers.

### The "Glass & Gradient" Rule
To add "soul" to the professional aesthetic:
- **Glassmorphism:** Use `surface-container-lowest` with 80% opacity and a `24px` backdrop-blur for floating headers or modal overlays.
- **Signature Gradients:** For primary CTAs, use a subtle linear gradient from `primary` (#002068) to `primary_container` (#003399) at a 135-degree angle. This prevents the "flat-button" look and adds a tactile, premium sheen.

---

## 3. Typography: The Editorial Voice
We utilize a dual-font strategy to balance character with utility.

*   **Display & Headlines (Manrope):** Chosen for its geometric precision and modern retail feel. It commands attention without being aggressive.
*   **Body & Labels (Inter):** The gold standard for readability. It ensures that technical specs and retail pricing remain legible even at small scales.

| Level | Token | Font | Size | Weight | Intent |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Display** | `display-lg` | Manrope | 3.5rem | 700 | Hero Statements |
| **Headline**| `headline-md` | Manrope | 1.75rem | 600 | Category Titles |
| **Title**   | `title-md` | Inter | 1.125rem | 600 | Product Names |
| **Body**    | `body-lg` | Inter | 1rem | 400 | Descriptions |
| **Label**   | `label-md` | Inter | 0.75rem | 500 | Metadata / Tags |

---

## 4. Elevation & Depth
In this system, light and shadow follow the laws of physics, not the laws of CSS defaults.

*   **The Layering Principle:** Avoid shadows where color shifts can work. A `surface-container-lowest` card sitting on a `surface-container-low` background creates a "soft lift" that is cleaner than a drop shadow.
*   **Ambient Shadows:** When an element must float (e.g., a cart drawer), use an ultra-diffused shadow: `box-shadow: 0 20px 40px rgba(26, 28, 30, 0.06);`.
*   **The "Ghost Border" Fallback:** If accessibility requires a border (e.g., high-contrast mode), use `outline-variant` at **15% opacity**. Never use a 100% opaque border.

---

## 5. Components

### High-Fidelity Buttons
*   **Primary:** Gradient (`primary` to `primary_container`), `on_primary` text, `0.375rem (md)` corner radius. 
*   **Secondary:** No background, `outline` at 20% opacity, `primary` text.
*   **Interaction:** On hover, the primary button should shift the gradient slightly and increase the `Ambient Shadow` blur.

### Product Cards
*   **Forbid Dividers:** Do not use lines to separate the image from the text. Use a `1.5rem` vertical spacing gap.
*   **Structure:** Image (Top) -> `title-md` -> `body-sm` (Description) -> `title-lg` (Price in `primary` color).
*   **Container:** `surface-container-lowest` with a `lg` (0.5rem) corner radius.

### Input Fields
*   **Stateful Design:** Use `surface-container-low` as the default background. On focus, transition the background to `surface-container-lowest` and add a `2px` "Ghost Border" using the `primary` color.
*   **Typography:** Floating labels use `label-md`.

### Navigation Elements
*   **The Signature Bar:** A glassmorphic top bar using `surface` at 85% opacity with a `20px` blur. 
*   **Active State:** Use a 4px `secondary` (Red) underline that is rounded and centered under the nav item, rather than a full-width line.

---

## 6. Do’s and Don’ts

### Do
*   **DO** use "Negative Space" as a design element. If a layout feels crowded, increase the padding rather than adding a divider.
*   **DO** use `secondary` (#bb0014) sparingly for "Urgency" (Sale tags, Low stock) and `error` (#ba1a1a) for "System Alerts."
*   **DO** ensure all automated selectors for QE use stable `data-testid` attributes, as the visual layering (z-index) will be complex.

### Don’t
*   **DON'T** use 1px solid borders for layout. It breaks the "Gallery" feel and creates a "Bootstrap" appearance.
*   **DON'T** use pure black (#000000) for text. Always use `on_surface` (#1a1c1e) to maintain the premium tonal balance.
*   **DON'T** use sharp corners. The `DEFAULT` (0.25rem) or `md` (0.375rem) roundedness scale is mandatory to maintain the "Modern Retail" approachable aesthetic.

---

## 7. Accessibility & Automation Note
While this system is visually "soft," it is functionally "hard." Every surface transition must maintain a minimum contrast ratio of 4.5:1. For QE Automation, every component should be identified by its functional role (e.g., `primary-cta`) rather than its visual token, allowing the "Gallery" aesthetic to evolve without breaking test scripts.