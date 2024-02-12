import { Comment } from "./Comment";

export function CommentList({ comments }) {
  return comments.map((comment) => (
    <div key={comment.id}>
      <Comment {...comment} />
    </div>
  ));
}
