import classes from "./Switch.module.css";

export default function Switch({ ...props }) {
  return (
    <label className={classes.switch}>
      <input type="checkbox" {...props} />
      <span className={classes.slider}></span>
    </label>
  )
}