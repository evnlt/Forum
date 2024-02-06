import { useState, useEffect } from "react";
import { getPosts } from "../services/posts";

export function PostsList() {
  const [posts, setPosts] = useState([]);

  useEffect(() => {
    getPosts().then(setPosts);
  }, []);

  return posts.map((post) => {
    return (
      <h1 key={post.id}>
        <a>{post.title}</a>
      </h1>
    );
  });
}
