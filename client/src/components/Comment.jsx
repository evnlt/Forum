import { useState } from "react";
import { usePost } from "../contexts/PostContext";
import { CommentList } from "./CommentList";
import { CommentForm } from "./CommentForm";
import { IconBtn } from "./IconBtn";
import { FaEdit, FaHeart, FaRegHeart, FaReply, FaTrash } from "react-icons/fa";
import { useAsyncFn } from "../hooks/useAsync";
import {
  createComment,
  updateComment,
  deleteComment,
  toggleCommentLike,
} from "../services/comments";
import { useIdentity } from "../contexts/IdentityContext";

const dateFormater = new Intl.DateTimeFormat(undefined, {
  dateStyle: "medium",
  timeStyle: "short",
});

export function Comment({
  id,
  message,
  user,
  createdAt,
  likeCount,
  likedByMe,
}) {
  const {
    post,
    getReplies,
    createLocalComment,
    updateLocalComment,
    deleteLocalComment,
    toggleLocalCommentLike,
  } = usePost();
  const createCommentFn = useAsyncFn(createComment);
  const updateCommentFn = useAsyncFn(updateComment);
  const deleteCommentFn = useAsyncFn(deleteComment);
  const toggleCommentLikeFn = useAsyncFn(toggleCommentLike);
  const childComments = getReplies(id);
  const [areChildrenHidden, setAreChildrenHidden] = useState(true);
  const [isReplying, setIsReplying] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const currentUser = useIdentity();

  function onCommentReply(message) {
    return createCommentFn
      .execute({ postId: post.id, message, parentId: id })
      .then((comment) => {
        setIsReplying(false);
        setAreChildrenHidden(false);
        createLocalComment(comment);
      });
  }

  function onCommentUpdate(message) {
    return updateCommentFn
      .execute({ postId: post.id, message, id })
      .then((comment) => {
        setIsEditing(false);
        setAreChildrenHidden(false);
        updateLocalComment(id, comment.message);
      });
  }

  function onCommentDelete() {
    return deleteCommentFn.execute({ postId: post.id, id }).then((comment) => {
      setAreChildrenHidden(false);
      deleteLocalComment(id);
    });
  }

  function onToggleCommentLike() {
    return toggleCommentLikeFn
      .execute({ commentId: id, postId: post.id })
      .then((value) => toggleLocalCommentLike(id, value));
  }

  return (
    <>
      <div className="comment">
        <div className="header">
          <span className="name">{user.name}</span>
          <span className="date">
            {dateFormater.format(Date.parse(createdAt))}
          </span>
        </div>
        {isEditing ? (
          <CommentForm
            autoFocus
            initialValue={message}
            onSubmit={onCommentUpdate}
            loading={updateCommentFn.loading}
            error={updateCommentFn.error}
          />
        ) : (
          <div className="message">{message}</div>
        )}
        <div className="footer">
          <IconBtn
            onClick={onToggleCommentLike}
            disabled={toggleCommentLikeFn.loading}
            Icon={likedByMe ? FaHeart : FaRegHeart}
          >
            {likeCount}
          </IconBtn>
          <IconBtn
            Icon={FaReply}
            isActive={isReplying}
            onClick={() => setIsReplying((prev) => !prev)}
          />
          {currentUser.id === user.id && (
            <>
              <IconBtn
                Icon={FaEdit}
                isActive={isEditing}
                onClick={() => setIsEditing((prev) => !prev)}
              />
              <IconBtn
                Icon={FaTrash}
                disabled={deleteCommentFn.loading}
                onClick={() => onCommentDelete(id)}
                color="danger"
              />
            </>
          )}
        </div>
      </div>
      {isReplying && (
        <div className="mt-1 ml-3">
          <CommentForm
            autoFocus
            onSubmit={onCommentReply}
            loading={createCommentFn.loading}
            error={createCommentFn.error}
          />
        </div>
      )}
      {childComments?.length > 0 && (
        <>
          <div
            className={`nested-comments-stack ${
              areChildrenHidden ? "hide" : ""
            }`}
          >
            <button
              className="collapse-line"
              onClick={() => setAreChildrenHidden(true)}
            ></button>
            <div className="nested-comments">
              <CommentList comments={childComments} />
            </div>
          </div>
          <button
            className={`btn mt-1 ${!areChildrenHidden ? "hide" : ""}`}
            onClick={() => setAreChildrenHidden(false)}
          >
            Show Replies
          </button>
        </>
      )}
    </>
  );
}
