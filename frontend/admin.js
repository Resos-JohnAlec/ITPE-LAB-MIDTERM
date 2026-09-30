import { request, eventDate, element } from "./api.js";
const byId = (id) => document.getElementById(id);
let accessKey = "";
let requestVersion = 0;

function status(message, isError = false) {
  byId("admin-status").textContent = message;
  byId("admin-status").hidden = !message;
  byId("admin-status").classList.toggle("error", isError);
}

async function loadAttendees() {
  const version = ++requestVersion;
  const eventId = byId("admin-event").value;
  byId("attendee-rows").replaceChildren();
  byId("attendee-empty").hidden = true;
  if (!eventId) {
    status("There are no events to display.");
    return;
  }
  status("Loading the guest list…");
  byId("refresh-attendees").disabled = true;
  try {
    const attendees = await request(`/api/admin/events/${eventId}/attendees`, {
      headers: { "X-Admin-Key": accessKey },
    });
    if (version !== requestVersion) return;
    attendees.forEach((attendee) => {
      const row = element("tr");
      row.append(
        element("td", "", attendee.fullName),
        element("td", "", attendee.email),
        element("td", "", eventDate(attendee.registeredAt)),
      );
      byId("attendee-rows").append(row);
    });
    const title = byId("admin-event").selectedOptions[0].textContent;
    byId("attendee-caption").textContent =
      `${title} · ${attendees.length} registered attendee${attendees.length === 1 ? "" : "s"}`;
    byId("attendee-empty").hidden = attendees.length > 0;
    status(
      `${attendees.length} attendee${attendees.length === 1 ? "" : "s"} loaded.`,
    );
  } catch (error) {
    if (version === requestVersion) status(error.message, true);
  } finally {
    if (version === requestVersion) byId("refresh-attendees").disabled = false;
  }
}

byId("access-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  accessKey = byId("admin-key").value;
  byId("unlock-button").disabled = true;
  status("Checking organizer access…");
  try {
    const events = await request("/api/admin/events", {
      headers: { "X-Admin-Key": accessKey },
    });
    byId("admin-event").replaceChildren(
      ...events.map((event) => {
        const option = element("option", "", event.title);
        option.value = event.eventId;
        return option;
      }),
    );
    byId("admin-key").value = "";
    byId("access-panel").hidden = true;
    byId("attendees-panel").hidden = false;
    byId("attendees-title").focus();
    await loadAttendees();
  } catch (error) {
    accessKey = "";
    status(error.message, true);
  } finally {
    byId("unlock-button").disabled = false;
  }
});
byId("admin-event").addEventListener("change", loadAttendees);
byId("refresh-attendees").addEventListener("click", loadAttendees);
byId("lock-button").addEventListener("click", () => {
  requestVersion++;
  accessKey = "";
  byId("admin-key").value = "";
  byId("attendee-rows").replaceChildren();
  byId("admin-event").replaceChildren();
  byId("attendees-panel").hidden = true;
  byId("access-panel").hidden = false;
  status("Organizer access is locked.");
  byId("admin-key").focus();
});
