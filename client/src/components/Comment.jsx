import { IconBtn } from "./IconBtn";
import { FaEdit, FaHeart, FaReply, FaTrash } from "react-icons/fa";

const dateFormater = new Intl.DateTimeFormat(undefined, {
  dateStyle: "medium",
  timeStyle: "short",
});

export function Comment({ id, message, user, createdAt }) {
  return (
    <>
      <div className="comment">
        <div className="header">
          <span className="name"></span>
          <span className="date">
            {dateFormater.format(Date.parse(createdAt))}
          </span>
        </div>
        <div className="message">{message}</div>
        <div className="footer">
          <IconBtn Icon={FaHeart}>4</IconBtn>
          <IconBtn Icon={FaReply} />
          <IconBtn Icon={FaEdit} />
          <IconBtn Icon={FaTrash} color="danger" />
        </div>
      </div>
    </>
  );
}
