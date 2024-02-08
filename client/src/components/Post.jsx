import { usePost } from "../contexts/PostContext";

export function Post() {
  const { post } = usePost();

  return (
    <>
      <h1>{post.title}</h1>
    </>
  );
}
