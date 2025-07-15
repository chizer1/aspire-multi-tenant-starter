import {
  Box,
  Flex,
  HStack,
  Text,
  Spacer,
  Button,
} from "@chakra-ui/react";
import { Link } from "@tanstack/react-router";
import React, { useContext } from "react";
import { KeycloakContext } from "../../KeycloakContext";

const MenuBar: React.FC = () => {
  const keycloak = useContext(KeycloakContext);

  const tenantCode = keycloak?.tokenParsed?.tenant_code;
  const username = keycloak?.tokenParsed?.preferred_username;
  const onLogout = () => keycloak?.logout();
  const roles = keycloak?.tokenParsed?.realm_access?.roles || [];
  const isAdmin = roles.includes("Admin");

  console.log(keycloak?.tokenParsed!);

  return (
    <Box
      px={6}
      py={4}
      bg={"border"}
      boxShadow="sm"
      position="sticky"
      top="0"
      zIndex="1000"
    >
      <Flex alignItems="center">
        <Text fontSize="xl" fontWeight="bold" mr={8}>
          MyApp | {tenantCode}
        </Text>

        <HStack as="nav" fontWeight="medium">
          <Link to="/">
            Home
          </Link>
          <Link to="/products">
            Products
          </Link>
          {isAdmin && <Link to="/tenants">Tenants</Link>}
        </HStack>

        <Spacer />

        <HStack>
          {username && <Text color="gray.600">{username}</Text>}
          <Button variant="outline" size="sm" onClick={onLogout}>
            Logout
          </Button>
        </HStack>
      </Flex>
    </Box>
  );
};

export default MenuBar;