import { logout } from "../util/auth";
import { Navigate } from "react-router-dom";

export async function logoutAction({ request }) {
  const url = new URL(request.url);
  await logout({ redirectUri: url.origin });

  return <Navigate to="/" replace />;
}

export default function Logout() {
  return <h1>Not Visible</h1>;
}