import React from "react";
import { getAuthToken, login } from "../util/auth";
import { Form, Navigate, useSearchParams } from "react-router-dom";
import Button from "../components/ui/Button.jsx";

export default function Login() {
  const [searchParams] = useSearchParams();
  const token = getAuthToken();

  if (token && token !== "EXPIRED") {
    const redirectUrl = searchParams.get("redirectUrl") || "/";
    return <Navigate to={redirectUrl} replace />;
  }

  return (
    <Form method="post">
      <h1>Login</h1>
      <Button type="submit">Login with Keycloak</Button>
    </Form>
  );
}

export async function loginAction() {
  await login();
}