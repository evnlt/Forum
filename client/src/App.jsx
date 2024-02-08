import { PostsList } from "./components/PostsList";
import { Routes, Route } from "react-router-dom";
import { PostProvider } from "./contexts/PostContext";
import { Post } from "./components/Post";

export default function App() {
  return (
    <div className="container">
      <Routes>
        <Route path="/" element={<PostsList />} />
        <Route
          path="/posts/:id"
          element={
            <PostProvider>
              <Post />
            </PostProvider>
          }
        />
      </Routes>
    </div>
  );
}
