import classes from "./Button.module.css";

export default function Button({ children, textOnly, ...props }) {
  const cssClasses = textOnly ? classes.textButton : classes.button;

  return (
    <button className={cssClasses} {...props}>
      {children}
    </button>
  );
}