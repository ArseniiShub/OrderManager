import { Link } from "react-router-dom";
import buttonClasses from "./Button.module.css";
import anchorClasses from "./Anchor.module.css";

export default function Anchor({ children, to, textOnly, ...props }) {
  const cssClasses = textOnly ? buttonClasses.textButton : buttonClasses.button;

  return (
    <Link className={`${cssClasses} ${anchorClasses.anchor}`} to={to} {...props}>
      {children}
    </Link>
  );
}