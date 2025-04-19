import Keycloak from 'keycloak-js';

const keycloakUrl = import.meta.env.VITE_KEYCLOAK_URL;
const keycloakRealm = import.meta.env.VITE_KEYCLOAK_REALM;
const keycloakClientId = import.meta.env.VITE_KEYCLOAK_CLIENT_ID;

const keycloakConfig = {
  url: keycloakUrl,
  realm: keycloakRealm,
  clientId: keycloakClientId
};

const keycloak = new Keycloak(keycloakConfig);

export default keycloak;
