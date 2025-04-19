import OrderForm from "../components/OrderForm.jsx";
import { useRouteLoaderData } from "react-router-dom";

export default function EditOrder() {
  const data = useRouteLoaderData("orderDetail");

  return <OrderForm mode="Edit" order={data.order} />;
}
