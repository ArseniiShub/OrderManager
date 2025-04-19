import { createBrowserRouter, RouterProvider } from "react-router-dom";
import { useEffect, useState } from "react";

import Error from "./pages/Error.jsx";
import Home from "./pages/Home.jsx";
import RootLayout from "./pages/Root.jsx";
import Logout, { logoutAction } from "./pages/Logout.jsx";
import { authenticationLoader, initKeycloak, tokenLoader } from "./util/auth";
import Orders, { ordersLoader } from "./pages/Orders.jsx";
import Login, { loginAction } from "./pages/Login.jsx";
import Loading from "./components/Loading.jsx";
import { saveOrderAction } from "./components/OrderForm.jsx";
import OrderDetail, { orderDetailLoader } from "./pages/OrderDetail.jsx";
import EditOrder from "./pages/EditOrder.jsx";
import NewOrder from "./pages/NewOrder.jsx";

export const WMApiUrl = import.meta.env.VITE_WM_API_URL;
export const RMApiUrl = import.meta.env.VITE_RM_API_URL;

const router = createBrowserRouter([
  {
    path: "/",
    element: <RootLayout />,
    errorElement: <Error />,
    loader: tokenLoader,
    id: "root",
    children: [
      { index: true, element: <Home /> },
      {
        path: "auth",
        element: <Login />,
        action: loginAction
      },
      {
        path: "logout",
        element: <Logout />,
        action: logoutAction
      },
      {
        path: "orders",
        loader: authenticationLoader,
        children: [
          { index: true, element: <Orders />, loader: ordersLoader },
          { path: "new", element: <NewOrder />, loader: authenticationLoader, action: saveOrderAction },
          {
            path: ":orderId", id: "orderDetail", loader: orderDetailLoader, children: [
              { index: true, element: <OrderDetail /> },
              { path: "edit", element: <EditOrder />, action: saveOrderAction },
            ]
          },
        ]
      },
    ],
  },
]);

function App() {
  const [keycloakInitialized, setKeycloakInitialized] = useState(false);

  useEffect(() => {
    initKeycloak(() => {
      setKeycloakInitialized(true);
    });
  }, []);

  if (!keycloakInitialized) {
    return <Loading />;
  }

  return <RouterProvider router={router} />;
}

export default App;
