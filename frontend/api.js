export async function request(path, options = {}) {
  let response;
  try {
    response = await fetch(path, {
      ...options,
      signal: AbortSignal.timeout(15000),
    });
  } catch {
    throw new Error(
      "We could not reach the server. Check your connection and try again.",
    );
  }
  const data = await response.json().catch(() => null);
  if (!response.ok)
    throw new Error(
      data?.message || "The request could not be completed. Please try again.",
    );
  return data;
}

export const dateFormat = new Intl.DateTimeFormat("en-PH", {
  timeZone: "Asia/Manila",
  month: "short",
  day: "numeric",
  year: "numeric",
});
export const timeFormat = new Intl.DateTimeFormat("en-PH", {
  timeZone: "Asia/Manila",
  hour: "numeric",
  minute: "2-digit",
});
export function eventDate(value) {
  return `${dateFormat.format(new Date(value))} · ${timeFormat.format(new Date(value))} PHT`;
}
export function element(tag, className, text) {
  const node = document.createElement(tag);
  if (className) node.className = className;
  if (text !== undefined) node.textContent = text;
  return node;
}
