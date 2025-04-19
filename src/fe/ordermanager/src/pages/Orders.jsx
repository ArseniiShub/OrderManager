import { Link, useLoaderData, useNavigate } from "react-router-dom";
import OrderItem from "../components/OrderItem.jsx";
import classes from "./Orders.module.css";
import Button from "../components/ui/Button.jsx";
import { RMApiUrl } from "../App.jsx";

function Orders() {
  const { orders } = useLoaderData();
  const navigate = useNavigate();

  function handleAddOrder() {
    navigate("new");
  }

  return (
    <>
      <div className={classes.div}>
        <h1>Orders</h1>
        <Button onClick={handleAddOrder}>+ Order</Button>
      </div>
      <table className={classes.table}>
        <thead>
        <tr>
          <th></th>
          <th>Id</th>
          <th>Delivery Address</th>
          <th>Status</th>
          <th>Archived</th>
          <th>Action</th>
          <td></td>
        </tr>
        </thead>
        <tbody>
        {orders
          .sort((a, b) => a.id.localeCompare(b.id))
          .map(order => <OrderItem order={order} key={order.id} />)
        }
        </tbody>
      </table>
    </>
  );
}

export default Orders;

async function loadOrders() {
  const response = await fetch(`${RMApiUrl}orders`, {
    method: "GET",
    headers: { "Content-Type": "application/json" }
  });

  if (!response.ok) {
    throw Response.json({ message: "Could not fetch orders." }, { status: 500, });
  } else {
    const resData = await response.json();
    return resData.orders;
  }
}

export async function ordersLoader() {
  return { orders: await loadOrders() };
}