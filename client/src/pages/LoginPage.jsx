import { useState } from "react";
import { useAsyncFn } from "../hooks/useAsync";
import { useIdentity } from "../contexts/IdentityContext";
import { useNavigate } from "react-router-dom";

export default function LoginPage() {
  const [email, setEmail] = useState("user1@test.com");
  const [password, setPassword] = useState("password");

  const { login } = useIdentity();

  const { loading, error, execute: loginFn } = useAsyncFn(login);

  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    loginFn({ email, password }).then(() => {
      navigate("/");
    });
  };

  return (
    <div>
      <h2>Login</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Email:</label>
          <input
            type="text"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>
        <div>
          <label>Password:</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </div>
        {error && <div>{error}</div>}
        <button type="submit">Login</button>
      </form>
    </div>
  );
}
