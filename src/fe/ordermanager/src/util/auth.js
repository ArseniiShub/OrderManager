import keycloak from "../services/keycloak.js";
import { redirect } from "react-router-dom";

let keycloakInitPromise = null;

function removeAuthToken() {
  localStorage.removeItem("token");
  localStorage.removeItem("expiration");
}

function getTokenDuration() {
  const storedExpirationDate = localStorage.getItem("expiration");
  const expirationDate = new Date(storedExpirationDate);
  const now = new Date();
  return expirationDate.getTime() - now.getTime();
}

function setAuthToken(token) {
  localStorage.setItem("token", token);
  const expiration = new Date();
  expiration.setHours(expiration.getHours() + 1);
  localStorage.setItem("expiration", expiration.toISOString());
}

export async function initKeycloak(onAuthenticatedCallback) {
  if (!keycloakInitPromise) {
    keycloakInitPromise = async () => {
      try {
        const authenticated = await keycloak.init({
          onLoad: "check-sso",
          flow: "implicit",
          responseMode: "fragment"
        });

        if (authenticated) {
          setAuthToken(keycloak.token);
          keycloak.onTokenExpired = async () => {
            try {
              const refreshed = await keycloak.updateToken(30);
              if (refreshed) {
                setAuthToken(keycloak.token);
              }
            } catch (error) {
              removeAuthToken();
              await keycloak.login();
            }
          };
        }

        onAuthenticatedCallback();
      } catch (error) {
        console.error("Keycloak initialization error:", error);
        keycloakInitPromise = null;
      }
    };
    keycloakInitPromise();
  }
}

export async function login() {
  await keycloak.login();
}

export async function logout(options) {
  removeAuthToken();
  await keycloak.logout(options);
}

export function getAuthToken() {
  const token = localStorage.getItem("token");
  if (!token) {
    return null;
  }

  const duration = getTokenDuration();
  if (duration < 0) {
    return "EXPIRED";
  }

  return token;
}

//#region Loaders

export function tokenLoader() {
  return getAuthToken();
}

export function authenticationLoader({ request }) {
  const token = getAuthToken();

  if (!token || token === "EXPIRED") {
    const url = new URL(request.url);
    const fullPath = url.pathname + url.search;
    return redirect(`/auth?redirectUrl=${encodeURIComponent(fullPath)}`);
  }

  return null;
}

//#endregion