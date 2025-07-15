import React, { useEffect, useState } from "react";
import ReactDOM from "react-dom/client";
import { Provider } from "./components/ui/provider";
import { RouterProvider } from "@tanstack/react-router";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import Keycloak from "keycloak-js";
import { router } from "./routes";
import { KeycloakContext } from "../KeycloakContext";

const keycloak = new Keycloak({
  url: "http://localhost:8080",
  realm: "Test",
  clientId: "workspaces-client",
});

const queryClient = new QueryClient();

function AppWrapper() {
  const [initialized, setInitialized] = useState(false);

  useEffect(() => {
    keycloak
      .init({
        onLoad: "login-required",
        checkLoginIframe: false,
        pkceMethod: "S256",
      })
      .then((authenticated) => {
        if (authenticated) {
          setInitialized(true);
        } else {
          keycloak.login();
        }
      });
  }, []);

  if (!initialized) return <div>Loading...</div>;

  return (
    <KeycloakContext.Provider value={keycloak}>
      <Provider>
        <QueryClientProvider client={queryClient}>
          <RouterProvider router={router} />
        </QueryClientProvider>
      </Provider>
    </KeycloakContext.Provider>
  );
}

ReactDOM.createRoot(document.getElementById("root")!).render(<AppWrapper />);