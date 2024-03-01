import { useState } from "react";
import { useAsyncFn } from "../hooks/useAsync";
import { login } from "../services/identity";
import { useIdentity } from "../contexts/IdentityContext";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const { loading, error, execute: loginFn } = useAsyncFn(login);

  const { setAccessToken } = useIdentity();

  const handleSubmit = async (e) => {
    e.preventDefault();
    loginFn({ email, password }).then((accessToken) => {
      setAccessToken(accessToken);
    });
    // try {
    //   // Call your backend API to authenticate user
    //   const response = await axios.post("/api/auth/login", { email, password });
    //   const { token, user } = response.data;
    //   localStorage.setItem("token", token);
    //   setUser(user);
    // } catch (error) {
    //   console.error("Login error:", error);
    //   throw error;
    // }
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
