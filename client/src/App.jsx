import { PostsList } from "./components/PostsList";
import { Routes, Route } from "react-router-dom";

export default function App() {
  return (
    <div className="container">
      <Routes>
        <Route path="/" element={<PostsList />} />
        <Route path="/posts/:id" element={null} />
      </Routes>
    </div>
  );
}
