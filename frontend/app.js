import { request, eventDate, element } from "./api.js";

const byId = (id) => document.getElementById(id);
const dialog = byId("registration-dialog");
const form = byId("registration-form");
const nameInput = byId("full-name");
const emailInput = byId("email");
let events = [];
let selectedEvent = null;
let universityDomain = "dlsud.edu.ph";
let submitting = false;
let trigger = null;

function renderEvents() {
  const query = byId("event-search").value.trim().toLowerCase();
  const matching = events.filter((event) =>
    `${event.title} ${event.description} ${event.location}`
      .toLowerCase()
      .includes(query),
  );
  byId("event-count").textContent =
    `${matching.length} upcoming event${matching.length === 1 ? "" : "s"} · Philippine time`;
  byId("event-grid").replaceChildren();
  byId("catalog-status").hidden = matching.length > 0;
  byId("catalog-status").textContent = events.length
    ? "No events match your search. Try another topic or place."
    : "No upcoming events just yet. Check back soon.";
  matching.forEach((event) => {
    const index = events.indexOf(event);
    const card = element("article", "event-card");
    const art = element("div", `event-art tone-${index % 6}`);
    art.setAttribute("aria-hidden", "true");
    art.append(
      element("span", "art-tag", "THE CAMPUS COLLECTION"),
      element(
        "span",
        "art-word",
        [
          "Create together.",
          "Go further.",
          "Grow with us.",
          "Think differently.",
          "Stay inspired.",
          "Find your people.",
        ][index % 6],
      ),
    );
    const badge = element("div", "event-date-badge");
    badge.append(
      element(
        "span",
        "",
        new Intl.DateTimeFormat("en", {
          month: "short",
          timeZone: "Asia/Manila",
        })
          .format(new Date(event.startsAt))
          .toUpperCase(),
      ),
      element(
        "strong",
        "",
        new Intl.DateTimeFormat("en", {
          day: "2-digit",
          timeZone: "Asia/Manila",
        }).format(new Date(event.startsAt)),
      ),
    );
    art.append(badge);
    const content = element("div", "event-content");
    const date = element("time", "event-meta", eventDate(event.startsAt));
    date.dateTime = event.startsAt;
    const heading = element("h3", "", event.title);
    const bottom = element("div", "event-card-bottom");
    const register = element(
      "button",
      "register-button",
      event.availableSeats > 0 ? "Save a seat ↗" : "Fully booked",
    );
    register.type = "button";
    register.disabled = event.availableSeats <= 0;
    register.setAttribute(
      "aria-label",
      event.availableSeats > 0
        ? `Register for ${event.title}`
        : `${event.title} is fully booked`,
    );
    register.addEventListener("click", () => openRegistration(event, register));
    bottom.append(
      element(
        "span",
        "seat-count",
        `${event.availableSeats} seat${event.availableSeats === 1 ? "" : "s"} left`,
      ),
      register,
    );
    content.append(
      date,
      heading,
      element("p", "event-description", event.description),
      element("p", "event-location", event.location),
      bottom,
    );
    card.append(art, content);
    byId("event-grid").append(card);
  });
}

async function loadEvents() {
  byId("retry-events").hidden = true;
  byId("catalog-status").hidden = false;
  byId("catalog-status").textContent = "Finding your next campus experience…";
  try {
    const [catalog, config] = await Promise.all([
      request("/api/events"),
      request("/api/config"),
    ]);
    events = catalog;
    universityDomain = config.universityDomain;
    byId("email-hint").textContent =
      `Use your @${universityDomain} email address.`;
    emailInput.placeholder = `your.name@${universityDomain}`;
    renderEvents();
  } catch (error) {
    byId("catalog-status").textContent = error.message;
    byId("event-count").textContent = "Events unavailable";
    byId("retry-events").hidden = false;
  }
}

function fieldError(input, message) {
  input.setAttribute("aria-invalid", String(Boolean(message)));
  byId(input === nameInput ? "name-error" : "email-error").textContent =
    message;
  return !message;
}

function validateName() {
  const value = nameInput.value.trim();
  return fieldError(
    nameInput,
    value.length < 2 || value.length > 100 || /[\x00-\x1f\x7f]/.test(value)
      ? "Enter your full name (2–100 characters)."
      : "",
  );
}

function validateEmail() {
  const value = emailInput.value.trim();
  const [local, domain, extra] = value.split("@");
  const valid =
    value.length <= 254 &&
    local &&
    local.length <= 64 &&
    /^[A-Za-z0-9.!#$%&'*+/=?^_`{|}~-]+$/.test(local) &&
    !local.startsWith(".") &&
    !local.endsWith(".") &&
    !local.includes("..") &&
    domain?.toLowerCase() === universityDomain.toLowerCase() &&
    extra === undefined;
  return fieldError(
    emailInput,
    valid ? "" : `Enter a valid @${universityDomain} university email.`,
  );
}

function openRegistration(event, button) {
  selectedEvent = event;
  trigger = button;
  form.reset();
  fieldError(nameInput, "");
  fieldError(emailInput, "");
  byId("form-error").hidden = true;
  byId("registration-success").hidden = true;
  form.hidden = false;
  byId("registration-title").textContent = event.title;
  byId("selected-event-details").textContent =
    `${eventDate(event.startsAt)} · ${event.location}`;
  dialog.showModal();
  nameInput.focus();
}

function closeDialog() {
  if (!submitting) dialog.close();
}
byId("close-dialog").addEventListener("click", closeDialog);
byId("done-button").addEventListener("click", closeDialog);
dialog.addEventListener("cancel", (event) => {
  if (submitting) event.preventDefault();
});
dialog.addEventListener("close", () => {
  if (trigger?.isConnected) trigger.focus();
  else byId("event-search").focus();
});
nameInput.addEventListener("blur", validateName);
emailInput.addEventListener("blur", validateEmail);
byId("event-search").addEventListener("input", renderEvents);
byId("retry-events").addEventListener("click", loadEvents);

form.addEventListener("submit", async (event) => {
  event.preventDefault();
  if (submitting) return;
  const nameValid = validateName();
  const emailValid = validateEmail();
  if (!nameValid || !emailValid) {
    (nameValid ? emailInput : nameInput).focus();
    return;
  }
  submitting = true;
  byId("submit-registration").disabled = true;
  byId("close-dialog").disabled = true;
  byId("submit-registration").textContent = "Saving your seat…";
  form.setAttribute("aria-busy", "true");
  byId("form-error").hidden = true;
  try {
    const result = await request("/api/registrations", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        eventId: selectedEvent.eventId,
        fullName: nameInput.value.trim(),
        email: emailInput.value.trim(),
      }),
    });
    form.hidden = true;
    byId("registration-success").hidden = false;
    byId("success-message").textContent =
      `Your seat at ${result.eventTitle} is confirmed. See you there!`;
    byId("confirmation-number").textContent =
      `Registration #${result.registrationId}`;
    byId("success-title").focus();
    await loadEvents();
  } catch (error) {
    byId("form-error").textContent = error.message;
    byId("form-error").hidden = false;
    await loadEvents();
  } finally {
    submitting = false;
    byId("submit-registration").disabled = false;
    byId("close-dialog").disabled = false;
    byId("submit-registration").textContent = "Confirm registration ↗";
    form.removeAttribute("aria-busy");
  }
});

loadEvents();
