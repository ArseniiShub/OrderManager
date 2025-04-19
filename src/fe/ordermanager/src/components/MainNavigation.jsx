import { Form, NavLink, useRouteLoaderData } from "react-router-dom";
import classes from "./MainNavigation.module.css";
import Button from "./ui/Button.jsx";

const ROUTES = {
  HOME: '/',
  ORDERS: '/orders',
  LOGOUT: '/logout'
};

function MainNavigation() {
  const token = useRouteLoaderData("root");
  
  const getLinkClassName = ({ isActive }) => isActive ? classes.active : undefined;
  
  return (
    <header className={classes.header}>
      <nav>
        <ul className={classes.list}>
          <NavItem to={ROUTES.HOME} getLinkClassName={getLinkClassName} end>
            Home
          </NavItem>
          
          <NavItem to={ROUTES.ORDERS} getLinkClassName={getLinkClassName}>
            Orders
          </NavItem>
          
          {token && (
            <li>
              <Form action={ROUTES.LOGOUT} method="post">
                <Button type="submit" textOnly>Logout</Button>
              </Form>
            </li>
          )}
        </ul>
      </nav>
    </header>
  );
}

function NavItem({ to, getLinkClassName, children, ...props }) {
  return (
    <li>
      <NavLink to={to} className={getLinkClassName} {...props}>
        {children}
      </NavLink>
    </li>
  );
}

export default MainNavigation;