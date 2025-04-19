import { Link, useLoaderData, useRouteLoaderData } from "react-router-dom";
import OrderForm from "../components/OrderForm.jsx";
import { RMApiUrl } from "../App.jsx";

export default function OrderDetail() {
  const { order } = useRouteLoaderData("orderDetail");

  return (
    <OrderForm mode="View" order={order} />
  );
}

export async function orderDetailLoader({ params }) {
  const id = params.orderId;

  const response = await fetch(`${RMApiUrl}orders/` + id);

  if (!response.ok) {
    throw Response.json({ message: "Could not fetch details for selected order." }, { status: 500, });
  } else {
    const order = await response.json();
    return { order };
  }
}