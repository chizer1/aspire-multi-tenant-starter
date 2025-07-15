import React from 'react';
import Keycloak from 'keycloak-js';

export const KeycloakContext = React.createContext<Keycloak | null>(null);