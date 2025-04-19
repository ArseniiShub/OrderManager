import { fetchWithAuth } from "../util/fetchWithAuth.js";
import classes from "./OrderItem.module.css";
import archiveImage from "../assets/icons/archive.svg";
import restoreImage from "../assets/icons/restore.svg";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Button from "./ui/Button.jsx";
import { RMApiUrl, WMApiUrl } from "../App.jsx";

const orderStatuses = ["New", "Dispatched", "OutForDelivery", "Delivered"];
const orderStatusActions = ["Dispatch", "OutForDelivery", "Deliver", ""];
const orderStatusActionsNames = ["Dispatch", "Send", "Deliver", ""];

export default function OrderItem({ order }) {

  const [orderState, setOrderState] = useState({ order, loading: false });
  const navigate = useNavigate();

  function getButtonElement() {

    if (orderState.order.status === "Delivered" || orderState.order.isArchived) {
      return undefined;
    }

    const statusIndex = orderStatuses.indexOf(orderState.order.status);

    return (
      <Button
        textOnly
        onClick={() =>
          fetchOrder(
            orderStatusActions[statusIndex],
            orderStatusActions[statusIndex] === "OutForDelivery" ? "Could not send order" : null
          )}
      >
        {orderStatusActionsNames[statusIndex]}
      </Button>
    );
  }

  async function fetchOrder(mode, errorMessageOverride = null) {
    setOrderState({ ...orderState, loading: true });
    const postResponse = await fetchWithAuth(`${WMApiUrl}orders/${orderState.order.id}/${mode}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" }
    });

    if (!postResponse.ok) {
      throw Response.json({ message: errorMessageOverride ?? `Could not ${mode} order` }, { status: 500 });
    }

    // Waiting RM
    await new Promise(resolve => setTimeout(resolve, 300));

    const getResponse = await fetch(`${RMApiUrl}orders/${orderState.order.id}`, {
      method: "GET",
      headers: { "Content-Type": "application/json" }
    });

    if (!getResponse.ok) {
      throw Response.json({ message: `Could not reload order` }, { status: 500 });
    }

    const resData = await getResponse.json();
    setOrderState({ order: resData, loading: false });
  }

  function handleOpenOrder() {
    navigate(orderState.order.id.toString());
  }

  let img = <img className={classes.img} src={archiveImage} alt="Archive image" title="Archive" />;
  if (orderState.order.isArchived) {
    img = <img className={classes.img} src={restoreImage} alt="Restore image" title="Restore" />;
  }

  return (
    <tr
      inert={orderState.loading === true}
      onDoubleClick={handleOpenOrder}
      className={`${classes.tr} ${orderState.order.isArchived ? classes.archived : ""}`}
    >
      <td>
        {orderState.loading === true ? "..." : ""}
      </td>
      <td>{orderState.order.id}</td>
      <td>{orderState.order.deliveryAddress}</td>
      <td>{orderState.order.status}</td>
      <td>
        <button
          onClick={() => fetchOrder(orderState.order.isArchived ? "restore" : "archive")}
          className={classes.button}
        >
          {img}
        </button>
      </td>
      <td>
        {getButtonElement()}
      </td>
      <td></td>
    </tr>
  );
}