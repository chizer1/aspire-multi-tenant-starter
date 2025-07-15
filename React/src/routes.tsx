import { RootRoute, Route, Router, Outlet } from '@tanstack/react-router';
import Home from './pages/Home';
import Products from './pages/Products';
import Tenants from './pages/Tenants';
import MenuBar from './components/MenuBar';
import React from 'react';

const rootRoute = new RootRoute({
    component: () => (
      <>
        <MenuBar />
        <Outlet />
      </>
    ),
  });

const homeRoute = new Route({
  getParentRoute: () => rootRoute,
  path: '/',
  component: Home,
});

const productsRoute = new Route({
  getParentRoute: () => rootRoute,
  path: '/products',
  component: Products,
});

const tenantsRoute = new Route({
  getParentRoute: () => rootRoute,
  path: '/tenants',
  component: Tenants,
});

const routeTree = rootRoute.addChildren([
  homeRoute,
  productsRoute,
  tenantsRoute,
]);

export const router = new Router({ routeTree });

export { homeRoute, productsRoute, tenantsRoute, rootRoute };