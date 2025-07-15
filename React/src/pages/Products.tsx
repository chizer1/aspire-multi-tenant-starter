import React, { useState } from "react";
import {
  Box,
  Button,
  Input,
  VStack,
  HStack,
  Text,
  Heading,
  Spinner,
} from "@chakra-ui/react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useContext } from 'react';
import { KeycloakContext } from '../../KeycloakContext';

interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
}

const Products: React.FC = () => {
  const keycloak = useContext(KeycloakContext);

  const [newProduct, setNewProduct] = useState({
    name: "",
    description: "",
    price: 0,
  });
  const [editProductId, setEditProductId] = useState<number | null>(null);
  const [editProductData, setEditProductData] = useState({
    name: "",
    description: "",
    price: 0,
  });

  const queryClient = useQueryClient();

  const fetchProducts = async (): Promise<Product[]> => {
    const token = keycloak?.token;
    const response = await fetch("https://localhost:7107/products", {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) throw new Error("Failed to fetch products");
    return response.json();
  };

  const {
    isPending,
    error,
    data: products,
  } = useQuery({
    queryKey: ["products"],
    queryFn: fetchProducts,
  });

  const addProductMutation = useMutation({
    mutationFn: async (product: Omit<Product, "id">) => {
      const token = keycloak?.token;
      const query = new URLSearchParams(product as any).toString();
      const response = await fetch(`https://localhost:7107/products?${query}`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) throw new Error("Failed to add product");
      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
    },
  });

  const updateProductMutation = useMutation({
    mutationFn: async ({ id, ...product }: Product) => {
      const token = keycloak?.token;

      const query = new URLSearchParams({
        id: id.toString(),
        name: product.name,
        description: product.description,
        price: product.price.toString(),
      }).toString();

      const response = await fetch(`https://localhost:7107/products?${query}`, {
        method: "PATCH",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) throw new Error("Failed to update product");
      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
      setEditProductId(null);
    },
  });

  const deleteProductMutation = useMutation({
    mutationFn: async (id: number) => {
      const token = keycloak?.token;
      const response = await fetch(`https://localhost:7107/products?id=${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) throw new Error("Failed to delete product");
      return id;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["products"] });
    },
  });

  const handleAdd = () => {
    addProductMutation.mutate(newProduct);
    setNewProduct({ name: "", description: "", price: 0 });
  };

  const handleUpdate = () => {
    if (editProductId !== null) {
      updateProductMutation.mutate({ id: editProductId, ...editProductData });
    }
  };

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
        An error has occurred: {(error as Error).message}
      </Box>
    );
  }

  return (
    <Box maxW="800px" mx="auto" p={6}>
      <Heading as="h2" size="lg" mb={4}>
        Products
      </Heading>
      <VStack align="stretch">
        {products.map((product: Product) => (
          <Box key={product.id} p={4} borderWidth="1px" borderRadius="lg">
            {editProductId === product.id ? (
              <VStack>
                <Input
                  placeholder="Name"
                  value={editProductData.name}
                  onChange={(e) =>
                    setEditProductData({
                      ...editProductData,
                      name: e.target.value,
                    })
                  }
                />
                <Input
                  placeholder="Description"
                  value={editProductData.description}
                  onChange={(e) =>
                    setEditProductData({
                      ...editProductData,
                      description: e.target.value,
                    })
                  }
                />
                <Input
                  type="number"
                  placeholder="Price"
                  value={editProductData.price}
                  onChange={(e) =>
                    setEditProductData({
                      ...editProductData,
                      price: Number(e.target.value),
                    })
                  }
                />
                <HStack>
                  <Button colorScheme="green" onClick={handleUpdate}>
                    Save
                  </Button>
                  <Button
                    variant="outline"
                    onClick={() => setEditProductId(null)}
                  >
                    Cancel
                  </Button>
                </HStack>
              </VStack>
            ) : (
              <HStack justify="space-between" align="start">
                <Box>
                  <Text fontWeight="bold">{product.name}</Text>
                  <Text>{product.description}</Text>
                  <Text>${product.price.toFixed(2)}</Text>
                </Box>
                <HStack>
                  <Button
                    size="sm"
                    colorScheme="blue"
                    onClick={() => {
                      setEditProductId(product.id);
                      setEditProductData({
                        name: product.name,
                        description: product.description,
                        price: product.price,
                      });
                    }}
                  >
                    Edit
                  </Button>
                  <Button
                    size="sm"
                    colorScheme="red"
                    onClick={() => deleteProductMutation.mutate(product.id)}
                  >
                    Delete
                  </Button>
                </HStack>
              </HStack>
            )}
          </Box>
        ))}
      </VStack>

      <Heading as="h3" size="md" mb={2}>
        Add Product
      </Heading>

      <VStack align="start">
        <Input
          placeholder="Name"
          value={newProduct.name}
          onChange={(e) =>
            setNewProduct({ ...newProduct, name: e.target.value })
          }
        />
        <Input
          placeholder="Description"
          value={newProduct.description}
          onChange={(e) =>
            setNewProduct({ ...newProduct, description: e.target.value })
          }
        />
        <Input
          type="number"
          placeholder="Price"
          value={newProduct.price}
          onChange={(e) =>
            setNewProduct({ ...newProduct, price: Number(e.target.value) })
          }
        />
        <Button colorScheme="teal" onClick={handleAdd}>
          Add Product
        </Button>
      </VStack>
    </Box>
  );
};

export default Products;
