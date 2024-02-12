import { usePost } from "../contexts/PostContext";
import { CommentList } from "./CommentList";

export function Post() {
  const { post, rootComments } = usePost();

  return (
    <>
      <h1>{post.title}</h1>
      <article>{post.body}</article>
      <h3>Comments</h3>
      <section>
        {rootComments != null && rootComments.length > 0 && (
          <div>
            <CommentList comments={rootComments} />
          </div>
        )}
      </section>
    </>
  );
}
