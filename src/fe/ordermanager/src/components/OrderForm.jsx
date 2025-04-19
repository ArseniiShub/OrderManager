import { Form, redirect, useActionData, useNavigation } from "react-router-dom";
import { fetchWithAuth } from "../util/fetchWithAuth.js";
import Button from "./ui/Button.jsx";
import Anchor from "./ui/Anchor.jsx";
import classes from "./OrderForm.module.css";
import Input from "./ui/Input.jsx";
import Switch from "./ui/Switch.jsx";
import { WMApiUrl } from "../App.jsx";

export default function OrderForm({ mode, order }) {
  const data = useActionData();
  const navigation = useNavigation();

  const isSubmitting = navigation.state === "submitting";

  return (
    <>
      <h1>{mode + " Order"}</h1>
      <Form method={mode === "Create" ? "post" : "patch"}>
        <article className={classes.order} inert={mode === "View" || isSubmitting}>
          {data && data.error && data.error.errors && (
            <ul>
              {data.error.errors.map((err) => (
                <li key={err.description}>{err.description}</li>
              ))}
            </ul>
          )}
          <p>
            <label htmlFor="deliveryAddress">Delivery Address</label>
            <Input
              id="deliveryAddress"
              type="text"
              name="deliveryAddress"
              required
              defaultValue={order ? order.deliveryAddress : ""}
            />
          </p>
          <p>
            <label htmlFor="productName">Product Name</label>
            <Input
              disabled={mode === "Edit"}
              id="productName"
              type="text"
              name="productName"
              required
              defaultValue={order ? order.productName : ""}
            />
          </p>
          <p>
            <label htmlFor="isArchived">Archived</label>
            <Switch
              disabled={mode === "Edit"}
              id="isArchived"
              type="checkbox"
              name="isArchived"
              defaultChecked={order ? order.isArchived : false}
            />
          </p>
        </article>
        <div className={classes.actions}>
          {
            <Anchor to=".." disabled={isSubmitting}>
              {mode === "Edit" ? "Cancel" : "Back to Orders"}
            </Anchor>
          }
          {
            mode === "View" &&
            <Anchor to="edit" disabled={isSubmitting}>
              Edit
            </Anchor>
          }
          {
            (mode === "Create" || mode === "Edit") &&
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? "Submitting..." : "Save"}
            </Button>
          }
        </div>
      </Form>
    </>
  );
}

export async function saveOrderAction({ request, params }) {
  const method = request.method;
  const fd = await request.formData();
  const orderData = Object.fromEntries(fd.entries());

  let url = `${WMApiUrl}orders`;

  if (method === "PATCH") {
    const orderId = "/" + params.orderId;
    url += orderId;
  }

  const response = await fetchWithAuth(url, {
    method: method,
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(orderData),
  });

  if (!response.ok) {
    const error = await response.json();
    if (error && error.title && error.title.includes("Validation")) {
      return { error };
    }

    throw Response.json({ message: "Could not save event." }, { status: 500 });
  }

  return redirect("/orders");
}
