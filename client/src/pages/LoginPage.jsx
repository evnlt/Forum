import { useState } from "react";
import { useAsyncFn } from "../hooks/useAsync";
import { login } from "../services/identity";
import { useIdentity } from "../contexts/IdentityContext";
import { useNavigate } from "react-router-dom";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const { loading, error, execute: loginFn } = useAsyncFn(login);
  const { setAccessToken } = useIdentity();

  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    loginFn({ email, password }).then((accessToken) => {
      setAccessToken(accessToken);
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
