import { getAuthToken } from "./auth";

export async function fetchWithAuth(url, options = {}) {
  const token = getAuthToken();

  const headers = {
    ...options.headers,
    "Authorization": `Bearer ${token}`
  };

  return fetch(url, {
    ...options,
    headers
  });
}