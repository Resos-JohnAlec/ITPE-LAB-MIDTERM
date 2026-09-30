# Task 2 — Interface and accessibility

## Page structure

`frontend/index.html` contains the student catalog and a native registration dialog. Header/nav, main, section, article (created by JavaScript), and footer describe the document. Cards show a title, event time, location, description, derived remaining seats, and registration action. Search narrows the loaded catalog. A labeled form validates full name and university email before submitting to the actual API. The success view displays the server-issued registration ID. `admin.html` provides the required attendee view with a locally configured organizer key.

The complete implementation is in `index.html`, `styles.css`, `api.js`, `app.js`, `admin.html`, and `admin.js`. No frontend build or runtime library is required. The ASP.NET Core host serves these files on the API origin. The event data is fictional seed content read from a real SQL Server database; the browser does not simulate successful persistence.

## Accessibility and POUR mapping

| Principle | Implementation | Verification boundary |
|---|---|---|
| Perceivable | High-contrast semantic palette; visible form labels; event text also contains all decorative-art information; textual errors | Automated axe scans and screenshot inspection; human visual/assistive-technology review remains |
| Operable | Native buttons, links, dialog, input, select; skip link; focus rings; keyboard opening and Escape; disabled submission while pending | Browser keyboard/focus checks, responsive viewport checks |
| Understandable | Specific inline errors, exact university-domain hint, labeled required inputs, explicit loading/empty/error/success messages | Wrong-domain, empty input, duplicate, and success flow automation |
| Robust | Semantic landmarks/headings; label associations; aria-invalid, aria-describedby, role=status/alert; native dialog accessible title | Automated WCAG A/AA scans for catalog, form, success, and both administrator states |

All graphics are decorative CSS compositions or text inside aria-hidden regions; there are no informative raster images needing alt text. Body copy and controls use native system fonts to avoid external font dependencies. Reduced-motion preferences are honored. The search field has an explicit accessible name rather than relying only on placeholder text.

## Design decisions

The UI/UX skill informed the focus, label, contrast, loading, responsive-layout, and motion checks. The final visual direction uses a deep green/cream palette, an editorial hero, and restrained event cards suited to the DLSU-D campus context. It adapts the skill's suggestions to plain HTML/CSS, without adding animation libraries, a framework, unofficial university marks, or unnecessary features.

## Assumptions

The university domain is `dlsud.edu.ph`, supplied by the user. Events are fictional samples, displayed in Asia/Manila. Email ownership is not verified. This is a local classroom prototype. An organizer key provides a minimal attendee-data guard rather than a full login system. Dates and counts are supplied by the database, not hardcoded into the UI.
