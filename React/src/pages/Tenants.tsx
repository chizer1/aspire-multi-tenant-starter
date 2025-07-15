import React, { useContext, useEffect, useState } from 'react';
import {
  Box,
  Heading,
  VStack,
  Spinner,
  Text,
  Flex,
  Input,
  Button,
  HStack,
} from '@chakra-ui/react';
import { useNavigate } from '@tanstack/react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { KeycloakContext } from '../../KeycloakContext';

interface Tenant {
  id: number;
  name: string;
  code: string;
}

const Tenants: React.FC = () => {
  const keycloak = useContext(KeycloakContext);
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const roles = keycloak?.tokenParsed?.realm_access?.roles || [];
  const isAdmin = roles.includes('Admin');

  const [newTenant, setNewTenant] = useState({ name: '', code: '' });

  useEffect(() => {
    if (!isAdmin) {
      navigate({ to: '/' });
    }
  }, [isAdmin, navigate]);

  const fetchTenants = async (): Promise<Tenant[]> => {
    const token = keycloak?.token;
    const response = await fetch('https://localhost:7107/tenants', {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });
    if (!response.ok) throw new Error('Failed to fetch tenants');
    return response.json();
  };

  const {
    isPending,
    error,
    data: tenants,
  } = useQuery({
    queryKey: ['tenants'],
    queryFn: fetchTenants,
    enabled: isAdmin,
  });

  const addTenantMutation = useMutation({
    mutationFn: async ({ name, code }: { name: string; code: string }) => {
      const token = keycloak?.token;
      const query = new URLSearchParams({
        tenantName: name,
        tenantCode: code,
      }).toString();

      const response = await fetch(`https://localhost:7107/tenants?${query}`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) throw new Error('Failed to add tenant');
      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tenants'] });
      setNewTenant({ name: '', code: '' });
    },
  });

  const handleAddTenant = () => {
    if (newTenant.name && newTenant.code) {
      addTenantMutation.mutate({ name: newTenant.name, code: newTenant.code });
    }
  };

  if (!isAdmin) return null;

  if (isPending) {
    return (
      <Box textAlign="center" mt={10}>
        <Spinner size="xl" />
      </Box>
    );
  }

  if (error) {
    return (
      <Box textAlign="center" mt={10} color="red.500">
        An error occurred: {(error as Error).message}
      </Box>
    );
  }

  return (
    <Box maxW="800px" mx="auto" p={6}>
      <Heading as="h2" size="lg" mb={4}>
        Tenants
      </Heading>

      <VStack align="stretch" spacing={4} mb={8}>
        {tenants?.map((tenant) => (
          <Box key={tenant.id} p={4} borderWidth="1px" borderRadius="lg">
            <Flex direction="column">
              <Text fontWeight="bold">{tenant.name}</Text>
              <Text>{tenant.code}</Text>
            </Flex>
          </Box>
        ))}
      </VStack>

      <Heading as="h3" size="md" mb={2}>
        Add Tenant
      </Heading>

      <VStack align="start" spacing={2}>
        <Input
          placeholder="Tenant Name"
          value={newTenant.name}
          onChange={(e) => setNewTenant({ ...newTenant, name: e.target.value })}
        />
        <Input
          placeholder="Tenant Code"
          value={newTenant.code}
          onChange={(e) => setNewTenant({ ...newTenant, code: e.target.value })}
        />
        <HStack>
          <Button colorScheme="teal" onClick={handleAddTenant}>
            Add Tenant
          </Button>
        </HStack>
      </VStack>
    </Box>
  );
};

export default Tenants;