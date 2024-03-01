import { PostsList } from "./components/PostsList";
import { Routes, Route } from "react-router-dom";
import { PostProvider } from "./contexts/PostContext";
import { IdentityProvider } from "./contexts/IdentityContext";
import { Post } from "./components/Post";
import LoginPage from "./pages/LoginPage";

export default function App() {
  return (
    <div className="container">
      <IdentityProvider>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
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
      </IdentityProvider>
    </div>
  );
}
